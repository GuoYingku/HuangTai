using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateTime : MonoBehaviour
{
    readonly string[] Day = new string[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
    private TMP_Text _time;

    // Start is called before the first frame update
    void Start()
    {
        _time = this.gameObject.transform.GetComponent<TMP_Text>();
        _time.transform.localPosition += new Vector3(200f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        //时间换行并移动 ljz
        _time.text = DateTime.Now.ToString("yyyy年MM月dd日\n");
        string week = Day[Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))].ToString();
        _time.text += week;
        _time.text += DateTime.Now.ToString("HH:mm:ss");
    }
}
