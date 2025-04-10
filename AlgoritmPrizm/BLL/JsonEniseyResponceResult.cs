using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmPrizm.BLL
{
    public class JsonEniseyResponceResult
    {
        public string version;
        public Int64? reqTimestamp;
        public string reqId;
        public string inst;
        public string description;
        public List<JsonEniseyResponceResultCode> codes = new List<JsonEniseyResponceResultCode>();
        public int code;
    }
}
