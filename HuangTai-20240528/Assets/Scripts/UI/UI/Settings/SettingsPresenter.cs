using System.Collections.Generic;

public class SettingsPresenter : UIPresenter<SettingsView>, ISettingsPresenter
{
    private SettingPanel _currentPanel;
    public SettingPanel CurrentPanel
    {
        set
        {
            if (_currentPanel == value)
            {
                return;
            }
            view.ExitPanel(_currentPanel);
            _currentPanel = value;
            view.EnterPanel(_currentPanel);
        }
    }

    public AccountInfo InsertAccount(string index, string account, string password, string name, string department, string job)
    {
        AccountInfo accountInfo = new AccountInfo
        {
            index = int.Parse(index),
            account = account,
            name = name,
            department = department,
            job = job,
            password = password,
            defaultPassword = password
        };
        if (DataManager.Instance.InsertAccount(accountInfo))
        {
            UnityEngine.Debug.Log("插入成功");
            return accountInfo;
        }
        else
        {
            UnityEngine.Debug.Log("插入失败");
            return null;
        }
    }

    public void RemoveAccount(string account)
    {
        if (DataManager.Instance.RemoveAccount(account))
        {
            UnityEngine.Debug.Log("删除成功");
        }
        else
        {
            UnityEngine.Debug.Log("删除失败");
        }
    }

    public AccountInfo UpdateAccount(string index, string account, string password, string name, string department, string job)
    {
        AccountInfo accountInfo = new AccountInfo
        {
            index = int.Parse(index),
            account = account,
            name = name,
            department = department,
            job = job,
            password = string.Empty,
            defaultPassword = password
        };
        if (DataManager.Instance.UpdateAccount(accountInfo))
        {
            UnityEngine.Debug.Log("更新成功");
            return accountInfo;
        }
        else
        {
            UnityEngine.Debug.Log("更新失败");
            return null;
        }
    }

    public void UpdatePassword(string account, string newPassword)
    {
        DataManager.Instance.UpdatePassword(account, newPassword);
    }

    public void ResetPassword(string account)
    {
        DataManager.Instance.ResetPassword(account);
    }

    public string GetDefaultPassword(string account)
    {
        return DataManager.Instance.GetDefaultPassword(account);
    }

    public override void ShowView()
    {
        base.ShowView();
        view.SetAdminElementsActive(DataManager.Instance.CurrentAccount.isAdmin);
        view.EnterPanel(SettingPanel.ACCOUNT);
    }

    public AccountInfo GetAccountInfo(string account)
    {
        return DataManager.Instance.GetAccountInfo(account);
    }

    public List<AccountInfo> GetAccountList()
    {
        return DataManager.Instance.AccountInfos;
    }

    public List<AccountOperationInfo> GetAccountOperationList()
    {
        return DataManager.Instance.AccountOperations;
    }
}