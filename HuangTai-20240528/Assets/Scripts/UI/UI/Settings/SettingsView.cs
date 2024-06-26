using Cysharp.Threading.Tasks.Triggers;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;
using Utility.ObjectPool;

[CanvasIndex(7)]
public class SettingsView : UIView<SettingsPresenter>, ISettingsView
{
	private Button _accountBtn;
	private Button _managerBtn;
	//private Button _operationBtn;

	#region account panel
	private Transform _accountPnl;
	private TMP_InputField _infoAccountFieldInput;
	private TMP_InputField _infoNameFieldInput;
	private TMP_InputField _infoPasswordFieldInput;
	private Button _infoPasswordChangeBtn;

	#region password panel
	private Transform _passwordPnl;
	private TMP_InputField _infoPasswordNewInput;
	private TMP_InputField _infoPasswordConfirmInput;
	private Button _infoPasswordConfirmBtn;
	private Button _infoPasswordCancelBtn;
	#endregion
	#endregion

	#region manager panel
	private GameObject _accountPoolParent;
	private ObjectPool _accountRecordPool;

	private Transform _managerPnl;
	private Button _insertBtn;
	private Button _queryBtn;
	private ScrollRect _infoListScroll;

	#region insert panel
	private Transform _insertPnl;
	private Button _insertConfirmBtn;
	private Button _insertCancelBtn;
	private TMP_InputField _insertIndexInput;
	private TMP_InputField _insertAccountInput;
	private TMP_InputField _insertNameInput;
	private TMP_InputField _insertDepartmentInput;
	private TMP_InputField _insertJobInput;
	private TMP_InputField _insertPasswordInput;
    #endregion

	#region modify panel
	private Transform _modifyPnl;
	private Button _modifyConfirmBtn;
	private Button _modifyCancelBtn;
	private TMP_InputField _modifyIndexInput;
	private TMP_InputField _modifyAccountInput;
	private TMP_InputField _modifyNameInput;
	private TMP_InputField _modifyDepartmentInput;
	private TMP_InputField _modifyJobInput;
	private TMP_InputField _modifyPasswordInput;

	private Transform _editingAccount;
    #endregion

    #region account operation panel
    private GameObject _accountOperationPoolParent;
    private ObjectPool _accountOperationRecordPool;
	private Transform _accountOperationPnl;
    private ScrollRect _accountOperationListScroll;
	private Button _accountOperationCloseBtn;
	#endregion

	#endregion

	#region operation panel
	//private GameObject _operationPoolParent;
	//private ObjectPool _operationRecordPool;

	//private Transform _operationPnl;
	//private ScrollRect _operationListScroll;
	#endregion

	public override void InitUIElements()
	{
		_accountBtn = RootObj.transform.FindComponent<Button>("ButtonPnl/AccountBtn");
		_managerBtn = RootObj.transform.FindComponent<Button>("ButtonPnl/ManagerBtn");
		//_operationBtn = RootObj.transform.FindComponent<Button>("ButtonPnl/OperationBtn");

		_accountBtn.onClick.AddListener(() =>
		{
			presenter.CurrentPanel = SettingPanel.ACCOUNT;
		});
		_managerBtn.onClick.AddListener(() =>
		{
			presenter.CurrentPanel = SettingPanel.MANAGER;
		});
		//_operationBtn.onClick.AddListener(() =>
		//{
		//	presenter.CurrentPanel = SettingPanel.OPERATION;
		//});

        _accountPnl = RootObj.transform.Find("DetailPnl/AccountPnl");
        _passwordPnl = _accountPnl.Find("PasswordPnl");
        _managerPnl = RootObj.transform.Find("DetailPnl/ManagerPnl");
        _insertPnl = _managerPnl.Find("InsertPnl");
		_modifyPnl = _managerPnl.Find("ModifyPnl");

        #region account panel
        _infoAccountFieldInput = _accountPnl.FindComponent<TMP_InputField>("InfoPnl/AccountField/ValueInput");
		_infoNameFieldInput = _accountPnl.FindComponent<TMP_InputField>("InfoPnl/NameField/ValueInput");
		_infoPasswordFieldInput = _accountPnl.FindComponent<TMP_InputField>("InfoPnl/PasswordField/ValueInput");
		_infoPasswordChangeBtn = _accountPnl.FindComponent<Button>("InfoPnl/PasswordField/ChangeBtn");

		_infoPasswordNewInput = _passwordPnl.FindComponent<TMP_InputField>("PasswordField/ValueInput");
		_infoPasswordConfirmInput = _passwordPnl.FindComponent<TMP_InputField>("ConfirmField/ValueInput");
		_infoPasswordConfirmBtn = _passwordPnl.FindComponent<Button>("ConfirmBtn");
		_infoPasswordCancelBtn = _passwordPnl.FindComponent<Button>("CancelBtn");

		_infoPasswordChangeBtn.onClick.AddListener(() =>
		{
			_infoPasswordChangeBtn.interactable = false;
			_infoPasswordNewInput.text = _infoPasswordConfirmInput.text = string.Empty;
			_passwordPnl.gameObject.SetActive(true);
		});
		_infoPasswordConfirmBtn.onClick.AddListener(() =>
		{
			if (_infoPasswordNewInput.text.Equals(_infoPasswordConfirmInput.text))
			{
				_infoPasswordFieldInput.text = _infoPasswordNewInput.text;
				presenter.UpdatePassword(_infoAccountFieldInput.text, _infoPasswordNewInput.text);
				_infoPasswordChangeBtn.interactable = true;
				_passwordPnl.gameObject.SetActive(false);
			}
			else
			{
				Debug.Log("不匹配");
			}
		});
		_infoPasswordCancelBtn.onClick.AddListener(() =>
		{
			_infoPasswordChangeBtn.interactable = true;
			_passwordPnl.gameObject.SetActive(false);
		});
		#endregion

		#region manager panel
		_insertBtn = _managerPnl.FindComponent<Button>("InsertBtn");
		_queryBtn = _managerPnl.FindComponent<Button>("QueryBtn");
		_infoListScroll = _managerPnl.FindComponent<ScrollRect>("InfoListScroll");
		_accountOperationPnl = _managerPnl.Find("AccountOperationPnl");
		_accountOperationListScroll = _accountOperationPnl.FindComponent<ScrollRect>("AccountOperationListScroll");
		_accountOperationCloseBtn = _accountOperationPnl.FindComponent<Button>("CloseBtn");

        _insertIndexInput = _insertPnl.FindComponent<TMP_InputField>("Fields/IndexField/ValueInput");
		_insertAccountInput = _insertPnl.FindComponent<TMP_InputField>("Fields/AccountField/ValueInput");
		_insertNameInput = _insertPnl.FindComponent<TMP_InputField>("Fields/NameField/ValueInput");
		_insertDepartmentInput = _insertPnl.FindComponent<TMP_InputField>("Fields/DepartmentField/ValueInput");
		_insertJobInput = _insertPnl.FindComponent<TMP_InputField>("Fields/JobField/ValueInput");
		_insertPasswordInput = _insertPnl.FindComponent<TMP_InputField>("Fields/PasswordField/ValueInput");
		_insertConfirmBtn = _insertPnl.FindComponent<Button>("ConfirmBtn");
		_insertCancelBtn = _insertPnl.FindComponent<Button>("CancelBtn");

		_modifyIndexInput = _modifyPnl.FindComponent<TMP_InputField>("Fields/IndexField/ValueInput");
		_modifyAccountInput = _modifyPnl.FindComponent<TMP_InputField>("Fields/AccountField/ValueInput");
		_modifyNameInput = _modifyPnl.FindComponent<TMP_InputField>("Fields/NameField/ValueInput");
		_modifyDepartmentInput = _modifyPnl.FindComponent<TMP_InputField>("Fields/DepartmentField/ValueInput");
		_modifyJobInput = _modifyPnl.FindComponent<TMP_InputField>("Fields/JobField/ValueInput");
		_modifyPasswordInput = _modifyPnl.FindComponent<TMP_InputField>("Fields/PasswordField/ValueInput");
		_modifyConfirmBtn = _modifyPnl.FindComponent<Button>("ConfirmBtn");
		_modifyCancelBtn = _modifyPnl.FindComponent<Button>("CancelBtn");

		_accountPoolParent = _infoListScroll.content.Find("AccountRecordPoolParent").gameObject;
		_accountRecordPool = new ObjectPool("AccountRecordPool", ConstStr.ACCOUNT_RECORD_PREFAB.LoadAssetAtAddress<GameObject>(), 1000, _accountPoolParent, false);

		_accountOperationPoolParent = _accountOperationListScroll.content.Find("AccountOperationRecordPoolParent").gameObject;
		_accountOperationRecordPool = new ObjectPool("AccountOperationRecordPool", ConstStr.ACCOUNT_OPERATION_RECORD_PREFAB.LoadAssetAtAddress<GameObject>(), 1000, _accountOperationPoolParent, false);

        _insertBtn.onClick.AddListener(() =>
		{
			_insertPnl.gameObject.SetActive(true);
			_insertIndexInput.text = "0";
			_insertAccountInput.text = string.Empty;
			_insertNameInput.text = string.Empty;
			_insertDepartmentInput.text = string.Empty;
			_insertJobInput.text = string.Empty;
			_insertPasswordInput.text = string.Empty;
		});
		_queryBtn.onClick.AddListener(() =>
		{
			_accountOperationPnl.gameObject.SetActive(true);
			ShowAccountOperationList(presenter.GetAccountOperationList());
        });

		_insertConfirmBtn.onClick.AddListener(() =>
        {
            AccountInfo res = presenter.InsertAccount(_insertIndexInput.text, _insertAccountInput.text, _insertPasswordInput.text, _insertNameInput.text, _insertDepartmentInput.text, _insertJobInput.text);
            if (res != null)
            {
                AddNewInfo(res);
                DataManager.Instance.RecordNewAccountOperation($"添加账户：{res.account}");
            }
            _insertPnl.gameObject.SetActive(false);
        });
        _insertCancelBtn.onClick.AddListener(() =>
		{
			_insertPnl.gameObject.SetActive(false);
		});
		_modifyConfirmBtn.onClick.AddListener(() =>
		{
			UpdateAccountRecord(presenter.UpdateAccount(_modifyIndexInput.text, _modifyAccountInput.text, _modifyPasswordInput.text, _modifyNameInput.text, _modifyDepartmentInput.text, _modifyJobInput.text));
			_modifyPnl.gameObject.SetActive(false);
		});
		_modifyCancelBtn.onClick.AddListener(() =>
		{
			_modifyPnl.gameObject.SetActive(false);
		});
		_accountOperationCloseBtn.onClick.AddListener(() =>
		{
			_accountOperationPnl.gameObject.SetActive(false);
			_accountOperationRecordPool.FreeAll();
		});
        #endregion

        #region operation panel
  //      _operationPnl = RootObj.transform.Find("DetailPnl/OperationPnl");
		//_operationListScroll = _operationPnl.FindComponent<ScrollRect>("OperationListScroll");

  //      _operationPoolParent = _operationListScroll.content.Find("OperationRecordPoolParent").gameObject;
  //      _operationRecordPool = new ObjectPool("OperationRecordPool", ConstStr.OPERATION_RECORD_PREFAB.LoadAssetAtAddress<GameObject>(), 1000, _operationPoolParent, false);

        #endregion

        _accountPnl.gameObject.SetActive(false);
		_passwordPnl.gameObject.SetActive(false);
		_managerPnl.gameObject.SetActive(false);
		_insertPnl.gameObject.SetActive(false);
		_modifyPnl.gameObject.SetActive(false);
		//_operationPnl.gameObject.SetActive(false);
		_accountOperationPnl.gameObject.SetActive(false);

		EnterPanel(SettingPanel.ACCOUNT);
	}

	private void ShowInfo(AccountInfo info)
	{
		if (info == null)
		{
			_infoAccountFieldInput.text = "";
			_infoNameFieldInput.text = "";
			_infoPasswordFieldInput.text = "";
		}
		else
		{
			_infoAccountFieldInput.text = info.account;
			_infoNameFieldInput.text = info.name;
			_infoPasswordFieldInput.text = info.password;
		}
	}
    private void AddNewInfo(AccountInfo info)
    {
        Transform record = _accountRecordPool.GetFreeObject().transform;
        record.SetParent(_infoListScroll.content);
        TMP_Text recordIndexTxt = record.FindComponent<TMP_Text>("Fields/IndexField/IndexTxt");
        recordIndexTxt.text = info.index.ToString();
        TMP_Text recordAccountTxt = record.FindComponent<TMP_Text>("Fields/AccountField/AccountTxt");
        recordAccountTxt.text = info.account;
        TMP_Text recordNameTxt = record.FindComponent<TMP_Text>("Fields/NameField/NameTxt");
        recordNameTxt.text = info.name;
        TMP_Text recordDepartmentTxt = record.FindComponent<TMP_Text>("Fields/DepartmentField/DepartmentTxt");
        recordDepartmentTxt.text = info.department;
        TMP_Text recordJobTxt = record.FindComponent<TMP_Text>("Fields/JobField/JobTxt");
        recordJobTxt.text = info.job;
        Button recordModifyBtn = record.FindComponent<Button>("Fields/OperationField/ModifyBtn");
        Button recordResetBtn = record.FindComponent<Button>("Fields/OperationField/ResetBtn");
        Button recordRemoveBtn = record.FindComponent<Button>("Fields/OperationField/RemoveBtn");
        recordModifyBtn.onClick.RemoveAllListeners();
        recordModifyBtn.onClick.AddListener(() =>
        {
            _editingAccount = record;

            _modifyPnl.gameObject.SetActive(true);
            _modifyIndexInput.text = recordIndexTxt.text;
            _modifyAccountInput.text = recordAccountTxt.text;
            _modifyNameInput.text = recordNameTxt.text;
            _modifyDepartmentInput.text = recordDepartmentTxt.text;
            _modifyJobInput.text = recordJobTxt.text;
            _modifyPasswordInput.text = presenter.GetDefaultPassword(recordAccountTxt.text);
        });
        recordResetBtn.onClick.RemoveAllListeners();
        recordResetBtn.onClick.AddListener(() =>
        {
            presenter.ResetPassword(recordAccountTxt.text);
        });
        recordRemoveBtn.onClick.RemoveAllListeners();
        recordRemoveBtn.onClick.AddListener(() =>
        {
            presenter.RemoveAccount(recordAccountTxt.text);
            DataManager.Instance.RecordNewAccountOperation($"移除账户：{recordAccountTxt.text}");
            GameObject recordObj = record.gameObject;
            _accountRecordPool.FreeObject(ref recordObj);
        });
        if (info.account.Equals(DataManager.Instance.CurrentAccount.account))
        {
            recordRemoveBtn.interactable = false;
        }
        else
        {
            recordRemoveBtn.interactable = true;
        }
    }

    private void ShowInfoList(List<AccountInfo> infos)
    {
        foreach (AccountInfo info in infos)
        {
            AddNewInfo(info);
        }
    }

    private void UpdateAccountRecord(AccountInfo info)
	{
		if (info == null || _editingAccount == null)
		{
			return;
		}
		_editingAccount.FindComponent<TMP_Text>("Fields/IndexField/IndexTxt").text = info.index.ToString();
		_editingAccount.FindComponent<TMP_Text>("Fields/NameField/NameTxt").text = info.name;
		_editingAccount.FindComponent<TMP_Text>("Fields/DepartmentField/DepartmentTxt").text = info.department;
		_editingAccount.FindComponent<TMP_Text>("Fields/JobField/JobTxt").text = info.job;
	}

	//private void ShowOperationList(List<OperationInfo> infos)
	//{
	//	foreach (OperationInfo info in infos)
	//	{
	//		Transform record = _operationRecordPool.GetFreeObject().transform;
	//		record.SetParent(_operationListScroll.content);
	//		record.FindComponent<TMP_Text>("Fields/IndexField/IndexTxt").text = info.index.ToString();
	//		record.FindComponent<TMP_Text>("Fields/TimeField/TimeTxt").text = info.time;
	//		record.FindComponent<TMP_Text>("Fields/DetailField/DetailTxt").text = info.detail;
	//	}
	//}

	private void ShowAccountOperationList(List<AccountOperationInfo> infos)
	{
		foreach (AccountOperationInfo info in infos)
		{
			Transform record = _accountOperationRecordPool.GetFreeObject().transform;
			record.SetParent(_accountOperationListScroll.content);
			record.FindComponent<TMP_Text>("Fields/IndexField/IndexTxt").text = info.index.ToString();
			record.FindComponent<TMP_Text>("Fields/TimeField/TimeTxt").text = info.time;
			record.FindComponent<TMP_Text>("Fields/OperatorField/OperatorTxt").text = info.operatorName;
			record.FindComponent<TMP_Text>("Fields/DetailField/DetailTxt").text = info.detail;
		}
	}

	public void ExitPanel(SettingPanel panel)
	{
		switch (panel)
		{
			case SettingPanel.ACCOUNT:
				_accountPnl.gameObject.SetActive(false);
				_passwordPnl.gameObject.SetActive(false);
				_accountBtn.image.sprite = ConstStr.ACCOUNT_TAB_OFF.LoadAssetAtAddress<Sprite>();
				break;
			case SettingPanel.MANAGER:
				_managerPnl.gameObject.SetActive(false);
				_insertPnl.gameObject.SetActive(false);
				_modifyPnl.gameObject.SetActive(false);
				_accountRecordPool.FreeAll();
				_accountOperationRecordPool.FreeAll();
				_managerBtn.image.sprite = ConstStr.MANAGER_TAB_OFF.LoadAssetAtAddress<Sprite>();
				break;
			//case SettingPanel.OPERATION:
			//	_operationBtn.image.sprite = ConstStr.OPERATION_TAB_OFF.LoadAssetAtAddress<Sprite>();
			//	_operationPnl.gameObject.SetActive(false);
			//	_operationRecordPool.FreeAll();
			//	break;
		}
	}
	public void EnterPanel(SettingPanel panel)
	{
		switch (panel)
		{
			case SettingPanel.ACCOUNT:
				ShowInfo(DataManager.Instance.CurrentAccount);
				_accountPnl.gameObject.SetActive(true);
				_infoPasswordChangeBtn.interactable = true;
				_accountBtn.image.sprite = ConstStr.ACCOUNT_TAB_ON.LoadAssetAtAddress<Sprite>();

                _accountBtn.image.sprite = ConstStr.ACCOUNT_TAB_ON.LoadAssetAtAddress<Sprite>();
                _managerBtn.image.sprite = ConstStr.MANAGER_TAB_OFF.LoadAssetAtAddress<Sprite>();
                _accountBtn.onClick.Invoke();//保证切换登录页面都是同一页面 -ljz
                break;
			case SettingPanel.MANAGER:
				ShowInfoList(presenter.GetAccountList());
				_managerPnl.gameObject.SetActive(true);
				_managerBtn.image.sprite = ConstStr.MANAGER_TAB_ON.LoadAssetAtAddress<Sprite>();
				break;
			//case SettingPanel.OPERATION:
			//	ShowOperationList(presenter.GetOperationList());
			//	_operationPnl.gameObject.SetActive(true);
			//	_operationBtn.image.sprite = ConstStr.OPERATION_TAB_ON.LoadAssetAtAddress<Sprite>();
			//	break;
		}
    }

    public void SetAdminElementsActive(bool active)
    {
		_managerBtn.gameObject.SetActive(active);
    }
}