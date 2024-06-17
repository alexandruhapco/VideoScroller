using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Selenium.Helper;
using VideoScroller;

var scrapper = new SeasonvarScrapper();

scrapper.StartBrowser("http://seasonvar.ru/serial-36176-Devochka-holodil_nik_pslpeub.html");
await scrapper.Download();

//using IHost host = Host.CreateDefaultBuilder(args).Build();
//IConfiguration config = host.Services.GetRequiredService<IConfiguration>();

//string login = config.GetValue<string>("Seasonvar:Login")!;
//string password = config.GetValue<string>("Seasonvar:Password")!;

//scrapper.StartBrowser();
//scrapper.Login(login, password);
//var keyLogger = new KeyLogger(scrapper);
//keyLogger.StartKeyLogger();

