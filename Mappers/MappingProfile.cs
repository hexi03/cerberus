using cerberus.Models.edmx;
using cerberus.Models.Reports;

namespace cerberus.Mappers
{
    using AutoMapper;
    using cerberus.Models;
    using cerberus.Models.ViewModels;
    using cerberus.Models.ViewModels.Reports;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Collections.Generic;
    using System.Linq;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Department, DepartmentEditModel>().ReverseMap();
            CreateMap<Department, DepartmentViewModel>().ReverseMap();
            CreateMap<Department, DepartmentCreateModel>().ReverseMap();
            CreateMap<Department, DepartmentRolesEditModel>().ConvertUsing(new DepartmentRolesEditModel.Mapper());
            //CreateMap<Department, DepartmentRolesViewModel>().ConvertUsing(new DepartmentRolesViewModel.Mapper());


            CreateMap<FactorySite, FactorySiteEditModel>().ReverseMap();
            CreateMap<FactorySite, FactorySiteViewModel>().ReverseMap();
            CreateMap<FactorySite, FactorySiteCreateModel>().ReverseMap();
            CreateMap<FactorySite, FactorySiteRolesEditModel>().ConvertUsing(new FactorySiteRolesEditModel.Mapper());
            //FactorySiteRolesViewModel.MapperConfig.Configure(CreateMap<FactorySite, FactorySiteRolesViewModel>());
            CreateMap<FactorySite, FactorySiteSupplyEditModel>().ConvertUsing(new FactorySiteSupplyEditModel.Mapper());
            //FactorySiteSupplyViewModel.MapperConfig.Configure(CreateMap<FactorySite, FactorySiteSupplyViewModel>());



            CreateMap<Warehouse, WarehouseEditModel>().ReverseMap();
            CreateMap<Warehouse, WarehouseViewModel>().ReverseMap();
            CreateMap<Warehouse, WarehouseCreateModel>().ReverseMap();
            CreateMap<Warehouse, WareHouseRolesEditModel>().ConvertUsing(new WareHouseRolesEditModel.Mapper());
            //WareHouseRolesViewModel.MapperConfig.Configure(CreateMap<Warehouse, WareHouseRolesViewModel>());

            CreateMap<FSSupplyRequirementReport, FSSupplyRequirementReportFormViewModel>().ConvertUsing(new FSSupplyRequirementReportFormViewModel.Mapper());
            CreateMap<FSWorkShiftReport, FSWorkShiftReportFormViewModel>().ConvertUsing(new FSWorkShiftReportFormViewModel.Mapper());
            CreateMap<WHInventarisationReport, WHInventarisationReportFormViewModel>().ConvertUsing(new WHInventarisationReportFormViewModel.Mapper());
            CreateMap<WHReleaseReport, WHReleaseReportFormViewModel>().ConvertUsing(new WHReleaseReportFormViewModel.Mapper());
            CreateMap<WHReplenishmentReport, WHReplenishmentReportFormViewModel>().ConvertUsing(new WHReplenishmentReportFormViewModel.Mapper());
            CreateMap<WHShipmentReport, WHShipmentReportFormViewModel>().ConvertUsing(new WHShipmentReportFormViewModel.Mapper());
            CreateMap<WHWorkShiftReplenishmentReport, WHWorkShiftReplenishmentReportFormViewModel>().ConvertUsing(new WHWorkShiftReplenishmentReportFormViewModel.Mapper());


            CreateMap<Report, ReportViewModel>().ReverseMap();



            CreateMap<ItemsRegistry, ItemViewModel>().ReverseMap();
            CreateMap<ItemsRegistry, ItemEditModel>().ReverseMap();
            CreateMap<ItemsRegistry, ItemCreateModel>().ReverseMap();

            /*
            CreateMap<ProductionRegistry, ProductionItemViewModel>().ReverseMap();
            CreateMap<ProductionRegistry, ProductionItemEditModel>().ReverseMap();
            CreateMap<ProductionRegistry, ProductionItemCreateModel>().ReverseMap();
*/
            CreateMap<IdentityRole, GroupViewModel>().ReverseMap();
            CreateMap<IdentityRole, GroupCreateModel>().ReverseMap();

/*
            CreateMap<IGrouping<ApplicationUser, IList<IdentityRole>>, ApplicationUserCreateModel>().ConstructUsing(new ApplicationUserCreateModel.Mapper());
            CreateMap<(ApplicationUser, IList<IdentityRole>), ApplicationUserEditModel>().ConstructUsing(new ApplicationUserEditModel.Mapper());

            CreateMap<IGrouping<ApplicationUser, IList<IdentityRole>>, ApplicationUserViewModel>().ConstructUsing(new ApplicationUserViewModel.Mapper());
*/


        }
    }
}