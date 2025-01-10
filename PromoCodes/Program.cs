using FluentValidation;
using PromoCodes.Hubs;
using PromoCodes.Models;
using PromoCodes.Utils;
using PromoCodes.Validators;
using ILogger = PromoCodes.Utils.ILogger;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("promocodeclient", builder =>
    {
        // builder.WithOrigins("http://localhost:5173")
        //     .AllowAnyHeader()
        //     .AllowAnyMethod()
        //     .AllowCredentials();
        
        builder.SetIsOriginAllowed(origin =>
                new Uri(origin).Host == "localhost")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddTransient<ILogger, ConsoleLogger>();
builder.Services.AddSingleton<IValidator<GenerateRequest>, GeneralRequestValidator>();
builder.Services.AddSingleton<IValidator<UseCodeRequest>, UseCodeRequestValidator>();

var app = builder.Build();


app.MapHub<DiscountHub>("/discount");

app.UseCors("promocodeclient");
app.Run();
