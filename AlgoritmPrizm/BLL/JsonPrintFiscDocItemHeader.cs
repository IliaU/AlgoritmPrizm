using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace AlgoritmPrizm.BLL
{
    public class JsonPrintFiscDocItemHeader
    {
        public string accept;
        [JsonProperty("accept-charset")]
        public string AcceptCharset;
        [JsonProperty("access-control-allow-headers")]
        public string AccessControlAllowHeaders;
        [JsonProperty("access-control-allow-methods")]
        public string AccessControlAllowMethods;
        [JsonProperty("access-control-allow-origin")]
        public string AccessControlAllowOrigin;
        [JsonProperty("access-control-expose-headers")]
        public string AccessControlExposeHeaders;
        public string allow;
        [JsonProperty("auto-delete")]
        public string AutoDelete;
        [JsonProperty("cache-control")]
        public string CacheControl;
        public string connection;
        [JsonProperty("content-length")]
        public string ContentLength;
        [JsonProperty("content-type")]
        public string ContentType;
        public string contentrange;
        public DateTime date;
        public string expiration;
        [JsonProperty("http-status-code")]
        public string HttpStatusCode;
        [JsonProperty("keep-alive")]
        public string KeepAlive;
        public string reqduration;
        public string server;
        public string tid;
        [JsonProperty("x-powered-by")]
        public string XPoweredBy;
        [JsonProperty("x-remote-server")]
        public string XRemoteServer;
        [JsonProperty("x-ua-compatible")]
        public string XUaCompatible;
    }
}
