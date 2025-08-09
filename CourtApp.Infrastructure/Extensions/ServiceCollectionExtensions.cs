using CourtApp.Application.Interfaces.CacheRepositories;
using CourtApp.Application.Interfaces.CacheRepositories.FormBuilder;
using CourtApp.Application.Interfaces.Contexts;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Application.Interfaces.Repositories.Accounting;
using CourtApp.Application.Interfaces.Repositories.FormBuilder;
using CourtApp.Infrastructure.CacheRepositories;
using CourtApp.Infrastructure.DbContexts;
using CourtApp.Infrastructure.Repositories;
using CourtApp.Infrastructure.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CourtApp.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPersistenceContexts(this IServiceCollection services, IConfiguration configuration)
        {
            
            services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            #region Repositories

            services.AddTransient(typeof(IRepositoryAsync<>), typeof(RepositoryAsync<>));

            services.AddTransient<ILogRepository, LogRepository>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddTransient<IBookTypeRepository, BookTypeRepository>();
            services.AddTransient<IBookTypeCacheRepository, BookTypeCacheRepository>();

            services.AddTransient<IBookMasterRepository, BookMasterRepository>();
            services.AddTransient<IBookMasterCacheRepository, BookMasterCacheRepository>();

            services.AddTransient<IPublicationCacheRepository, PublisherCacheRepository>();
            services.AddTransient<IPublicationRepository, PublisheRepository>();

            services.AddTransient<ICaseKindCacheRepository, CaseKindCacheRepository>();
            services.AddTransient<ICaseKindRepository, CaseKindRepository>();

            services.AddTransient<ICaseNatureCacheRepository, CaseNatureCacheRepository>();
            services.AddTransient<ICaseNatureRepository, CaseNatureRepository>();

            services.AddTransient<ISubjectCacheRepository, SubjectCacheRepository>();
            services.AddTransient<ISubjectRepository, SubjectRepository>();

            services.AddTransient<ICaseKindRepository, CaseKindRepository>();
            services.AddTransient<ICaseKindCacheRepository, CaseKindCacheRepository>();

            services.AddTransient<ICaseStageRepository, CaseStageRepository>();
            services.AddTransient<ICaseStageCacheRepository, CaseStageCacheRepository>();

            services.AddTransient<ITypeOfCasesRepository, TypeOfCasesRepository>();
            services.AddTransient<ITypeOfCasesCacheRepository, TypeOfCasesCacheRepository>();

            services.AddTransient<ICourtMasterCacheRepository, CourtMasterCacheRepository>();
            services.AddTransient<ICourtMasterRepository, CourtMasterRepository>();

            services.AddTransient<IStateMasterCacheRepository, StateMasterCacheRepository>();
            services.AddTransient<IStateMasterRepository, StateMasterRepository>();

            services.AddTransient<IDsitrictMasterCacheRepository, DistrictMasterCacheRepository>();
            services.AddTransient<IDistrictMasterRepository, DistrictMasterRepository>();

            services.AddTransient<ICourtFeeStructureCacheRepository, CourtFeeStructureCacheRepository>();
            services.AddTransient<ICourtFeeStructureRepository, CourtFeeStructureRepository>();

            services.AddTransient<IClientRepository, ClientRepository>();
            services.AddTransient<IClientCacheRepository, ClientCacheRepository>();

            services.AddTransient<ICourtTypeRepository, CourtTypeRepository>();
            services.AddTransient<ICourtTypeCacheRepository, CourtTypeCacheRepository>();

            services.AddTransient<IUserCaseRepository, UserCaseRepository>();
            services.AddTransient<IUserCaseCacheRepository, UserCaseCacheRepository>();


            services.AddTransient<IProceedingHeadRepository, ProceedingHeadRepository>();
            services.AddTransient<IProceedingSubHeadRepository, ProceedingSubHeadRepository>();
            services.AddTransient<IWorkMasterRepository, WorkMasterRepository>();
            services.AddTransient<IWorkMasterSubRepository, WorkMasterSubRepository>();
            services.AddTransient<ICaseManagmentRepository, CaseManagmentRepository>();
            services.AddTransient<ICaseTitleRepository, CaseTitleRepository>();

            services.AddTransient<ICourtDistrictRepository, CourtDistrictRepository>();
            services.AddTransient<ICourtDistrictCacheRepository, CourtDistrictCacheRepository>();

            services.AddTransient<ICourtComplexRepository, CourtComplexRepository>();
            services.AddTransient<ICourtComplexCacheRepository, CourtComplexCacheRepository>();

            services.AddTransient<ICourtBenchRepository, CourtBenchRepository>();
            services.AddTransient<ICaseAgainstRepository, CaseAgainstRepository>();
            services.AddTransient<ICaseProceedingRepository, CaseProceedingRepository>();
            services.AddTransient<ICaseWorkRepository, CaseWorkRepository>();
            services.AddTransient<IDOTypeCacheRepository, DOTypeCacheRepository>();
            services.AddTransient<IDOTypeRepository, DOTypeRepository>();
            services.AddTransient<ICaseDocsRepository, CaseDocsRepository>();

            services.AddTransient<ICadreMasterCacheRepository, CadreMasterCacheRepository>();
            services.AddTransient<ICadreMasterRepository, CadreMasterRepository>();

            #endregion Repositories

            #region First & Secound Title Services
            services.AddTransient<IFSTitleCacheRepository, FSTitleCacheRepository>();
            services.AddTransient<IFSTitleRepository, FSTitleRepository>();
            #endregion

            #region Lawyer Master Information
            services.AddTransient<ILawyerCacheRepository, LawyerCacheRepository>();
            services.AddTransient<ILawyerRepository, LawyerMasterRepository>();
            #endregion

            #region FormBuilder 
            services.AddTransient<IFormBuilderCacheRepository, FormBuilderCacheRepository>();
            services.AddTransient<IFormBuilderRepository, FormBuilderRepository>();

            services.AddTransient<ITemplateInfoCacheRepository, TemplateInfoCacheRepository>();
            services.AddTransient<ITemplateInfoRepository, TemplateInfoRepository>();

            services.AddTransient<IFormTempMappingRepository, FormTempMappingRepository>();
            #endregion

            #region Specility 
            services.AddTransient<ISpecilityCacheRepository, SpecilityCacheRepository>();
            services.AddTransient<ISpecilityRepository, SpecilityRepository>();
            #endregion

            #region Case Related 
            services.AddTransient<ICaseDraftingCacheRepository, CaseDraftingCacheRepository>();
            services.AddTransient<ICaseDraftingRepository, CaseDraftingRepository>();

            services.AddTransient<ICaseAssignedRepository, CaseAssignedRepository>();
            services.AddScoped<ICaseHelperRepository, CaseHelperRepository>();
            #endregion

            #region Language & Court Form Print
            services.AddTransient<ILanguageRepository, LanguageRepository>();
            services.AddTransient<ICourtFormTypeRepository, CourtFormTypeRepository>();
            #endregion

            #region Laywer Billing Detail
            services.AddTransient<IBillingDetailRepository, BillingDetailRepository>();
            #endregion

        }
    }
}