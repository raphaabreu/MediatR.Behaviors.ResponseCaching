using System;
using Newtonsoft.Json;

namespace MediatR.Behaviors.ResponseCaching
{
    public static class DefaultRequestHasher
    {
        public static string JsonMd5<T>(T request)
        {
            var requestJson = JsonConvert.SerializeObject(request);

            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var textData = System.Text.Encoding.UTF8.GetBytes(requestJson);
                var hash = md5.ComputeHash(textData);
                return new Guid(hash).ToString("N");
            }
        }
    }
}
