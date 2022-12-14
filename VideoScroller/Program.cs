using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SharpHook;

const string url = "http://seasonvar.ru";
const string video = "document.getElementsByTagName('video')[0]";
const string xPathPrev = "//*[@id='oframehtmlPlayer']/pjsdiv[7]/pjsdiv[3]";
const string xPathNext = "//*[@id='oframehtmlPlayer']/pjsdiv[8]/pjsdiv[3]";
const string xPathFullScreen = "//*[@id='oframehtmlPlayer']/pjsdiv[15]/pjsdiv[4]";
const string CurrentMomentClass = "svico-mwatch";

using IHost host = Host.CreateDefaultBuilder(args).Build();
IConfiguration config = host.Services.GetRequiredService<IConfiguration>();

string login = config.GetValue<string>("login");
string password = config.GetValue<string>("password");

ChromeDriver driver;

StartBrowser();
StartKeyLogger();

void StartKeyLogger() 
{
    var hook = new TaskPoolGlobalHook();

    hook.KeyTyped += (e, w) => ActionOnPress(w.RawEvent.Keyboard.RawCode);

    hook.Run();
}

void ActionOnPress(int keyCode) 
{
    switch(keyCode) 
    {
        case 106: ScrollVideo(5); break;        // - numpad /
        case 111: ScrollVideo(-5); break;       // - numpad *
        case 109: TogglePlayPause(); break;     // - numpad -
        case 104: Prev(); break;                // - numpad 8
        case 105: Next(); break;                // - numpad 9
        case 101: ToggleFullScreen(); break;    // - numpad 5
        case 102: SaveCurrentTime(); break;     // - numpad 5
        default: /*Console.WriteLine(keyCode);*/ break;
    };
}

void StartBrowser() 
{
    driver = new ChromeDriver {
        Url = $"{url}/?mod=login"
    };

    var loginFld = driver.FindElement(By.Name("login"));
    var passwordFld = driver.FindElement(By.Name("password"));

    loginFld.SendKeys(login);
    passwordFld.SendKeys(password);

    passwordFld.Submit();

    driver.Url = $"{url}/?mod=pause";
}

void ScrollVideo(int seconds) 
{
    ExecuteJS($"{video}.currentTime = {video}.currentTime + {seconds}");
}

void TogglePlayPause() 
{
    var isPaused = driver.FindElement(By.TagName("video")).GetDomProperty("paused") == "True";

    if (isPaused) 
    {
        ExecuteJS($"{video}.play()");
    } 
    else 
    {
        ExecuteJS($"{video}.pause()");
    }
}

void Next() 
{
    ClickNonClickableByXpath(xPathNext);
}

void Prev() 
{
    ClickNonClickableByXpath(xPathPrev);
}

void ToggleFullScreen() 
{
    ClickNonClickableByXpath(xPathFullScreen);
}

void SaveCurrentTime() 
{
    try 
    {
        driver.FindElement(By.ClassName(CurrentMomentClass)).Click();
    } 
    catch (Exception) 
    {
        Console.WriteLine($"{nameof(SaveCurrentTime)} failed");
    }
}

void ClickNonClickableByXpath(string xPath) 
{
    ExecuteJS($"document.evaluate(\"{xPath}\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
}

void ExecuteJS(string js) 
{
    try 
    {
        ((IJavaScriptExecutor)driver).ExecuteScript(js);
    } 
    catch (Exception) 
    {
        Console.WriteLine($"{nameof(ExecuteJS)} failed: {js}");
    }
}