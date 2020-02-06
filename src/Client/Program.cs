using Blazor.Extensions;
using Blazor.Extensions.Logging;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using BlazorStyled;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Threading.Tasks;

namespace logR.Client
{
    public class Program
    {
        public async  static Task Main(string[] args)
        {
        var host =    CreateHostBuilder(args).Build();

            host.Services.UseBootstrapProviders();
            host.Services.UseFontAwesomeIcons();
            await host.RunAsync();
        }

        public static WebAssemblyHostBuilder CreateHostBuilder(string[] args)
        {
           var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services
            .AddBlazorise(options =>
            {
                options.ChangeTextOnKeyPress = true;
            })
            .AddBootstrapProviders()
            .AddFontAwesomeIcons();
            //builder.Services.AddTransient<IOptions<HttpConnectionOptions>>((svc) => Options.Create<HttpConnectionOptions>(new HttpConnectionOptions()
            //{

            //}));
            //builder.Services.AddTransient<HttpConnectionFactory>((svc) => new HttpConnectionFactory(Options.Create<HttpConnectionOptions>(new HttpConnectionOptions()), svc.GetService<ILoggerFactory>()));

            builder.Services.AddTransient<HubConnectionBuilder>();
            builder.Services.AddBlazorStyled();
            builder.Services.AddLogging(builder => builder
        .AddBrowserConsole() // Add Blazor.Extensions.Logging.BrowserConsoleLogger
        .SetMinimumLevel(LogLevel.Trace));

           
         
            builder.RootComponents.Add<App>("app");
            builder.RootComponents.Add<ClientSideStyled>("#styled");
            return builder;

        }
       
    }
}
