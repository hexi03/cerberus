using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;

namespace cerberus.Models.Reports
{
    public class WHWorkShiftReplenishmentReport : Report
    {
        public int workshift_id;
        public int warehouse_id;

        public Dictionary<int, int> items;

        public WHWorkShiftReplenishmentReport() : base(Types.WHWorkShiftReplenishment) { items = new Dictionary<int, int>(); }

    }
}