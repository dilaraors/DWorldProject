using System;
using System.Collections.Generic;

namespace DWorldProject.Models.IyziPay
{
    public class IyzipayResource
    {
        private static readonly string AUTHORIZATION = "Authorization";
        private static readonly string RANDOM_HEADER_NAME = "x-iyzi-rnd";
        private static readonly string CLIENT_VERSION_HEADER_NAME = "x-iyzi-client-version";
        private static readonly string IYZIWS_HEADER_NAME = "IYZWS ";
        private static readonly string COLON = ":";
        public static readonly string CLIENT_VERSION = "iyzipay-dotnet-2.1.38";

        public string Status { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorGroup { get; set; }
        public string Locale { get; set; }
        public long SystemTime { get; set; }
        public string ConversationId { get; set; }

        public IyzipayResource()
        {
        }

        protected static Dictionary<string, string> GetHttpHeaders(BaseRequest request, Options options)
        {
            string randomString = DateTime.Now.ToString("ddMMyyyyhhmmssffff");
            Dictionary<string, string> headers = new Dictionary<string, string>();

            headers.Add("Accept", "application/json");
            headers.Add(RANDOM_HEADER_NAME, randomString);
            headers.Add(CLIENT_VERSION_HEADER_NAME, CLIENT_VERSION);
            headers.Add(AUTHORIZATION, PrepareAuthorizationString(request, randomString, options));
            return headers;
        }

        private static string PrepareAuthorizationString(BaseRequest request, string randomString, Options options)
        {
            string hash = HashGenerator.GenerateHash(options.ApiKey, options.SecretKey, randomString, request);
            return IYZIWS_HEADER_NAME + options.ApiKey + COLON + hash;
        }
    }
}
