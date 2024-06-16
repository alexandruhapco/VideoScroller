using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Selenium.Helper;
using VideoScroller;

using IHost host = Host.CreateDefaultBuilder(args).Build();
IConfiguration config = host.Services.GetRequiredService<IConfiguration>();

string login = config.GetValue<string>("login");
string password = config.GetValue<string>("password");

var scrapper = new SeasonvarScrapper();


scrapper.StartBrowser();
scrapper.Download("http://seasonvar.ru/serial-38721-Patcany-4-season.html");

//scrapper.StartBrowser();
//scrapper.Login(login, password);

//var keyLogger = new KeyLogger(scrapper);
//keyLogger.StartKeyLogger();

