[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(cerberus.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(cerberus.App_Start.NinjectWebCommon), "Stop")]

namespace cerberus.App_Start
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using AutoMapper;
    using cerberus.Mappers;
    using cerberus.Models;
    using cerberus.Models.edmx;
    using cerberus.Models.Reports;
    using cerberus.Models.ViewModels.Reports;
    using cerberus.Models.ViewModels;
    using cerberus.Services;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Common;
    using Ninject.Web.Common.WebHost;
    using Ninject.Web.Mvc;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application.
        /// </summary>
        public static void Start() 
        {
            //DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            //DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();

            DependencyResolver.SetResolver(new NinjectDependencyResolver(kernel));
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {

            kernel.Bind<CerberusDBEntities>().To<CerberusDBEntities>();
            //kernel.Bind<IdentityDbContext>().To<IdentityDbContext>();

            kernel.Bind<ApplicationUserManager>().ToMethod(ctx => new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext())));
            kernel.Bind<RoleManager<IdentityRole>>().ToMethod(ctx => new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext())));

            kernel.Bind<IUserService>().To<UserService>();
            kernel.Bind<IGroupService>().To<GroupService>();

            kernel.Bind<IDepartmentAccessService>().To<DepartmentAccessService>();
            kernel.Bind<IFactorySiteAccessService>().To<FactorySiteAccessService>();
            kernel.Bind<IWareHouseAccessService>().To<WareHouseAccessService>();

            kernel.Bind<IFactorySiteSupplyManagementService>().To<FactorySiteSupplyManagementService>();



            kernel.Bind<IProductionRegistryService>().To<ProductionRegistryService>();

            kernel.Bind<IItemsRegistryService>().To<ItemsRegistryService>();

            kernel.Bind<IFactorySiteService>().To<FactorySiteService>();

            kernel.Bind<IWarehouseService>().To<WarehouseService>();



            kernel.Bind<IMapper>().ToMethod(ctx => new MapperConfiguration(cfg =>
            {
                //cfg.AddProfile(new MappingProfile());
                cfg.CreateMap<Department, DepartmentEditModel>().ReverseMap();
                cfg.CreateMap<Department, DepartmentViewModel>().ReverseMap();
                cfg.CreateMap<Department, DepartmentCreateModel>().ReverseMap();
                cfg.CreateMap<Department, DepartmentRolesEditModel>().ConvertUsing(new DepartmentRolesEditModel.Mapper());
                //CreateMap<Department, DepartmentRolesViewModel>().ConvertUsing(new DepartmentRolesViewModel.Mapper());


                cfg.CreateMap<FactorySite, FactorySiteEditModel>().ReverseMap();
                cfg.CreateMap<FactorySite, FactorySiteViewModel>().ReverseMap();
                cfg.CreateMap<FactorySite, FactorySiteCreateModel>().ReverseMap();
                cfg.CreateMap<FactorySite, FactorySiteRolesEditModel>().ConvertUsing(new FactorySiteRolesEditModel.Mapper());
                //FactorySiteRolesViewModel.MapperConfig.Configure(CreateMap<FactorySite, FactorySiteRolesViewModel>());
                cfg.CreateMap<FactorySite, FactorySiteSupplyEditModel>().ConvertUsing(new FactorySiteSupplyEditModel.Mapper());
                //FactorySiteSupplyViewModel.MapperConfig.Configure(CreateMap<FactorySite, FactorySiteSupplyViewModel>());



                cfg.CreateMap<Warehouse, WarehouseEditModel>().ReverseMap();
                cfg.CreateMap<Warehouse, WarehouseViewModel>().ReverseMap();
                cfg.CreateMap<Warehouse, WarehouseCreateModel>().ReverseMap();
                cfg.CreateMap<Warehouse, WareHouseRolesEditModel>().ConvertUsing(new WareHouseRolesEditModel.Mapper());
                //WareHouseRolesViewModel.MapperConfig.Configure(CreateMap<Warehouse, WareHouseRolesViewModel>());

                cfg.CreateMap<FSSupplyRequirementReport, FSSupplyRequirementReportFormViewModel>().ConvertUsing(new FSSupplyRequirementReportFormViewModel.Mapper());
                cfg.CreateMap<FSWorkShiftReport, FSWorkShiftReportFormViewModel>().ConvertUsing(new FSWorkShiftReportFormViewModel.Mapper());
                cfg.CreateMap<WHInventarisationReport, WHInventarisationReportFormViewModel>().ConvertUsing(new WHInventarisationReportFormViewModel.Mapper());
                cfg.CreateMap<WHReleaseReport, WHReleaseReportFormViewModel>().ConvertUsing(new WHReleaseReportFormViewModel.Mapper());
                cfg.CreateMap<WHReplenishmentReport, WHReplenishmentReportFormViewModel>().ConvertUsing(new WHReplenishmentReportFormViewModel.Mapper());
                cfg.CreateMap<WHShipmentReport, WHShipmentReportFormViewModel>().ConvertUsing(new WHShipmentReportFormViewModel.Mapper());
                cfg.CreateMap<WHWorkShiftReplenishmentReport, WHWorkShiftReplenishmentReportFormViewModel>().ConvertUsing(new WHWorkShiftReplenishmentReportFormViewModel.Mapper());


                cfg.CreateMap<Report, ReportViewModel>().ReverseMap();



                cfg.CreateMap<ItemsRegistry, ItemViewModel>().ReverseMap();
                cfg.CreateMap<ItemsRegistry, ItemEditModel>().ReverseMap();
                cfg.CreateMap<ItemsRegistry, ItemCreateModel>().ReverseMap();

                /*
                CreateMap<ProductionRegistry, ProductionItemViewModel>().ReverseMap();
                CreateMap<ProductionRegistry, ProductionItemEditModel>().ReverseMap();
                CreateMap<ProductionRegistry, ProductionItemCreateModel>().ReverseMap();
    */
                cfg.CreateMap<IdentityRole, GroupViewModel>().ReverseMap();
                cfg.CreateMap<IdentityRole, GroupCreateModel>().ReverseMap();


                cfg.CreateMap<ApplicationUser, ApplicationUserCreateModel>().ReverseMap();
                cfg.CreateMap<ApplicationUser, ApplicationUserEditModel>().ReverseMap();
                cfg.CreateMap<ApplicationUser, ApplicationUserViewModel>().ReverseMap();
            }).CreateMapper()).InSingletonScope();
    }


    }
}