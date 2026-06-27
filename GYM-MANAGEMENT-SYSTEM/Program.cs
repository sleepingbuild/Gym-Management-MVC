using GYM_MANAGEMENT_SYSTEM.AI.Services;
using GYM_MANAGEMENT_SYSTEM.Data;
using GYM_MANAGEMENT_SYSTEM.Repositories;
using GYM_MANAGEMENT_SYSTEM.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<KnowledgeBaseService>();

builder.Services.AddScoped<DatasetImportService>();

builder.Services.AddScoped<IMembershipPackageRepository, MembershipPackageRepository>();
builder.Services.AddScoped<IMembershipPackageService, MembershipPackageService>();

builder.Services.AddScoped<IMembershipRepository, MembershipRepository>();
builder.Services.AddScoped<IMembershipService, MembershipService>();

builder.Services.AddScoped<IMembershipRenewalService, MembershipRenewalService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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
app.MapRazorPages();

app.Run();
