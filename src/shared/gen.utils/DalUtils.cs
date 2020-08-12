using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace gen.utils
{
    public class DalUtils
    {
        public static string GetBaseClassName(string originalName)
        {
            return ToPascalCase($@"({originalName.Replace("nc_", "").Replace("sp_", "").Replace("prc_", "")}");
        }

        public static string StripUnderscorePrefix(string name)
        {
            int uderscoreIndex;
            if (string.IsNullOrWhiteSpace(name)
                || (uderscoreIndex = name.IndexOf('_')) < 0)
            {
                return name;
            }
            else
            {
                return name.Substring(uderscoreIndex + 1, name.Length - (uderscoreIndex + 1));
            }
        }

        public static string ToPascalCase(string original)
        {
            Regex invalidCharsRgx = new Regex("[^_a-zA-Z0-9]");
            Regex whiteSpace = new Regex(@"(?<=\s)");
            Regex startsWithLowerCaseChar = new Regex("^[a-z]");
            Regex firstCharFollowedByUpperCasesOnly = new Regex("(?<=[A-Z])[A-Z0-9]+$@");
            Regex lowerCaseNextToNumber = new Regex("(?<=[0-9])[a-z]");
            Regex upperCaseInside = new Regex("(?<=[A-Z])[A-Z]+?((?=[A-Z][a-z])|(?=[0-9]))");

            // replace white spaces with undescore, then replace all invalid chars with empty string
            var pascalCase = invalidCharsRgx.Replace(whiteSpace.Replace(original, "_"), string.Empty)
                // split by underscores
                .Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
                // set first letter to uppercase
                .Select(w => startsWithLowerCaseChar.Replace(w, m => m.Value.ToUpper()))
                // replace second and all following upper case letters to lower if there is no next lower (ABC -> Abc)
                .Select(w => firstCharFollowedByUpperCasesOnly.Replace(w, m => m.Value.ToLower()))
                // set upper case the first lower case following a number (Ab9cd -> Ab9Cd)
                .Select(w => lowerCaseNextToNumber.Replace(w, m => m.Value.ToUpper()))
                // lower second and next upper case letters except the last if it follows by any lower (ABcDEf -> AbcDef)
                .Select(w => upperCaseInside.Replace(w, m => m.Value.ToLower()));

            return string.Concat(pascalCase);
        }

        public static string ToProtoCase(string original)
        {
            Regex invalidCharsRgx = new Regex("[^a-zA-Z0-9]");
            Regex whiteSpace = new Regex(@"(?<=\s)");
            Regex startsWithLowerCaseChar = new Regex("^[a-z]");

            // replace white spaces with and all invalid chars with empty string
            var pascalCase = invalidCharsRgx.Replace(whiteSpace.Replace(original, string.Empty), string.Empty).ToLower();

            // set first letter to uppercase
            pascalCase = startsWithLowerCaseChar.Replace(pascalCase, m => m.Value.ToUpper());

            return string.Concat(pascalCase);
        }

        public static string ToLowerFirst(string original)
        {
            if (string.IsNullOrWhiteSpace(original))
            {
                return original;
            }

            var charArray = original.ToCharArray();

            charArray[0] = char.ToLower(charArray[0]);

            return new string(charArray);
        }

        public static Type GetBaseCSharpType(string sqlType)
        {
            switch ((sqlType ?? string.Empty).ToLower())
            {
                case "bigint": return typeof(long);
                case "binary": return typeof(byte[]);
                case "bit": return typeof(bool);
                case "char": return typeof(string);
                case "date": return typeof(DateTime);
                case "filestream": return typeof(byte[]);
                case "image": return typeof(byte[]);
                case "tinyint": return typeof(byte);
                case "int": return typeof(int);
                case "float": return typeof(double);
                case "decimal": return typeof(decimal);
                case "money": return typeof(decimal);
                case "nchar": return typeof(string);
                case "ntext": return typeof(string);
                case "numeric": return typeof(decimal);
                case "nvarchar": return typeof(string);
                case "real": return typeof(Single);
                case "rowversion": return typeof(byte[]);
                case "smalldatetime": return typeof(DateTime);
                case "smallint": return typeof(short);
                case "smallmoney": return typeof(decimal);
                case "sql_variant": return typeof(object);
                case "text": return typeof(string);
                case "time": return typeof(TimeSpan);
                case "timestamp": return typeof(byte[]);

                case "uniqueidentifier": return typeof(Guid);
                case "varbinary": return typeof(byte[]);
                case "varchar": return typeof(string);
                case "xml": return typeof(string);
                case "datetime": return typeof(DateTime);
                case "datetime2": return typeof(DateTime);
                case "datetimeoffset": return typeof(DateTimeOffset);

                default: return typeof(object);
            }
        }

        public static string GetCSharpFriendlyType(string sqlType,string tvpClassName)
        {
            switch ((sqlType ?? string.Empty).ToLower())
            {
                case "bigint": return "long";
                case "binary": return "byte[]";
                case "bit": return "bool";
                case "char": return "string";
                case "date": return "DateTime";
                case "filestream": return "byte[]";
                case "image": return "byte[]";
                case "tinyint": return "byte";
                case "int": return "int";
                case "float": return "double";
                case "decimal": return "decimal";
                case "money": return "decimal";
                case "nchar": return "string";
                case "ntext": return "string";
                case "numeric": return "decimal";
                case "nvarchar": return "string";
                case "real": return "Single";
                case "rowversion": return "byte[]";
                case "smalldatetime": return "DateTime";
                case "smallint": return "short";
                case "smallmoney": return "decimal";
                case "sql_variant": return "object";
                case "text": return "string";
                case "time": return "TimeSpan";
                case "timestamp": return "byte[]";

                case "uniqueidentifier": return "Guid";
                case "varbinary": return "byte[]";
                case "varchar": return "string";
                case "xml": return "string";
                case "datetime": return "DateTime";
                case "datetime2": return "DateTime";
                case "datetimeoffset": return "DateTimeOffset";

                case "structured": return tvpClassName;

                default: return "object";
            }
        }

        public static string GetCSharpNullableFriendlyType(string sqlType, string tvpClassName)
        {
            switch ((sqlType ?? string.Empty).ToLower())
            {
                case "bigint": return "long?";
                case "binary": return "byte[]";
                case "bit": return "bool?";
                case "char": return "string";
                case "date": return "DateTime?";
                case "filestream": return "byte[]";
                case "image": return "byte[]";
                case "tinyint": return "byte?";
                case "int": return "int?";
                case "float": return "double?";
                case "decimal": return "decimal?";
                case "money": return "decimal?";
                case "nchar": return "string";
                case "ntext": return "string";
                case "numeric": return "decimal?";
                case "nvarchar": return "string";
                case "real": return "Single?";
                case "rowversion": return "byte[]";
                case "smalldatetime": return "DateTime?";
                case "smallint": return "short?";
                case "smallmoney": return "decimal?";
                case "sql_variant": return "object";
                case "text": return "string";
                case "time": return "TimeSpan?";
                case "timestamp": return "byte[]";

                case "uniqueidentifier": return "Guid?";
                case "varbinary": return "byte[]";
                case "varchar": return "string";
                case "xml": return "string";
                case "datetime": return "DateTime?";
                case "datetime2": return "DateTime?";
                case "datetimeoffset": return "DateTimeOffset?";

                case "structured": return tvpClassName;

                default: return "object";
            }
        }

        public static string GetJavascriptFriendlyType(string sqlType, string tvpClassName)
        {
            switch ((sqlType ?? string.Empty).ToLower())
            {
                case "bigint": return "number";
                case "binary": return "Uint8Array";
                case "bit": return "boolean";
                case "char": return "string";
                case "date": return "protobuf_net_bcl_pb.DateTime";
                case "filestream": return "Uint8Array";
                case "image": return "Uint8Array";
                case "tinyint": return "number";
                case "int": return "number";
                case "float": return "number";
                case "decimal": return "number";
                case "money": return "number";
                case "nchar": return "string";
                case "ntext": return "string";
                case "numeric": return "number";
                case "nvarchar": return "string";
                case "real": return "number";
                case "rowversion": return "Uint8Array";
                case "smalldatetime": return "protobuf_net_bcl_pb.DateTime";
                case "smallint": return "number";
                case "smallmoney": return "number";
                case "sql_variant": return "Uint8Array";
                case "text": return "string";
                case "time": return "protobuf_net_bcl_pb.TimeSpan";
                case "timestamp": return "Uint8Array";

                case "uniqueidentifier": return "protobuf_net_bcl_pb.Guid";
                case "varbinary": return "Uint8Array";
                case "varchar": return "string";
                case "xml": return "string";
                case "datetime": return "protobuf_net_bcl_pb.DateTime";
                case "datetime2": return "protobuf_net_bcl_pb.DateTime";
                case "datetimeoffset": return "number";

                case "structured": return tvpClassName;

                default: return "Uint8Array";
            }
        }

        public static bool IsDatetime(string sqlType)
        {
            switch ((sqlType ?? string.Empty).ToLower())
            {

                case "smalldatetime": 
                case "datetime": 
                case "datetime2": 
                    return true;
                default: return false;
            }
        }

        public static string GetSqlDbType(string type)
        {
            switch (type.ToLower())
            {
                case "bigint": return "BigInt";
                case "binary": return "VarBinary";
                case "char": return "Char";
                case "filestream": return "VarBinary";
                case "float": return "Float";
                case "geometry":
                case "hierarchyid":
                case "geography":
                    return "Udt";
                case "image": return "Image";
                case "bit": return "Bit";
                case "tinyint": return "TinyInt";
                case "int": return "Int";
                case "money": return "Money";
                case "decimal": return "Decimal";
                case "nchar": return "NChar";
                case "ntext": return "NText";
                case "numeric": return "Decimal";
                case "nvarchar": return "NVarChar";
                case "real": return "Real";
                case "rowversion": return "Timestamp";
                case "smalldatetime": return "SmallDateTime";
                case "smallint": return "SmallInt";
                case "smallmoney": return "SmallMoney";
                case "sql_variant": return "Variant";
                case "text": return "Text";
                case "time": return "Time";
                case "timestamp": return "Timestamp";
                case "uniqueidentifier": return "UniqueIdentifier";
                case "varbinary": return "VarBinary";
                case "varchar": return "VarChar";
                case "xml": return "Xml";

                case "date": return "Date";
                case "datetime": return "DateTime";
                case "datetime2": return "DateTime2";
                case "datetimeoffset": return "DateTimeOffset";
                default:
                    return "Structured";
            }
        }

        public static string GenerateDataRowReadValue(string sqlType, string x)
        {
            switch ((sqlType ?? string.Empty).ToLower())
            {
                case "bit": return $@"dr.GetBoolean({x})";
                case "tinyint": return $@"dr.GetByte({x})";
                case "smallint": return $@"dr.GetInt16({x})";
                case "int": return $@"dr.GetInt32({x})";
                case "smallmoney": return $@"dr.GetDecimal({x})";
                case "money": return $@"dr.GetSqlMoney({x})";
                case "float": return $@"dr.GetFloat({x})";
                case "numeric":
                case "decimal": return $@"dr.GetDecimal({x})";
                case "bigint": return $@"dr.GetInt64({x})";
                case "uniqueidentifier": return $@"dr.GetGuid({x})";

                case "nvarchar":
                case "ntext":
                case "nchar":
                case "xml":
                case "varchar":
                case "text":
                case "char": return $@"dr.GetString({x})";

                case "timestamp":
                case "filestream":
                case "varbinary":
                case "binary": return $@"dr.GetBytes({x})";

                case "time": return $@"dr.GetTimeSpan({x})";
                case "smalldatetime":
                case "datetime":
                case "datetime2":
                case "date": return $@"dr.GetDateTime({x})";
                case "datetimeoffset": return $@"dr.GetDateTimeOffset({x})";
              

                default: return "not implemented";
            }
        }
    }
}