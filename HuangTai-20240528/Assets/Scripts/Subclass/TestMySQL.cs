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


    #region ִ�в�ѯ��䣬����MySqlDataReader

    /// <summary>
    /// ִ�в�ѯ��䣬����MySqlDataReader
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

    #region ִ�д������Ĳ�ѯ��䣬���� MySqlDataReader

    /// <summary>
    /// ִ�д������Ĳ�ѯ��䣬����MySqlDataReader
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

    #region ִ��sql���,����ִ������

    /// <summary>
    /// ִ��sql���,����ִ������
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

    #region ִ�д�������sql��䣬������ִ������

    /// <summary>
    /// ִ�д�������sql��䣬������ִ������
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

    #region ִ�в�ѯ��䣬����DataSet

    /// <summary>
    /// ִ�в�ѯ��䣬����DataSet
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

    #region ִ�д������Ĳ�ѯ��䣬����DataSet

    /// <summary>
    /// ִ�д������Ĳ�ѯ��䣬����DataSet
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

    #region ִ�д�������sql��䣬������ object

    /// <summary>
    /// ִ�д�������sql��䣬������object
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
    /// ִ�д洢����,�������ݼ�
    /// </summary>
    /// <param name="storedProcName">�洢������</param>
    /// <param name="parameters">�洢���̲���</param>
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
    /// ���� SqlCommand ����(��������һ���������������һ������ֵ)
    /// </summary>
    /// <param name="connection">���ݿ�����</param>
    /// <param name="storedProcName">�洢������</param>
    /// <param name="parameters">�洢���̲���</param>
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

    #region װ��MySqlCommand����

    /// <summary>
    /// װ��MySqlCommand����
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
            //���ʹ�ô���
            DataTable dt = new DataTable("Name");
            dt.Columns.Add(new DataColumn("ID", typeof(Int32)));
            dt.Columns.Add(new DataColumn("Name", typeof(string)));
            dt.Columns.Add(new DataColumn("Sex", typeof(string)));
            dt.Columns.Add(new DataColumn("Addr", typeof(string)));

            //���һ�����ݵ�����
            DataRow dr = dt.NewRow();
            dr["ID"] = 1;
            dr["Name"] = "����";
            dr["Sex"] = "δ֪";
            dr["addr"] = "̩��";

            //���һ�����ݵ�����
            dt.Rows.Add(dr);

            Debug.Log("�б�������" + dt.Rows.Count);

            string sex = dt.Rows[0][2].ToString();
            Debug.Log("�Ա�" + sex);



        //MySQL����ο�����
        DateTime time = DateTime.Now;
        string info = "�豸���ϱ���";
        string operatorname = "wph";

        string sql = String.Format("insert into warning (time, info, operator) values('{0}', '{1}','{2}');", time, info, operatorname, Encoding.UTF8);
        int ret = MySqlHelper.ExecuteSql(sql);
        */

        //MySQL��ȡ���ݲο�����
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

            Debug.Log("�ܳ��ȣ�" + DT.Rows.Count + "��ID��" + _id + "��ʱ�䣺" + _time + "���������ݣ�" + _info + "��������Ա��" + _operatorname);
            }
        }
        */

        /*
        //MySQL ��ʱ����Ҳο�����
        string sql = String.Format("SELECT * FROM logs where time between '{0}' and  '{1}'; ", "2023-12-19 14:48:00", "2023-12-19 15:57:21");//starttime      endtime
        DataSet dataSet = MySqlHelper.GetDataSet(sql);
        DataTable DT = dataSet.Tables[0];

        for (int i = 0; i < DT.Rows.Count; i++)
        {
            string _id = DT.Rows[i][0].ToString();
            string _time = DT.Rows[i][1].ToString();
            string _info = DT.Rows[i][2].ToString();
            string _operatorname = DT.Rows[i][3].ToString();

            Debug.Log("�ܳ��ȣ�" + DT.Rows.Count + "��ID��" + _id + "��ʱ�䣺" + _time + "���������ݣ�" + _info + "��������Ա��" + _operatorname);
        }
        */
        


    }

    // Update is called once per frame
    void Update()
    {

    }
}
