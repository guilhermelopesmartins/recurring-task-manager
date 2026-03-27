using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RecurringTaskManager.Application.Service;
using RecurringTaskManager.Infrastructure.Persistence;
using RecurringTaskManager.Application.Service;
using Hangfire;
using Hangfire.PostgreSql;
using Scalar.AspNetCore;
using RecurringTaskManager.Worker;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddHangfireServer();

builder.Services.AddScoped<IRecurringTaskRepository, RecurringTaskRepository>();
builder.Services.AddScoped<IRecurringTaskService, RecurringTaskService>();
builder.Services.AddScoped<RecurringTaskWorker>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Recurring Task API";

        options.OpenApiRoutePattern = "/swagger/{documentName}/swagger.json";
    });
    app.UseHangfireDashboard();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();