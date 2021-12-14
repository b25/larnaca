using gen.utils;
using Microsoft.Extensions.Options;
using mssql.collector.types;
using mssql.utils;
using Newtonsoft.Json;
using standard.types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace mssql.collector
{
    public class SqlCollectorService
    {
        private SqlCollectorServiceOptions options;
        private SqlCredential _credential = null;

        public SqlCollectorService(IOptions<SqlCollectorServiceOptions> secrets)
        {
            options = secrets.Value;

            if (!string.IsNullOrEmpty(options.ConnectionUser))
            {
                var sc = new SecureString();
                foreach (char c in options.ConnectionPassword ?? "") sc.AppendChar(c);
                sc.MakeReadOnly();

                _credential = new SqlCredential(options.ConnectionUser, sc);
            }
        }

        #region SQL collector...

        public async Task<OperationResult> WriteDatabaseMetaToFile(string workingDirectory)
        {
            try
            {
                var prevMetaFile = Path.Join(workingDirectory, options.PreviousResultFile);
                //build index
                var idx = new ContractOrderDictionary(File.Exists(prevMetaFile) ? JsonConvert.DeserializeObject<DatabaseMeta>(File.ReadAllText(prevMetaFile)) : null);

                var resp = await GetDatabaseMeta(idx).ConfigureAwait(false);
                if (resp.Fail())
                {
                    return new OperationResult(resp);
                }

                var jsonFile = Path.Join(workingDirectory, options.ResultFile);
                File.WriteAllText(jsonFile, JsonConvert.SerializeObject(resp.Data, Formatting.Indented));
                return new OperationResult { StatusMessage = $"write succesfully database meta to: {jsonFile}" };
            }
            catch (Exception ex)
            {
                return new OperationResult(ex);
            }
        }

        public async Task<OperationResult<DatabaseMeta>> GetDatabaseMeta(ContractOrderDictionary idx)
        {
            var procsResult = await GetProcedures(idx).ConfigureAwait(false);
            if (procsResult.Fail() || procsResult.Data.EmptyIfNull().Any(r => r.Fail()))
            {
                return new OperationResult<DatabaseMeta>($"Errors on collect procedures data: [{ string.Join(",", (procsResult as IOperationResult).Yield().Union(procsResult.Data.EmptyIfNull()).Where(i => i.Fail()).Select(i => i.StatusMessage).ToArray()) }]");
            }
            else
            {
                SqlConnectionStringBuilder conBuilder = new SqlConnectionStringBuilder(options.ConnectionString);
                return new OperationResult<DatabaseMeta>(new DatabaseMeta()
                {
                    Name = DalUtils.StripUnderscorePrefix(conBuilder.InitialCatalog),
                    Procedures = procsResult.Data.Select(i => i.Data).ToArray()
                });
            }
        }

        public async Task<OperationResult<List<OperationResult<ProcedureMeta>>>> GetProcedures(ContractOrderDictionary idx)
        {
            try
            {
                var rg = new Regex(options.ProcedurePattern);

                var procList = new List<string>();
                using (var dr = new DataReader(options.ConnectionString, _credential))
                {
                    var dt = dr.GetSchema("Procedures", new string[] { null, null, null, "PROCEDURE" });

                    foreach (DataRow row in dt.Rows)
                    {
                        procList.Add(row["SPECIFIC_NAME"].ToString());
                    }
                }
                var list = new List<OperationResult<ProcedureMeta>>();

                foreach (var procedureName in procList)
                {
                    if (!rg.IsMatch(procedureName)) continue;

                    list.Add(await GetProcedureParams(procedureName, idx).ConfigureAwait(false));
                }

                var successfullSPs = list.Where(op => !op.IsFail()).Select((op, i) => op.Data).ToList();

                return new OperationResult<List<OperationResult<ProcedureMeta>>>(list);
            }
            catch (Exception ex)
            {
                return new OperationResult<List<OperationResult<ProcedureMeta>>>(ex.Message);
            }
        }

        public async Task<OperationResult<ProcedureMeta>> GetProcedureParams(string spName, ContractOrderDictionary idx)
        {
            var spmeta = new ProcedureMeta { SpName = spName };
            try
            {
                var opReq = GetProcedureRequestParams(spName, idx);

                if (opReq.Fail()) return new OperationResult<ProcedureMeta>(opReq);
                spmeta.Request = opReq.Data.data;

                if (options.SkipOutputParams) return new OperationResult<ProcedureMeta>(spmeta);
                if (!opReq.Data.hasSchemaParam)
                {
                    spmeta.Errors.Add($"{spName} do not have _schema in the input parameters, skip harvesting output params");
                    return new OperationResult<ProcedureMeta>
                    {
                        Data = spmeta,
                        StatusCode = (int)EOperationCode.Error,
                        StatusMessage = $"{spName} do not have _schema in the input parameters, skip harvesting output params"
                    };
                }

                var opResp = await GetProcedureResponseParams(spmeta.Request, spName, idx).ConfigureAwait(false);

                if (opResp.Fail())
                {
                    return new OperationResult<ProcedureMeta>(opResp.StatusCode, opResp.StatusMessage, spmeta);
                }
                spmeta.Responses = opResp.Data;

                return new OperationResult<ProcedureMeta>(spmeta);
            }
            catch (Exception ex)
            {
                return new OperationResult<ProcedureMeta>($"[{spName}] {ex.Message}");
            }
        }

        public OperationResult<(bool hasSchemaParam, List<ParamMeta> data)> GetProcedureRequestParams(string spName, ContractOrderDictionary idx)
        {
            try
            {
                var schema = new DataTable();
                bool hasSchemaParam = false;
                using (SqlConnection connection = new SqlConnection(options.ConnectionString))
                {
                    connection.Open();
                    SqlCommand getSchemaCmd = new SqlCommand(@"
SELECT params.name AS PARAMETER_NAME
, t.name AS SQL_TYPE
, params.is_nullable AS HAS_DEFAULT
FROM sys.parameters params
INNER JOIN sys.procedures procs ON  params.object_id = procs.object_id
INNER JOIN sys.types t ON t.system_type_id = params.system_type_id AND params.user_type_id = t.user_type_id
WHERE procs.name = @procName", connection);
                    getSchemaCmd.Parameters.AddWithValue("@procName", spName);
                    schema.Load(getSchemaCmd.ExecuteReader());
                }
                var list = new List<ParamMeta>();

                foreach (DataRow row in schema.Rows)
                {
                    var paramName = row["PARAMETER_NAME"].ToString().Replace("@", "");
                    if (paramName.StartsWith("_schema", StringComparison.InvariantCultureIgnoreCase))
                    {
                        hasSchemaParam = true;
                        continue;
                    }
                    list.Add(new ParamMeta
                    {
                        Name = paramName,
                        SqlType = row["SQL_TYPE"].ToString(),
                        HasDefaultValue = (bool)row["HAS_DEFAULT"],
                        Order = idx.GetOrAddRequestIndex(spName, paramName)
                    });
                }

                //check for tvp
                foreach (var x in list)
                {
                    if (GetSlqDbType(x.SqlType) == SqlDbType.Structured)
                    {
                        x.TVP = GetTvp(spName,x.SqlType, options.ConnectionString, _credential, idx);
                    }
                }

                return new OperationResult<(bool, List<ParamMeta>)>((hasSchemaParam, list));
            }
            catch (Exception ex)
            {
                return new OperationResult<(bool, List<ParamMeta>)>($"[{spName}] {ex.Message}");
            }
        }

        public static List<TvpParamMeta> GetTvp(string spName, string tvpName, string connectionString, SqlCredential credential, ContractOrderDictionary idx)
        {
            int tvpId = 0;
            var list = new List<TvpParamMeta>();
            using (DataReader dr = new DataReader(connectionString, credential))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = @"
                                select
	                            tvp_schema = ss.name,
	                            tvp_name = stt.name,
	                            stt.type_table_object_id
                            from sys.table_types stt
                            inner join sys.schemas ss on stt.schema_id = ss.schema_id
							where stt.name=@tvpName
                        ";
                    command.Parameters.Add("@tvpName", SqlDbType.VarChar).Value = tvpName;
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 60;

                    dr.Execute(command);

                    if (!dr.Reader.HasRows)
                    {
                        throw new Exception($@"could not found tvp {tvpName}");
                    }

                    dr.Reader.Read();
                    tvpId = dr.Reader.GetInt32(dr.Reader.GetOrdinal("type_table_object_id"));
                }
            }

            using (DataReader dr = new DataReader(connectionString, credential))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = @"
                            select sc.name,
	                            st.name as data_type,
                                sc.is_nullable
                            from sys.columns sc
                            inner join sys.types st on sc.system_type_id = st.system_type_id and sc.user_type_id = st.user_type_id
                            where sc.object_id = @tvp_id
                        ";
                    command.Parameters.Add("@tvp_id", SqlDbType.Int).Value = tvpId;
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 60;

                    dr.Execute(command);

                    if (!dr.Reader.HasRows)
                    {
                        throw new Exception($@"could not found tvp props {tvpName} type_table_object_id:{tvpId}");
                    }

                    while (dr.Reader.Read())
                    {
                        var paramName = (string)dr.Reader.GetSqlString(dr.Reader.GetOrdinal("name"));
                        list.Add(new TvpParamMeta
                        {
                            Name = paramName,
                            SqlType = (string)dr.Reader.GetSqlString(dr.Reader.GetOrdinal("data_type")),
                            IsNullable = (bool)dr.Reader.GetSqlBoolean(dr.Reader.GetOrdinal("is_nullable")),
                            Order = idx.GetOrAddRequestTvpIndex(spName, tvpName, paramName)
                        });
                    }
                }
            }
            return list;
        }

        public static List<ParamMeta> GetProcedureResultSetParams(SqlDataReader dr, string spName)
        {
            var list = new List<ParamMeta>();

            var rows = dr.GetSchemaTable()?.Rows;
            if (rows != null)
            {
                foreach (DataRow row in rows)
                {
                    var paramName = row["ColumnName"].ToString();
                    list.Add(new ParamMeta
                    {
                        Name = paramName,
                        SqlType = row["DataTypeName"].ToString(),
                    });
                }
            }
            return list;
        }

        public static SqlCommand GenerateProcedureRequestCommand(List<ParamMeta> list, string spname)
        {
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
            var cmd = new SqlCommand { CommandText = spname };
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities

            cmd.Parameters.Add(new SqlParameter
            {
                Direction = ParameterDirection.Input,
                ParameterName = "@_schema",
                SqlDbType = SqlDbType.Bit,
                Value = true
            });

            foreach (var item in list)
            {
                cmd.Parameters.Add(GetDefaultSqlParam(item.Name, item.SqlType, item.TVP));
            }

            return cmd;
        }

        public static SqlParameter GetDefaultSqlParam(string sqlName, string sqlType, List<TvpParamMeta> tvp)
        {
            var type = GetSlqDbType(sqlType);

            var prm = new SqlParameter
            {
                Direction = ParameterDirection.Input,
                ParameterName = sqlName,
                SqlDbType = type,
                Value = GetDefaultSqlParameterValue(tvp)
            };

            return prm;
        }

        public static object GetDefaultSqlParameterValue(List<TvpParamMeta> tvp)
        {
            if (!(tvp?.Any() ?? false)) return DBNull.Value;

            var dt = new DataTable();
            foreach (var x in tvp)
            {
                dt.Columns.Add(x.Name, GetCSharpType(x.SqlType, false));
            }
            return dt;
        }

        public async Task<OperationResult<List<ResponseItem>>> GetProcedureResponseParams(List<ParamMeta> request, string spName, ContractOrderDictionary idx)
        {
            try
            {
                using (DataReader dr = new DataReader(options.ConnectionString, _credential))
                {
                    var responses = new List<ResponseItem>();
                    var cmd = GenerateProcedureRequestCommand(request, spName);

                    await dr.ExecuteSpAsync(cmd).ConfigureAwait(false);

                    var resultSetCount = 1;
                  
                    do
                    {
                        var resultName = $"ResultSet{resultSetCount}";
                        var currentRespones = GetProcedureResultSetParams(dr.Reader, spName);
                        // Order =
                        if (currentRespones.Any())
                        {
                            var item = new ResponseItem
                            {
                                Name = "ResultSet0",
                                Params = currentRespones,
                            };
                            responses.Add(item);
                            if (resultSetCount==1 && item.IsOperationResult()) continue;

                            item.Order = idx.GetOrAddResponseIndex(spName, resultName);
                            item.Name = resultName;
                       
                            //fix items order
                            foreach (var x in item.Params)
                            {
                                x.Order = idx.GetOrAddResponseResultSetIndex(spName, resultName, x.Name);
                            }
                            //increment resultset counter
                            resultSetCount++;


                        }
                    }
                    while (await dr.NextResultAsync().ConfigureAwait(false));

                    return new OperationResult<List<ResponseItem>>(responses);
                }
            }
            catch (Exception ex)
            {
                return new OperationResult<List<ResponseItem>>($"{spName} get output params {ex.Message}");
            }
        }

        #endregion SQL collector...

        #region>>>helpers...

        public static SqlDbType GetSlqDbType(string sqlType)
        {
            switch ((sqlType ?? string.Empty).ToLower())
            {
                case "bigint":
                    return SqlDbType.BigInt;

                case "binary":
                    return SqlDbType.Binary;

                case "bit":
                    return SqlDbType.Bit;

                case "tinyint":
                    return SqlDbType.TinyInt;

                case "smallint":
                    return SqlDbType.SmallInt;

                case "int":
                    return SqlDbType.Int;

                case "float":
                    return SqlDbType.Float;

                case "money":
                    return SqlDbType.Money;

                case "decimal":
                    return SqlDbType.Decimal;

                case "numeric":
                    return SqlDbType.Decimal;

                case "image":
                    return SqlDbType.Image;

                case "filestream":
                    return SqlDbType.VarBinary;

                case "char":
                    return SqlDbType.Char;

                case "nchar":

                    return SqlDbType.NChar;

                case "ntext":
                    return SqlDbType.NText;

                case "nvarchar":
                    return SqlDbType.NVarChar;

                case "real":
                    return SqlDbType.Real;

                case "rowversion":
                    return SqlDbType.Binary;

                case "smalldatetime":
                    return SqlDbType.SmallDateTime;

                case "smallmoney":
                    return SqlDbType.SmallMoney;
                    ;
                case "sql_variant":
                    return SqlDbType.Variant;

                case "text":
                    return SqlDbType.Text;

                case "time":
                    return SqlDbType.Time;

                case "timestamp":
                    return SqlDbType.Timestamp;

                case "uniqueidentifier":
                    return SqlDbType.UniqueIdentifier;

                case "varbinary":
                    return SqlDbType.VarBinary;

                case "varchar":
                    return SqlDbType.VarChar;

                case "xml":
                    return SqlDbType.Xml;

                case "date":
                    return SqlDbType.Date;

                case "datetime":
                    return SqlDbType.DateTime;

                case "datetime2":
                    return SqlDbType.DateTime2;

                case "datetimeoffset":
                    return SqlDbType.DateTimeOffset;

                default:
                    return SqlDbType.Structured;
            }
        }

        public static Type GetCSharpType(string sqlType, bool nullable)
        {
            if (nullable)
            {
                return typeof(Nullable<>).MakeGenericType(DalUtils.GetBaseCSharpType(sqlType));
            }
            else
            {
                return DalUtils.GetBaseCSharpType(sqlType);
            }
        }

        #endregion >>>get Procedures parameters...
    }
}