using TMPro;
using UnityEngine.UI;
public interface ILoginView : IUIView
{
    TMP_InputField AccountInput { get; }
    TMP_InputField PasswordInput { get; }

    void Login();
}