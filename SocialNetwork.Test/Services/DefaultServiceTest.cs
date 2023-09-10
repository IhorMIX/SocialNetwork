using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SocialNetwork.DAL;
using SocialNetwork.DAL.Options;
using SocialNetwork.DAL.Services;
using SocialNetwork.Test.Extensions;
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
            services.AddSingleton(typeof(CacheService<>));
            services.Configure<CacheOptions>(Configuration.GetSection("CacheOptions"));
        }

        private void SetUpConfiguration()
        {
            Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"CacheOptions:CacheTime", "00:10:00"},
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