using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace WarehouseBLL.Localization
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        private readonly IDistributedCache _cache;
        public JsonStringLocalizer(IDistributedCache cache)
        {
            _cache = cache;
        }

        public LocalizedString this[string name]
        {
            get
            {
                var value = GetString(name);
                return new LocalizedString(name, value);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var actualValue = this[name];
                return !actualValue.ResourceNotFound
                    ? new LocalizedString(name, string.Format(actualValue.Value, arguments))
                    : actualValue;
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            throw new NotImplementedException();
        }

        private string GetString(string key)
        {
            //var filePath = $"Resources/{Thread.CurrentThread.CurrentCulture.Name}.json";
            var filePath = $"./wwwroot/assets/json/locales/{Thread.CurrentThread.CurrentCulture.Name}.json";
            var fullFilePath = Path.GetFullPath(filePath);

            if (File.Exists(fullFilePath))
            {
                var cacheKey = $"locale_{Thread.CurrentThread.CurrentCulture.Name}_{key}";
                var cacheValue = _cache.GetString(cacheKey);

                if (!string.IsNullOrEmpty(cacheValue))
                    return cacheValue;

                //  var result = GetOrInsertValueInJSON(key, fullFilePath);
                var result = GetValueFromJSON(key, fullFilePath);

                if (!string.IsNullOrEmpty(result))
                    _cache.SetString(cacheKey, result);

                return result;
            }

            return string.Empty;
        }

        private string GetValueFromJSON(string propertyName, string filePath)
        {
            if (string.IsNullOrEmpty(propertyName) || string.IsNullOrEmpty(filePath))
                return string.Empty;

            JObject jsonObject;
            using (FileStream stream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (StreamReader streamReader = new(stream))
            using (JsonTextReader reader = new(streamReader))
            {
                jsonObject = JObject.Load(reader);
            }

            // Traverse the JSON to find the requested property
            var propertyPath = propertyName.Split('.');
            JToken currentToken = jsonObject;

            foreach (var part in propertyPath)
            {
                if (currentToken[part] == null)
                {
                    return string.Empty; // Key not found
                }

                currentToken = currentToken[part];
            }

            return currentToken?.ToString() ?? string.Empty; // Return the found value or empty string

        }
    }
}
