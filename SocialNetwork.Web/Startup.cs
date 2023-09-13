using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SocialNetwork.BL.Services;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.DAL;
using SocialNetwork.DAL.Options;
using SocialNetwork.DAL.Repository;
using SocialNetwork.DAL.Repository.Interfaces;
using SocialNetwork.DAL.Services;
using SocialNetwork.Web.Extensions;
using SocialNetwork.Web.Helpers;
using SocialNetwork.Web.Options;
using SocialNetwork.Web.Validators;

namespace SocialNetwork.Web;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    //service to adding dependency injection 
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers().AddNewtonsoftJson();
        
        //automapper
        services.AddAutoMapper(typeof(Startup));

        //inject dependency of FluentValidation
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssemblyContaining<UserValidator>();
        services.AddValidatorsFromAssemblyContaining<UserUpdateValidator>(); //added update validator in controller
        services.AddValidatorsFromAssemblyContaining<AuthorizeValidator>();
        services.AddValidatorsFromAssemblyContaining<ChatValidator>();
        services.AddSingleton(typeof(CacheService<>));
        services.Configure<CacheOptions>(Configuration.GetSection("CacheOptions"));
        services.Configure<RoleOption>(Configuration.GetSection("Roles"));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,

                    ValidIssuer = AuthOption.AuthOptions.ISSUER,
                    
                    ValidateAudience = true,

                    ValidAudience = AuthOption.AuthOptions.AUDIENCE,

                    ValidateLifetime = true,

      
                    IssuerSigningKey = AuthOption.AuthOptions.GetSymmetricSecurityKey(),
                    ValidateIssuerSigningKey = true,
                };
            });

        services.AddSingleton<TokenHelper>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        
        services.AddScoped<IFriendshipRepository, FriendshipRepository>();
        services.AddScoped<IFriendshipService, FriendshipService>();
        
        services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();
        services.AddScoped<IFriendRequestService, FriendRequestService>();
        
        services.AddScoped<IChatService,ChatService>();
        services.AddScoped<IChatRepository,ChatRepository>();
        services.AddScoped<IRoleRepository,RoleRepository>();
        services.AddScoped<IChatMemberRepository,ChatMemberRepository>();
        
        var connectionString = Environment.GetEnvironmentVariable("SQLSERVER_CONNECTION_STRING") ?? Configuration.GetConnectionString("ConnectionString");

        services.AddDbContext<SocialNetworkDbContext>(options =>
            options.UseSqlServer(connectionString));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        //check current project configuration
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }
        app.UseMiddleware<ErrorHandlingMiddleware>(); 

        app.UseRouting();
        app.UseCors(b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
        });
    }
}
