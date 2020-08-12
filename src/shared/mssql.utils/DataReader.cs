using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace mssql.utils
{
    public sealed class DataReader : IDisposable
    {
        private string _connectionString = null;

        private SqlConnection DbConnection { get; set; }

        public SqlDataReader Reader { get; private set; }

        public DataReader(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw (new Exception("connectionString is null"));
            }
            _connectionString = connectionString;
        }

        private void InitDB()
        {
            if (DbConnection == null)
            {
                DbConnection = new SqlConnection(_connectionString);

                DbConnection.Open();
            }
        }

        public DataTable GetSchema(string collection, string[] restrictionValues)
        {
            InitDB();
            return DbConnection.GetSchema(collection, restrictionValues);
        }

        public async Task ExecuteSpAsync(SqlCommand cmd, int timeout = 1000 * 30)
        {
            if (DbConnection == null)
            {
                DbConnection = new SqlConnection(_connectionString);

                await DbConnection.OpenAsync().ConfigureAwait(false);
            }

            cmd.Connection = DbConnection;
            cmd.CommandTimeout = timeout;
            cmd.CommandType = CommandType.StoredProcedure;

            foreach (SqlParameter parameter in cmd.Parameters)
            {
                if (parameter.Value == null)
                {
                    // replace null values with DBNull.Value so the parameters are sent to db
                    parameter.Value = DBNull.Value;
                }
            }

            Reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
        }

        public void Execute(SqlCommand cmd)
        {
            InitDB();
            cmd.Connection = DbConnection;
            Reader = cmd.ExecuteReader();
        }
        public Task<bool> ReadAsync()
        {
            return Reader.ReadAsync();
        }
        public Task<bool> NextResultAsync()
        {
            return Reader.NextResultAsync();
        }

        public List<string>  ValidateColumns(DataReaderColumnDefinition[] columns, bool strict)
        {
            var drColumns = new HashSet<string>();
            var missingColumns = new HashSet<string>(); 
           
            for (int i = 0; i < Reader.FieldCount; i++)
            {
                drColumns.Add(Reader.GetName(i));
            }
            var errors = new List<string>();
            foreach (var x in columns)
            {
                try
                {
                    if (drColumns.Remove(x.Name))
                    {
                        x.Order = Reader.GetOrdinal(x.Name);
                        var rowType = Reader.GetFieldType(x.Order);
                        if (rowType != x.Type)
                        {
                            errors.Add($"Column: '{x.Name}' Expected Type:'{x.Type.FullName}' Sql Data Type:'{rowType.FullName}'");
                        }
                    }
                    else
                    {
                        if (strict)
                        {
                            missingColumns.Add(x.Name);
                        }
                        x.Order = -1;
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"[{x.Name}] {ex.Message}");
                }
            }
            if (strict)
            {
                if (drColumns.Count > 0)
                {
                    errors.Add($"Unexpected columns in sql data: {string.Join(",", drColumns)}.");
                }
                if (missingColumns.Count > 0)
                {
                    errors.Add($"Expected columns missing from sql data: {string.Join(",", missingColumns)}.");
                }
            }
            return errors;
        }
        public void CheckFieldType(string columnName, Type type, StringBuilder sb)
        {
            var rowType = Reader.GetFieldType(Reader.GetOrdinal(columnName));
            if (rowType != type)
            {
                sb.Append($"[{columnName}] Expects:{type.FullName} row:{rowType.FullName}");
            };
        }
        public int GetOrdinal(string key)
        {
            return Reader.GetOrdinal(key);
        }
        public byte[] GetBytes(int ordinal)
        {
            byte[] result = null;

            if (!(Reader?.IsDBNull(ordinal) ?? false))
            {
                long size = Reader.GetBytes(ordinal, 0, null, 0, 0); //get the length of data
                result = new byte[size];
                int bufferSize = 1024;
                long bytesRead = 0;
                int curPos = 0;
                while (bytesRead < size)
                {
                    bytesRead += Reader.GetBytes(ordinal, curPos, result, curPos, bufferSize);
                    curPos += bufferSize;
                }
            }

            return result;
        }

        public byte? GetByte(int ordinal)
        {
            return Reader.IsDBNull(ordinal) ? (byte?)null : Reader.GetByte(ordinal);
        }

        public bool? GetBoolean(int ordinal)
        {
            return Reader.IsDBNull(ordinal) ? (bool?)null : Reader.GetBoolean(ordinal);
        }

        public short? GetInt16(int ordinal)
        {
            return Reader.IsDBNull(ordinal) ? (short?)null : Reader.GetInt16(ordinal);
        }

        public int? GetInt32(int ordinal)
        {
            return Reader.IsDBNull(ordinal) ? (int?)null : Reader.GetInt32(ordinal);
        }

        public long? GetInt64(int ordinal)
        {
            return Reader.IsDBNull(ordinal) ? (long?)null : Reader.GetInt64(ordinal);
        }

        public Guid? GetGuid(int ordinal)
        {
            return Reader.IsDBNull(ordinal) ? (Guid?)null : Reader.GetGuid(ordinal);
        }

        public float? GetFloat(int ordinal)
        {
            return Reader.IsDBNull(ordinal) ? (float?)null : Reader.GetFloat(ordinal);
        }

        public double? GetDouble(int ordinal)
        {
            return Reader.IsDBNull(ordinal) ? (double?)null : Reader.GetDouble(ordinal);
        }

        public decimal? GetDecimal(int ordinal)
        {
            return Reader.IsDBNull(ordinal) ? (decimal?)null : Reader.GetDecimal(ordinal);
        }

        public DateTime? GetDateTime(int ordinal)
        {
            return Reader.IsDBNull(ordinal) ? (DateTime?)null : Reader.GetDateTime(ordinal);
        }

        public DateTimeOffset? GetDateTimeOffset(int ordinal)
        {
            return Reader.IsDBNull(ordinal) ? (DateTimeOffset?)null : Reader.GetDateTimeOffset(ordinal);
        }

        public TimeSpan? GetTimeSpan(int ordinal)
        {
            return Reader.IsDBNull(ordinal) ? (TimeSpan?)null : Reader.GetTimeSpan(ordinal);
        }

        public string GetString(int ordinal)
        {
            return Reader.IsDBNull(ordinal) ? (string)null : Reader.GetString(ordinal);
        }

        public decimal? GetSqlMoney(int ordinal)
        {
            return Reader.IsDBNull(ordinal) ? (decimal?)null : (decimal)Reader.GetSqlMoney(ordinal);
        }



        #region >>>dispose...

        private bool disposed = false;

        ~DataReader()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Reader?.Dispose();
                    DbConnection?.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion >>>dispose...
    }
}