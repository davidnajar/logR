using logR.Server.Extensions;
using logR.Server.Hubs;
using logR.Server.Services;
using logR.Server.Services.Gateways.UDP;
using logR.Server.Services.Interfaces;
using logR.Shared.Gateways.Files;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Hosted;
using System.Linq;
using System.Threading;

namespace logR.Server
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; set; }
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSignalR();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
            services.AddSingleton<CancellationTokenSource>(new CancellationTokenSource());
            
            services.AddTransient<LogPacketHandler>();
            services.AddTransient<ILogStorage, LogStorageService>();
            services.AddTransient<ILogBroadcaster, LogBroadcastService>(); 
            services.AddTransient<ILogProcessor, LogProcessorService>();
            services.AddTransient<LogFileReader, LogFileReader>();
            services.AddTransient<LogTcpReceiver, LogTcpReceiver>();
            services.AddSingleton<LogServerReceiver>();
            var embedded = new HostedMongoDbServer(dbPath: "./data", persistent: true);
            services.Configure<LogTcpSettings>(Configuration.GetSection("LogTcpClient"));
            services.Configure<FileGatewayConfiguration>(Configuration.GetSection("LogFileReader"));
            services.AddSingleton<HostedMongoDbServer>(embedded);
            services.AddTransient<MongoDB.Driver.IMongoClient>((svc) => svc.GetService<HostedMongoDbServer>()?.Client);

            services.AddSingleton<IServiceCollection>(services);


        }
        LogFileReader reader;
        LogTcpReceiver receiver;
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            app.UseResponseCompression();
            app.UseLogServerReceiver(app.ApplicationServices);
      //    reader=  app.ApplicationServices.GetRequiredService<LogFileReader>();
            receiver = app.ApplicationServices.GetRequiredService<LogTcpReceiver>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBlazorDebugging();
            }

            app.UseStaticFiles();
            app.UseClientSideBlazorFiles<Client.Program>();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapHub<LoggerHub>("/loggerHub");
                endpoints.MapFallbackToClientSideBlazor<Client.Program>("index.html");
            });
        }
    }
}
