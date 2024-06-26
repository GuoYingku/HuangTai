using Utility.DesignPatterns;

public class TaskPresenter : UIPresenter<TaskView>, ITaskPresenter
{
    private TaskInfoBase _taskInfo;

    private TaskType _currentTask;

    public TaskType CurrentTask
    {
        set
        {
            if (_currentTask == value)
            {
                return;
            }
            view.ExitTask(_currentTask);
            _currentTask = value;
            view.EnterTask(value);
        }
    }
    public override void ShowView()
    {
        Subject.Instance.Register(ConstStr.AREA_SELECT_SUBJECT, SwitchArea);
        base.ShowView();
    }
    public override void HideView()
    {
        base.HideView();
        Subject.Instance.Unregister(ConstStr.AREA_SELECT_SUBJECT, SwitchArea);
    }
    public string GenerateTaskIndex()
    {
        return "NewIndex";
    }
    public bool CheckTaskValid()
    {
        return true;
    }
    private void SwitchArea(object areaName)
    {
        view.SwitchArea(_currentTask, areaName as string);
    }

    public void SaveTask()
    {
        _taskInfo = view.GetTaskInfo(_currentTask);
    }

    public void PerformTask()
    {

    }
}