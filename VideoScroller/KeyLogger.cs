using Selenium.Helper;
using SharpHook;

namespace VideoScroller;
public class KeyLogger
{
    private readonly SeasonvarScraper _scraper;

    public KeyLogger(SeasonvarScraper scraper)
    {
        _scraper = scraper;
    }

    public void StartKeyLogger()
    {
        var hook = new TaskPoolGlobalHook();

        hook.KeyTyped += (e, w) => ActionOnPress(w.RawEvent.Keyboard.RawCode);

        hook.Run();
    }

    public void ActionOnPress(int keyCode)
    {
        switch (keyCode)
        {
            case 106: _scraper.ScrollVideo(5); break;        // - numpad /
            case 111: _scraper.ScrollVideo(-5); break;       // - numpad *
            case 109: _scraper.TogglePlayPause(); break;     // - numpad -
            case 104: _scraper.Prev(); break;                // - numpad 8
            case 105: _scraper.Next(); break;                // - numpad 9
            case 101: _scraper.ToggleFullScreen(); break;    // - numpad 5
            case 102: _scraper.SaveCurrentTime(); break;     // - numpad 5
            default: /*Console.WriteLine(keyCode);*/ break;
        };
    }
}

