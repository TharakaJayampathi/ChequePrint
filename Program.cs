using ChequePrint.Interfaces.ChequePrint;
using ChequePrint.Interfaces.ChequePrintReport;
using ChequePrint.Repository.ChequePrint;
using ChequePrint.Repository.ChequePrintReport;
using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllersWithViews();
// Register your repository
builder.Services.AddScoped<IChequePrintRepository, ChequePrintRepository>();
builder.Services.AddScoped<IChequePrintReportRepository, ChequePrintReportRepository>();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Index}/{id?}")
    .WithStaticAssets();
app.Run();