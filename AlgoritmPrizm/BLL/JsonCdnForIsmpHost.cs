using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace AlgoritmPrizm.BLL
{
    public class JsonCdnForIsmpHost
    {
        public string host;

        [JsonIgnore]
        public int? png;

        public JsonCdnForIsmpHost(string host)
        {
            this.host = host;
        }
    }
}
