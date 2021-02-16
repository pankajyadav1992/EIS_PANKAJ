using EmployeeInformationSystem.Core.Contracts;
using EmployeeInformationSystem.Core.Models;
using EmployeeInformationSystem.DataAccess.SQL;
using EmployeeInformationSystem.WebUI.Controllers;
using EmployeeInformationSystem.WebUI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using Unity;
using Unity.Injection;

namespace EmployeeInformationSystem.WebUI
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below.
            // Make sure to add a Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your type's mappings here.
            // container.RegisterType<IProductRepository, ProductRepository>();
            container.RegisterType<IRepository<Department>, SQLRepository<Department>>();
            container.RegisterType<IRepository<Designation>, SQLRepository<Designation>>();
            container.RegisterType<IRepository<Discipline>, SQLRepository<Discipline>>();
            container.RegisterType<IRepository<EmployeeDetail>, SQLRepository<EmployeeDetail>>();
            container.RegisterType<IRepository<EmployeeAsHoD>, SQLRepository<EmployeeAsHoD>>();
            container.RegisterType<IRepository<HoD>, SQLRepository<HoD>>();
            container.RegisterType<IRepository<Level>, SQLRepository<Level>>();
            container.RegisterType<IRepository<Organisation>, SQLRepository<Organisation>>();
            container.RegisterType<IRepository<PayScale>, SQLRepository<PayScale>>();
            container.RegisterType<IRepository<Degree>, SQLRepository<Degree>>();
            container.RegisterType<IRepository<PastExperience>, SQLRepository<PastExperience>>();
            container.RegisterType<IRepository<PostingDetail>, SQLRepository<PostingDetail>>();
            container.RegisterType<IRepository<PromotionDetail>, SQLRepository<PromotionDetail>>();
            container.RegisterType<IRepository<QualificationDetail>, SQLRepository<QualificationDetail>>();
            container.RegisterType<IRepository<DependentDetail>, SQLRepository<DependentDetail>>();
            container.RegisterType<IRepository<TelephoneExtension>, SQLRepository<TelephoneExtension>>();
            container.RegisterType<IRepository<LeaveType>, SQLRepository<LeaveType>>();

            container.RegisterType<IRepository<LeaveMaster>, SQLRepository<LeaveMaster>>();
            container.RegisterType<IRepository<EmployeeLeaveBalance>, SQLRepository<EmployeeLeaveBalance>>();
            container.RegisterType<IRepository<EmployeeLeaveDetails>, SQLRepository<EmployeeLeaveDetails>>();

          }
    }
}