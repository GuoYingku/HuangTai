using System;

public class EventListener
{
    public delegate void OnBoolChangeDelegate(bool newVal,int id, string newInfo, string newUser,bool IsEnterSql);//�����ﴫ�ݲ���
    public event OnBoolChangeDelegate OnVariableChange;//�¼�

    private bool m_boolean = false;
    private int m_id = 0;
    public string Info = "";
    public string User = "111";
    public bool IsEnterSql = true; //������ Ĭ����� -ljz
    public bool Boolean
    {
        get
        {
            return m_boolean;
        }
        set
        {
            if (m_boolean == value) return;
            if (OnVariableChange != null)
            {
                if (DataManager.Instance.CurrentAccount == null)
                {
                    User = "";
                }
                else
                {
                    User = DataManager.Instance.CurrentAccount.name;
                }
                OnVariableChange(!m_boolean, ID, Info, User,IsEnterSql);//�����Ϊ�վͷ�����ֵ
            }
            m_boolean = value;
        }
    }

    public int ID
    {
        get
        {
            return m_id;
        }
        set
        {
            if (m_id == value) return;
            if (OnVariableChange != null)
            {
                if(DataManager.Instance.CurrentAccount==null)
                {
                    User = "";
                }
                else
                {
                    User = DataManager.Instance.CurrentAccount.name;
                }
                OnVariableChange(!m_boolean, ID, Info, User,IsEnterSql);//�����Ϊ�վͷ�����ֵ
            }
            m_id = value;
        }
    }

}
