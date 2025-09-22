using Asp.Versioning;
using FullStackAssignment.Application.Mappers;
using FullStackAssignment.Bootstrapper.StartUpConfig;
using FullStackAssignment.Infrastructure.DbContexts;
using FullStackAssignment.Presentation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//services Adding
builder.Services.ConfigureServices(builder);

// Get the folder path from configuration (default fallback)
var productImagesFolder = builder.Configuration["ProductImagesFolder"] ??
                          Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FullStack Assignment API v1");
    });
}


app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(productImagesFolder),
    RequestPath = "/images/products"
});

app.UseHsts();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAngularClient");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
public partial class Program { }