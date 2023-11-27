using System.Linq;

namespace cerberus.Services
{
    using cerberus.Models.edmx;
    using System.Collections.Generic;

    public interface IItemsRegistryService
    {
        IList<ItemsRegistry> get_variants();
        IList<(ItemsRegistry, int)> get_list(IDictionary<int, int> items);
    }
    public class ItemsRegistryService : IItemsRegistryService
    {
        CerberusDBEntities _db;
        public ItemsRegistryService(CerberusDBEntities db)
        {
            _db = db;
        }
        public IList<ItemsRegistry> get_variants()
        {

            return _db.ItemsRegistries.ToList();

        }

        public IList<(ItemsRegistry, int)> get_list(IDictionary<int, int> items)
        {
            IList<(ItemsRegistry, int)> values = new List<(ItemsRegistry, int)>();
            var keys = items.Keys;
            foreach (var e in _db.ItemsRegistries.Where(it => keys.Contains(it.id)))
            {
                values.Add((e, items[e.id]));
            }
            return values;

        }
    }
}