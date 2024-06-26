
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public partial class DataManager
{
    private static readonly string _accountOperationHistory = Application.streamingAssetsPath + "/AccountHistory.json";

    public List<AccountOperationInfo> AccountOperations
    {
        get => GetAccountOperations();
    }

    private List<AccountOperationInfo> GetAccountOperations()
    {
        List<AccountOperationInfo> res = new List<AccountOperationInfo>();
        string query = $"SELECT * FROM `{ConstStr.DATABASE_ACCOUNTOPERATION_TABLE}` ORDER BY `{ConstStr.DATA_ACCOUNT_OPERATION_INDEX}`";
        DataSet dataSet = MySqlHelper.GetDataSet(query);
        try
        {
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                res.Add(new AccountOperationInfo
                {
                    index = (int)row[ConstStr.DATA_ACCOUNT_OPERATION_INDEX],
                    time = row[ConstStr.DATA_ACCOUNT_OPERATION_TIME].ToString(),
                    operatorName = row[ConstStr.DATA_ACCOUNT_OPERATION_OPERATOR] as string,
                    detail = row[ConstStr.DATA_ACCOUNT_OPERATION_DETAIL] as string
                });
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return null;
        }
        return res;
    }

    public void RecordNewAccountOperation(string detail)
    {
        string sql = $"INSERT INTO {ConstStr.DATABASE_ACCOUNTOPERATION_TABLE} (`{ConstStr.DATA_ACCOUNT_OPERATION_TIME}`,`{ConstStr.DATA_ACCOUNT_OPERATION_OPERATOR}`,`{ConstStr.DATA_ACCOUNT_OPERATION_DETAIL}`) VALUES ('{DateTime.Now}','{CurrentAccount.name}','{detail}')";
        MySqlHelper.ExecuteSql(sql);
    }
}