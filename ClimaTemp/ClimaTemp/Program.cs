var builder = WebApplication.CreateBuilder(args);

// Adicione os serviços ao contêiner.
builder.Services.AddControllersWithViews();

// ---> ADICIONE ESTA LINHA AQUI <---
builder.Services.AddHttpClient();

var app = builder.Build();

// Configurações do pipeline de requisição HTTP (já deve estar no seu ficheiro)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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