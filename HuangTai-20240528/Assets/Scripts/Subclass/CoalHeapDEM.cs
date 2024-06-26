using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScanConnection;

namespace HuangtaiPowerPlantControlSystem
{
    public class CoalHeapDEM
    {
        public string QUERY_SYSTEM;
        public int DATA_TYPE;
        public int QUERY_TYPE;

        public DateTime TIME;
        public string COALHOUSE_NO;
        public float RAIDUS;//煤场半径，米
        public float SLOPE;  //煤场锥形地面坡度，度数
        public float WALLHEIGHT; //煤场圆柱墙面高度，米

        public float VOLUME_TOTAL; //m3
        public float WEIGHT_TOTAL; //t

        public int REGION_NUM;
        public REGION[] REGION_LIST;

        public float X0; // m
        public float Z0; // m
        public float DX; // m
        public float DZ; // m
        public int NX;
        public int NZ;

        public float[,] DEM;  // m

        public int SYS_STATUS; //三维扫描子系统的工作状态，-2-内存不足，-1-CPU占用太大，0-功能未激活，1-正常

        public CoalHeapDEM()
        {

            QUERY_SYSTEM = "MC";
            DATA_TYPE = 3;
            QUERY_TYPE = 3;

            TIME = DateTime.Now;
            COALHOUSE_NO = "001";
            RAIDUS = 55;
            SLOPE = 7;
            WALLHEIGHT = 17.5f;

            VOLUME_TOTAL = 110000;

            WEIGHT_TOTAL = 120000;

            X0 = 0;
            Z0 = 0;
            DX = 0.2f;
            DZ = 0.2f;
            NX = 500;
            NZ = 500;
            REGION_NUM = 6;

            REGION_LIST = new REGION[REGION_NUM];
            float REG_INTERVAL = 360.0f / REGION_NUM;
            for (int i = 0; i < REGION_NUM; i++)
            {
                REGION_LIST[i] = new REGION();
                REGION_LIST[i].REG_ID = i;
                REGION_LIST[i].BEGIN = i * REG_INTERVAL;
                REGION_LIST[i].END = (i + 1) * REG_INTERVAL;
                REGION_LIST[i].COAL_TYPE = i.ToString();
                REGION_LIST[i].DENSITY = 1100;   // kg/m3
                REGION_LIST[i].VOLUME = 5000;    // m3
                REGION_LIST[i].WEIGHT = 5500;      //t
            }

            DEM = new float[NZ, NX];

            for (int row = 0; row < NZ; row++)
                for (int col = 0; col < NX; col++)
                    DEM[row, col] = (row + 1) * (col + 1);

            SYS_STATUS = 1;

        }
    }
}
