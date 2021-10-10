using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AlgoritmPrizm.BLL;

namespace AlgoritmPrizm.Lib
{
    public class EventSQLList : EventArgs
    {
        public ItemSQL nItemSql;

        public EventSQLList(ItemSQL nItemSql)
        {
            this.nItemSql = nItemSql;
        }
    }
}
