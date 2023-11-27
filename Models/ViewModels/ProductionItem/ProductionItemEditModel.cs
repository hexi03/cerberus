using cerberus.Attributes.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace cerberus.Models.ViewModels
{
    public class ProductionItemEditModel
    {
        public ItemViewModel production_item { get; set; }
        public Dictionary<ItemViewModel, int> requirement_items { get; set; }

        [Required]
        [ItemID]
        public int production_id { get; set; }

        [Required]
        [ItemIDStringList]
        public Dictionary<string, string> requirement_ids { get; set; } //ID -> Count

    }
}