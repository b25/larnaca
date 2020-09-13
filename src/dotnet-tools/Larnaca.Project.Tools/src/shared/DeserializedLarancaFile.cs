using mssql.collector.types;
using System;
using System.IO;

namespace Larnaca.Project.Tools
{
    internal class DeserializedLarancaFile
    {
        public DatabaseMeta DatabaseMeta { get; set; }

        public static OperationResult<DeserializedLarancaFile> Load(string path)
        {
            try
            {
                var contents = File.ReadAllText(path);
                var dbMeta = Newtonsoft.Json.JsonConvert.DeserializeObject<DatabaseMeta>(contents);

                var deserializedFile = new DeserializedLarancaFile()
                {
                    DatabaseMeta = dbMeta
                };

                return deserializedFile.ToOperationResult();
            }
            catch (Exception ex)
            {
                return new OperationResult<DeserializedLarancaFile>(ex.ToString());
            }
        }
    }
}