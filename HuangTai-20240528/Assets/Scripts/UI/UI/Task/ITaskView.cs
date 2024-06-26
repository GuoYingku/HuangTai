public interface ITaskView : IUIView
{
    void SwitchArea(TaskType taskType, string areaName);
    void ExitTask(TaskType task);
    void EnterTask(TaskType task);
    TaskInfoBase GetTaskInfo(TaskType task);
}