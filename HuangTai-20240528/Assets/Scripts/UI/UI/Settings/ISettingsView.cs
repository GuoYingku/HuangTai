using System.Collections.Generic;

public interface ISettingsView : IUIView
{
    void EnterPanel(SettingPanel panel);
    void ExitPanel(SettingPanel panel);
    void SetAdminElementsActive(bool active);
}