using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Utility.DesignPatterns;

public partial class DataManager
{
    public List<AccountInfo> AccountInfos
    {
        get => GetAccountList();
    }
    public bool CheckLoginInfo(string account, string password)
    {
        AccountInfo info = GetAccountInfo(account);
        if (info == null)
        {
            return false;
        }
        else
        {
            if (info.password.Equals(password))
            {
                _currentAccount = info;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public bool UpdatePassword(string account, string newPassword)
    {
        string query = $"UPDATE `{ConstStr.DATABASE_LOGIN_TABLE}` SET `{ConstStr.DATA_PASSWORD}` = '{newPassword}' WHERE `{ConstStr.DATA_USERNAME}` = '{account}'";
        return MySqlHelper.ExecuteSql(query) > 0;
    }
    public string GetDefaultPassword(string account)
    {
        string query = $"SELECT `{ConstStr.DATA_DEFAULT_PASSWORD}` FROM `{ConstStr.DATABASE_LOGIN_TABLE}` WHERE `{ConstStr.DATA_USERNAME}` = \"{account}\"";
        DataSet res = MySqlHelper.GetDataSet(query);
        if (res != null)
        {
            try
            {
                return res.Tables[0].Rows[0][ConstStr.DATA_DEFAULT_PASSWORD] as string;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        return null;
    }
    public bool ResetPassword(string account)
    {
        string defaultPassword = GetDefaultPassword(account);
        if (defaultPassword != null)
        {
            string setPasswordQuery = $"UPDATE `{ConstStr.DATABASE_LOGIN_TABLE}` SET `{ConstStr.DATA_PASSWORD}` = '{defaultPassword}' WHERE `{ConstStr.DATA_USERNAME}` = '{account}'";
            return MySqlHelper.ExecuteSql(setPasswordQuery) > 0;
        }
        return false;
    }
    public bool RemoveAccount(string account)
    {
        string query = $"DELETE FROM `{ConstStr.DATABASE_LOGIN_TABLE}` WHERE `{ConstStr.DATA_USERNAME}` = '{account}'";
        return MySqlHelper.ExecuteSql(query) > 0;
    }
    public bool InsertAccount(AccountInfo account)
    {
        string query = $"INSERT INTO `{ConstStr.DATABASE_LOGIN_TABLE}` (`{ConstStr.DATA_USERNAME}`, `{ConstStr.DATA_PASSWORD}`, `{ConstStr.DATA_IS_ADMIN}`, `{ConstStr.DATA_NAME}`, `{ConstStr.DATA_DEPARTMENT}`, `{ConstStr.DATA_JOB}`, `{ConstStr.DATA_DEFAULT_PASSWORD}`, `{ConstStr.DATA_INDEX}`) VALUES ('{account.account}', '{account.password}', 0, '{account.name}', '{account.department}', '{account.job}', '{account.defaultPassword}', {account.index})";
        return MySqlHelper.ExecuteSql(query) > 0;
    }
    public bool UpdateAccount(AccountInfo account)
    {
        string query = $"UPDATE `{ConstStr.DATABASE_LOGIN_TABLE}` SET `{ConstStr.DATA_DEFAULT_PASSWORD}` = '{account.defaultPassword}', `{ConstStr.DATA_NAME}` = '{account.name}', `{ConstStr.DATA_DEPARTMENT}` = '{account.department}', `{ConstStr.DATA_JOB}` = '{account.job}', `{ConstStr.DATA_INDEX}` = {account.index} WHERE `{ConstStr.DATA_USERNAME}` = '{account.account}'";
        return MySqlHelper.ExecuteSql(query) > 0;
    }
    public AccountInfo GetAccountInfo(string account)
    {
        string query = $"SELECT * FROM `{ConstStr.DATABASE_LOGIN_TABLE}` WHERE `{ConstStr.DATA_USERNAME}` = \"{account}\"";
        DataSet dataSet = MySqlHelper.GetDataSet(query);
        try
        {
            DataRow row = dataSet.Tables[0].Rows[0];
            ulong val = (ulong)row[ConstStr.DATA_IS_ADMIN];
            return new AccountInfo
            {
                account = row[ConstStr.DATA_USERNAME] as string,
                defaultPassword = row[ConstStr.DATA_DEFAULT_PASSWORD] as string,
                password = row[ConstStr.DATA_PASSWORD] as string,
                department = row[ConstStr.DATA_DEPARTMENT] as string,
                job = row[ConstStr.DATA_JOB] as string,
                index = (int)row[ConstStr.DATA_INDEX],
                name = row[ConstStr.DATA_NAME] as string,
                isAdmin = (val == 1)
            };
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return null;
        }
    }
    public List<AccountInfo> GetAccountList()
    {
        List<AccountInfo> res = new List<AccountInfo>();
        string query = $"SELECT * FROM `{ConstStr.DATABASE_LOGIN_TABLE}`";
        DataSet dataSet = MySqlHelper.GetDataSet(query);
        try
        {
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                ulong val = (ulong)row[ConstStr.DATA_IS_ADMIN];
                res.Add(new AccountInfo
                {
                    account = row[ConstStr.DATA_USERNAME] as string,
                    defaultPassword = row[ConstStr.DATA_DEFAULT_PASSWORD] as string,
                    password = row[ConstStr.DATA_PASSWORD] as string,
                    department = row[ConstStr.DATA_DEPARTMENT] as string,
                    job = row[ConstStr.DATA_JOB] as string,
                    index = (int)row[ConstStr.DATA_INDEX],
                    name = row[ConstStr.DATA_NAME] as string,
                    isAdmin = (val == 1)
                });
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        return res;
    }
}