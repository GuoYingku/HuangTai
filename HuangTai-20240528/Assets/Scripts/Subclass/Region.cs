using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HuangtaiPowerPlantControlSystem
{
    public class Region 
    {
        public int REG_ID;
        public float BEGIN; //業方
        public float END;    //業方
        public string COAL_TYPE;
        public float DENSITY;    // kg/m3
        public float VOLUME;    // m3
        public float WEIGHT;      //t
    }
}
