using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace mssql.adapter
{
    public partial class DalHelper
    {
       static partial void CreateProtoImpl()
       {
            var generator = new ProtoBuf.Grpc.Reflection.SchemaGenerator(); // optional controls on here, we can add more add needed
            var schema = generator.GetSchema<IDalService>();
            var path = Path.Join(Directory.GetCurrentDirectory(), "proto", "service.proto");

             File.WriteAllText(path, schema);

             Console.WriteLine($"Proto definitions dumped to {path}");
       }
    }
}
