using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrapeFinra
{
    static class common
    {
        public const string STATE_IDLE = "IDLE";
        public const string STATE_LINKSET = "LSET";
        public const string STATE_ITEM = "ITEM";
        public const string STATE_BASEAPI = "BAPI";
        public const string STATE_APIBANNER = "ABNR";
        public const string STATE_APITAX = "ATAX";
    }
}
