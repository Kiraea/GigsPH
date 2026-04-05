using System.Collections.Immutable;
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
using Amazon.S3;
using Amazon.S3.Model;
using GigPH.Features.Auth.CheckToken;
using GigPH.Features.Post.CreatePost;
using GigPH.Features.Post.GetMyPosts;
using GigPH.Features.Post.GetPublicPosts;
using GigPH.Features.User.Onboard;
using GigPH.Infrastructure.Config;
using Microsoft.Extensions.Options;
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

builder.Services.Configure<S3Options>(builder.Configuration.GetSection("S3"));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddSingleton<IAmazonS3>(o =>
{
    var opts = o.GetService<IOptions<S3Options>>()!.Value;
    var config = new AmazonS3Config()
    {
        ServiceURL = opts.ServiceUrl,
        ForcePathStyle = true,
        UseHttp = true,
    };
    return new AmazonS3Client(opts.AccessKey, opts.SecretKey, config);
});

builder.Services.AddScoped<IS3Service, S3Service>();

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


builder.Services.AddScoped <LoginHandler>();
builder.Services.AddScoped <RegisterHandler>();

builder.Services.AddScoped<OnboardHandler>();
builder.Services.AddScoped<CheckTokenHandler>();

builder.Services.AddScoped<CreatePostHandler>();
builder.Services.AddScoped<GetMyPostsHandler>();
builder.Services.AddScoped<GetPublicPostsHandler>();

builder.Services.AddScoped <GetMyProfileHandler>();
builder.Services.AddScoped<GetPublicProfileHandler>();



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

    using (var scoped = app.Services.CreateAsyncScope())
    {
        var context = scoped.ServiceProvider.GetRequiredService<IAmazonS3>();
        var bucketName = builder.Configuration["S3:BucketName"]!;

        var buckets = await context.ListBucketsAsync();
        var exists = buckets.Buckets?.Any(b => b.BucketName == bucketName) ?? false;
        if (!exists)
        {
            await context.PutBucketAsync(bucketName);
        }
        else
        {
            // 2. Ask AWS for the list of files currently in the bucket
            var listResponse = await context.ListObjectsV2Async(new ListObjectsV2Request 
            { 
                BucketName = bucketName 
            });

            // 3. Loop through and delete them one by one
            if (listResponse.S3Objects != null)
            {
                foreach (var s3Obj in listResponse.S3Objects)
                {
                    await context.DeleteObjectAsync(bucketName, s3Obj.Key);
                }    
            }
            
        }
        
        
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