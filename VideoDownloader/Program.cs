using Selenium.Helper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

IConfiguration config = app.Services.GetRequiredService<IConfiguration>();
string login = config.GetValue<string>("Seasonvar:Login");
string password = config.GetValue<string>("Seasonvar:Password");

app.MapGet("/open", () =>
{
    var scrapper = new SeasonvarScraper();
    scrapper.StartBrowser();
    scrapper.Login(login, password);
})
.WithName("OpenSeasonvar")
.WithOpenApi();

app.Run();
