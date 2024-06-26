using Utility.DesignPatterns;

public class RuntimeMonitorPresenter : UIPresenter<RuntimeMonitorView>, IRuntimeMonitorPresenter
{
    public override void ShowView()
    {
        Subject.Instance.Register(ConstStr.TASK_PROGRESS_SUBJECT, UpdateProgress);
        Subject.Instance.Register(ConstStr.RUNTIME_DATA_SUBJECT, UpdateData);
        base.ShowView();
    }
    public override void HideView()
    {
        base.HideView();
        Subject.Instance.Unregister(ConstStr.TASK_PROGRESS_SUBJECT, UpdateProgress);
        Subject.Instance.Unregister(ConstStr.RUNTIME_DATA_SUBJECT, UpdateData);
    }

    public void PauseTask()
    {
    }

    public void StopTask()
    {
    }

    void UpdateProgress(object arg)
    {
        view.UpdateProgress((float)arg);
    }

    void UpdateData(object arg)
    {
        view.UpdateData((RuntimeDataBase)arg);
    }
}