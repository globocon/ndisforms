using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ndisforms.Data;
using ndisforms.Data.Models;
using ndisforms.Data.Providers;
using ndisforms.Data.Services;


var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;
// Add services to the container.
builder.Services.AddRazorPages();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<NDISDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.Configure<EmailOptions>(Configuration.GetSection(EmailOptions.Email));
builder.Services.AddScoped<IViewDataService, ViewDataService>();
builder.Services.AddScoped<IIrDataProvider, IrDataProvider>();
builder.Services.AddScoped<IEmailAndNotificationService, EmailAndNotificationService>();
builder.Services.AddScoped<IEmailDataProvider, EmailDataProvider>();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapControllers();
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Change the default page
//app.MapGet("/", context =>
//{
//    context.Response.Redirect("/PdfForms/IncidentReportForm");
//    return Task.CompletedTask;
//});

app.Run();

