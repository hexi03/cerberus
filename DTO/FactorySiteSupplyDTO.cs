using cerberus.Models.edmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cerberus.DTO
{
    public class FactorySiteSupplyDTO
    {
        public FactorySite factorySite{  get; set; }
        public List<Warehouse> warehouses { get; set; }
        public FactorySiteSupplyDTO() { }
    }
}