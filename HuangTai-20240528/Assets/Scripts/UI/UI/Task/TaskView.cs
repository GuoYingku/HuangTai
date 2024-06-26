using UnityEngine.UI;
using TMPro;
using Utility;
using UnityEngine;

[CanvasIndex(1)]
public class TaskView : UIView<TaskPresenter>, ITaskView
{
    private Button _stackBtn;
    private Button _reclaimBtn;
    private Button _scrapeBtn;
    private GameObject _stackScroll;
    private GameObject _reclaimScroll;
    private GameObject _scrapeScroll;

    private TMP_InputField _stackIndexInput;
    private TMP_InputField _stackAreaInput;
    private TMP_Dropdown _stackTypeDropdown;
    private TMP_InputField _stackWeightInput;
    private TMP_InputField _stackTimeInput;

    private TMP_InputField _reclaimIndexInput;
    private TMP_InputField _reclaimAreaInput;
    private TMP_Dropdown _reclaimTypeDropdown;

    private TMP_InputField _scrapeIndexInput;
    private TMP_InputField _scrapeAreaInput;

    private Button _saveBtn;
    private Button _performBtn;

    public override void InitUIElements()
    {
        _stackBtn = RootObj.transform.FindComponent<Button>("ButtonPnl/StackBtn");
        _reclaimBtn = RootObj.transform.FindComponent<Button>("ButtonPnl/ReclaimBtn");
        _scrapeBtn = RootObj.transform.FindComponent<Button>("ButtonPnl/ScrapeBtn");
        _stackScroll = RootObj.transform.Find("DetailPnl/StackScroll").gameObject;
        _reclaimScroll = RootObj.transform.Find("DetailPnl/ReclaimScroll").gameObject;
        _scrapeScroll = RootObj.transform.Find("DetailPnl/ScrapeScroll").gameObject;
        _stackIndexInput = RootObj.transform.FindComponent<TMP_InputField>("DetailPnl/StackScroll/Viewport/Content/IndexField/ValueInput");
        _stackAreaInput = RootObj.transform.FindComponent<TMP_InputField>("DetailPnl/StackScroll/Viewport/Content/AreaField/ValueInput");
        _stackTypeDropdown = RootObj.transform.FindComponent<TMP_Dropdown>("DetailPnl/StackScroll/Viewport/Content/TypeField/ValueInput");
        _stackWeightInput = RootObj.transform.FindComponent<TMP_InputField>("DetailPnl/StackScroll/Viewport/Content/WeightField/ValueInput");
        _stackTimeInput = RootObj.transform.FindComponent<TMP_InputField>("DetailPnl/StackScroll/Viewport/Content/TimeField/ValueInput");
        _reclaimIndexInput = RootObj.transform.FindComponent<TMP_InputField>("DetailPnl/ReclaimScroll/Viewport/Content/IndexField/ValueInput");
        _reclaimAreaInput = RootObj.transform.FindComponent<TMP_InputField>("DetailPnl/ReclaimScroll/Viewport/Content/AreaField/ValueInput");
        _reclaimTypeDropdown = RootObj.transform.FindComponent<TMP_Dropdown>("DetailPnl/ReclaimScroll/Viewport/Content/TypeField/ValueInput");
        _scrapeIndexInput = RootObj.transform.FindComponent<TMP_InputField>("DetailPnl/ScrapeScroll/Viewport/Content/IndexField/ValueInput");
        _scrapeAreaInput = RootObj.transform.FindComponent<TMP_InputField>("DetailPnl/ScrapeScroll/Viewport/Content/AreaField/ValueInput");

        _saveBtn = RootObj.transform.FindComponent<Button>("DetailPnl/ConfirmButtonPnl/SaveBtn");
        _performBtn = RootObj.transform.FindComponent<Button>("DetailPnl/ConfirmButtonPnl/PerformBtn");

        _stackBtn.onClick.AddListener(() =>
        {
            presenter.CurrentTask = TaskType.STACK;
        });
        _reclaimBtn.onClick.AddListener(() =>
        {
            presenter.CurrentTask = TaskType.RECLAIM;
        });
        _scrapeBtn.onClick.AddListener(() =>
        {
            presenter.CurrentTask = TaskType.SCRAPE;
        });
        _saveBtn.onClick.AddListener(() =>
        {
            presenter.SaveTask();
        });
        _performBtn.onClick.AddListener(() =>
        {
            presenter.PerformTask();
        });

        _stackScroll.SetActive(true);
        _reclaimScroll.SetActive(false);
        _scrapeScroll.SetActive(false);
    }

    public void ExitTask(TaskType task)
    {
        switch (task)
        {
            case TaskType.STACK:
                ExitStackTask();
                break;
            case TaskType.RECLAIM:
                ExitReclaimTask();
                break;
            case TaskType.SCRAPE:
                ExitScrapeType();
                break;
        }
    }

    public void EnterTask(TaskType task)
    {
        switch (task)
        {
            case TaskType.STACK:
                EnterStackTask();
                break;
            case TaskType.RECLAIM:
                EnterReclaimTask();
                break;
            case TaskType.SCRAPE:
                EnterScrapeTask();
                break;
        }
    }

    public void SwitchArea(TaskType taskType, string areaName)
    {
        switch (taskType)
        {
            case TaskType.STACK:
                _stackAreaInput.text = areaName;
                break;
            case TaskType.RECLAIM:
                break;
            case TaskType.SCRAPE:
                break;
        }
    }

    public TaskInfoBase GetTaskInfo(TaskType task)
    {
        return null;
    }

    private void ExitStackTask()
    {
        _stackScroll.SetActive(false);
    }
    private void EnterStackTask()
    {
        _stackScroll.SetActive(true);
        _stackIndexInput.text = presenter.GenerateTaskIndex();
    }
    private void ExitReclaimTask()
    {
        _reclaimScroll.SetActive(false);
    }
    private void EnterReclaimTask()
    {
        _reclaimScroll.SetActive(true);
        _reclaimIndexInput.text = presenter.GenerateTaskIndex();
    }
    private void ExitScrapeType()
    {
        _scrapeScroll.SetActive(false);
    }
    private void EnterScrapeTask()
    {
        _scrapeScroll.SetActive(true);
        _scrapeIndexInput.text = presenter.GenerateTaskIndex();
    }
}