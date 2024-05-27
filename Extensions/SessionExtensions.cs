//Liam Knipe St10355049
// REFRENCES: https://www.newtonsoft.com/json/help/html/serializingjson.htm

using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace ST10355049.Extensions
{
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}


