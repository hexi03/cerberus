using cerberus.Attributes.Validation;
using System.Collections.Generic;

namespace cerberus.Models.ViewModels
{
    public class ProductionItemCreateModel
    {

        [ItemID]
        public int production_id { get; set; }
        [ItemIDStringList]
        public Dictionary<string, string> requirement_ids { get; set; } //ID -> Count


    }
}