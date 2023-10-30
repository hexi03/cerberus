﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;

namespace cerberus.Models.Reports
{
    public class MSalesReport : Report
    {
        public int warehouse_id;

        public Dictionary<int, int> items;

        public MSalesReport() : base(Types.WHRelease) { items = new Dictionary<int, int>(); }
    }
}