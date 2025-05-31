using AzureOpenAI_Task2.Data;
using AzureOpenAI_Task2.Services;
using Microsoft.EntityFrameworkCore;
using AzureOpenAI_Task2.UOW;

namespace AzureOpenAI_Task2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpClient<IDocumentIntelligenceService, DocumentIntelligenceService>();
            builder.Services.AddScoped<IDocumentService, DocumentService>();
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddHttpClient<IOpenAIService, OpenAIService>();

            builder.Services.AddDbContext<OpenAIServiceDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("AzureOpenAIServiceDbContext"));
            });
            var app = builder.Build();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.MapControllers();

            app.Run();
        }
    }
}
