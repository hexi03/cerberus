using AutoMapper;
using cerberus.Attributes.Validation;
using cerberus.Models.edmx;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace cerberus.Models.ViewModels
{
    public class FactorySiteSupplyEditModel
    {
        [Required]
        [FactorySiteID]
        public int id { get; set; }
        //[WareHouseIDDict]
        public Dictionary<string, string> warehouse_ids { get; set; }
        public FactorySiteSupplyEditModel() { }

        public List<int> getWareHouseIDs()
        {
            return warehouse_ids.Values.Select(x => Convert.ToInt32(x)).ToList();
        }
        public class Mapper : ITypeConverter<FactorySite, FactorySiteSupplyEditModel>
        {
            public FactorySiteSupplyEditModel Convert(FactorySite source, FactorySiteSupplyEditModel destination, ResolutionContext context)
            {
                destination = new FactorySiteSupplyEditModel();
                destination.id = source.id;
                destination.warehouse_ids = source.FactorySiteWareHouseClaims.ToDictionary(e => e.warehouse_id.ToString(), e => e.warehouse_id.ToString());
                return destination;
            }
        }


    }
}