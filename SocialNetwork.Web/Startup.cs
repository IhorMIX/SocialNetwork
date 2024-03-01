using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json.Converters;
using SocialNetwork.BLL.Helpers;
using SocialNetwork.BLL.Services;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.BLL.Settings;
using SocialNetwork.DAL;
using SocialNetwork.DAL.Options;
using SocialNetwork.DAL.Repository;
using SocialNetwork.DAL.Repository.Interfaces;
using SocialNetwork.Web.Extensions;
using SocialNetwork.Web.Helpers;
using SocialNetwork.Web.Hubs;
using SocialNetwork.Web.Validators;
using Swashbuckle.AspNetCore.SwaggerUI;

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
        services.AddControllers().AddNewtonsoftJson(opt => 
            opt.SerializerSettings.Converters.Add(new StringEnumConverter()));

        //Options
        services.Configure<MailSettingsOptions>(Configuration.GetSection("MailSettings"));
        services.Configure<TemplatePatheOptions>(Configuration.GetSection("TemplatePathes"));
        services.Configure<LinkConfig>(Configuration.GetSection("LinkConfig"));
        services.Configure<CacheOptions>(Configuration.GetSection("CacheOptions"));
        services.Configure<RoleOption>(Configuration.GetSection("Roles"));
        services.Configure<HexKeyConfig>(Configuration.GetSection("HexKeyConfig"));
        services.Configure<ChunkSizeOfDisconnectedUsers>(Configuration.GetSection("ChunkSizeOfDisconnectedUsers"));
        
        //automapper
        services.AddAutoMapper(typeof(Startup));

        //inject dependency of FluentValidation
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssemblyContaining<UserValidator>();
        services.AddValidatorsFromAssemblyContaining<UserUpdateValidator>(); //added update validator in controller
        services.AddValidatorsFromAssemblyContaining<AuthorizeValidator>();
        services.AddValidatorsFromAssemblyContaining<ChatValidator>();
        services.AddValidatorsFromAssemblyContaining<ResetPasswordEmailValidator>();
        
        services.AddJwtAuth();

        services.AddSignalR();
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

        services.AddScoped<IMailService, MailService>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IMessageService, MessageService>();

        services.AddScoped<IBlackListService, BlackListService>();
        services.AddScoped<IBlackListRepository, BlackListRepository>();

        services.AddScoped<IReactionService, ReactionService>();
        services.AddScoped<IReactionRepository, ReactionRepository>();
        
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IMessageReadStatusRepository, MessageReadStatusRepository>();
        
        services.AddSingleton<IDbReadySignal, DbContextReadySignal>();
        services.AddSingleton<DelayedWriter>();
        services.AddSingleton<IUserInChatTracker, UserInChatTracker>();
            
        var connectionString = Environment.GetEnvironmentVariable("SQLSERVER_CONNECTION_STRING") ?? Configuration.GetConnectionString("ConnectionString");

        services.AddDbContext<SocialNetworkDbContext>(options =>
            options.UseSqlServer(connectionString));
        
        services.AddSwagger();
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
        app.UseStaticFiles();
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
            endpoints.MapHub<ChatHub>("/chatHub", options =>
            {
                options.Transports = HttpTransportType.LongPolling | HttpTransportType.WebSockets;
            });
            endpoints.MapHub<OnlineStatusHub>("/connection", options =>
            {
                options.Transports = HttpTransportType.LongPolling | HttpTransportType.WebSockets;
            });
            endpoints.MapHub<NotificationHub>("/notification", options =>
            {
                options.Transports = HttpTransportType.LongPolling | HttpTransportType.WebSockets;
            });
        });
        
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            c.RoutePrefix = string.Empty; 
            c.DocExpansion(DocExpansion.List); 
        });
    }
}
