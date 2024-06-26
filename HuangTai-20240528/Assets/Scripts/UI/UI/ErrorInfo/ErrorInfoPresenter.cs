using Utility.DesignPatterns;

public class ErrorInfoPresenter : UIPresenter<ErrorInfoView>, IErrorInfoPresenter
{
    public override void ShowView()
    {
        Subject.Instance.Register(ConstStr.ERROR_INFO_SUBJECT, ReceiveErrorInfo);
        base.ShowView();
    }
    public override void HideView()
    {
        base.HideView();
        Subject.Instance.Unregister(ConstStr.ERROR_INFO_SUBJECT, ReceiveErrorInfo);
    }
    private void ReceiveErrorInfo(object errorInfo)
    {
        view.SetErrorText("ErrorText");
    }
}