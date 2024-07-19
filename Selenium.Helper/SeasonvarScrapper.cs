using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Net;

namespace Selenium.Helper;

public class SeasonvarScrapper
{
    const string _url = "http://seasonvar.ru";
    const string _video = "document.getElementsByTagName('video')[0]";
    const string _xPathPrev = "//*[@id='oframehtmlPlayer']/pjsdiv[7]/pjsdiv[3]";
    const string _xPathPrevCheck = "//*[@id='oframehtmlPlayer']/pjsdiv[7]";
    const string _xPathNext = "//*[@id='oframehtmlPlayer']/pjsdiv[8]/pjsdiv[3]";
    const string _xPathNextCheck = "//*[@id='oframehtmlPlayer']/pjsdiv[8]";
    const string _xPathFullScreen = "//*[@id='oframehtmlPlayer']/pjsdiv[15]/pjsdiv[4]";
    const string _currentMomentClass = "svico-mwatch";

    const string _playlistId = "htmlPlayer_playlist";

    public ChromeDriver Driver { get; set; }

    public async Task Download(string path, string dubs = "", int count = 0)
    {
        var i = 0;

        var urls = new HashSet<string>();

        SwitchDub(dubs);

        do
        {
            var videoUrl = GetVideoTagSrc();
            urls.Add(videoUrl);
            Console.WriteLine($"Url scrapped: {videoUrl}");

            i++;
            if (count != 0 && i >= count)
            {
                break;
            }
        } while (Next());

        var lastVideoUrl = GetVideoTagSrc();
        urls.Add(lastVideoUrl);
        Console.WriteLine($"Url scrapped: {lastVideoUrl}");

        Driver.Close();

        var options = new ParallelOptions()
        {
            MaxDegreeOfParallelism = 12
        };

        await Parallel.ForEachAsync(urls, options, async (videoUrl, ct) =>
        {
            var name = videoUrl.Split("/").Last();
            await DownloadFile(videoUrl, $"{path}\\{name}");
        });
    }

    private void SwitchDub(string dubs)
    {
        if (!string.IsNullOrEmpty(dubs))
        {
            Console.WriteLine($"Dub set started: {dubs}");
            var videoUrl = GetVideoTagSrc();
            SelectDub(dubs);
            var dubVideoUrl = GetVideoTagSrc();
            while (videoUrl == dubVideoUrl)
            {
                Thread.Sleep(500);
                dubVideoUrl = GetVideoTagSrc();
            }
            Console.WriteLine($"Dub set done: {dubs}");
        }
    }

    private void SelectDub(string dub)
    {
        var xPath = $"//ul[@class='pgs-trans']/li[text()='{dub}']";
        var dubItem = Driver.FindElement(By.XPath(xPath));
        dubItem.Click();
    }

    private async Task DownloadFile(string url, string path)
    {
        using var client = new WebClient();
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        Console.WriteLine($"Download started: {path}");
        client.DownloadFile(url, path);
        Console.WriteLine($"Download finished: {path}");
    }

    public string GetVideoTagSrc()
    {
        var videoTag = "";
        try
        {
            videoTag = Driver.FindElement(By.TagName("video")).GetAttribute("src");
        }
        catch (Exception)
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
            IWebElement videoElement = wait.Until(x => x.FindElement(By.TagName("video")));
            videoTag = Driver.FindElement(By.TagName("video")).GetAttribute("src");
        }

        return videoTag;
    }

    public void StartBrowser(string url = null)
    {
        Driver = new ChromeDriver
        {
            Url = url ?? $"{_url}/?mod=login"
        };
    }

    public void Login(string login, string password)
    {
        var loginFld = Driver.FindElement(By.Name("login"));
        var passwordFld = Driver.FindElement(By.Name("password"));

        loginFld.SendKeys(login);
        passwordFld.SendKeys(password);

        passwordFld.Submit();

        Driver.Url = $"{_url}/?mod=pause";
    }

    public void ScrollVideo(int seconds)
    {
        ExecuteJS($"{_video}.currentTime = {_video}.currentTime + {seconds}");
    }

    public void TogglePlayPause()
    {
        var isPaused = Driver.FindElement(By.TagName("video")).GetDomProperty("paused") == "True";

        if (isPaused)
        {
            ExecuteJS($"{_video}.play()");
        } else
        {
            ExecuteJS($"{_video}.pause()");
        }
    }

    public bool Next()
    {
        ClickNonClickableByXpath(_xPathNext);
        var next = Driver.FindElement(By.XPath(_xPathNextCheck)).GetCssValue("opacity");
        return next == "1";
    }

    public bool Prev()
    {
        ClickNonClickableByXpath(_xPathPrev);
        var prev = Driver.FindElement(By.XPath(_xPathPrevCheck)).GetCssValue("opacity");
        return prev == "1";
    }

    public void ToggleFullScreen()
    {
        ClickNonClickableByXpath(_xPathFullScreen);
    }

    public void SaveCurrentTime()
    {
        try
        {
            Driver.FindElement(By.ClassName(_currentMomentClass)).Click();
        }
        catch (Exception)
        {
            Console.WriteLine($"{nameof(SaveCurrentTime)} failed");
        }
    }

    public void ClickNonClickableByXpath(string xPath)
    {
        ExecuteJS($"document.evaluate(\"{xPath}\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
    }

    public void ExecuteJS(string js)
    {
        try
        {
            ((IJavaScriptExecutor)Driver).ExecuteScript(js);
        }
        catch (Exception)
        {
            Console.WriteLine($"{nameof(ExecuteJS)} failed: {js}");
        }
    }
}