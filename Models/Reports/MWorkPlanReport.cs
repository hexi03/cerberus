using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;

namespace cerberus.Models.Reports
{
    public class MWorkPlanReport : Report
    {
        public int factorysite_id;

        public Dictionary<int, int> items;

        public MWorkPlanReport() : base(Types.WHRelease) { items = new Dictionary<int, int>(); }
    }
}