using Unity.VisualScripting;

public interface ITaskPresenter : IUIPresenter
{
    TaskType CurrentTask { set; }
    string GenerateTaskIndex();
    bool CheckTaskValid();
    void SaveTask();
    void PerformTask();
}