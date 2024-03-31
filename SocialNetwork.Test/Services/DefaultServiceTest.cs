using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.BLL.Settings;
using SocialNetwork.DAL;
using SocialNetwork.DAL.Options;
using SocialNetwork.DAL.Repository.Interfaces;
using SocialNetwork.DAL.Repository;
using SocialNetwork.Web;

namespace SocialNetwork.Test.Services
{
    public abstract class DefaultServiceTest<TService> where TService : class
    {
        protected IServiceProvider ServiceProvider;
        protected IServiceCollection ServiceCollection;

        public virtual TService Service => ServiceProvider.GetRequiredService<TService>();

        public IConfiguration Configuration;
        
        protected virtual void SetUpAdditionalDependencies(IServiceCollection services)
        {
            services.AddScoped<TService>();
            services.AddScoped<ILogger<TService>, NullLogger<TService>>();
            services.AddAutoMapper(typeof(Startup));
            services.Configure<CacheOptions>(Configuration.GetSection("CacheOptions"));
            services.Configure<RoleOption>(Configuration.GetSection("Roles"));
            services.Configure<HexKeyConfig>(Configuration.GetSection("HexKeyConfig"));
            services.Configure<LinkConfig>(Configuration.GetSection("HexKeyConfig"));
            services.AddScoped<IMailService, FakeMailService>();

            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IGroupMemberRepository, GroupMemberRepository>();
            services.AddScoped<IRoleGroupRepository, RoleGroupRepository>();
            services.AddScoped<IBannedUserListRepository, BannedUserListRepository>();
        }

        private void SetUpConfiguration()
        {
            Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"CacheOptions:CacheTime", "00:10:00"},
                    {"Roles:RoleAdminId", "1"},
                    {"Roles:RoleP2PAdminId", "2"},
                    {"HexKeyConfig:Key", "5caa56bd57b99d03e5ed256a0efdcec6348f447b5d5429bcc56980956e57c252"},
                    {"HexKeyConfig:Iv", "19eee43699e956394d904bb88e91f58b"},
                    {"LinkConfig:FrontUrl", "http//localhost:8080"}
                }!)
                .Build();
        }
        
        
        [SetUp]
        public virtual void SetUp()
        {
            ServiceCollection = new ServiceCollection();
            SetUpConfiguration();
            ServiceCollection.AddDbContext<SocialNetworkDbContext>(options =>
                options.UseInMemoryDatabase("TestSocialNetworkDB"));
            ServiceCollection.AddLogging();
            
            SetUpAdditionalDependencies(ServiceCollection);
            ServiceCollection.AddScoped<SocialNetworkDbContext>();

            var rootServiceProvider = ServiceCollection.BuildServiceProvider(new ServiceProviderOptions()
                { ValidateOnBuild = true, ValidateScopes = true });

            var spScope = rootServiceProvider.CreateScope();
            ServiceProvider = spScope.ServiceProvider;
        }
    }

    public abstract class DefaultServiceTest<TServiceInterface, TService> : DefaultServiceTest<TService>
        where TService : class, TServiceInterface where TServiceInterface : class
    {
        public override TService Service => ServiceProvider.GetRequiredService<TService>();

        protected override void SetUpAdditionalDependencies(IServiceCollection services)
        {
            services.AddScoped<TServiceInterface, TService>();
            base.SetUpAdditionalDependencies(services);
        }
    }
}