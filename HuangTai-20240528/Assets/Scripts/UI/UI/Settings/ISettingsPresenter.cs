using System.Collections.Generic;

public interface ISettingsPresenter : IUIPresenter
{
    SettingPanel CurrentPanel { set; }
    void ResetPassword(string account);
    string GetDefaultPassword(string account);
    void RemoveAccount(string account);
    AccountInfo InsertAccount(string index, string account, string password, string name, string department, string job);
    AccountInfo UpdateAccount(string index, string account, string password, string name, string department, string job);
    AccountInfo GetAccountInfo(string account);
    List<AccountInfo> GetAccountList();
    List<AccountOperationInfo> GetAccountOperationList();
}