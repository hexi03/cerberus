//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace cerberus.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class FactorySite
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FactorySite()
        {
            this.FactorySiteWareHouseClaims = new HashSet<FactorySiteWareHouseClaim>();
            this.GroupFactorySiteClaims = new HashSet<GroupFactorySiteClaim>();
        }
    
        public int? id { get; set; }
        public string name { get; set; }
        public Nullable<int> department_id { get; set; }
    
        public virtual Department Department { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FactorySiteWareHouseClaim> FactorySiteWareHouseClaims { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GroupFactorySiteClaim> GroupFactorySiteClaims { get; set; }
    }
}
