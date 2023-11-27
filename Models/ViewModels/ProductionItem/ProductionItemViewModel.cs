using System.Collections.Generic;

namespace cerberus.Models.ViewModels
{
    public class ProductionItemViewModel
    {
        public ItemViewModel production_item { get; set; }
        public Dictionary<ItemViewModel, int> requirement_items { get; set; }

    }
}