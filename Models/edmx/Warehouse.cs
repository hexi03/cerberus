
namespace cerberus.Models.edmx
{
    using System;
    using System.Collections.Generic;
    
    public partial class Warehouse
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Warehouse()
        {
            this.FactorySiteWareHouseClaims = new HashSet<FactorySiteWareHouseClaim>();
            this.GroupWareHouseClaims = new HashSet<GroupWareHouseClaim>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
        public int department_id { get; set; }
    
        public virtual Department Department { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FactorySiteWareHouseClaim> FactorySiteWareHouseClaims { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GroupWareHouseClaim> GroupWareHouseClaims { get; set; }
    }
}
