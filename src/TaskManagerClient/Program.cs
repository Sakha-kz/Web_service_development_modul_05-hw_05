using TaskManagerClient.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Swagger configuration for Web API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure named HttpClient "TaskApi" using IHttpClientFactory
builder.Services.AddHttpClient("TaskApi", client =>
{
    var baseAddress = builder.Configuration["ApiSettings:BaseAddress"] ?? "http://localhost:5000/";
    client.BaseAddress = new Uri(baseAddress);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Register TaskApiService in DI Container
builder.Services.AddScoped<ITaskApiService, TaskApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tasks API v1");
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Tasks}/{action=Index}/{id?}");

app.MapControllers();

app.Run();
