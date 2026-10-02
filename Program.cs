using TaskManagerClient.Services;

var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов MVC
builder.Services.AddControllersWithViews();

// Получение URL Web API из конфигурации
var apiBaseUrl = builder.Configuration["TaskApi:BaseUrl"] ?? "https://localhost:7000/";

// Регистрация Именованного HttpClient через IHttpClientFactory
builder.Services.AddHttpClient("TaskApiClient", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Регистрация сервиса Web API
builder.Services.AddScoped<ITaskApiService, TaskApiService>();

var app = builder.Build();

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
    pattern: "{controller=Tasks}/{action=Index}/{id?}");

app.Run();