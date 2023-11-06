using cerberus.Models.edmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cerberus.DTO
{
    public class ProductionRegistryItem
    {
        public ItemsRegistry production_item { get; set; }
        public Dictionary<ItemsRegistry, int> requirement_items { get; set; }
        public int production_id { get; set; }
        public Dictionary<string, string> requirement_ids { get; set; }
    }
}