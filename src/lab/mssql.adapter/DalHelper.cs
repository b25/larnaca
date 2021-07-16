using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace mssql.adapter
{
    public partial interface IDalServiceN { }

    public static partial class DalHelper
    {
        static partial void ConfigureServicesImpl(IServiceCollection services, IConfiguration configuration);

        static partial void ConfigureImpl(IApplicationBuilder app, IWebHostEnvironment en);

        static partial void CreateProtoImpl();

        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            ConfigureServicesImpl(services, configuration);
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