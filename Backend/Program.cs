using System.Text;
using System.Text.Json.Serialization;
using Amazon;
using Amazon.S3;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PrjA.Domain.Entities;
using PrjA.Features.Auth.CheckToken;
using PrjA.Features.Auth.Login;
using PrjA.Features.Auth.Register;
using PrjA.Features.Bands.LeaveBand;
using PrjA.Features.Bands.SendInvitation;
using PrjA.Features.Bands.UpdateBandMemberRole;
using PrjA.Features.Genres;
using PrjA.Features.Instruments;
using PrjA.Features.Posts.CreatePost;
using PrjA.Features.Posts.DeletePost;
using PrjA.Features.Posts.UpdatePost;
using PrjA.Features.Posts.GetPublicPosts;
using PrjA.Features.Posts.SetLikeStatusPost;
using PrjA.Features.Posts.GetPost;
using PrjA.Features.Uploads;
using PrjA.Features.User.GetPublicProfile;
using PrjA.Features.User.Onboarding;
using PrjA.Features.User.UpdateProfile;
using PrjA.Infrastructure;
using PrjA.Infrastructure.Email;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(
        namingPolicy:null, allowIntegerValues:false));
    // naming policy mean that it wont follow the camelcase like facebook it will still be Facebook
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Configuration.AddJsonFile("appsettings.Custom.json", false, true);



builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.Configure<AWSOptions>(builder.Configuration.GetSection("AWS"));


// amazozn doesnt recommend putting the creds on the return amazonclientS3 should be in ~/.aws.credentials
builder.Services.AddSingleton<IAmazonS3>(sp =>
{

    if (string.IsNullOrEmpty(builder.Configuration["AWS:ServiceUrl"]) ||
        string.IsNullOrEmpty(builder.Configuration["AWS:Region"]))
    {
        throw new Exception("invalid AWS creds");
    }

    var s3Config = new AmazonS3Config()
    {
        ServiceURL = builder.Configuration["AWS:ServiceUrl"],
        AuthenticationRegion = builder.Configuration["AWS:Region"],
        ForcePathStyle = true,
        // force path style isbasically http://localhost:4566/(bucket)/(objectkey)
        // if not then http://(bucket).(localhost):/4566/(objectkey)
    };
    return new AmazonS3Client(s3Config);
});
builder.Services.AddSingleton<S3MediaStorage>(sp =>
{
    var s3Client = sp.GetRequiredService<IAmazonS3>();
    
    var bucketName =  builder.Configuration["S3:BucketName"];
    if (string.IsNullOrWhiteSpace(bucketName))
    {
        throw new Exception("bucket name is empty");
    }
    return new S3MediaStorage(s3Client, bucketName);
});

// This is for wrapper since we are making a Service




builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
    
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters.RequireAudience = true;
    options.TokenValidationParameters.ValidAudience = builder.Configuration["Jwt:Audience"];
    options.TokenValidationParameters.ValidIssuer = builder.Configuration["Jwt:Issuer"];
    options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!));

    options.Events = new JwtBearerEvents()
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["access_token"];
            return Task.CompletedTask;
        }
    };
});
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddIdentityCore<AppUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
    }).AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders().AddSignInManager();

builder.Services.AddScoped<LoginHandler>();
builder.Services.AddScoped<RegisterHandler>();
builder.Services.AddScoped<CheckTokenHandler>();
builder.Services.AddScoped<UpdateProfileHandler>();
builder.Services.AddScoped<GetGenresHandler>();
builder.Services.AddScoped<GetInstrumentsHandler>();

builder.Services.AddScoped<GetPublicProfileHandler>();

builder.Services.AddScoped<EmailSender>();
builder.Services.AddScoped<OnboardingHandler>();
builder.Services.AddScoped<SendInvitationHandler>();
builder.Services.AddScoped<LeaveBandHandler>();
builder.Services.AddScoped<UpdateBandMemberRoleHandler>();
builder.Services.AddScoped<CreatePostHandler>();
builder.Services.AddScoped<DeletePostHandler>();
builder.Services.AddScoped<UpdatePostHandler>();
builder.Services.AddScoped<GetPublicPostsHandler>();
builder.Services.AddScoped<SetLikeStatusPostHandler>();
builder.Services.AddScoped<GetPostHandler>();
builder.Services.AddScoped<UploadHandler>();



const string FrontendCors = "Frontend";

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCors, policy =>
    {
        policy.WithOrigins(allowedOrigins!).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{

    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var canConnect = await db.Database.CanConnectAsync();
    Console.WriteLine(canConnect);
}



using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await SeedDb.SeedGenresAsync(db);
    await SeedDb.SeedInstrumentsAsync(db);
    await SeedDb.SeedRolesAsync(roleManager);
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();


app.UseCors(FrontendCors);
app.UseAuthorization();

app.MapControllers();

app.Run();
