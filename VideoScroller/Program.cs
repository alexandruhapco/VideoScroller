using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Selenium.Helper;
using SSHClient;

var scrapper = new SeasonvarScrapper();

Console.WriteLine("Folder Name:");
var folderName = Console.ReadLine()!;
Console.WriteLine("Url:");
var url = Console.ReadLine()!;
Console.WriteLine("Number of episodes:");
var numberOfEpisodes = int.Parse(Console.ReadLine()!);


using IHost host = Host.CreateDefaultBuilder(args).Build();
IConfiguration config = host.Services.GetRequiredService<IConfiguration>();

var plexConfig = config.GetSection("Plex");
var seasonvarConfig = config.GetSection("Seasonvar");
var foldersConfig = config.GetSection("Folders");

var localPath = Path.Combine(foldersConfig["LocalFolder"]!, folderName!);
var myPlexPath = $"{foldersConfig["PlexFolder"]}/{folderName}";

scrapper.StartBrowser(url);
await scrapper.Download(localPath, seasonvarConfig["Dubs"]!, numberOfEpisodes);

SshService.SendFolder(
    plexConfig["Host"]!,
    22,
    plexConfig["Username"]!,
    plexConfig["Password"]!,
    localPath,
    myPlexPath);


//string login = config.GetValue<string>("Seasonvar:Login")!;
//string password = config.GetValue<string>("Seasonvar:Password")!;

//scrapper.StartBrowser();
//scrapper.Login(login, password);
//var keyLogger = new KeyLogger(scrapper);
//keyLogger.StartKeyLogger();

