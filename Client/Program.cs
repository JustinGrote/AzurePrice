using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;

namespace Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            // Allow the baseaddress to be overriden to the API path in the wwwroot appsettings, in this case Azure Functions
            // https://swimburger.net/blog/dotnet/how-to-deploy-aspnet-blazor-webassembly-to-azure-static-web-apps#integrating-blazor-with-azure-functions
            var baseAddress = builder.Configuration["BaseAddress"] ?? builder.HostEnvironment.BaseAddress;
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddress) });
            
            // Enable Blazorise
            builder.Services
                .AddBlazorise( options =>
                {
                    options.ChangeTextOnKeyPress = true;
                } )
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();
                
            await builder.Build().RunAsync();
        }
    }
}
