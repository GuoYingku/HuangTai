using System;
using System.Data;
using UnityEngine;
using Utility.DesignPatterns;

public partial class DataManager : ISingleton<DataManager>, IUpdateSystem
{
    private float _queryDataTimer = 0;
    private float _queryDataInterval = 1;

    private AccountInfo _currentAccount;
    public AccountInfo CurrentAccount
    {
        get => _currentAccount;
    }

    private DataRow _latestData;

    public void Init()
    {
    }

    public void UpdateFunc(float deltaTime)
    {
        _queryDataTimer -= deltaTime;
        if (_queryDataTimer < 0)
        {
            _queryDataTimer = _queryDataInterval;
            //GetLatestData();
        }
    }

    public static DataManager Instance
    {
        get => ISingleton<DataManager>.Instance;
    }
    private void GetLatestData()
    {
        DataSet res = MySqlHelper.GetDataSet($"SELECT * FROM `{ConstStr.DATABASE_VARIABLE_TABLE}` ORDER BY `{ConstStr.DATA_TIME_STAMP}` DESC LIMIT 1");
        try
        {
            _latestData = res.Tables[0].Rows[0];
        }
        catch(Exception e)
        {
            Debug.LogException(e);
            _latestData = null;
        }
    }
}