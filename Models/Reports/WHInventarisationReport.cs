using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;

namespace cerberus.Models.Reports
{
    public class WHInventarisationReport : Report
    {
        public int warehouse_id;
        public int inventarisation_plan_id;

        public Dictionary<int, int> items;

        public WHInventarisationReport() : base(Types.WHInventarisation) { items = new Dictionary<int, int>(); }
    }
}