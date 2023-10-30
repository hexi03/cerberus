using Microsoft.Owin.Security.Twitter.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;

namespace cerberus.Models.Reports
{
    public class FSWorkShiftReport : Report
    {
        public int factorysite_id;
        public int target_warehouse_id;
        public int workplan_id;

        public Dictionary<int, int> produced;
        public Dictionary<int, int> losses;
        public Dictionary<int, int> remains;

        public FSWorkShiftReport() : base(Types.FSWorkShift) {
            produced = new Dictionary<int, int>();
            losses = new Dictionary<int, int>();
            remains = new Dictionary<int, int>();
        }

    }
    
}