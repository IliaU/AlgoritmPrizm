using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace AlgoritmPrizm.BLL
{
    /// <summary>
    /// объект для списка товаров
    /// </summary>
    public class JsonGetInventorySbsinventoryprices
    {
        public string link;
        public double margin_amt;
        public double margin_with_tax_amt;
        public double margin_percent;
        public double markup_percent;
        public double coefficient;
    }
}
