using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using Microsoft.AspNetCore.Builder;

namespace StoryBooks.Language
{
    public static class LocalizationExtensions
    {
        public static IServiceCollection AddAppLocalization(this IServiceCollection services)
        {
            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resource";
            });

            services.Configure<RequestLocalizationOptions>(options =>
            {   
                var supportedCultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("fr"),
                    new CultureInfo("ar"),
                    new CultureInfo("es")
                };

                options.SetDefaultCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            return services;
        }
    }
}
