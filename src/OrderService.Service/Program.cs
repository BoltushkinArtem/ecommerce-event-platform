using Messaging.Kafka.DependencyInjection;
using OrderService.Service.Endpoints;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

#region Host & Logging

builder.Host.UseSerilog((ctx, cfg) =>
    cfg.ReadFrom.Configuration(ctx.Configuration));

#endregion

#region Services

builder.Services.AddOpenApi();

builder.Services.AddKafkaCoreMessaging(builder.Configuration);
builder.Services.AddKafkaProducerMessaging(builder.Configuration);

#endregion

var app = builder.Build();

#region HTTP pipeline

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

#endregion

#region Endpoints

app.MapCreateOrder();

#endregion

app.Run();