using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;

namespace cerberus.Models.Reports
{
    public class WHShipmentReport : Report
    {
        public int warehouse_id;
        public int sales_id;
        public Dictionary<int, int> items;

        public WHShipmentReport() : base(Types.WHShipment) { items = new Dictionary<int, int>(); }

    }
}