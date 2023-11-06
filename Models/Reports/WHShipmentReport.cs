using cerberus.Models.edmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;

namespace cerberus.Models.Reports
{
    public class WHShipmentReport : Report
    {
        public int warehouse_id { get; set; }
        public Dictionary<string, string> items { get; set; }

        public WHShipmentReport() : base(Types.WHShipment) { items = new Dictionary<string, string>(); }

        public Report to_generic()
        {
            serialized = JsonSerializer.Serialize(this);
            return new Report(this);
        }

    }
}