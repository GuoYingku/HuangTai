using TMPro;
using UnityEngine;
using Utility.DesignPatterns;

public class LoginPresenter : UIPresenter<LoginView>, ILoginPresenter
{
    public void Login(string username, string password)
    {
        if (DataManager.Instance.CheckLoginInfo(username, password))
        {
            UISystem.Instance.SetActive(UIID.Login, false);
            UISystem.Instance.SetActive(UIID.TopPnl, true);
            UISystem.Instance.SetActive(UIID.BottomPnlRight, true);

            Subject.Instance.Notify(ConstStr.LOGIN_SUBJECT, DataManager.Instance.CurrentAccount);
        }
    }
}