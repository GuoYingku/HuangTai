using Utility.DesignPatterns;

public class TopPnlPresenter : UIPresenter<TopPnlView>, ITopPnlPresenter
{
    public void Logout()
    {
        UISystem.Instance.SetActive(UIID.Login, true);
        UISystem.Instance.SetActive(UIID.TopPnl, false);
        UISystem.Instance.SetActive(UIID.BottomPnlRight, false);
    }
    public override void ShowView()
    {
        base.ShowView();
        Subject.Instance.Register(ConstStr.LOGIN_SUBJECT, UpdateCurrentAccount);
        view.WorkScreenPanel.SetActive(true);
    }
    public override void HideView()
    {
        Subject.Instance.Unregister(ConstStr.LOGIN_SUBJECT, UpdateCurrentAccount);
        base.HideView();
        view.WorkScreenPanel.SetActive(false);
    }

    private void UpdateCurrentAccount(object arg)
    {
        view.UpdateCurrentAccount(arg as AccountInfo);
    }
}