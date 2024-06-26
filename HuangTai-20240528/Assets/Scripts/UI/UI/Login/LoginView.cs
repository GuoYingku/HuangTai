using TMPro;
using UnityEngine.UI;
using Utility;

[CanvasIndex(7)]
public class LoginView : UIView<LoginPresenter>, ILoginView
{
    private TMP_InputField _accountInput;
    private TMP_InputField _passwordInput;
    private Button _loginBtn;
    public TMP_InputField AccountInput => _accountInput;

    public TMP_InputField PasswordInput => _passwordInput;

    public override void InitUIElements()
    {
        _accountInput = RootObj.transform.FindComponent<TMP_InputField>("LoginPnl/AccountInput");
        _passwordInput = RootObj.transform.FindComponent<TMP_InputField>("LoginPnl/PasswordInput");
        _loginBtn = RootObj.transform.FindComponent<Button>("LoginPnl/LoginBtn");

        _loginBtn.onClick.AddListener(Login);
    }

    public void Login()
    {
        presenter.Login(_accountInput.text, _passwordInput.text);
    }
}