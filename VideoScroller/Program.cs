using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Selenium.Helper;
using SSHClient;

var scrapper = new SeasonvarScrapper();


var folderName = "Dexter Season 5";
var url = "http://seasonvar.ru/serial-1585--Dekster-_pstaszk-05-sezon.html";



using IHost host = Host.CreateDefaultBuilder(args).Build();
IConfiguration config = host.Services.GetRequiredService<IConfiguration>();

var plexConfig = config.GetSection("Plex");
var seasonvarConfig = config.GetSection("Seasonvar");
var foldersConfig = config.GetSection("Folders");

var localPath = Path.Combine(foldersConfig["LocalFolder"], folderName);
var myPlexPath = $"{foldersConfig["PlexFolder"]}/{folderName}";

//scrapper.StartBrowser(url);
//await scrapper.Download(localPath, seasonvarConfig["Dubs"]);

SshService.SendFolder(
    plexConfig["Host"],
    22,
    plexConfig["Username"],
    plexConfig["Password"],
    localPath,
    myPlexPath);


//string login = config.GetValue<string>("Seasonvar:Login")!;
//string password = config.GetValue<string>("Seasonvar:Password")!;

//scrapper.StartBrowser();
//scrapper.Login(login, password);
//var keyLogger = new KeyLogger(scrapper);
//keyLogger.StartKeyLogger();

