using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PersonalWealth.Application.Events;
using PersonalWealth.Application.Tenancy;
using PersonalWealth.Infrastructure.Documents;
using PersonalWealth.Infrastructure.Persistence;
using PersonalWealth.Infrastructure.Persistence.Outbox;
using PersonalWealth.Worker;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<ITenantContext>(new TenantContext(Guid.NewGuid()));
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddDocumentServices(builder.Configuration);
builder.Services.AddOptions<OutboxPublisherOptions>()
    .Bind(builder.Configuration.GetSection(OutboxPublisherOptions.SectionName));
builder.Services.AddScoped<OutboxPublisher>();
builder.Services.AddSingleton<EventHandlerRegistry>();
builder.Services.AddScoped<IEventBus>(sp =>
    sp.GetRequiredService<EventHandlerRegistry>()
        .CreateEventBus(sp.GetRequiredService<IProcessedEventStore>()));
builder.Services.AddHostedService<OutboxPublisherWorker>();

IHost host = builder.Build();
host.Run();
