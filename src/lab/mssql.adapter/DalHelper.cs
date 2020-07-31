using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace mssql.adapter
{
    public static partial class DalHelper
    {
        static partial void ConfigureServicesImpl(IServiceCollection services);

        static partial void ConfigureImpl(IApplicationBuilder app, IWebHostEnvironment en);

        static partial void CreateProtoImpl();

        public static void ConfigureServices(IServiceCollection services)
        {
            ConfigureServicesImpl(services);
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ConfigureImpl(app,env);
        }

        public static void CreateProto()
        {
            CreateProtoImpl();
        }
    }
}