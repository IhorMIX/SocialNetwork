using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.DAL;
using SocialNetwork.Test.Extensions;

namespace SocialNetwork.Test.Services
{
    public abstract class DefaultServiceTest<TService> where TService : class
    {
        protected DbContextOptionsBuilder<SocialNetworkDbContext> _optionsBuilder;
        protected SocialNetworkDbContext _context;
        protected IServiceProvider ServiceProvider;
        protected IServiceCollection ServiceCollection;

        protected virtual bool ResetBeforeInvokingService => false;

        public virtual TService Service
        {
            get
            {
                if (ResetBeforeInvokingService)
                {
                    DetachContext();
                }

                return ServiceProvider.GetRequiredService<TService>();
            }
        }

        protected void DetachContext()
        {
            _context.DetachAllEntities();
        }

        protected virtual void SetUpAdditionalDependencies(IServiceCollection services)
        {
            services.AddScoped<TService>();
        }

        [SetUp]
        public virtual void SetUp()
        {
            _optionsBuilder = new DbContextOptionsBuilder<SocialNetworkDbContext>();
            _optionsBuilder.UseInMemoryDatabase("TestSocialNetworkDB");
            _context = new SocialNetworkDbContext(_optionsBuilder.Options);

            ServiceCollection = new ServiceCollection();
            ServiceCollection.AddDbContext<SocialNetworkDbContext>(options =>
                options.UseInMemoryDatabase("TestSocialNetworkDB"));

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
        public override TService Service
        {
            get
            {
                if (ResetBeforeInvokingService)
                {
                    DetachContext();
                }

                return ServiceProvider.GetRequiredService<TService>();
            }
        }

        protected override void SetUpAdditionalDependencies(IServiceCollection services)
        {
            services.AddScoped<TServiceInterface, TService>();
            base.SetUpAdditionalDependencies(services);
        }
    }
}