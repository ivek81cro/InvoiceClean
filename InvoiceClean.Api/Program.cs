using FluentValidation;
using InvoiceClean.Application.Common.Behaviors;
using InvoiceClean.Application.Invoices;
using InvoiceClean.Infrastructure.Invoices;
using InvoiceClean.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// EF
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("db") ?? "Data Source=invoices.db"));

// Ports
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();

// MediatR + validation pipeline
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(InvoiceClean.Application.Invoices.IInvoiceRepository).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(InvoiceClean.Application.Invoices.IInvoiceRepository).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ResultValidationBehavior<,>));

// ProblemDetails (minimal)
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler(exceptionApp =>
{
    exceptionApp.Run(async context =>
    {
        var feature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        var ex = feature?.Error;

        if (ex is ValidationException vex)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new ValidationProblemDetails(
                vex.Errors.GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
            ));
            return;
        }

        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Title = "Unexpected error",
            Detail = ex?.Message
        });
    });
});

if (app.Environment.IsDevelopment())
{

}

app.MapControllers();
app.Run();
