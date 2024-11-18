using System;
using LearningRedis.Controllers;
using LearningRedis.Infrastructure.Caching;
using LearningRedis.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ToDoListDbContext>(o => o.UseInMemoryDatabase("ToDoListDb"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICachingService, CachingService>();
builder.Services.AddStackExchangeRedisCache(o => {
    o.InstanceName = "instance";
    o.Configuration = $"{Environment.GetEnvironmentVariable("REDIS_HOST") ?? "localhost"}:{Environment.GetEnvironmentVariable("REDIS_PORT") ?? "6379"}";
});
builder.WebHost.ConfigureKestrel(oprions =>
{
    oprions.ListenAnyIP(80);
    oprions.ListenAnyIP(443);
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.AddEndpoints();
app.UseHttpsRedirection();



app.Run();
