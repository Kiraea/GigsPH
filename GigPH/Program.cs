using System.Text;
using System.Text.Json.Nodes;
using FluentValidation;
using GigPH.Domain;
using GigPH.Features.Auth.Login;
using GigPH.Features.Auth.Register;
using GigPH.Features.User.GetProfileById;
using GigPH.Infrastructure;
using MicroElements.AspNetCore.OpenApi.FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text.Json.Nodes;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    options.AddFluentValidationRules();
    options.AddSchemaTransformer((schema, context, token) =>
    {
        if (context.JsonTypeInfo.Type == typeof(RegisterRequest))
        {
            if (schema.Properties["email"] is OpenApiSchema emailSchema)
                emailSchema.Default = JsonValue.Create("test@test.com");

            if (schema.Properties["password"] is OpenApiSchema passwordSchema)
                passwordSchema.Default = JsonValue.Create("Abc123!");
        }
        
        if (context.JsonTypeInfo.Type == typeof(LoginRequest))
        {
            if (schema.Properties["usernameOrEmail"] is OpenApiSchema emailSchema)
                emailSchema.Default = JsonValue.Create("test@test.com");

            if (schema.Properties["password"] is OpenApiSchema passwordSchema)
                passwordSchema.Default = JsonValue.Create("Abc123!");
        }

        return Task.CompletedTask;
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policyBuilder =>
    {
        policyBuilder.WithOrigins([builder.Configuration["AllowedOrigins"]!])
            .AllowAnyHeader()
            .AllowCredentials()
            .AllowAnyMethod();
    });
});



builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbConnection")));



builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddControllers();
builder.Services.AddIdentityCore<AppUser>(options =>
        options.User.RequireUniqueEmail = true)
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddSignInManager();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters.ValidIssuer = builder.Configuration["Jwt:Issuer"];
    options.TokenValidationParameters.ValidAudience = builder.Configuration["Jwt:Audience"];
    options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SigningKey"]!));
    options.Events = new JwtBearerEvents()
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["AccessToken"];
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddFluentValidationRulesToOpenApi();

builder.Services.AddAuthorization();


builder.Services.AddScoped<GetPublicProfileHandler>();
builder.Services.AddScoped <LoginHandler>();
builder.Services.AddScoped <RegisterHandler>();
builder.Services.AddScoped <GetMyProfileHandler>();



builder.Configuration.AddJsonFile("appsettingsHidden.json");


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using (var scoped = app.Services.CreateAsyncScope())
    {
        var context = scoped.ServiceProvider.GetRequiredService<AppDbContext>();
        var something = context.Database.ExecuteSqlRaw("DROP SCHEMA public CASCADE; CREATE SCHEMA public");

        context.Database.Migrate();
    }

    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);

        options
            .AddPreferredSecuritySchemes("Bearer")
            .AddHttpAuthentication(
                "Bearer",
                auth =>
                {
                    auth.Token = "";
                }
            )
            .EnablePersistentAuthentication();
    });
}




using (var scoped = app.Services.CreateAsyncScope())
{
    var roleManager = scoped.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    List<string> roles = new List<string> { "User", "Admin", "Other" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }
    }
}
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}


app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();