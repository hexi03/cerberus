//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace cerberus.Models.edmx
{
    using System;
    using System.Collections.Generic;
    
    public partial class FactorySiteWareHouseClaim
    {
        public int id { get; set; }
        public int factorysite_id { get; set; }
        public int warehouse_id { get; set; }
    
        public virtual FactorySite FactorySite { get; set; }
        public virtual Warehouse Warehouse { get; set; }
    }
}
