public interface IRuntimeMonitorView : IUIView
{
    void UpdateProgress(float progress);
    void UpdateData(RuntimeDataBase data);
}