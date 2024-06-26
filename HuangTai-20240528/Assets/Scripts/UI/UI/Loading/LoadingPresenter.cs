using Utility.DesignPatterns;

public class LoadingPresenter : UIPresenter<LoadingView>, ILoadingPresenter
{
    public override void ShowView()
    {
        Subject.Instance.Register(ConstStr.LOADING_PROGRESS_SUBJECT, UpdateProgress);
        base.ShowView();
    }
    public override void HideView()
    {
        base.HideView();
        Subject.Instance.Unregister(ConstStr.LOADING_PROGRESS_SUBJECT, UpdateProgress);
    }
    public void UpdateProgress(object progress)
    {
        view.UpdateProgress((float)progress);
    }
}