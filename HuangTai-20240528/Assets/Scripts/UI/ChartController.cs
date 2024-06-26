using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility.ObjectPool;

public class ChartController : MonoBehaviour
{
    public Button scraperMotor1CurrentBtn;
    public Button scraperMotor2CurrentBtn;
    public Button materialFetchingAmplitudeMotorCurrentBtn;
    public Button materialFetchingRotaryMotorCurrentBtn;
    public Button materialStackingBeltMotorCurrentBtn;
    public Button materialStackingRotaryMotorCurrentBtn;

    public TMP_InputField startYear;
    public TMP_InputField startMonth;
    public TMP_InputField startDay;
    public TMP_InputField startHour;
    public TMP_InputField startMinute;
    public TMP_InputField endYear;
    public TMP_InputField endMonth;
    public TMP_InputField endDay;
    public TMP_InputField endHour;
    public TMP_InputField endMinute;

    public Button searchBtn;
    public Button clearBtn;

    public List<TMP_Text> hScales = new List<TMP_Text>();
    public RectTransform pointParent;
    public RectTransform segmentParent;
    public GameObject pointPrefab;
    public GameObject segmentPrefab;

    private bool _searching;

    Dictionary<Button, CanvasGroup> btn2Group = new Dictionary<Button, CanvasGroup>();

    const string M_MaterialStackingBeltMotorCurrent = "M_MaterialStackingBeltMotorCurrent";
    const string M_MaterialStackingRotaryMotorCurrent = "M_MaterialStackingRotaryMotorCurrent";
    const string M_ScraperMotor1Current = "M_ScraperMotor1Current";
    const string M_ScraperMotor2Current = "M_ScraperMotor2Current";
    const string M_MaterialFetchingRotaryMotorCurrent = "M_MaterialFetchingRotaryMotorCurrent";
    const string M_MaterialFetchingAmplitudeMotorCurrent = "M_MaterialFetchingAmplitudeMotorCurrent";

    private string _currentCol = M_ScraperMotor1Current;

    private List<DateTime> _timeList = new List<DateTime>();
    private List<long> _dataList = new List<long>();
    private List<Vector2> _pointList = new List<Vector2>();

    private void Awake()
    {
        btn2Group.Add(scraperMotor1CurrentBtn, scraperMotor1CurrentBtn.GetComponent<CanvasGroup>());
        btn2Group.Add(scraperMotor2CurrentBtn, scraperMotor2CurrentBtn.GetComponent<CanvasGroup>());
        btn2Group.Add(materialFetchingAmplitudeMotorCurrentBtn, materialFetchingAmplitudeMotorCurrentBtn.GetComponent<CanvasGroup>());
        btn2Group.Add(materialFetchingRotaryMotorCurrentBtn, materialFetchingRotaryMotorCurrentBtn.GetComponent<CanvasGroup>());
        btn2Group.Add(materialStackingBeltMotorCurrentBtn, materialStackingBeltMotorCurrentBtn.GetComponent<CanvasGroup>());
        btn2Group.Add(materialStackingRotaryMotorCurrentBtn, materialStackingRotaryMotorCurrentBtn.GetComponent<CanvasGroup>());

        scraperMotor1CurrentBtn.onClick.AddListener(() =>
        {
            EnableTab(scraperMotor1CurrentBtn);
            _currentCol = M_ScraperMotor1Current;
        });
        scraperMotor2CurrentBtn.onClick.AddListener(() =>
        {
            EnableTab(scraperMotor2CurrentBtn);
            _currentCol = M_ScraperMotor2Current;
        });
        materialFetchingAmplitudeMotorCurrentBtn.onClick.AddListener(() =>
        {
            EnableTab(materialFetchingAmplitudeMotorCurrentBtn);
            _currentCol = M_MaterialFetchingAmplitudeMotorCurrent;
        });
        materialFetchingRotaryMotorCurrentBtn.onClick.AddListener(() =>
        {
            EnableTab(materialFetchingRotaryMotorCurrentBtn);
            _currentCol = M_MaterialFetchingRotaryMotorCurrent;
        });
        materialStackingBeltMotorCurrentBtn.onClick.AddListener(() =>
        {
            EnableTab(materialStackingBeltMotorCurrentBtn);
            _currentCol = M_MaterialStackingBeltMotorCurrent;
        });
        materialStackingRotaryMotorCurrentBtn.onClick.AddListener(() =>
        {
            EnableTab(materialStackingRotaryMotorCurrentBtn);
            _currentCol = M_MaterialStackingRotaryMotorCurrent;
        });

        searchBtn.onClick.AddListener(Search);
        clearBtn.onClick.AddListener(ClearSearch);

        EnableTab(materialStackingBeltMotorCurrentBtn);
        _currentCol = M_MaterialStackingBeltMotorCurrent;
    }

    private void Update()
    {
        if (!_searching)
        {
            string sql = $"SELECT `time`,`{_currentCol}` FROM `criticalparameter` ORDER BY `id` DESC LIMIT 50";
            DataSet data = MySqlHelper.GetDataSet(sql);
            DataTable dt = data.Tables[0];
            DrawChart(dt);
            return;
        }

    }

    void EnableTab(Button btn)
    {
        _searching = false;
        foreach (var kv in btn2Group)
        {
            if (kv.Key == btn)
            {
                kv.Value.alpha = 1;
            }
            else
            {
                kv.Value.alpha = 0.6f;
            }
        }
    }

    void Search()
    {
        string startTime = null, endTime = null;
        _searching = true;
        try
        {
            int startyear = Convert.ToInt32(startYear.text);
            int startmonth = Convert.ToInt32(startMonth.text);
            int startday = Convert.ToInt32(startDay.text);
            int starthour = Convert.ToInt32(startHour.text);
            int startminute = Convert.ToInt32(startMinute.text);
            int endyear = Convert.ToInt32(endYear.text);
            int endmonth = Convert.ToInt32(endMonth.text);
            int endday = Convert.ToInt32(endDay.text);
            int endhour = Convert.ToInt32(endHour.text);
            int endminute = Convert.ToInt32(endMinute.text);
            startTime = startyear + "-" + startmonth + "-" + startday + " " + starthour + ":" + startminute + ":00";
            endTime = endyear + "-" + endmonth + "-" + endday + "-" + endhour + ":" + endminute + ":59";
        }
        catch
        {
            Debug.Log("时间无效");
            _searching = false;
        }

        string sql = $"SELECT `time`,`{_currentCol}` FROM `criticalparameter` WHERE `time` BETWEEN '{startTime}' AND '{endTime}' ORDER BY `id` DESC";
        DataSet data = MySqlHelper.GetDataSet(sql);
        DataTable dt = data.Tables[0];
        DrawChart(dt);
    }
    public void ClearSearch()
    {
        startYear.text = startMonth.text = startDay.text = startHour.text = startMinute.text = endYear.text = endMonth.text = endDay.text = endHour.text = endMinute.text = string.Empty;
        _searching = false;
    }
    void DrawChart(DataTable dt)
    {
        _timeList.Clear();
        _dataList.Clear();
        _pointList.Clear();
        int minVal = 0, maxVal = 300;
        foreach (DataRow row in dt.Rows)
        {
            DateTime timestamp = (DateTime)row[0];
            long cntdata = Convert.ToInt32(row[1]);
            _timeList.Insert(0, timestamp);
            _dataList.Insert(0, cntdata);
        }
        int pointCount = _timeList.Count;
        int segCount = pointCount - 1;
        DateTime minTime = DateTime.Now, maxTime = DateTime.Now;
        int cntPointCount = pointParent.childCount;
        int cntSegCount = segmentParent.childCount;
        if (pointCount != 0)
        {
            minTime = _timeList[0];
            maxTime = _timeList[pointCount - 1];
        }
        else
        {
            pointCount = segCount = 0;
        }

        int timePeriodCount = hScales.Count - 1;
        for(int i=0;i<hScales.Count;++i)
        {
            hScales[i].text = (minTime + (maxTime - minTime) * i / timePeriodCount).ToString();
        }

        if (cntPointCount < pointCount)
        {
            for (int i = cntPointCount; i < pointCount; ++i)
            {
                Instantiate(pointPrefab, pointParent);
            }
        }
        else if (cntPointCount > pointCount)
        {
            for (int i = cntPointCount - 1; i >= pointCount; --i)
            {
                Destroy(pointParent.GetChild(i).gameObject);
            }
        }
        if (cntSegCount < segCount)
        {
            for (int i = cntSegCount; i < segCount; ++i)
            {
                Instantiate(segmentPrefab, segmentParent);
            }
        }
        else if (cntSegCount > segCount)
        {
            for (int i = cntSegCount - 1; i >= segCount; --i)
            {
                Destroy(segmentParent.GetChild(i).gameObject);
            }
        }

        for (int i = 0; i < pointCount; ++i)
        {
            RectTransform point = pointParent.GetChild(i).GetComponent<RectTransform>();
            Vector2 pos = new Vector2(
                1f * (_timeList[i].Ticks - minTime.Ticks) / (maxTime.Ticks - minTime.Ticks) * (pointParent.rect.width) + pointParent.rect.xMin,
                1f * (_dataList[i] - minVal) / (maxVal - minVal) * (pointParent.rect.height) + pointParent.rect.yMin);
            point.anchoredPosition = pos;
            _pointList.Add(pos);
        }
        for (int i = 0; i < segCount; ++i)
        {
            RectTransform seg = segmentParent.GetChild(i).GetComponent<RectTransform>();
            seg.anchoredPosition = _pointList[i];
            Vector2 diff = _pointList[i + 1] - _pointList[i];
            seg.sizeDelta = new Vector2(diff.magnitude, seg.sizeDelta.y);
            seg.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);
        }
    }
}