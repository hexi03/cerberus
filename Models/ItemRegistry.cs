using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cerberus.Models.edmx
{
    using System;
    using System.Collections.Generic;
    using System.EnterpriseServices;

    public partial class ItemsRegistry
    {
        public static IList<ItemsRegistry> get_variants()
        {
            using (var context = new CerberusDBEntities()) {
                return context.ItemsRegistries.ToList();
            }
        }

        public static IList<(ItemsRegistry, int)> get_list(CerberusDBEntities db, IDictionary<int, int> items) {
            IList<(ItemsRegistry, int)> values = new List<(ItemsRegistry, int)> ();
            var keys = items.Keys;
            foreach (var e in db.ItemsRegistries.Where(it => keys.Contains(it.id))) {
                values.Add((e, items[e.id]));
            }
            return values;

        } 
    }
}