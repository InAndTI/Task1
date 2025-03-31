using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllers().AddXmlSerializerFormatters();
        builder.Services.AddEndpointsApiExplorer(); 
        builder.Services.AddSwaggerGen();
        builder.Services.AddHttpClient(); 
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.Use(async (context, next) =>
        {
            Console.WriteLine($"Incoming request: {context.Request.Method} {context.Request.Path}");
            await next();
        });

        app.MapControllers(); 
        app.Run();
    }
}