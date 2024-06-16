using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Selenium.Helper;

public class SeasonvarScrapper
{
    const string _url = "http://seasonvar.ru";
    const string _video = "document.getElementsByTagName('video')[0]";
    const string _xPathPrev = "//*[@id='oframehtmlPlayer']/pjsdiv[7]/pjsdiv[3]";
    const string _xPathNext = "//*[@id='oframehtmlPlayer']/pjsdiv[8]/pjsdiv[3]";
    const string _xPathFullScreen = "//*[@id='oframehtmlPlayer']/pjsdiv[15]/pjsdiv[4]";
    const string _currentMomentClass = "svico-mwatch";

    const string _playlistId = "htmlPlayer_playlist";

    public ChromeDriver Driver { get; set; }

    public async Task Download(string url)
    {
        Driver.Navigate().GoToUrl(url);
        TogglePlayPause();
        TogglePlayPause();
        var videoUrl = GetVideoTagSrc();

        await DownloadFile(videoUrl);
    }

    private async Task DownloadFile(string url)
    {
        var filePath = "qwe.mp4";
        using var client = new HttpClient();
        using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        var totalBytes = response.Content.Headers.ContentLength;
        using Stream contentStream = await response.Content.ReadAsStreamAsync(), fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);
        var totalRead = 0L;
        var buffer = new byte[8192];
        var isMoreToRead = true;

        do
        {
            var read = await contentStream.ReadAsync(buffer);
            if (read == 0)
            {
                isMoreToRead = false;
                Console.WriteLine("Download complete.");
                continue;
            }

            await fileStream.WriteAsync(buffer.AsMemory(0, read));

            totalRead += read;

            if (totalBytes.HasValue)
            {
                Console.WriteLine($"Progress: {totalRead}/{totalBytes} ({(totalRead * 1d / totalBytes.Value * 100):0.00}%)");
            }
            else
            {
                Console.WriteLine($"Progress: {totalRead} bytes downloaded");
            }
        }
        while (isMoreToRead);
    }

    public string GetVideoTagSrc()
    {
        return Driver.FindElement(By.TagName("video")).GetAttribute("src");
    }

    public void Download(string url, int episode)
    {

    }

    public void StartBrowser()
    {
        Driver = new ChromeDriver
        {
            Url = $"{_url}/?mod=login"
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
        }
        else
        {
            ExecuteJS($"{_video}.pause()");
        }
    }

    public void Next()
    {
        ClickNonClickableByXpath(_xPathNext);
    }

    public void Prev()
    {
        ClickNonClickableByXpath(_xPathPrev);
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