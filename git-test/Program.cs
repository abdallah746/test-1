using git_test.Data;
using Microsoft.EntityFrameworkCore;
using Nest;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("student_portal")));

builder.Services.AddSingleton<IElasticClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var settings = new ConnectionSettings(config["cloudId"], new Elasticsearch.Net.BasicAuthenticationCredentials(
        "elastic", config["password"])).DefaultIndex("git-test");

    return new ElasticClient(settings);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
