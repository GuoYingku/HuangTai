using System;
using System.Data;
using UnityEngine;
using MySql.Data.MySqlClient;

public class MySqlHelper
{
    public static string IP = Address.serviceIP;
    public static string Database = "csr";
    public static string Username = "root";
    public static string Password = "123456";
    public static string connstr = "server=" + IP + ";database= " + Database + ";username=" + Username + ";password=" + Password + ";Charset=utf8";


    #region 执行查询语句，返回MySqlDataReader

    /// <summary>
    /// 执行查询语句，返回MySqlDataReader
    /// </summary>
    /// <param name="sqlString"></param>
    /// <returns></returns>
    public static MySqlDataReader ExecuteReader(string sqlString)
    {
        MySqlConnection connection = new MySqlConnection(connstr);
        MySqlCommand cmd = new MySqlCommand(sqlString, connection);
        MySqlDataReader myReader = null;
        try
        {
            connection.Open();
            myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            return myReader;
        }
        catch (MySql.Data.MySqlClient.MySqlException e)
        {
            connection.Close();
            throw new Exception(e.Message);
        }
        finally
        {
            if (myReader == null)
            {
                cmd.Dispose();
                connection.Close();
            }
        }
    }
    #endregion

    #region 执行带参数的查询语句，返回 MySqlDataReader

    /// <summary>
    /// 执行带参数的查询语句，返回MySqlDataReader
    /// </summary>
    /// <param name="sqlString"></param>
    /// <param name="cmdParms"></param>
    /// <returns></returns>
    public static MySqlDataReader ExecuteReader(string sqlString, params MySqlParameter[] cmdParms)
    {
        MySqlConnection connection = new MySqlConnection(connstr);
        MySqlCommand cmd = new MySqlCommand();
        MySqlDataReader myReader = null;
        try
        {
            PrepareCommand(cmd, connection, null, sqlString, cmdParms);
            myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            cmd.Parameters.Clear();
            return myReader;
        }
        catch (MySql.Data.MySqlClient.MySqlException e)
        {
            connection.Close();
            throw new Exception(e.Message);
        }
        finally
        {
            if (myReader == null)
            {
                cmd.Dispose();
                connection.Close();
            }
        }
    }
    #endregion

    #region 执行sql语句,返回执行行数

    /// <summary>
    /// 执行sql语句,返回执行行数
    /// </summary>
    /// <param name="sql"></param>
    /// <returns></returns>
    public static int ExecuteSql(string sql)
    {
        using (MySqlConnection conn = new MySqlConnection(connstr))
        {
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                try
                {
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (MySql.Data.MySqlClient.MySqlException e)
                {
                    conn.Close();
                    //throw e;
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    cmd.Dispose();
                    conn.Close();
                }
            }
        }

        return -1;
    }
    #endregion

    #region 执行带参数的sql语句，并返回执行行数

    /// <summary>
    /// 执行带参数的sql语句，并返回执行行数
    /// </summary>
    /// <param name="sqlString"></param>
    /// <param name="cmdParms"></param>
    /// <returns></returns>
    public static int ExecuteSql(string sqlString, params MySqlParameter[] cmdParms)
    {
        using (MySqlConnection connection = new MySqlConnection(connstr))
        {
            using (MySqlCommand cmd = new MySqlCommand())
            {
                try
                {
                    PrepareCommand(cmd, connection, null, sqlString, cmdParms);
                    int rows = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    return rows;
                }
                catch (MySql.Data.MySqlClient.MySqlException e)
                {
                    throw new Exception(e.Message);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }
    }
    #endregion

    #region 执行查询语句，返回DataSet

    /// <summary>
    /// 执行查询语句，返回DataSet
    /// </summary>
    /// <param name="sql"></param>
    /// <returns></returns>
    public static DataSet GetDataSet(string sql)
    {
        using (MySqlConnection conn = new MySqlConnection(connstr))
        {
            DataSet ds = new DataSet();
            try
            {
                conn.Open();
                MySqlDataAdapter DataAdapter = new MySqlDataAdapter(sql, conn);
                DataAdapter.Fill(ds);
            }
            catch (Exception ex)
            {
                //throw ex;
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return ds;
        }
    }
    #endregion

    #region 执行带参数的查询语句，返回DataSet

    /// <summary>
    /// 执行带参数的查询语句，返回DataSet
    /// </summary>
    /// <param name="sqlString"></param>
    /// <param name="cmdParms"></param>
    /// <returns></returns>
    public static DataSet GetDataSet(string sqlString, params MySqlParameter[] cmdParms)
    {
        using (MySqlConnection connection = new MySqlConnection(connstr))
        {
            MySqlCommand cmd = new MySqlCommand();
            PrepareCommand(cmd, connection, null, sqlString, cmdParms);
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                DataSet ds = new DataSet();
                try
                {
                    da.Fill(ds, "ds");
                    cmd.Parameters.Clear();
                }
                catch (MySql.Data.MySqlClient.MySqlException  ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
                return ds;
            }
        }
    }
    #endregion

    #region 执行带参数的sql语句，并返回 object

    /// <summary>
    /// 执行带参数的sql语句，并返回object
    /// </summary>
    /// <param name="sqlString"></param>
    /// <param name="cmdParms"></param>
    /// <returns></returns>
    public static object GetSingle(string sqlString, params MySqlParameter[] cmdParms)
    {
        using (MySqlConnection connection = new MySqlConnection(connstr))
        {
            using (MySqlCommand cmd = new MySqlCommand())
            {
                try
                {
                    PrepareCommand(cmd, connection, null, sqlString, cmdParms);
                    object obj = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    if ((System.Object.Equals(obj, null)) || (System.Object.Equals(obj, System.DBNull.Value)))
                    {
                        return null;
                    }
                    else
                    {
                        return obj;
                    }
                }
                catch (MySql.Data.MySqlClient.MySqlException  e)
                {
                    throw new Exception(e.Message);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }
    }

    #endregion

    /// <summary>
    /// 执行存储过程,返回数据集
    /// </summary>
    /// <param name="storedProcName">存储过程名</param>
    /// <param name="parameters">存储过程参数</param>
    /// <returns>DataSet</returns>
    public static DataSet RunProcedureForDataSet(string storedProcName, IDataParameter[] parameters)
    {
        using (MySqlConnection connection = new MySqlConnection(connstr))
        {
            DataSet dataSet = new DataSet();
            connection.Open();
            MySqlDataAdapter sqlDA = new MySqlDataAdapter();
            sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
            sqlDA.Fill(dataSet);
            connection.Close();
            return dataSet;
        }
    }

    /// <summary>
    /// 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)
    /// </summary>
    /// <param name="connection">数据库连接</param>
    /// <param name="storedProcName">存储过程名</param>
    /// <param name="parameters">存储过程参数</param>
    /// <returns>SqlCommand</returns>
    private static MySqlCommand BuildQueryCommand(MySqlConnection connection, string storedProcName,
        IDataParameter[] parameters)
    {
        MySqlCommand command = new MySqlCommand(storedProcName, connection);
        command.CommandType = CommandType.StoredProcedure;
        foreach (MySqlParameter parameter in parameters)
        {
            command.Parameters.Add(parameter);
        }
        return command;
    }

    #region 装载MySqlCommand对象

    /// <summary>
    /// 装载MySqlCommand对象
    /// </summary>
    private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, string cmdText,
        MySqlParameter[] cmdParms)
    {
        if (conn.State != ConnectionState.Open)
        {
            conn.Open();
        }
        cmd.Connection = conn;
        cmd.CommandText = cmdText;
        if (trans != null)
        {
            cmd.Transaction = trans;
        }
        cmd.CommandType = CommandType.Text; //cmdType;
        if (cmdParms != null)
        {
            foreach (MySqlParameter parm in cmdParms)
            {
                cmd.Parameters.Add(parm);
            }
        }
    }
    #endregion
}


public class TestMySQL : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        /*
            //表格使用代码
            DataTable dt = new DataTable("Name");
            dt.Columns.Add(new DataColumn("ID", typeof(Int32)));
            dt.Columns.Add(new DataColumn("Name", typeof(string)));
            dt.Columns.Add(new DataColumn("Sex", typeof(string)));
            dt.Columns.Add(new DataColumn("Addr", typeof(string)));

            //添加一行数据到列中
            DataRow dr = dt.NewRow();
            dr["ID"] = 1;
            dr["Name"] = "张三";
            dr["Sex"] = "未知";
            dr["addr"] = "泰国";

            //添加一行数据到表单中
            dt.Rows.Add(dr);

            Debug.Log("列表行数：" + dt.Rows.Count);

            string sex = dt.Rows[0][2].ToString();
            Debug.Log("性别：" + sex);



        //MySQL插入参考代码
        DateTime time = DateTime.Now;
        string info = "设备故障报警";
        string operatorname = "wph";

        string sql = String.Format("insert into warning (time, info, operator) values('{0}', '{1}','{2}');", time, info, operatorname, Encoding.UTF8);
        int ret = MySqlHelper.ExecuteSql(sql);
        */

        //MySQL获取数据参考代码
        /*
        string sql = "Select * from warning;";
        DataSet date01 = MySqlHelper.GetDataSet(sql);
        DataTable DT = date01.Tables[0];

        for (int i = DT.Rows.Count; i > 0; i--)
        {
            string _id = DT.Rows[i][0].ToString();
            string _time = DT.Rows[i][1].ToString();
            string _info = DT.Rows[i][2].ToString();
            string _operatorname = DT.Rows[i][3].ToString();

            Debug.Log("总长度：" + DT.Rows.Count + "；ID：" + _id + "；时间：" + _time + "；报警内容：" + _info + "；操作人员：" + _operatorname);
            }
        }
        */

        /*
        //MySQL 按时间查找参考代码
        string sql = String.Format("SELECT * FROM logs where time between '{0}' and  '{1}'; ", "2023-12-19 14:48:00", "2023-12-19 15:57:21");//starttime      endtime
        DataSet dataSet = MySqlHelper.GetDataSet(sql);
        DataTable DT = dataSet.Tables[0];

        for (int i = 0; i < DT.Rows.Count; i++)
        {
            string _id = DT.Rows[i][0].ToString();
            string _time = DT.Rows[i][1].ToString();
            string _info = DT.Rows[i][2].ToString();
            string _operatorname = DT.Rows[i][3].ToString();

            Debug.Log("总长度：" + DT.Rows.Count + "；ID：" + _id + "；时间：" + _time + "；报警内容：" + _info + "；操作人员：" + _operatorname);
        }
        */
        


    }

    // Update is called once per frame
    void Update()
    {

    }
}
