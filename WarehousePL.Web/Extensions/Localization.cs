using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using WarehouseBLL.Localization;

namespace WarehousePL.Web.Extensions
{
    public static class Localization
    {

        public static IServiceCollection AddJsonLocalization(this IServiceCollection Service)
        {
            Service.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();

            Service.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                        factory.Create(typeof(JsonStringLocalizerFactory));
                });

            Service.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("ar"),
                        //new CultureInfo("ar-EG"),
                        new CultureInfo("en")
                    };
                options.DefaultRequestCulture = new RequestCulture(culture: supportedCultures[0]);
                options.SupportedCultures = supportedCultures;
            });
            return Service;
        }

        public static RequestLocalizationOptions localizationOptions()
        {
            var supportedCultures = new[] { "ar", 
                //"ar-EG",
                "en" };
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures);
            return localizationOptions;
        }
    }
}
