using TMPro;
using UnityEngine.UI;
using Utility;

[CanvasIndex(2)]
public class ErrorInfoView : UIView<ErrorInfoPresenter>, IErrorInfoView
{
	private TMP_Text _errorTxt;
	private Button _confirmBtn;
	public override void InitUIElements()
	{
		_errorTxt = RootObj.transform.FindComponent<TMP_Text>("ErrorInfoPnl/ErrorTxt");
		_confirmBtn = RootObj.transform.FindComponent<Button>("ErrorInfoPnl/ConfirmBtn");

		_confirmBtn.onClick.AddListener(() =>
		{
			UISystem.Instance.SetActive(UIID.ErrorInfo, false);
		});
	}

    public void SetErrorText(string text)
    {
		_errorTxt.text = text;
    }
}