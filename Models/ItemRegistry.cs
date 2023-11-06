using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cerberus.Models.edmx
{
    using System;
    using System.Collections.Generic;

    public partial class ItemsRegistry
    {
        public static IList<ItemsRegistry> get_variants()
        {
            using (var context = new CerberusDBEntities()) {
                return context.ItemsRegistries.ToList();
            }
        }
    }
}