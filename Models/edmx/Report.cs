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
    
    public partial class Report
    {
        public int id { get; set; }
        public string report_type { get; set; }
        public string creator_id { get; set; }
        public string serialized { get; set; }
        public int department_id { get; set; }
        public System.DateTime timestamp { get; set; }
    
        public virtual Department Department { get; set; }
    }
}
