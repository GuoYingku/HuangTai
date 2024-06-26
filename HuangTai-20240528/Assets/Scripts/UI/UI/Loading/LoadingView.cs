using TMPro;
using UnityEngine.UI;
using Utility;
using Utility.DesignPatterns;

[CanvasIndex(9)]
public class LoadingView : UIView<LoadingPresenter>, ILoadingView
{
    private TMP_Text _loadingTxt;
    private Slider _progressSlider;

    public override void InitUIElements()
    {
        _loadingTxt = RootObj.transform.FindComponent<TMP_Text>("LoadingTxt");
        _progressSlider = RootObj.transform.FindComponent<Slider>("ProgressSlider");

        _progressSlider.value = 0;
    }

    public void UpdateProgress(float progress)
    {
        GlobalManager.Instance.AddCrossThreadOperation(() =>
        {
            _progressSlider.value = progress;
        });
    }
}