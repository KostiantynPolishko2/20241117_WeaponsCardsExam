using ClientPageServer.PL;
using ClientPageServer.PL.EF;
using ClientPageServer.PL.Handlers;
using ClientPageServer.PL.Interfaces;
using ClientPageServer.PL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

// server weapon items
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<WeaponsItemsContext>(configure => configure.UseSqlServer(builder.Configuration.GetConnectionString("AzureSqlDb")));
builder.Services.AddScoped<IWeaponsItemRepository, WeaponsItemRepository>();

// Add service of HttpRequestMessage to the container.
// Add service of HttpRequestMessage to the container.
//builder.Services.AddTransient<HttpRequestMessage>(serviceProvider =>
//{
//    // get connection string to out server
//    string? url = builder.Configuration["Azure:AdminServer"];

//    if (string.IsNullOrEmpty(url)){
//        throw new ArgumentNullException(nameof(url));
//    }

//    // initialize and return the instance of HttpRequestMessage
//    var httpRequestMsg = new HttpRequestMessage(HttpMethod.Get, new Uri(url));
//    httpRequestMsg.Headers.Add("Accept", "application/json");

//    // inject instance in controller
//    return httpRequestMsg;
//});

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddControllers(options =>
{
    options.InputFormatters.Insert(0, MyJPIF.GetJsonPatchInputFormatter());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ClientPageServer.PL", Version = "v1" });
});

//add CORS policy for permission treat request from other protocols and ports
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<HandlerException>();

app.UseRouting();

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
