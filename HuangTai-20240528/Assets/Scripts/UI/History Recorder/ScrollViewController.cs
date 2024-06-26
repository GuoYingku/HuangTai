using UnityEngine;
using TMPro;
using System;
using System.Data;
using System.Globalization;
using Unity.VisualScripting;
using System.Collections;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Collections.Generic;

public class ScrollViewController : MonoBehaviour
{
    //public TextMeshProUGUI num; 
    //public TextMeshProUGUI text; 
    //public TextMeshProUGUI time; 
    //public TextMeshProUGUI condition;

    public GameObject AlarmHanding;
    public Transform Content; // 父物体
    public string Tables;
    public TMP_InputField startYear;
    public TMP_InputField startMouth;
    public TMP_InputField startDay;
    public TMP_InputField endYear;
    public TMP_InputField endMouth;
    public TMP_InputField endDay;
    public TMP_InputField OperatorPerson;

    private MySqlDataReader _dataReader = null;
    private Coroutine _readingCoroutine = null;

    private EventListener MySqlDataChanged = new EventListener();

    void Start()
    {
        MySqlDataChanged.OnVariableChange += CreateAlarmHandingList;
        clearHistory();
    }

    private void CreateAlarmHandingList(bool newVal, int id, string newInfo, string newUser, bool IsEnterSql)
    {
        SearchAlarmHandingList();
    }

    void Update()
    {
        string sql = "Select COUNT(*) from " + Tables + ";";
        DataSet date = MySqlHelper.GetDataSet(sql);
        DataTable DT = date.Tables[0];
        MySqlDataChanged.ID = (int)(long)DT.Rows[0][0];
    }

    public void CreateAlarmHandingList(bool value, int id, string info, string user)
    {
        SearchAlarmHandingList();
    }

    public void clearHistory()
    {
        startYear.text = string.Empty;
        endYear.text = string.Empty;
        startMouth.text = string.Empty;
        startDay.text = string.Empty;
        endMouth.text = string.Empty;
        endDay.text = string.Empty;
        OperatorPerson.text = string.Empty;
        CreateAlarmHandingList(false, 0, "", "");
    }

    public void SearchAlarmHandingList()
    {
        string startTime = null, endTime = null;

        bool useDate = true, useOperator = OperatorPerson.text != string.Empty;

        try
        {
            int startyear = Convert.ToInt32(startYear.text);
            int startmouth = Convert.ToInt32(startMouth.text);
            int startday = Convert.ToInt32(startDay.text);
            int endyear = Convert.ToInt32(endYear.text);
            int endmouth = Convert.ToInt32(endMouth.text);
            int endday = Convert.ToInt32(endDay.text);

            startTime = startyear + "-" + startmouth + "-" + startday + " 00:00:00";
            endTime = endyear + "-" + endmouth + "-" + endday + " 23:59:59";
        }
        catch
        {
            Debug.Log("日期无效");
            useDate = false;
        }

        string sql = $"SELECT * FROM {Tables} WHERE ";

        if (!useDate && !useOperator)
        {
            sql = "Select * from " + Tables + " ORDER BY id DESC LIMIT 50;";
        }
        else if (useDate && useOperator)
        {
            sql += $"`operator` = '{OperatorPerson.text}' AND `time` BETWEEN '{startTime}' AND '{endTime}' ORDER BY `id` DESC;";
        }
        else if (useDate)
        {
            sql += $"`time` BETWEEN '{startTime}' AND '{endTime}' ORDER BY `id` DESC;";
        }
        else
        {
            sql += $"`operator` = '{OperatorPerson.text}' ORDER BY `id` DESC;";
        }

        if (_dataReader != null)
        {
            GlobalManager.Instance.StopCoroutine(_readingCoroutine);
            _dataReader.Close();
        }
        _dataReader = MySqlHelper.ExecuteReader(sql);
        _readingCoroutine = GlobalManager.Instance.StartCoroutine(ReaderCoroutine());
    }

    private IEnumerator ReaderCoroutine()
    {
        for (int i = 0; i < Content.transform.childCount; i++)
        {
            Destroy(Content.GetChild(i).gameObject);
        }
        int counter = 0;
        while(_dataReader.Read())
        {
            string _id = _dataReader[0].ToString();
            string _time = _dataReader[1].ToString();
            string _info = _dataReader[2].ToString();
            string _operatorname = _dataReader[3].ToString();
            GameObject AlarmHandingText = Instantiate(AlarmHanding, Content);
            TextMeshProUGUI[] newTextComponents = AlarmHandingText.GetComponentsInChildren<TextMeshProUGUI>();
            newTextComponents[0].text = _id;
            newTextComponents[1].text = _time;
            newTextComponents[2].text = _info;
            newTextComponents[3].text = _operatorname;
            ++counter;
            if (counter == 50)
            {
                counter = 0;
                yield return null;
            }
        }
        _dataReader.Close();
        _readingCoroutine = null;
        _dataReader = null;
    }
}