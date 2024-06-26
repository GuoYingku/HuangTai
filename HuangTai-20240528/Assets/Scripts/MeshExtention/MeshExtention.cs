using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Windows;

namespace FrameWorkSong
{
    public static class MeshExtention
    {
        /// <summary>
        /// 创建正方形面
        /// </summary>
        /// <param name="size">尺寸</param>
        /// <param name="num">细分</param>
        /// <returns></returns>
        public static Mesh CreateSquareArea(this Mesh mesh, float spacing, int num, List<double[]> DEM)
        {
            //一共多少行/列
            float size = spacing * num;     //创建Mesh的尺寸  间距乘以模型结构点数得出尺寸
            int row = num;//2 * num + 1;    //Mesh的一行的模型结构点数
            int column = num;               //Mesh的一列的模型结构点数
            int pointsCount = row * column;  //所有的模型结构点数
            Vector3[] meshPoints = new Vector3[pointsCount];
            int[] triangles = new int[24 * num * num];
            Vector2[] uvs = new Vector2[pointsCount];
            float unitUV = spacing / 100 - 0.000015f; //1.00f / (num * 2.00f);
            float unitSize = spacing;
            Vector3 cornerPoint = new Vector3(0, 0, 0);
            Vector3 unitVec_x = new Vector3(unitSize, 0, 0);
            Vector3 unitVec_z = new Vector3(0, 0, unitSize);
            Vector2 uvVec_x = new Vector2(unitUV, 0);
            Vector2 uvVec_y = new Vector2(0,unitUV);
            int k = 0;
            //List<PointData> dataList = new List<PointData>();//创建分区数据列表 -ljz 
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    //PointData pointData = new PointData();

                    meshPoints[i * row + j] = cornerPoint + j * unitVec_x + i * unitVec_z;      //设置Mesh每一个顶点的位置与高度
                    meshPoints[i * row + j].y = Mathf.Abs((float)DEM[i][j]);                    //更新Mesh每一个顶点的高度

                    //pointData.x0 = meshPoints[i * row + j].x;
                   // pointData.z0 = meshPoints[i * row + j].z;
                    //dataList.Add(pointData);//收集所有要分区的数据

                    uvs[i * row + j] = j * uvVec_x + i * uvVec_y;
                   
                    if (i < row - 1 && j < column - 1)
                    {
                        triangles[k] = (i * row) + j+1;
                        triangles[k + 1] = (i * row) + j;
                        triangles[k + 2] = ((i + 1) * row) + j;
                        triangles[k + 3] = (i * row) + j+1;
                        triangles[k + 4] = ((i + 1) * row) + j;
                        triangles[k + 5] = ((i + 1) * row) + j+1;
                        k += 6;
                    }
                }
            }
            //Dictionary<AreaData, List<PointData>>  newDataDir = dealPartitionData(dataList, 20.0f, 50.0f, 6, 55); //分区数据整理为字典 -ljz
            mesh.indexFormat = IndexFormat.UInt32;
            mesh.vertices = meshPoints;
            
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;

        }

        public static Mesh CreateArea() {

            //读取文件

            return new Mesh();
        }


        public struct AreaData {
            public float originAngle;
            public float endAngle;
        }

        public struct PointData 
        {
            public float x0;
            public float z0;
        }

        /// <summary>
        /// 传入点分区 -ljz
        /// </summary>
        /// <param name="dataArr">所有数据</param>
        /// <param name="originAngle">起始角度</param>
        /// <param name="angle">要分配的角度</param>
        /// <param name="AreaCount">分区数</param>
        /// <param name="radius">分区半径</param>
        /// <returns></returns>
        public static Dictionary<AreaData, List<PointData>> dealPartitionData(List<PointData> dataArr, float originAngle, float angle, int AreaCount,float radius) {
            
            Dictionary<AreaData, List<PointData>> resultDir = new Dictionary<AreaData, List<PointData>>();

            List<AreaData> areaList = new List<AreaData>();

            for (int i = 1; i <= AreaCount; i++)//创建分区字典
            {
                AreaData areaData = new AreaData();
                areaData.originAngle = originAngle;
                areaData.endAngle = originAngle+angle;
                originAngle += angle;//更新下个起始角度
                areaList.Add(areaData);
            }

            foreach (AreaData areaData in areaList) {//实例化结果字典
                List<PointData> pointList = new List<PointData>();
                resultDir.Add(areaData, pointList);
            }

            //数据分区
            foreach (PointData data in dataArr) {
                float newX0 = data.x0 - radius;
                float newZ0 = data.z0 - radius;//更改坐标系

                float nowAngle = ((float)(Math.Atan2(newX0, newZ0) / Math.PI * 180)>0f)?
                    (float)(Math.Atan2(newX0, newZ0) / Math.PI * 180):(float)(Math.Atan2(newX0, newZ0) / Math.PI * 180)+360.0f;//计算角度校正为正数

                foreach (AreaData item in areaList)//判断点位是否在分区
                {
                    if (nowAngle > item.originAngle && nowAngle <= item.endAngle) {
                        //从字典取出来当前列表 存入数据
                        List<PointData> nowDataList = resultDir[item];
                        nowDataList.Add(data);
                        resultDir[item] = nowDataList;
                    }
                }
            }

            return resultDir;
        }
    }
}
