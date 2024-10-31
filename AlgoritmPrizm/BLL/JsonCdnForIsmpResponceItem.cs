using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using AlgoritmPrizm.Lib;
using AlgoritmPrizm.Com;

namespace AlgoritmPrizm.BLL
{
    public class JsonCdnForIsmpResponceItem
    {
        public string cis;
        public bool valid;
        public bool verified;
        public string message;
        public bool found;
        public bool realizable;
        public bool utilised;
        public bool isBlocked;
        public int errorCode;

        [JsonIgnore]
        public string requestMatrixCode;
    }
}
