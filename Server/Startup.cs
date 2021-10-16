using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Server.Interfaces;
using Server.Services;

namespace Server
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<ICryptoService, CryptoService>();
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("stage1", "/stage1", new
                {
                    controller = "Crypto", action = "Stage1"
                });
                
                endpoints.MapControllerRoute("stage2", "/stage2", new
                {
                    controller = "Crypto", action = "Stage2"
                });
                
                endpoints.MapControllerRoute("stage3", "/stage3", new
                {
                    controller = "Crypto", action = "Stage3"
                });
            });
        }
    }
}