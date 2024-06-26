using UnityEngine.UI;
using Utility;

[CanvasIndex(1)]
public class RemoteControlView : UIView<RemoteControlPresenter>, IRemoteControlView
{
    #region belt
    private Button _beltStartBtn;
    private Button _beltStopBtn;
    private RepeatButton _beltCwBtn;
    private RepeatButton _beltCcwBtn;
    #endregion
    #region scraper
    private Button _scraperStartBtn;
    private Button _scraperStopBtn;
    private RepeatButton _scraperUpBtn;
    private RepeatButton _scraperDownBtn;
    private RepeatButton _scraperCwBtn;
    private RepeatButton _scraperCcwBtn;
    #endregion
    public override void InitUIElements()
    {
        #region belt
        _beltStartBtn = RootObj.transform.FindComponent<Button>("BeltPnl/ConveyPnl/StartBtn");
        _beltStopBtn = RootObj.transform.FindComponent<Button>("BeltPnl/ConveyPnl/StopBtn");
        _beltCwBtn = RootObj.transform.FindComponent<RepeatButton>("BeltPnl/RotatePnl/CwBtn");
        _beltCcwBtn = RootObj.transform.FindComponent<RepeatButton>("BeltPnl/RotatePnl/CcwBtn");

        _beltStartBtn.onClick.AddListener(presenter.StartBelt);
        _beltStopBtn.onClick.AddListener(presenter.StopBelt);
        _beltCwBtn.onRepeat.AddListener(presenter.BeltRotateClockwise);
        _beltCcwBtn.onRepeat.AddListener(presenter.BeltRotateCounterclockwise);
        #endregion
        #region scraper
        _scraperStartBtn = RootObj.transform.FindComponent<Button>("ScraperPnl/ScraperPnl/StartBtn");
        _scraperStopBtn = RootObj.transform.FindComponent<Button>("ScraperPnl/ScraperPnl/StopBtn");
        _scraperUpBtn = RootObj.transform.FindComponent<RepeatButton>("ScraperPnl/PitchPnl/UpBtn");
        _scraperDownBtn = RootObj.transform.FindComponent<RepeatButton>("ScraperPnl/PitchPnl/DownBtn");
        _scraperCwBtn = RootObj.transform.FindComponent<RepeatButton>("ScraperPnl/RotatePnl/CwBtn");
        _scraperCcwBtn = RootObj.transform.FindComponent<RepeatButton>("ScraperPnl/RotatePnl/CcwBtn");

        _scraperStartBtn.onClick.AddListener(presenter.StartScraper);
        _scraperStopBtn.onClick.AddListener(presenter.StopScraper);
        _scraperUpBtn.onRepeat.AddListener(presenter.ScraperPitchUp);
        _scraperDownBtn.onRepeat.AddListener(presenter.ScraperPitchDown);
        _scraperCwBtn.onRepeat.AddListener(presenter.ScraperRotateClockwise);
        _scraperCcwBtn.onRepeat.AddListener(presenter.ScraperRotateCounterclockwise);
        #endregion
    }
}