using MongoDB.Driver;
using Planner;
using Planner.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var cs = builder.Configuration.GetSection("MongoDbConnection").Value;
var mc = new MongoClient(cs);
builder.Services.AddScoped<IMongoClient>(sp => mc);
builder.Services.AddTransient<IPlanner, Planner.Planner>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

