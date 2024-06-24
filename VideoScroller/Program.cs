using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Selenium.Helper;
using System.Xml.Linq;
using VideoScroller;

var scrapper = new SeasonvarScrapper();

scrapper.StartBrowser("http://seasonvar.ru/serial-4694--Mastera_mecha_onlajn.html");
await scrapper.Download($"F:\\SAO");

//using IHost host = Host.CreateDefaultBuilder(args).Build();
//IConfiguration config = host.Services.GetRequiredService<IConfiguration>();

//string login = config.GetValue<string>("Seasonvar:Login")!;
//string password = config.GetValue<string>("Seasonvar:Password")!;

//scrapper.StartBrowser();
//scrapper.Login(login, password);
//var keyLogger = new KeyLogger(scrapper);
//keyLogger.StartKeyLogger();

