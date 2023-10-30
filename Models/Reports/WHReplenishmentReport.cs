using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;

namespace cerberus.Models.Reports
{
    public class WHReplenishmentReport : Report
    {
        public int warehouse_id;
        public int producements_id;

        public Dictionary<int, int> items;

        public WHReplenishmentReport() : base(Types.WHReplenishment) { items = new Dictionary<int, int>(); }


    }
}