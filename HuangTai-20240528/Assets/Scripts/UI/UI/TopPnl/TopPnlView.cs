using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utility;

[CanvasIndex(8)]
public class TopPnlView : UIView<TopPnlPresenter>, ITopPnlView
{
	private Button _controlBtn;
	private Button _superviseBtn;
	private Button _alarmBtn;
	private Button _userBtn;
    private TMP_Text _userTxt;
    private Transform _userPnl;
    private Button _settingBtn;
	private Button _logoutBtn;
    private GameObject StatePanel;
    public GameObject WorkScreenPanel;

    public override void InitUIElements()
	{
		_controlBtn = RootObj.transform.FindComponent<Button>("BG/Control");
        _superviseBtn = RootObj.transform.FindComponent<Button>("BG/Supervise");
		_alarmBtn = RootObj.transform.FindComponent<Button>("BG/Alarm");
		_userBtn = RootObj.transform.FindComponent<Button>("BG/User");
        _userTxt = RootObj.transform.FindComponent<TMP_Text>("BG/User/Text (TMP)");
        _userPnl = RootObj.transform.Find("BG/UserPnl");
		_settingBtn = _userPnl.FindComponent<Button>("SettingBtn");
		_logoutBtn = _userPnl.FindComponent<Button>("LogoutBtn");
        StatePanel = GameObject.Find("StatePanel");
        StatePanel.SetActive(false);
        WorkScreenPanel = GameObject.Find("WorkScreenPanel");

		_userTxt.text = DataManager.Instance.CurrentAccount.name;

        _controlBtn.onClick.AddListener(() =>//打开远程操作界面
        {
            //presenter.CurrentActive = UIID.RemoteControl;
            WorkScreenPanel.gameObject.SetActive(true);
            UISystem.Instance.SetActive(UIID.HistoryWarning, false);
            UISystem.Instance.SetActive(UIID.Settings, false);
            /*
            if (WorkScreenPanel.activeSelf)
            {
                WorkScreenPanel.gameObject.SetActive(false);
            }
            else
            {
                WorkScreenPanel.gameObject.SetActive(true);
            }
			*/
        });
		_superviseBtn.onClick.AddListener(() =>
		{
            //presenter.CurrentActive = UIID.RuntimeMonitor;
            //SceneManager.LoadScene("Monitor");
            StatePanel.SetActive(true);
            UISystem.Instance.HideCanvasParent();
        });
		_alarmBtn.onClick.AddListener(() =>//打开报警处理界面
        {
            UISystem.Instance.SetActive(UIID.HistoryWarning, true);
            UISystem.Instance.SetActive(UIID.Settings, false);
            WorkScreenPanel.gameObject.SetActive(false);
        });
		_userBtn.onClick.AddListener(() =>
		{
			_userPnl.gameObject.SetActive(!_userPnl.gameObject.activeSelf);
		});
		_settingBtn.onClick.AddListener(() =>
        {
            _userPnl.gameObject.SetActive(false);
            UISystem.Instance.SetActive(UIID.Settings, true);
            UISystem.Instance.SetActive(UIID.HistoryWarning, false);
        });
		_logoutBtn.onClick.AddListener(() =>
        {
            _userPnl.gameObject.SetActive(false);
            presenter.Logout();
        });

		_userPnl.gameObject.SetActive(false);
	}

	public void UpdateCurrentAccount(AccountInfo account)
	{
		_userTxt.text = account.name;
	}


}