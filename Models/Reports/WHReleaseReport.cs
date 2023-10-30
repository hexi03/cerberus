using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;

namespace cerberus.Models.Reports
{
    public class WHReleaseReport : Report
    {
        public int warehouse_id;
        public int supply_requirement_id;

        public Dictionary<int, int> items;

        public WHReleaseReport() : base(Types.WHRelease) { items = new Dictionary<int, int>(); }
    }
}