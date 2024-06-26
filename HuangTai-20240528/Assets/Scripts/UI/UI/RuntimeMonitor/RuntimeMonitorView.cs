using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

[CanvasIndex(1)]
public class RuntimeMonitorView : UIView<RuntimeMonitorPresenter>, IRuntimeMonitorView
{
	private TMP_Text _titleTxt;

	private Slider _progressSlider;
	private TMP_Text _progressTxt;

	private ScrollRect _dataScroll;

	private Button _pauseBtn;
	private Button _stopBtn;
	public override void InitUIElements()
	{
		_titleTxt = RootObj.transform.FindComponent<TMP_Text>("MonitorPnl/TitleTxt");
		_progressSlider = RootObj.transform.FindComponent<Slider>("MonitorPnl/ProgressPnl/ProgressSlider");
		_progressTxt = RootObj.transform.FindComponent<TMP_Text>("MonitorPnl/ProgressPnl/ProgressTxt");
		_dataScroll = RootObj.transform.FindComponent<ScrollRect>("MonitorPnl/DataPnl/DataScroll");
		_pauseBtn = RootObj.transform.FindComponent<Button>("MonitorPnl/ButtonPnl/PauseBtn");
		_stopBtn = RootObj.transform.FindComponent<Button>("MonitorPnl/ButtonPnl/StopBtn");

		_pauseBtn.onClick.AddListener(presenter.PauseTask);
		_stopBtn.onClick.AddListener(presenter.StopTask);
	}

    public void UpdateProgress(float progress)
    {
		_progressSlider.value = progress;
		_progressTxt.text = string.Format("{0:D}%", progress * 100);
    }
    public void UpdateData(RuntimeDataBase data)
    {

    }
}