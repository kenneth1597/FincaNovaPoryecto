using FincaNovaGestionLotes.Web.Data;
using FincaNovaGestionLotes.Web.Services.Auditoria;
using Microsoft.EntityFrameworkCore;
using FincaNovaGestionLotes.Web.Services;
using FincaNovaGestionLotes.Web.Filters;
using FincaNovaGestionLotes.Web.Services.Labores;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<RequiereSesionFilter>();

    var mensajes = options.ModelBindingMessageProvider;
    mensajes.SetValueMustBeANumberAccessor(campo => $"{campo} debe ser un número.");
    mensajes.SetValueMustNotBeNullAccessor(valor => "Este campo es obligatorio.");
    mensajes.SetAttemptedValueIsInvalidAccessor((valor, campo) => $"El valor «{valor}» no es válido para {campo}.");
    mensajes.SetValueIsInvalidAccessor(valor => $"El valor «{valor}» no es válido.");
    mensajes.SetUnknownValueIsInvalidAccessor(campo => $"El valor indicado no es válido para {campo}.");
    mensajes.SetMissingBindRequiredValueAccessor(campo => $"Falta el valor de {campo}.");
    mensajes.SetMissingKeyOrValueAccessor(() => "Falta un valor obligatorio.");
    mensajes.SetNonPropertyAttemptedValueIsInvalidAccessor(valor => $"El valor «{valor}» no es válido.");
    mensajes.SetNonPropertyUnknownValueIsInvalidAccessor(() => "El valor indicado no es válido.");
    mensajes.SetNonPropertyValueMustBeANumberAccessor(() => "El campo debe ser un número.");
    mensajes.SetMissingRequestBodyRequiredValueAccessor(() => "Falta información obligatoria en la solicitud.");
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddTransient<IEmailService, SmtpEmailService>();

builder.Services.AddScoped<IAuditService, AuditService>();

builder.Services.Configure<ParametrosJornada>(builder.Configuration.GetSection("Jornada"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await DbSeeder.SeedAsync(scope.ServiceProvider);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
