using AspNetCoreRateLimit;
using Microsoft.EntityFrameworkCore;
using Task.BusinessLogic;
using Task.DBContext;
using Task.Interfaces;

var builder = WebApplication.CreateBuilder(args);

string connectionStrings = builder.Configuration.GetValue<string>("ConnectionStrings:AmazonSQL")!;

builder.Services.AddDbContext<DogsContext>(options =>
                options.UseSqlServer(connectionStrings));
builder.Services.AddControllers();

//IpRateLimiting
//builder.Services.AddMemoryCache();

builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));

// Add services to the container.

builder.Services.AddTransient<IDogsActionsBL, DogsActionsBL>();
builder.Services.AddTransient<ILoggerBL, LoggerBL>();
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
builder.Services.AddInMemoryRateLimiting();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseIpRateLimiting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

