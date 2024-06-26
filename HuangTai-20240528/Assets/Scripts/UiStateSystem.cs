using HuangtaiPowerPlantControlSystem;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Renci.SshNet.Common;
using System.IO.Compression;
using System.IO;
using WebSocket = BestHTTP.WebSocket.WebSocket;
using Color = UnityEngine.Color;
using System.Net.WebSockets;

public class ShareRes
{
    public int count = 0;
    public static Mutex mutex = new Mutex();
    public static void WaitMutex()
    {
        mutex.WaitOne();

    }

    public static void ReleaseMutex()
    {
        mutex.ReleaseMutex();
    }

}

public class UiStateSystem : MonoBehaviour
{
    //公开变量
    public TextMeshProUGUI SecondConfirmText;
    public GameObject WarningPanel;
    public TextMeshProUGUI WarningText;
    public GameObject stackingArmMesh;
    public GameObject feedingArmMesh;
    public GameObject scraperMesh;
    public GameObject AlarmPanelPrefab;
    public TMP_InputField alarmTextbox;
    public GameObject SecondConfirmPanel;
    public ModelAnglesDisplayer angleDisplayer;
    private float delayTime = 0.7f;
    private int _saveCriticalParameterEveryXRead = 10;//取数据的时间间隔 -ljz
    private int _currentSaveCriticalParameterCounter = 0;

    private float resetThreeSpeedDataDelayTime = 20.0f;//重置参数延迟时间 -ljz
    private float highLightTime = 2.0f;//高光时间 -ljz

    public Button AutoDataButton1;//自动堆料参数更新按钮
    public Button AutoDataButton2;//自动取料参数更新按钮
    private bool IsChange = true;//是不是 还未更新？

    private Coroutine myToggleHighLightTime1 = null;
    public TextMeshProUGUI myToggleHighLightTimeText1;//回转换向字体变色
    private Coroutine myToggleHighLightTime2 = null;
    public TextMeshProUGUI myToggleHighLightTimeText2;//堆料回转速度字体变色
    private Coroutine myToggleHighLightTime3 = null;
    public TextMeshProUGUI myToggleHighLightTimeText3;//取料回转速度字体变色
    private Coroutine myToggleHighLightTime4 = null;
    public TextMeshProUGUI myToggleHighLightTimeText4;//取料俯仰速度字体变色
    private Coroutine myToggleHighLightTime5 = null;
    public TextMeshProUGUI myToggleHighLightTimeText5;//自动堆料停止字体变色
    private Coroutine myToggleHighLightTime6 = null;
    public TextMeshProUGUI myToggleHighLightTimeText6;//自动取料停止字体变色
    private Coroutine myToggleHighLightTime7 = null;
    public TextMeshProUGUI myToggleHighLightTimeText7;//堆料参数更新字体变色
    private Coroutine myToggleHighLightTime8 = null;
    public TextMeshProUGUI myToggleHighLightTimeText8;//取料参数更新字体变色
    private Coroutine myToggleHighLightTime9 = null;
    public TextMeshProUGUI myToggleHighLightTimeText9;//复位字体变色
    private Coroutine myToggleHighLightTime10 = null;
    public TextMeshProUGUI myToggleHighLightTimeText10;//启车报警变色

    private Coroutine myToggleHighLightTime11 = null;
    public TextMeshProUGUI myToggleHighLightTimeText11;//堆料角度校准
    private Coroutine myToggleHighLightTime12 = null;
    public TextMeshProUGUI myToggleHighLightTimeText12;//取料刮板电流设置
    private Coroutine myToggleHighLightTime13 = null;
    public TextMeshProUGUI myToggleHighLightTimeText13;//取料角度校准

    private Coroutine PeekingRotateSpeedSliderTimerCoroutine;//取料回转角度 倒计时重置协程 -ljz
    private Coroutine PeekingTiltSpeedSliderTimerCoroutine;//取料俯仰角度 倒计时重置协程 
    private Coroutine RemoteManualStackingRotateSpeedSliderTimerCoroutine;//堆料回转角度 倒计时重置协程

    public Slider PeekingRotateSpeedSliderChangeByDelayTime;//取料回转角度 滑条 -ljz
    public Toggle SetPeekingRotateSpeedSliderValueButton;//取料回转角度 设置按钮
    public Slider PeekingTiltSpeedSliderChangeByDelayTime;//取料俯仰角度 滑条
    public Toggle SetPeekingTiltSpeedSliderValueButton;//设置按钮
    public Slider RemoteManualStackingRotateSpeedSliderChangeByDelayTime;//堆料回转角度 滑条
    public Toggle SetRemoteManualStackingRotateSpeedSliderValueButton;//设置按钮

    private static bool isChangeByCode1 = false;//判断内容是人还是代码更改 默认是人为更改 -ljz
    private static bool isChangeByCode2 = false;
    private static bool isChangeByCode3 = false;

    public Button AutoPeekingStartButtonHighLight;//自动取料按钮高光改变 -ljz
    public Button AutoRemoteManualStackingStartButtonHighLight;//自动堆料按钮高光改变 -ljz

    public GameObject WorkScreenPanel; //遮罩面板
    public TextMeshProUGUI IntroductoryText;//操作字体
    public TextMeshProUGUI IntroductoryText2;//操作字体2
    public TextMeshProUGUI CountdownText;//倒计时字体
    private float SecondTime = 3.0f; //倒计时3s
    private bool SendState = true;

    private int updateThreeDModelTime = 60000;//三维模型更新时间
    private int updateState;//三维扫描时 系统堆取料状态
    public Toggle modelBtnTog;
    private bool modelBtnIsOnClick = false;
    public Toggle LeftLowerCollisionTog;
    private bool LeftLowerCollisionPreventionOnClick = false;//左侧料位计下俯防撞投入/切除
    public Toggle RightLowerCollisionTog;
    private bool RightLowerCollisionPreventionOnClick = false;
    public Toggle LeftRotateCollisionTog;
    private bool LeftRotateCollisionPreventionOnClick = false;
    public Toggle RightRotateCollisionTog;
    private bool RightRotateCollisionPreventionOnClick = false;

    public Toggle ScraperMotorStartsSameTimeTog;//刮板电机同时启动按钮
    public Toggle ScraperMotorStartsInTurnTimeTog;//刮板电机轮流启动按钮

    public struct recentTimeAlarms //告警信息与倒计时结构体
    {
        public string alarm;
        public string createTime;
    }
    private List<recentTimeAlarms> recentAlarmsList = new List<recentTimeAlarms>();

    private int emergence_switch = 0;

    //读取状态UI
    #region 堆取料状态

    #region 堆料状态

    #region 堆料状态/系统状态
    //运行状态
    public Toggle StackingDriverCabinEmergencyStopButton;
    public Toggle StackingElectricalRoomEmergencyStopButton;
    public Toggle StackingDriverCabinFireAlarmSignal;
    public Toggle StackingElectricalRoomFireAlarmSignal;
    public Toggle StackingService_State;//todo: 堆料检修， 状态页面，YCQ，20231229
    public TMP_Text StackingServiceTxt;//YCQ，20231230
    public TMP_Text FeedingServiceTxt;//YCQ，20231230

    //public TMP_Text EmergencyStopTxt;//YCQ，20231230
    public BlinkImage EmergencyStopImage;

    //电源合闸
    public Toggle StackingLowVoltageControlPowerClosure;
    public Toggle StackingLowVoltagePowerClosure;
    //操作模式
    public Toggle StackingControlModeLocalControl;
    public Toggle StackingControlModeRemoteControl;
    public Toggle ManualStackingRemote;//手动堆料
    public Toggle AutoStackingRemote;//自动堆料
    #endregion

    #region 堆料状态/堆料回转机构
    //运行状态
    public Toggle MaterialStackingRotaryInverterOperationMonitoring;
    public Toggle MaterialStackingRotaryBrakeOperationMonitoring;
    //限位状态
    public Toggle StackingRotationLeftLimitSwitch;
    public Toggle StackingRotationLeftLimit;
    public Toggle StackingRotationRightLimitSwitch;
    public Toggle StackingRotationRightLimit;
    public Toggle StackingRotationLeftAntiCollisionLimit;
    public Toggle StackingRotationRightAntiCollisionLimit;
    //故障报警
    public Toggle MaterialStackingRotaryMainCircuitBreaker;
    public Toggle MaterialStackingRotaryInverterFault;
    //电机电流与角度
    public TextMeshProUGUI MaterialStackingRotaryAngle;//堆料回转角度
    public TextMeshProUGUI M_MaterialStackingRotaryMotorCurrent;//堆料回转电机电流
    #endregion

    #region 堆料状态/堆料胶带机构
    //运行状态
    public Toggle MaterialConveyorMaterialOperationMonitoring;
    public Toggle MaterialStackingBeltBrakeOperationMonitoring;
    //限位状态
    public Toggle MaterialStackingBeltPrimaryDeviation;
    public Toggle MaterialStackingBeltSecondaryDeviation;
    public Toggle MaterialStackingBeltRopeProtection;
    public Toggle MaterialStackingBeltLongitudinalTearDetection;
    public Toggle MaterialStackingArmLeftCollisionSwitch;
    public Toggle MaterialStackingArmRightCollisionSwitch;
    public Toggle MaterialStackingBeltSpeedDetection;
    //故障报警
    public Toggle MaterialConveyorMotorMainCircuitBreaker;
    public Toggle MaterialConveyorMotorProtection;
    public Toggle MaterialStackingCoalBucketBlockageSwitch;
    public Toggle FluidCouplingTemperatureSwitch;
    //电机电流与角度
    public TextMeshProUGUI M_MaterialStackingBeltMotorCurrent;
    #endregion

    #region 堆料状态/自动堆料设备
    //故障报警
    public Toggle BucketLevelSensorFailureLeft;
    public Toggle BucketLevelSensorFailureRight;
    public Toggle BucketArmRotationEncoderFailure;
    //限位状态
    public Toggle ArmAngleBucketLeftTurnProhibition;
    public Toggle ArmAngleBucketRightTurnProhibition;
    public Toggle ArmAngleBucketLeftTurnDecelerationZone;
    public Toggle ArmAngleBucketRightTurnDecelerationZone;
    #endregion
    #endregion

    #region 取料状态
    #region 取料状态/系统状态
    //运行状态
    public Toggle FeedingDriverCabinEmergencyStopButton;
    public Toggle FeedingElectricalRoomEmergencyStopButton;
    public Toggle FeedingDriverCabinFireAlarmSignal;
    public Toggle FeedingElectricalRoomFireAlarmSignal;
    public Toggle FeedingService_State;//todo: 取料检修， 状态页面，YCQ，20231229
    //电源合闸
    public Toggle FeedingLowVoltageControlPowerClosure;
    public Toggle FeedingLowVoltagePowerClosure;
    //操作模式
    public Toggle FeedingControlModeLocalControl;
    public Toggle FeedingControlModeRemoteControl;
    public Toggle ManualFeedingRemote;//手动取料
    public Toggle AutoFeedingRemote;//自动取料
    #endregion

    #region 取料状态/取料刮板机构
    //运行状态
    public Toggle ScraperMotor1Operation;
    public Toggle ScraperMotor2Operation;
    public Toggle ScraperReducerLubricationOilPumpMonitoring;
    //限位状态
    public Toggle ScraperTensionLimitSwitch;
    public Toggle ScraperLooseLimitSwitch;
    //故障报警
    public Toggle ScraperMotorMainCircuitBreaker;
    public Toggle ScraperMotorOverheatProtection;
    public Toggle ScraperMotorFault;
    //电机电流与角度
    public TextMeshProUGUI M_ScraperMotor1Current;
    public TextMeshProUGUI M_ScraperMotor2Current;
    #endregion

    #region 取料状态/取料变幅机构
    //运行状态
    public Toggle AmplitudeInverterOperationMonitoring;
    public Toggle AmplitudePrimaryBrakeOperationMonitoring;
    public Toggle AmplitudeSecondaryBrakeOperationMonitoring;
    public Toggle AmplitudeAirCooledMotorOperationMonitoring;
    public Toggle AmplitudeUp;
    public Toggle AmplitudeLower;
    public Toggle AmplitudeSpeedSelectionSwitch;
    //限位状态
    public Toggle AmplitudeBrakeOpeningLimit;
    public Toggle AmplitudeUpLimitSwitch;
    public Toggle AmplitudeUpExtremeLimit;
    public Toggle AmplitudeLowerLimitSwitch;
    public Toggle AmplitudeLowerExtreme;
    //故障报警
    public Toggle AmplitudeMotorMainCircuitBreaker;
    public Toggle AmplitudeMotorOverheatProtection;
    public Toggle AmplitudeInverterFault;
    public Toggle AmplitudeWeightLimiterOverloadAlarm1;
    public Toggle AmplitudeWeightLimiterOverloadAlarm2;
    public Toggle AmplitudeBrakeResistorOverheatSwitch;
    //电机电流与角度
    public TextMeshProUGUI M_MaterialFetchingAmplitudeMotorCurrent;
    public TextMeshProUGUI MaterialFetchingPitchAngle;
    #endregion

    #region 取料状态/取料回转机构
    //运行状态
    public Toggle MaterialFetchingRotaryInverterOperationMonitoring;
    public Toggle MaterialFetchingRotaryBrakeOperationMonitoring;
    public Toggle MaterialFetchingRotaryTurnLeft;
    public Toggle MaterialFetchingRotaryTurnRight;
    public Toggle MaterialFetchingRotarySpeedSelectionSwitch;
    //限位状态
    public Toggle MaterialFetchingRotaryRightTurnLimit;
    public Toggle MaterialFetchingRotaryRightTurnExtremeLimit;
    public Toggle MaterialFetchingRotaryLeftTurnLimit;
    public Toggle MaterialFetchingRotaryLeftTurnExtremeLimit;
    public Toggle MaterialFetchingRotaryBrakeResistorTemperatureAlarm;
    public Toggle MaterialFetchingRotaryRightCollisionSwitch1;
    public Toggle MaterialFetchingRotaryRightCollisionSwitch2;
    public Toggle MaterialFetchingRotaryLeftCollisionSwitch1;
    public Toggle MaterialFetchingRotaryLeftCollisionSwitch2;
    //故障报警
    public Toggle MaterialFetchingRotaryMainCircuitBreaker;
    public Toggle MaterialFetchingRotaryMotorCircuitBreaker;
    public Toggle MaterialFetchingRotaryBrakeCircuitBreaker;
    public Toggle MaterialFetchingRotaryInverterFault;
    //取料电机电流与角度
    public TextMeshProUGUI M_MaterialFetchingRotaryMotorCurrent;
    public TextMeshProUGUI MaterialFetchingRotaryAngle;
    #endregion

    #region 取料刮板电流保护区间 堆取料编码器角度手动校准 -ljz
    public TMP_InputField OverloadCurrent;//超载电流
    public TMP_InputField RestoreCurrent;//恢复电流
    public TMP_InputField MaterialFetchingEncodeAngle;//堆料编码器角度
    public TMP_InputField PeekingFetchingEncodeAngle;//取料编码器角度
    #endregion

    #region 取料状态/自动取料设备
    //限位状态
    public Toggle LeftFrontVerticalLevelMeterScrapingProtection;
    public Toggle RightFrontVerticalLevelMeterScrapingProtection;
    public Toggle LeftLevelMeterProtectionForbidLeftTurn;
    public Toggle RightLevelMeterProtectionForbidRightTurn;
    public Toggle SmallAngleScraperDownProtection;
    public Toggle LargeAngleScraperUpProtection;
    public Toggle SmallAngleScraperDownDecelerationZone;
    public Toggle LargeAngleScraperUpDecelerationZone;
    public Toggle ArmAngleScraperLeftTurnProhibition;
    public Toggle ArmAngleScraperRightTurnProhibition;
    public Toggle ArmAngleScraperLeftTurnDecelerationZone;
    public Toggle ArmAngleScraperRightTurnDecelerationZone;
    //故障报警
    public Toggle ArmRotationEncoderFailure;
    public Toggle UltrasonicLevelSensorFailureBottom;
    public Toggle AngleSensorFailure;
    public Toggle ScraperMotorHighCurrentAlarm;
    public Toggle VerticalLevelSensorFailureLeftFront;
    public Toggle InclinedLevelSensorFailureLeftFront;
    public Toggle VerticalLevelSensorFailureLeftRear;
    public Toggle InclinedLevelSensorFailureLeftRear;
    public Toggle VerticalLevelSensorFailureRightFront;
    public Toggle InclinedLevelSensorFailureRightFront;
    public Toggle VerticalLevelSensorFailureRightRear;
    public Toggle InclinedLevelSensorFailureRightRear;
    #endregion
    #endregion
    #endregion

    #region 作业画面组件

    #region 作业画面/堆料作业
    //堆料状态
    public Toggle WorkScreen_ManualStackingRemote;
    public Toggle WorkScreen_AutoStackingRemote;
    public Toggle SystemAllowStackingCommand;
    public Toggle MaterialBeltOperation;
    public Toggle MaterialStackingRotaryTurnLeft;
    public Toggle MaterialStackingRotaryTurnRight;
    //电源合闸
    public Toggle RemotePowerClosure;
    public Toggle RemoteControlPowerClosure;
    public Toggle InterlockSwitchWithSystem;
    public Toggle ControlModeLocalControl;
    public Toggle EquipmentFault;
    public Toggle ControlModeRemoteControl;
    public Toggle EquipmentStackingOperation;
    public Toggle FaultAlarm;
    public Toggle EquipmentFetchingOperation;

    public GameObject PlcBG;
    public GameObject PlcIsConnect;//Plc连接状态
    public Toggle PlcIsConnectTog;
    private bool TaoIsConnect = false;
    private bool oncePlcError = true;//是否是第一次断开
    private bool startColorChange = false;
    private bool saveReconnectLogOnce = false;//重新连接日志 一次
    private bool saveStopLogOnce = true;//断开日志 一次

    private float colorChangeInterval = 0.2f; // 切换间隔为1秒
    private float timer = 0.0f;

    //todo:堆料检修、取料检修
    public Toggle StackingService;           //主界面状态， YCQ，20231229
    public Toggle FeedingService;         //主界面状态， YCQ，20231229

    public Toggle StackingServiceBtn;  //堆料检修按钮， YCQ，20231229
    public Toggle FeedingServiceBtn;   //取料检修按钮， YCQ，20231229

    //堆料电机电流与角度
    public TextMeshProUGUI WorkScreen_M_MaterialStackingBeltMotorCurrent;
    public TextMeshProUGUI WorkScreen_M_MaterialStackingRotaryMotorCurrent;
    public TextMeshProUGUI StackerArmRotationAngle;
    public TextMeshProUGUI LeftStackerMaterialLevel;
    public TextMeshProUGUI RightStackerMaterialLevel;
    //堆料胶带操作按钮
    public Toggle RemoteManualStackingBeltStartCommand;
    public Toggle RemoteManualStackingBeltStopCommand;
    public Toggle RemoteAutoStackingModeSelectionCommand;//自动堆料     与手动堆料按钮互斥
    public Toggle RemoteManualStackingModeSelectionCommand;//手动堆料   与自动堆料按钮互斥
    //public Button RemoteManualStackingRotateSpeedSelection;//堆料回转速度选择（0：慢速，1：快速）
    public TextMeshProUGUI RemoteManualStackingRotateSpeedSelectionText;//堆料回转速度文本框
    public TMP_InputField RemoteManualStackingRotateSpeedInput;//todo:堆料回转速度输入框
    public Slider RemoteManualStackingRotateSpeedSlider;// 堆料回转速度滑条
    public Slider PeekingRotateSpeedSlider;// 取料回转速度滑条
    public Slider PeekingTiltSpeedSlider;// 取料俯仰速度滑条

    //堆料回转操作按钮
    public Toggle RemoteManualStackingRotateLeftCommand;
    public Toggle RemoteManualStackingRotateStopCommand;
    public Toggle RemoteManualStackingRotateRightCommand;
    //自动堆料操作按钮
    public TMP_InputField AutoStackingStartAngle;
    public TMP_InputField AutoStackingEndAngle;
    public Button SetAutoStackingAngleButton;
    public TMP_InputField AutoStackingHeightSetting;
    public Button SetAutoStackingHeightButton;
    public Toggle AutoStackingPauseHMI;
    public Toggle AutoStackingStartHMI;
    public Toggle AutoStackingStopHMI;
    //堆料车辆设备按钮
    public Toggle RemotePowerSupplyCloseCommand;
    public Toggle RemotePowerSupplyOpenCommand;
    public Toggle RemoteControlPowerSupplyCloseCommand;
    public Toggle RemoteControlPowerSupplyOpenCommand;
    public Toggle RemoteInterlockLockCommand;       //（0：解锁，1：连锁）
    public Toggle RemoteInterUnlockLockCommand;     //（0：解锁，1：连锁）
    public Toggle RemoteIlluminationCloseCommand;
    public Toggle RemoteIlluminationOpenCommand;
    public Toggle RemoteBypassCommand;
    public Toggle RemoteFaultResetCommand;
    public Toggle RemoteManualStartBellCommand;
    public Toggle RemoteEmergencyStopCommand;

    #endregion

    #region 作业画面/取料作业
    //取料状态
    public Toggle MaterialControlAutoFetching;
    public Toggle MaterialControlManualFetching;
    public Toggle SystemAllowFetchingCommand;
    public Toggle WorkScreen_ScraperMotor1Operation;
    public Toggle WorkScreen_ScraperMotor2Operation;
    public Toggle WorkScreen_AmplitudeUp;
    public Toggle AutoFetchingLeftTurnSignal;
    public Toggle AutoFetchingRightTurnSignal;
    public Toggle WorkScreen_AmplitudeLower;
    //左右前后部垂直料位计
    public TextMeshProUGUI LeftFrontVerticalMaterialLevel;
    public TextMeshProUGUI LeftFrontInclinedMaterialLevel;
    public TextMeshProUGUI LeftRearVerticalMaterialLevel;
    public TextMeshProUGUI LeftRearInclinedMaterialLevel;
    public TextMeshProUGUI RightFrontVerticalMaterialLevel;
    public TextMeshProUGUI RightFrontInclinedMaterialLevel;
    public TextMeshProUGUI RightRearVerticalMaterialLevel;
    public TextMeshProUGUI RightRearInclinedMaterialLevel;
    //电机电流与角度
    public TextMeshProUGUI WorkScreen_MaterialFetchingPitchAngle;
    public TextMeshProUGUI WorkScreen_MaterialFetchingRotaryAngle;
    public TextMeshProUGUI WorkScreen_M_ScraperMotor1Current;
    public TextMeshProUGUI WorkScreen_M_ScraperMotor2Current;
    public TextMeshProUGUI WorkScreen_M_MaterialFetchingRotaryMotorCurrent;
    public TextMeshProUGUI M_MaterialFetchingPickupTiltMotorCurrent;
    //取料刮板操作按钮
    public Toggle RemoteManualScraperStartStopCommand;
    public Toggle RemoteManualScraperStopCommand;
    public Toggle RemoteAutoMaterialPickupModeSelectionCommand;//自动取料     与手动取料按钮互斥
    public Toggle RemoteManualMaterialPickupModeSelectionCommand;//手动取料   与自动取料按钮互斥
    //public Button RemoteManualMaterialPickupRotateSpeedSelection;//取料回转速度选择（0：慢速，1：快速）
    public TextMeshProUGUI RemoteManualMaterialPickupRotateSpeedSelectionText;//取料回转速度文本框
    public TMP_InputField RemoteManualMaterialPickupRotateSpeedInput;//todo:取料回转速度输入框
    //public Button RemoteManualMaterialPickupTiltSpeedSelection;//取料俯仰速度选择（0：慢速，1：快速）
    public TextMeshProUGUI RemoteManualMaterialPickupTiltSpeedSelectionText;//取料俯仰速度文本框
    public TMP_InputField RemoteManualMaterialPickupTiltSpeedInput;//todo:取料俯仰速度输入框
    //取料回转操作按钮
    public Toggle RemoteManualMaterialPickupTurnLeftCommand;
    public Toggle RemoteManualMaterialPickupRotateStopCommand;
    public Toggle RemoteManualMaterialPickupTurnRightCommand;
    //取料俯仰操作按钮
    public Toggle RemoteManualMaterialPickupTiltUpCommand;
    public Toggle RemoteManualMaterialPickupTiltStopCommand;
    public Toggle RemoteManualMaterialPickupTiltDownCommand;
    //自动取料料操作按钮
    public TMP_InputField AutoFeedingStartAngle;
    public TMP_InputField AutoFeedingEndAngle;
    public Button SetAutoFeedingAngleButton;
    public TMP_InputField ScrapingDepthSetting;
    //public TMP_InputField RotationEntryPoint;
    public Button SetScrapingDepthSettingButton;
    //public Button SetRotationEntryPointButton;
    public Toggle AutoFeedingPauseHMI;
    public Toggle ConfirmScraping;
    public Toggle AutoFeedingStartHMI;
    public Toggle IsManualRotation;

    //public Toggle ConfirmCurrentJobPointHMI;
    public Toggle AutoFeedingStopHMI;
    #endregion
    #endregion

    public AudioSource faultAlarmAudioSource;

    int SecondConfirmMode = 0;//自动堆料启动停止 与 自动取料启动停止 （0：关闭程序，1：自动堆料启动，2：自动堆料停止，3：自动取料启动，4：自动取料停止，5：堆料胶带启动，6：堆料胶带停止，7：启动自动堆料，8：启动手动堆料）

    #region 生命周期函数 -ljz
    void Awake()
    {
#if !UNITY_EDITOR
                        System.Diagnostics.Process.Start("HuangtaiPowerPlantControlSystem.exe", "31415926");//程序启动打开远程驱动系统
#endif


        ConnectPlcServer();//连接远程驱动服务器

        ConnectThreeDServer();//连接三维扫描服务器
        
    }

    // Start is called before the first frame update
    void Start()
    {
        AlarmStartListener();//绑定所有的故障报警监听事件
        ResetSpeedByDelayTime();//绑定所有速度重置事件 -ljz

        Thread threadThreedModel = new Thread(ProcessThreeD);//程序启动 给三维 发1 4
        threadThreedModel.Start();

        Thread threadPlc = new Thread(ProccessPlc);
        threadPlc.Start();
    }

    void FixedUpdate()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount % 10 == 0)
        {
            mutexFull.WaitOne();//请求互斥锁

            try
            {
                AlarmUpdateBoolValueListener();//实时获取报警状态的Bool值
                SetStateVisualizing();//实时更新二维数据的状态
                SetStateModelVisualizing();//实时更新三维堆取料机模型的状态

                UpdateTwoAutoData(); //进入软件更新参数 -ljz

                if (SendState) {
                    SendState = false;
                    Thread updateModelThread = new Thread(UpdateModel);//更新三维扫描堆取料状态 -ljz
                    updateModelThread.Start();
                }

                changeBtnColorByState();//更新底部四个按钮状态 -ljz
            }
            finally
            {
                mutexFull.ReleaseMutex(); // 当完成时释放Mutex。
            }

            UpdateAlarmData();//清理过期告警数据并展示 -ljz
            changeTogColorByTime();
        }
    }
    #endregion

    #region 将状态更新到UI界面上
    public void OperationLogs()//DateTime time,string name,string user
    {
        //1.将操作记录写入数据库

        //2.从数据库中读取操作记录更新到操作日志中

    }
    public void SetStateVisualizing()//将二维数据更新到界面上
    {
        #region 堆料状态
        //堆料状态
        //系统状态
        //运行状态
        StackingDriverCabinEmergencyStopButton.isOn = fullVariables.DriverCabinEmergencyStopButton;
        StackingElectricalRoomEmergencyStopButton.isOn = fullVariables.ElectricalRoomEmergencyStopButton;
        StackingDriverCabinFireAlarmSignal.isOn = fullVariables.DriverCabinFireAlarmSignal;
        StackingElectricalRoomFireAlarmSignal.isOn = fullVariables.ElectricalRoomFireAlarmSignal;

        //EmergencyStopTxt.GetComponent<BlinkText>().Blinking =  fullVariables.RemoteEmergencyStopCommand;//YCQ, 20231230
        EmergencyStopImage.Blinking = fullVariables.RemoteEmergencyStopCommand;

        StackingService_State.isOn = fullVariables.MaterialStackerMaintenanceButton;   //YCQ, 20231229
        StackingServiceBtn.isOn = fullVariables.MaterialStackerMaintenanceButton;   //YCQ, 20231229
        if (fullVariables.MaterialStackerMaintenanceButton)
            StackingServiceTxt.text = "堆料检修"; //YCQ, 20231230
        else
            StackingServiceTxt.text = "堆料投入"; //YCQ, 20231230

        //电源合闸
        StackingLowVoltageControlPowerClosure.isOn = fullVariables.LowVoltageControlPowerClosure;
        StackingLowVoltagePowerClosure.isOn = fullVariables.LowVoltagePowerClosure;
        //操作模式
        StackingControlModeLocalControl.isOn = fullVariables.ControlModeLocalControl;
        StackingControlModeRemoteControl.isOn = fullVariables.ControlModeRemoteControl;
        ManualStackingRemote.isOn = fullVariables.ManualStackingRemote;//手动堆料状态显示

        AutoStackingRemote.isOn = fullVariables.AutoStackingRemote;//自动堆料状态显示

        //堆料回转机构
        //运行状态
        MaterialStackingRotaryInverterOperationMonitoring.isOn = fullVariables.MaterialStackingRotaryInverterOperationMonitoring;
        MaterialStackingRotaryBrakeOperationMonitoring.isOn = fullVariables.MaterialStackingRotaryBrakeOperationMonitoring;

        //限位状态
        //StackingRotationLeftLimitSwitch.isOn = fullVariables.StackingRotationLeftLimitSwitch;

        //StackingRotationLeftLimit.isOn = fullVariables.StackingRotationLeftLimit;

        //StackingRotationRightLimitSwitch.isOn = fullVariables.StackingRotationRightLimitSwitch;

        //StackingRotationRightLimit.isOn = fullVariables.StackingRotationRightLimit;

        StackingRotationLeftAntiCollisionLimit.isOn = fullVariables.StackingRotationLeftAntiCollisionLimit;

        StackingRotationRightAntiCollisionLimit.isOn = fullVariables.StackingRotationRightAntiCollisionLimit;
        //故障报警
        MaterialStackingRotaryMainCircuitBreaker.isOn = fullVariables.MaterialStackingRotaryMainCircuitBreaker;

        MaterialStackingRotaryInverterFault.isOn = !fullVariables.MaterialStackingRotaryInverterFault;//堆料回转机构变频器故障
        //电机电流与角度
        MaterialStackingRotaryAngle.text = "" + fullVariables.StackerArmRotationAngle.ToString("F2");   //堆料回转角度

        M_MaterialStackingRotaryMotorCurrent.text = "" + fullVariables.M_MaterialStackingRotaryMotorCurrent;    //堆料回转电机电流


        //堆料胶带机构
        //运行状态
        MaterialConveyorMaterialOperationMonitoring.isOn = fullVariables.MaterialConveyorMaterialOperationMonitoring;

        MaterialStackingBeltBrakeOperationMonitoring.isOn = fullVariables.MaterialStackingBeltBrakeOperationMonitoring;
        //限位状态
        //MaterialStackingBeltPrimaryDeviation.isOn = fullVariables.MaterialStackingBeltPrimaryDeviation;
        //MaterialStackingBeltSecondaryDeviation.isOn = fullVariables.MaterialStackingBeltSecondaryDeviation;
        //MaterialStackingBeltRopeProtection.isOn = fullVariables.MaterialStackingBeltRopeProtection;
        //MaterialStackingBeltLongitudinalTearDetection.isOn = fullVariables.MaterialStackingBeltLongitudinalTearDetection;
        //MaterialStackingArmLeftCollisionSwitch.isOn = fullVariables.MaterialStackingArmLeftCollisionSwitch;
        //MaterialStackingArmRightCollisionSwitch.isOn = fullVariables.MaterialStackingArmRightCollisionSwitch;
        MaterialStackingBeltSpeedDetection.isOn = fullVariables.MaterialStackingBeltSpeedDetection;//堆料胶带速度检测开关
        //故障报警
        MaterialConveyorMotorMainCircuitBreaker.isOn = fullVariables.MaterialConveyorMotorMainCircuitBreaker;
        MaterialConveyorMotorProtection.isOn = fullVariables.MaterialConveyorMotorProtection;
        MaterialStackingCoalBucketBlockageSwitch.isOn = !fullVariables.MaterialStackingCoalBucketBlockageSwitch;//堆料落料斗堵煤开关
        FluidCouplingTemperatureSwitch.isOn = fullVariables.FluidCouplingTemperatureSwitch;
        //电机电流与角度
        M_MaterialStackingBeltMotorCurrent.text = "" + fullVariables.M_MaterialStackingBeltMotorCurrent;



        //自动堆料设备
        //故障报警
        BucketLevelSensorFailureLeft.isOn = fullVariables.BucketLevelSensorFailureLeft;
        BucketLevelSensorFailureRight.isOn = fullVariables.BucketLevelSensorFailureRight;
        BucketArmRotationEncoderFailure.isOn = fullVariables.BucketArmRotationEncoderFailure;
        //限位状态
        ArmAngleBucketLeftTurnProhibition.isOn = !fullVariables.ArmAngleBucketLeftTurnProhibition;
        ArmAngleBucketRightTurnProhibition.isOn = !fullVariables.ArmAngleBucketRightTurnProhibition;
        ArmAngleBucketLeftTurnDecelerationZone.isOn = !fullVariables.ArmAngleBucketLeftTurnDecelerationZone;
        ArmAngleBucketRightTurnDecelerationZone.isOn = !fullVariables.ArmAngleBucketRightTurnDecelerationZone;
        #endregion

        #region 取料状态
        //取料状态
        //系统状态
        //运行状态
        FeedingDriverCabinEmergencyStopButton.isOn = fullVariables.DriverCabinEmergencyStopButton;
        FeedingElectricalRoomEmergencyStopButton.isOn = fullVariables.ElectricalRoomEmergencyStopButton;
        FeedingDriverCabinFireAlarmSignal.isOn = fullVariables.DriverCabinFireAlarmSignal;
        FeedingElectricalRoomFireAlarmSignal.isOn = fullVariables.ElectricalRoomFireAlarmSignal;

        FeedingService_State.isOn = fullVariables.MaterialFeederMaintenanceButton;   //YCQ, 20231229
        FeedingServiceBtn.isOn = fullVariables.MaterialFeederMaintenanceButton;   //YCQ, 20231229
        if (fullVariables.MaterialFeederMaintenanceButton)
            FeedingServiceTxt.text = "取料检修"; //YCQ, 20231230
        else
            FeedingServiceTxt.text = "取料投入"; //YCQ, 20231230


        //电源合闸
        FeedingLowVoltageControlPowerClosure.isOn = fullVariables.LowVoltageControlPowerClosure;
        FeedingLowVoltagePowerClosure.isOn = fullVariables.LowVoltagePowerClosure;
        //操作模式
        FeedingControlModeLocalControl.isOn = fullVariables.ControlModeLocalControl;
        FeedingControlModeRemoteControl.isOn = fullVariables.ControlModeRemoteControl;
        ManualFeedingRemote.isOn = fullVariables.ManualFeedingRemote;//手动取料状态显示
        AutoFeedingRemote.isOn = fullVariables.AutoFeedingRemote;//自动取料状态显示


        // 取料状态/取料刮板机构
        //运行状态
        ScraperMotor1Operation.isOn = fullVariables.ScraperMotor1Operation;
        ScraperMotor2Operation.isOn = fullVariables.ScraperMotor2Operation;
        ScraperReducerLubricationOilPumpMonitoring.isOn = fullVariables.ScraperReducerLubricationOilPumpMonitoring;
        //限位状态
        ScraperTensionLimitSwitch.isOn = fullVariables.ScraperTensionLimitSwitch;//刮板张紧限位
        ScraperLooseLimitSwitch.isOn = fullVariables.ScraperLooseLimitSwitch;//刮板过松限位
        //故障报警
        ScraperMotorMainCircuitBreaker.isOn = fullVariables.ScraperMotorMainCircuitBreaker;
        ScraperMotorOverheatProtection.isOn = fullVariables.ScraperMotorOverheatProtection;
        ScraperMotorFault.isOn = fullVariables.MaterialScraperMalfunction;

        //电机电流与角度
        M_ScraperMotor1Current.text = "" + fullVariables.M_ScraperMotor1Current;
        M_ScraperMotor2Current.text = "" + fullVariables.M_ScraperMotor2Current;

        M_ScraperMotor1Current.GetComponent<BlinkText>().Blinking = (fullVariables.M_ScraperMotor1Current >= 140);
        M_ScraperMotor2Current.GetComponent<BlinkText>().Blinking = (fullVariables.M_ScraperMotor2Current >= 140);

        //取料状态/取料变幅机构
        //运行状态
        AmplitudeInverterOperationMonitoring.isOn = fullVariables.AmplitudeInverterOperationMonitoring;
        AmplitudePrimaryBrakeOperationMonitoring.isOn = fullVariables.AmplitudePrimaryBrakeOperationMonitoring;
        AmplitudeSecondaryBrakeOperationMonitoring.isOn = fullVariables.AmplitudeSecondaryBrakeOperationMonitoring;
        AmplitudeAirCooledMotorOperationMonitoring.isOn = fullVariables.AmplitudeAirCooledMotorOperationMonitoring;
        AmplitudeUp.isOn = fullVariables.AmplitudeUp;
        AmplitudeLower.isOn = fullVariables.AmplitudeLower;
        AmplitudeSpeedSelectionSwitch.isOn = fullVariables.AmplitudeSpeedSelectionSwitch;
        //限位状态
        AmplitudeBrakeOpeningLimit.isOn = fullVariables.AmplitudeBrakeOpeningLimit;//制动器打开限位
        AmplitudeUpLimitSwitch.isOn = fullVariables.AmplitudeUpLimitSwitch;//取料变幅上仰限位
        AmplitudeUpExtremeLimit.isOn = fullVariables.AmplitudeUpExtremeLimit;//取料变幅上仰极限
        AmplitudeLowerLimitSwitch.isOn = fullVariables.AmplitudeLowerLimitSwitch;//取料变幅下俯限位
        AmplitudeLowerExtreme.isOn = fullVariables.AmplitudeLowerExtreme;//取料变幅下俯极限
        //故障报警
        AmplitudeMotorMainCircuitBreaker.isOn = fullVariables.AmplitudeMotorMainCircuitBreaker;
        AmplitudeMotorOverheatProtection.isOn = fullVariables.AmplitudeMotorOverheatProtection;//点击变幅过热保护
        AmplitudeInverterFault.isOn = !fullVariables.AmplitudeInverterFault;//变频器故障
        AmplitudeWeightLimiterOverloadAlarm1.isOn = !fullVariables.AmplitudeWeightLimiterOverloadAlarm1;//变幅起重限制器报警1
        AmplitudeWeightLimiterOverloadAlarm2.isOn = !fullVariables.AmplitudeWeightLimiterOverloadAlarm2;//变幅起重限制器报警2
        AmplitudeBrakeResistorOverheatSwitch.isOn = !fullVariables.AmplitudeBrakeResistorOverheatSwitch;//变幅制动电阻超温报警
        //取料电机电流与角度
        M_MaterialFetchingAmplitudeMotorCurrent.text = "" + fullVariables.M_MaterialFetchingAmplitudeMotorCurrent;//取料变幅电机电流
        MaterialFetchingPitchAngle.text = "" + fullVariables.Inclinometer.ToString("F2");//取料变幅角度

        //取料状态/取料回转机构
        //运行状态
        MaterialFetchingRotaryInverterOperationMonitoring.isOn = fullVariables.MaterialFetchingRotaryInverterOperationMonitoring;
        MaterialFetchingRotaryBrakeOperationMonitoring.isOn = fullVariables.MaterialFetchingRotaryBrakeOperationMonitoring;
        MaterialFetchingRotaryTurnLeft.isOn = fullVariables.MaterialFetchingRotaryTurnLeft;
        MaterialFetchingRotaryTurnRight.isOn = fullVariables.MaterialFetchingRotaryTurnRight;
        MaterialFetchingRotarySpeedSelectionSwitch.isOn = fullVariables.MaterialFetchingRotarySpeedSelectionSwitch;
        //限位状态
        MaterialFetchingRotaryRightTurnLimit.isOn = fullVariables.MaterialFetchingRotaryRightTurnLimit;
        MaterialFetchingRotaryRightTurnExtremeLimit.isOn = fullVariables.MaterialFetchingRotaryRightTurnExtremeLimit;
        MaterialFetchingRotaryLeftTurnLimit.isOn = fullVariables.MaterialFetchingRotaryLeftTurnLimit;
        MaterialFetchingRotaryLeftTurnExtremeLimit.isOn = fullVariables.MaterialFetchingRotaryLeftTurnExtremeLimit;
        MaterialFetchingRotaryBrakeResistorTemperatureAlarm.isOn = fullVariables.MaterialFetchingRotaryBrakeResistorTemperatureAlarm;
        MaterialFetchingRotaryRightCollisionSwitch1.isOn = fullVariables.MaterialFetchingRotaryRightCollisionSwitch1;
        MaterialFetchingRotaryRightCollisionSwitch2.isOn = fullVariables.MaterialFetchingRotaryRightCollisionSwitch2;
        MaterialFetchingRotaryLeftCollisionSwitch1.isOn = fullVariables.MaterialFetchingRotaryLeftCollisionSwitch1;
        MaterialFetchingRotaryLeftCollisionSwitch2.isOn = fullVariables.MaterialFetchingRotaryLeftCollisionSwitch2;
        //故障报警
        MaterialFetchingRotaryMainCircuitBreaker.isOn = fullVariables.MaterialFetchingRotaryMainCircuitBreaker;
        MaterialFetchingRotaryMotorCircuitBreaker.isOn = fullVariables.MaterialFetchingRotaryMotorCircuitBreaker;
        MaterialFetchingRotaryBrakeCircuitBreaker.isOn = fullVariables.MaterialFetchingRotaryBrakeCircuitBreaker;
        MaterialFetchingRotaryInverterFault.isOn = !fullVariables.MaterialFetchingRotaryInverterFault;//取料回转变频器故障
        //电机电流与角度
        M_MaterialFetchingRotaryMotorCurrent.text = "" + fullVariables.M_MaterialFetchingRotaryMotorCurrent;
        MaterialFetchingRotaryAngle.text = "" + fullVariables.MaterialArmRotationAngle.ToString("F2");     //取料回转角度

        // 取料状态/自动取料设备
        //限位状态
        LeftFrontVerticalLevelMeterScrapingProtection.isOn = !fullVariables.LeftFrontVerticalLevelMeterScrapingProtection;
        RightFrontVerticalLevelMeterScrapingProtection.isOn = !fullVariables.RightFrontVerticalLevelMeterScrapingProtection;
        LeftLevelMeterProtectionForbidLeftTurn.isOn = !fullVariables.LeftLevelMeterProtectionForbidLeftTurn;
        RightLevelMeterProtectionForbidRightTurn.isOn = !fullVariables.RightLevelMeterProtectionForbidRightTurn;
        SmallAngleScraperDownProtection.isOn = !fullVariables.SmallAngleScraperDownProtection;
        LargeAngleScraperUpProtection.isOn = !fullVariables.LargeAngleScraperUpProtection;
        SmallAngleScraperDownDecelerationZone.isOn = !fullVariables.SmallAngleScraperDownDecelerationZone;
        LargeAngleScraperUpDecelerationZone.isOn = !fullVariables.LargeAngleScraperUpDecelerationZone;
        ArmAngleScraperLeftTurnProhibition.isOn = !fullVariables.ArmAngleScraperLeftTurnProhibition;
        ArmAngleScraperRightTurnProhibition.isOn = !fullVariables.ArmAngleScraperRightTurnProhibition;
        ArmAngleScraperLeftTurnDecelerationZone.isOn = !fullVariables.ArmAngleScraperLeftTurnDecelerationZone;
        ArmAngleScraperRightTurnDecelerationZone.isOn = !fullVariables.ArmAngleScraperRightTurnDecelerationZone;
        //故障报警
        ArmRotationEncoderFailure.isOn = fullVariables.ArmRotationEncoderFailure;
        UltrasonicLevelSensorFailureBottom.isOn = fullVariables.UltrasonicLevelSensorFailureBottom;
        AngleSensorFailure.isOn = fullVariables.AngleSensorFailure;
        ScraperMotorHighCurrentAlarm.isOn = fullVariables.ScraperMotorHighCurrentAlarm;
        VerticalLevelSensorFailureLeftFront.isOn = fullVariables.VerticalLevelSensorFailureLeftFront;
        InclinedLevelSensorFailureLeftFront.isOn = fullVariables.InclinedLevelSensorFailureLeftFront;
        VerticalLevelSensorFailureLeftRear.isOn = fullVariables.VerticalLevelSensorFailureLeftRear;
        InclinedLevelSensorFailureLeftRear.isOn = fullVariables.InclinedLevelSensorFailureLeftRear;
        VerticalLevelSensorFailureRightFront.isOn = fullVariables.VerticalLevelSensorFailureRightFront;
        InclinedLevelSensorFailureRightFront.isOn = fullVariables.InclinedLevelSensorFailureRightFront;
        VerticalLevelSensorFailureRightRear.isOn = fullVariables.VerticalLevelSensorFailureRightRear;
        InclinedLevelSensorFailureRightRear.isOn = fullVariables.InclinedLevelSensorFailureRightRear;
        #endregion

        #region 作业画面/堆料作业
        //作业画面/堆料作业
        //堆料状态
        WorkScreen_ManualStackingRemote.isOn = fullVariables.ManualStackingRemote;
        WorkScreen_AutoStackingRemote.isOn = fullVariables.AutoStackingRemote;
        SystemAllowStackingCommand.isOn = fullVariables.SystemAllowStackingCommand;
        MaterialBeltOperation.isOn = fullVariables.MaterialBeltOperation;
        MaterialStackingRotaryTurnLeft.isOn = fullVariables.MaterialStackingRotaryTurnLeft;
        MaterialStackingRotaryTurnRight.isOn = fullVariables.MaterialStackingRotaryTurnRight;
        //电源合闸
        RemotePowerClosure.isOn = fullVariables.LowVoltagePowerClosure;//动力电源合闸
        RemoteControlPowerClosure.isOn = fullVariables.LowVoltageControlPowerClosure;//控制电源合闸
        InterlockSwitchWithSystem.isOn = fullVariables.RemoteInterlockLockCommand;//与系统联锁
        ControlModeLocalControl.isOn = fullVariables.ControlModeLocalControl;//本地控制
        EquipmentFault.isOn = fullVariables.CircularStackFault;//设备故障
        ControlModeRemoteControl.isOn = fullVariables.ControlModeRemoteControl;//远程控制F
        EquipmentStackingOperation.isOn = fullVariables.EquipmentStackingOperation;//设备堆料运行F
        FaultAlarm.isOn = fullVariables.FaultAlarm;//蜂鸣报警

        ScraperMotorStartsSameTimeTog.isOn = fullVariables.ManualSetScraperMotorStartButton;//同时启动
        ScraperMotorStartsInTurnTimeTog.isOn = !fullVariables.ManualSetScraperMotorStartButton;//轮流启动

        if (!TaoIsConnect)//Plc(远程驱动)断开 
        {
            
            plcIsStopWork();
        }
        else
        {
            Debug.Log(fullVariables.PLCSignal);
            if (fullVariables.PLCSignal == false)//false为plc与远程驱动断开 
            {
                plcIsStopWork2();
            }
            else
            {
                startColorChange = false;
                oncePlcError = true;
                PlcBG.SetActive(true);
                PlcIsConnect.SetActive(false);
            }
        }

        EquipmentFetchingOperation.isOn = fullVariables.EquipmentFetchingOperation;//设备取料运行
        //电机电流与角度
        WorkScreen_M_MaterialStackingBeltMotorCurrent.text = "" + fullVariables.M_MaterialStackingBeltMotorCurrent;
        WorkScreen_M_MaterialStackingRotaryMotorCurrent.text = "" + fullVariables.M_MaterialStackingRotaryMotorCurrent;
        StackerArmRotationAngle.text = "" + fullVariables.StackerArmRotationAngle.ToString("F2");      //堆料臂回转角度
        LeftStackerMaterialLevel.text = "" + fullVariables.LeftStackerMaterialLevel.ToString("F2");    //左侧料位高度
        RightStackerMaterialLevel.text = "" + fullVariables.RightStackerMaterialLevel.ToString("F2");      //右侧料位高度
        //堆料胶带操作按钮
        RemoteManualStackingBeltStartCommand.isOn = fullVariables.MaterialBeltOperation;
        RemoteManualStackingBeltStopCommand.isOn = !fullVariables.MaterialBeltOperation;
        RemoteAutoStackingModeSelectionCommand.isOn = fullVariables.AutoStackingRemote;//自动堆料     与手动堆料按钮互斥
        RemoteManualStackingModeSelectionCommand.isOn = fullVariables.ManualStackingRemote;//手动堆料   与自动堆料按钮互斥
        //堆料回转操作按钮
        RemoteManualStackingRotateLeftCommand.isOn = fullVariables.MaterialStackingRotaryTurnLeft;
        if (!fullVariables.MaterialStackingRotaryTurnLeft && !fullVariables.MaterialStackingRotaryTurnRight)//如果左转与右转没有运行就设置暂停
        {
            RemoteManualStackingRotateStopCommand.isOn = true;//fullVariables.RemoteManualStackingRotateStopCommand;
        }
        else
        {
            RemoteManualStackingRotateStopCommand.isOn = false;//fullVariables.RemoteManualStackingRotateStopCommand;
        }
        RemoteManualStackingRotateRightCommand.isOn = fullVariables.MaterialStackingRotaryTurnRight;
        //自动堆料操作按钮
        AutoStackingPauseHMI.isOn = fullVariables.AutoStackingPauseHMI;
        //AutoStackingStartHMI.isOn = fullVariables.AutoStackingStartHMI;
        //AutoStackingStopHMI.isOn = fullVariables.AutoStackingStopHMI;
        //堆料行车设备按钮
        RemotePowerSupplyCloseCommand.isOn = fullVariables.LowVoltagePowerClosure;//动力电源合闸
        RemotePowerSupplyOpenCommand.isOn = !fullVariables.LowVoltagePowerClosure;//动力电源分闸
        RemoteControlPowerSupplyCloseCommand.isOn = fullVariables.LowVoltageControlPowerClosure;//控制电源合闸
        RemoteControlPowerSupplyOpenCommand.isOn = !fullVariables.LowVoltageControlPowerClosure;//控制电源分闸
        RemoteInterlockLockCommand.isOn = fullVariables.RemoteInterlockLockCommand;//与系统联锁       //（0：解锁，1：连锁）
        RemoteInterUnlockLockCommand.isOn = !fullVariables.RemoteInterlockLockCommand;//与系统解锁     //（0：解锁，1：连锁）
        RemoteIlluminationCloseCommand.isOn = fullVariables.LightingPowerClosure;//照明开启
        RemoteIlluminationOpenCommand.isOn = !fullVariables.LightingPowerClosure;//照明关闭
        RemoteBypassCommand.isOn = fullVariables.RemoteBypassCommand;//旁路功能
        RemoteFaultResetCommand.isOn = fullVariables.RemoteFaultResetCommand;//故障重置
        RemoteManualStartBellCommand.isOn = fullVariables.RemoteManualStartBellCommand;//汽车报警

        RemoteEmergencyStopCommand.isOn = fullVariables.RemoteEmergencyStopCommand;//急停



        StackingService.isOn = fullVariables.MaterialStackerMaintenanceButton;//堆料检修 ，YCQ，20231229
        FeedingService.isOn = fullVariables.MaterialFeederMaintenanceButton;//取料检修，YCQ，20231229

        #endregion

        #region 作业画面//取料作业
        //作业画面/取料作业
        //取料状态
        MaterialControlAutoFetching.isOn = fullVariables.AutoFeedingRemote;//自动取料
        MaterialControlManualFetching.isOn = fullVariables.ManualFeedingRemote;//手动取料
        SystemAllowFetchingCommand.isOn = fullVariables.SystemAllowFetchingCommand;//允许设备取料
        WorkScreen_ScraperMotor1Operation.isOn = fullVariables.ScraperMotor1Operation;//刮板电机1运行
        WorkScreen_ScraperMotor2Operation.isOn = fullVariables.ScraperMotor2Operation;//刮板电机2运行
        WorkScreen_AmplitudeUp.isOn = fullVariables.AmplitudeUp;//变幅上仰运行
        AutoFetchingLeftTurnSignal.isOn = fullVariables.MaterialFetchingRotaryTurnLeft;//取料左转运行
        AutoFetchingRightTurnSignal.isOn = fullVariables.MaterialFetchingRotaryTurnRight;//取料右转运行
        WorkScreen_AmplitudeLower.isOn = fullVariables.AmplitudeLower;//变幅下俯运行
        //左右前后部垂直料位计
        LeftFrontVerticalMaterialLevel.text = "" + fullVariables.LeftFrontVerticalMaterialLevel.ToString("F2");
        LeftFrontInclinedMaterialLevel.text = "" + fullVariables.LeftFrontInclinedMaterialLevel.ToString("F2");
        LeftRearVerticalMaterialLevel.text = "" + fullVariables.LeftRearVerticalMaterialLevel.ToString("F2");
        LeftRearInclinedMaterialLevel.text = "" + fullVariables.LeftRearInclinedMaterialLevel.ToString("F2");
        RightFrontVerticalMaterialLevel.text = "" + fullVariables.RightFrontVerticalMaterialLevel.ToString("F2");
        RightFrontInclinedMaterialLevel.text = "" + fullVariables.RightFrontInclinedMaterialLevel.ToString("F2");
        RightRearVerticalMaterialLevel.text = "" + fullVariables.RightRearVerticalMaterialLevel.ToString("F2");
        RightRearInclinedMaterialLevel.text = "" + fullVariables.RightRearInclinedMaterialLevel.ToString("F2");
        //电机电流与角度
        WorkScreen_MaterialFetchingPitchAngle.text = "" + fullVariables.Inclinometer.ToString("F2");//取料俯仰角度
        WorkScreen_MaterialFetchingRotaryAngle.text = "" + fullVariables.MaterialArmRotationAngle.ToString("F2");//取料回转角度
        WorkScreen_M_ScraperMotor1Current.text = "" + fullVariables.M_ScraperMotor1Current;
        WorkScreen_M_ScraperMotor2Current.text = "" + fullVariables.M_ScraperMotor2Current;

        WorkScreen_M_ScraperMotor1Current.GetComponent<BlinkText>().Blinking = (fullVariables.M_ScraperMotor1Current >= 140);
        WorkScreen_M_ScraperMotor2Current.GetComponent<BlinkText>().Blinking = (fullVariables.M_ScraperMotor2Current >= 140);

        WorkScreen_M_MaterialFetchingRotaryMotorCurrent.text = "" + fullVariables.M_MaterialFetchingRotaryMotorCurrent;//取料回转电机电流
        M_MaterialFetchingPickupTiltMotorCurrent.text = "" + fullVariables.M_MaterialFetchingAmplitudeMotorCurrent;//取料俯仰电机电流
        //取料刮板操作按钮
        if (fullVariables.ScraperMotor1Operation && fullVariables.ScraperMotor2Operation)//如果刮板电机1与刮板电机2运行则刮板运行，否则刮板停止
        {
            RemoteManualScraperStartStopCommand.isOn = true;//fullVariables.RemoteManualScraperStartStopCommand;
            RemoteManualScraperStopCommand.isOn = false;//fullVariables.RemoteManualScraperStopCommand;
        }
        else
        {
            RemoteManualScraperStartStopCommand.isOn = false;//fullVariables.RemoteManualScraperStartStopCommand;
            RemoteManualScraperStopCommand.isOn = true;//fullVariables.RemoteManualScraperStopCommand;
        }

        RemoteAutoMaterialPickupModeSelectionCommand.isOn = fullVariables.AutoFeedingRemote;//自动取料     与手动取料按钮互斥

        RemoteManualMaterialPickupModeSelectionCommand.isOn = fullVariables.ManualFeedingRemote;//手动取料   与自动取料按钮互斥
        //取料回转操作按钮
        RemoteManualMaterialPickupTurnLeftCommand.isOn = fullVariables.MaterialFetchingRotaryTurnLeft;
        if (!fullVariables.MaterialFetchingRotaryTurnLeft && !fullVariables.MaterialFetchingRotaryTurnRight)//如果左转与右转没有运行就设置暂停
        {
            RemoteManualMaterialPickupRotateStopCommand.isOn = true;//fullVariables.RemoteManualMaterialPickupRotateStopCommand;
        }
        else
        {
            RemoteManualMaterialPickupRotateStopCommand.isOn = false;//fullVariables.RemoteManualMaterialPickupRotateStopCommand;
        }
        RemoteManualMaterialPickupTurnRightCommand.isOn = fullVariables.MaterialFetchingRotaryTurnRight;
        //取料俯仰操作按钮
        RemoteManualMaterialPickupTiltUpCommand.isOn = fullVariables.AmplitudeUp;
        if (!fullVariables.AmplitudeUp && !fullVariables.AmplitudeLower)//如果上仰与下附没有运行就设置暂停
        {
            RemoteManualMaterialPickupTiltStopCommand.isOn = true;//fullVariables.RemoteManualMaterialPickupTiltStopCommand;
        }
        else
        {
            RemoteManualMaterialPickupTiltStopCommand.isOn = false;//fullVariables.RemoteManualMaterialPickupTiltStopCommand;
        }
        RemoteManualMaterialPickupTiltDownCommand.isOn = fullVariables.AmplitudeLower;
        //自动取料料操作按钮
        ConfirmScraping.isOn = fullVariables.ConfirmScrapingButton;
        AutoFeedingPauseHMI.isOn = fullVariables.AutoFeedingPauseHMI;
        //AutoFeedingStartHMI.isOn = fullVariables.AutoFeedingStartHMI;
        IsManualRotation.isOn = fullVariables.IsManualRotation;

        //ConfirmCurrentJobPointHMI.isOn = fullVariables.ConfirmCurrentJobPointHMI;
        //AutoFeedingStopHMI.isOn = fullVariables.AutoFeedingStopHMI;
        #endregion
    }
    public void SetStateModelVisualizing()//将三维数据更新到界面上
    {
        stackingArmMesh.transform.eulerAngles = new Vector3(-90, 0, fullVariables.StackerArmRotationAngle);//fullVariables.StackerArmRotationAngle      UnityEngine.Random.Range(0, 120)
        feedingArmMesh.transform.eulerAngles = new Vector3(-90, 0, fullVariables.MaterialArmRotationAngle + 90);//fullVariables.MaterialArmRotationAngle
        scraperMesh.transform.localEulerAngles = new Vector3(0, -fullVariables.Inclinometer, 90);//fullVariables.Inclinometer
        angleDisplayer.UpdateTexts(
            fullVariables.StackerArmRotationAngle,
            fullVariables.MaterialArmRotationAngle,
            fullVariables.Inclinometer
            );
    }
    public void ShowCanvasParent()//隐藏主界面的UI框架
    {
        UISystem.Instance.ShowCanvasParent();
    }

    #endregion

    #region 作业画面按钮交互事件

    private void OperationLogs(string operationinfo)//增加操作日志
    {
        //1.将操作日志写入数据库, 此处登录用户怎么传入？？,  YCQ, 20231223
        string user = "";
        if (DataManager.Instance.CurrentAccount!=null) {
            user = DataManager.Instance.CurrentAccount.name;
        }
        
        string sql = String.Format("insert into logs (time, info, operator) values('{0}','{1}','{2}');", DateTime.Now, operationinfo, user, Encoding.UTF8);
        int ret = MySqlHelper.ExecuteSql(sql);
        //记录操作日志，将数据存入数据库
    }
    private void DeleteThreeMonthData() //删除电流表 日志表 和告警表三个月前的数据 -ljz
    {
        string sql = String.Format("delete from warning where time < '{0}' ;", DateTime.Now.AddMonths(-3), Encoding.UTF8);
        string sql1 = String.Format("delete from logs where time < '{0}' ;", DateTime.Now.AddMonths(-3), Encoding.UTF8);
        string sql2 = String.Format("delete from criticalparameter where time < '{0}' ;", DateTime.Now.AddMonths(-3), Encoding.UTF8);
        int ret = MySqlHelper.ExecuteSql(sql);
        int ret1 = MySqlHelper.ExecuteSql(sql1);
        int ret2 = MySqlHelper.ExecuteSql(sql2);
    }

    #region 堆料作业

    //堆料回转速度快慢速开关
    public void RemoteManualStackingRotateSpeedSelectionEvent()//堆料回转速度选择（0：慢速，1：快速）
    {
        try
        {
            if (fullVariables.RemoteManualStackingRotateSpeedSelection == false)//不确定是不是这个变量???
            {
                Debug.Log("堆料回转速度慢速，切换为快速");
                SendCommand(6, 2, "ROTATE_MODE");//向服务器发送指令  指令内容为flase
                RemoteManualStackingRotateSpeedSelectionText.text = "堆料回转速度：" + "快速";//堆料回转速度文本框
                OperationLogs("堆料回转速度切换为快速");//传入数据库记录操作历史记录
            }
            else if (fullVariables.RemoteManualStackingRotateSpeedSelection == true)
            {
                Debug.Log("堆料回转速度快速，切换为慢速");
                SendCommand(6, 2, "ROTATE_MODE");//向服务器发送指令  指令内容为flase
                RemoteManualStackingRotateSpeedSelectionText.text = "堆料回转速度：" + "慢速";//堆料回转速度文本框
                OperationLogs("堆料回转速度切换为慢速");//传入数据库记录操作历史记录
            }
            else
            {
                Debug.Log("堆料回转速度故障");
                RemoteManualStackingRotateSpeedSelectionText.text = "堆料回转速度：" + "";//堆料回转速度文本框
            }
        }
        catch
        {
            Debug.Log("指令发送异常");
        }
    }

    public void RemoteManualStackingRotateLeftCommandDown()//堆料回转左转
    {
        try
        {
            Debug.Log("堆料回转左转按钮按下");
            SendCommand(6, 2, "ROTATE_LEFT");//向服务器发送指令  指令内容为flase
            OperationLogs("堆料回转左转");//传入数据库记录操作历史记录
        }
        catch
        {
            Debug.Log("指令发送异常");
        }
    }
    public void RemoteManualStackingRotateStopCommandDown()//堆料回转停止
    {
        try
        {
            Debug.Log("堆料回转停止按钮按下");
            SendCommand(6, 2, "ROTATE_STOP");//向服务器发送指令  指令内容为flase
            OperationLogs("堆料回转停止");//传入数据库记录操作历史记录
        }
        catch
        {
            Debug.Log("指令发送异常");
        }

    }
    public void RemoteManualStackingRotateRightCommandDown()//堆料回转右转
    {
        try
        {
            Debug.Log("堆料回转右转按钮按下");
            SendCommand(6, 2, "ROTATE_RIGHT");//向服务器发送指令  指令内容为true
            OperationLogs("堆料回转右转");//传入数据库记录操作历史记录
        }
        catch
        {
            Debug.Log("指令发送异常");
        }

    }
    //单击以设置角度
    public void SetAutoStackingAngleEvent()//自动设置起始终止角度
    {
        Debug.Log("尝试发送起始终止角度参数到服务器");
        if (AutoStackingStartAngle.text != "" && AutoStackingEndAngle.text != "")
        {
            float StartAngle = float.Parse(AutoStackingStartAngle.text);
            float EndAngle = float.Parse(AutoStackingEndAngle.text);
            if (StartAngle < 0 || StartAngle > 360 || EndAngle < 0 || EndAngle > 360)
            {
                Debug.Log("输入参数有误，请重新输入");
                WarningText.text = "自动堆料角度参数设置有误,\n请重新输入!";
                WarningPanel.SetActive(true);
            }
            else
            {
                Debug.Log("自动堆料起终角度参数发送至服务器");
                SendCommand(6, 2, "STACKING_START_ANGLE", 1, StartAngle);//向服务器发送自动堆料起始角度参数
                SendCommand(6, 2, "STACKING_STOP_ANGLE", 1, EndAngle);//向服务器发送指自动堆料起始角度参数
                OperationLogs("设置自动堆料起始与终止角度");//传入数据库记录操作历史记录
            }
        }
        else
        {
            Debug.Log("输入参数有误，请重新输入");
            WarningText.text = "自动堆料角度参数设置有误,\n请重新输入!";
            WarningPanel.SetActive(true);
        }
    }
    //单击以设置高度
    public void SetAutoStackingHeightEvent()//自动设置高度
    {
        Debug.Log("尝试发送自动堆料高度参数到服务器");
        if (AutoStackingHeightSetting.text != "")
        {
            float StackingHeight = float.Parse(AutoStackingHeightSetting.text);
            if (StackingHeight < 0)
            {
                Debug.Log("输入参数有误，请重新输入");
                WarningText.text = "自动堆料高度参数设置有误,\n请重新输入!";
                WarningPanel.SetActive(true);
            }
            else
            {
                Debug.Log("自动堆料高度参数发送至服务器");
                SendCommand(6, 2, "STACKING_HEIGHT_SET", 1, StackingHeight);//向服务器发送自动堆料高度参数
                OperationLogs("设置自动堆料高度");//传入数据库记录操作历史记录
            }
        }
        else
        {
            Debug.Log("输入参数有误，请重新输入");
            WarningText.text = "自动堆料高度参数设置有误,\n请重新输入!";
            WarningPanel.SetActive(true);
        }
    }

    //单击自动更新自动堆取料界面的参数设置(起始角度,终止角度，高度，深度，切入点，堆料速度和取料回转俯仰速度....)        不需要发命令， YCQ， 20231229
    public void UpdateAutoStackingAndFeedingParameterSetting()
    {
        Debug.Log("自动设置堆取料参数");
        UpdateAutoFeedingParameterSetting();
        UpdateAutoStackingParameterSetting();
    }
    public void UpdateAutoStackingParameterSetting()
    {
        Debug.Log("自动设置堆料参数");

        //开启2秒字体高光 -ljz
        if (myToggleHighLightTime7 == null)
        {
            myToggleHighLightTime7 = StartCoroutine(ToggleHighLightTime7());
        }

        AutoStackingStartAngle.text = "" + fullVariables.AutoStackingStartAngle.ToString("F2");//10;//自动设置自动堆料起始角度     不需要发命令
        AutoStackingEndAngle.text = "" + fullVariables.AutoStackingEndAngle.ToString("F2");//30;//自动设置自动堆料终止角度     不需要发命令
        AutoStackingHeightSetting.text = "" + fullVariables.AutoStackingHeightSetting.ToString("F2");//10;//自动设置自动堆料高度        不需要发命令


        RemoteManualStackingRotateSpeedInput.text = "" + fullVariables.MaterialStackerRotationSpeed.ToString();  //从全局变量中获取堆料回转速度 ， YCQ， 20231229
        RemoteManualStackingRotateSpeedSlider.value = fullVariables.MaterialStackerRotationSpeed;// 堆料回转速度滑条，YCQ， 20231230

    }
    public void UpdateAutoFeedingParameterSetting()
    {
        Debug.Log("自动设置取料参数");

        //开启2秒字体高光 -ljz
        if (myToggleHighLightTime8 == null)
        {
            myToggleHighLightTime8 = StartCoroutine(ToggleHighLightTime8());
        }

        AutoFeedingStartAngle.text = "" + fullVariables.AutoFeedingStartAngle.ToString("F2");//10;//自动设置自动取料起始角度    不需要发送指令
        AutoFeedingEndAngle.text = "" + fullVariables.AutoFeedingEndAngle.ToString("F2"); //30;自动设置自动取料终止角度    不需要发送指令
        ScrapingDepthSetting.text = "" + fullVariables.ScrapingDepthSetting.ToString("F2");//10;自动设置自动取料吃料深度    不需要发送指令

        //更新刮板电流参数  
        changeOverloadDataAndRestoreCurrentData();

        RemoteManualMaterialPickupRotateSpeedInput.text = "" + fullVariables.MaterialFeederRotationSpeed.ToString();  //从全局变量中获取取料回转速度 ， YCQ， 20231229
        RemoteManualMaterialPickupTiltSpeedInput.text = "" + fullVariables.MaterialFeederPitchSpeed.ToString();  //从全局变量中获取取料俯仰速度 ， YCQ， 20231229

        PeekingRotateSpeedSlider.value = fullVariables.MaterialFeederRotationSpeed;// 取料回转速度滑条，YCQ， 20231230
        PeekingTiltSpeedSlider.value = fullVariables.MaterialFeederPitchSpeed; //取料俯仰速度滑条，YCQ， 20231230
    }

    public void AutoStackingPauseHMIClick()//自动堆料暂停
    {
        try
        {
            if (fullVariables.AutoStackingPauseHMI)
            {
                Debug.Log("当前为自动堆料暂停状态");
                SendCommand(6, 2, "AUTO_STACKING_PAUSE", 0);//向服务器发送指令  指令内容为false
                OperationLogs("自动堆料继续");//传入数据库记录操作历史记录
            }
            else
            {
                Debug.Log("当前为自动堆料运行状态");
                SendCommand(6, 2, "AUTO_STACKING_PAUSE", 1);//向服务器发送指令  指令内容为true
                OperationLogs("自动堆料暂停");//传入数据库记录操作历史记录
            }
        }
        catch
        {
            Debug.Log("指令发送异常");
        }
    }
    public void RemoteIlluminationCloseCommandDown()//照明关闭
    {
        try
        {
            Debug.Log("照明关闭按钮按下");
            SendCommand(6, 2, "LIGHTPOWER_OFF");//向服务器发送指令  指令内容为flase
            OperationLogs("照明关闭");//传入数据库记录操作历史记录
        }
        catch
        {
            Debug.Log("指令发送异常");
        }
    }
    public void RemoteIlluminationOpenCommandDown()//照明开启
    {
        try
        {
            Debug.Log("照明开启按钮按下");
            SendCommand(6, 2, "LIGHTPOWER_ON");//向服务器发送指令  指令内容为flase
            OperationLogs("照明开启");//传入数据库记录操作历史记录
        }
        catch
        {
            Debug.Log("指令发送异常");
        }
    }
    //todo: 堆料检修、取料检修
    public void StackingServiceCommandUp()   ///YCQ，20231229
    {
        try
        {
            if (fullVariables.MaterialStackerMaintenanceButton)
            {
                Debug.Log("堆料检修关闭，堆料投入启用");
                SendCommand(6, 2, "STACKER_MAINTENANCE", 0);//向服务器发送指令  指令内容为false
            }
            else
            {
                Debug.Log("堆料检修开启，堆料投入禁用");
                SendCommand(6, 2, "STACKER_MAINTENANCE", 1);//向服务器发送指令  指令内容为true
            }
        }
        catch
        {
            Debug.Log("指令发送异常");
        }
    }
    public void FeedingServiceCommandUp() ///YCQ，20231229
    {
        try
        {
            if (fullVariables.MaterialFeederMaintenanceButton)
            {
                Debug.Log("取料检修关闭，取料投入启用");
                SendCommand(6, 2, "FEEDER_MAINTENANCE", 0);//向服务器发送指令  指令内容为false
            }
            else
            {
                Debug.Log("取料检修开启，取料投入禁用");
                SendCommand(6, 2, "FEEDER_MAINTENANCE", 1);//向服务器发送指令  指令内容为true

            }
        }
        catch
        {
            Debug.Log("指令发送异常");
        }

    }
    public void RemoteManualStartBellCommandDown()//启车报警
    {
        try
        {
            Debug.Log("启车打铃报警按钮按下");
            //开启2秒字体高光 -ljz
            if (myToggleHighLightTime10 == null)
            {
                myToggleHighLightTime10 = StartCoroutine(ToggleHighLightTime10());
            }
            SendCommand(6, 2, "STARTUP_ALARM");//向服务器发送指令  指令内容为flase
            OperationLogs("启车报警");//传入数据库记录操作历史记录
        }
        catch
        {
            Debug.Log("指令发送异常");
        }
    }
    public void RemoteEmergencyStopCommandUp() //急停按钮    按下与松开 YCQ, 20231231
    {
        try
        {
            if (fullVariables.RemoteEmergencyStopCommand)
            {
                SendCommand(6, 2, "EMERGENCY_STOP", 0);//向服务器发送指令  指令内容为flase
                OperationLogs("取消急停");//传入数据库记录操作历史记录
                Debug.Log("急停按钮松开");
            }
            else
            {
                Debug.Log("急停按钮按下");
                SendCommand(6, 2, "EMERGENCY_STOP", 1);//向服务器发送指令  指令内容为true
                OperationLogs("启用急停");//传入数据库记录操作历史记录
            }
        }
        catch
        {
            Debug.Log("指令发送异常");
        }
    }

    #endregion

    #region 取料作业

    //堆料回转速度快慢速切换
    public void RemoteManualMaterialPickupRotateSpeedSelectionEvent()//取料回转速度选择（0：慢速，1：快速）
    {
        try
        {
            if (fullVariables.RemoteManualMaterialPickupRotateSpeedSelection == false)
            {
                Debug.Log("取料回转速度慢速，切换为快速");
                SendCommand(6, 2, "REVERSE_MODE", 1);//向服务器发送指令  指令内容为flase
                RemoteManualMaterialPickupRotateSpeedSelectionText.text = "取料回转速度：" + "快速";//取料回转速度文本框
                OperationLogs("取料回转速度切换为快速");//传入数据库记录操作历史记录
            }
            else if (fullVariables.RemoteManualMaterialPickupRotateSpeedSelection == true)
            {
                Debug.Log("取料回转速度快速，切换为慢速");
                SendCommand(6, 2, "REVERSE_MODE", 0);//向服务器发送指令  指令内容为flase
                RemoteManualMaterialPickupRotateSpeedSelectionText.text = "取料回转速度：" + "慢速";//取料回转速度文本框
                OperationLogs("取料回转速度切换为慢速");//传入数据库记录操作历史记录
            }
            else
            {
                Debug.Log("取料回转速度故障");
                RemoteManualMaterialPickupRotateSpeedSelectionText.text = "取料回转速度：" + "";//取料回转速度文本框
            }
        }
        catch
        {
            Debug.Log("指令发送异常");
        }

    }
    //取料俯仰速度快慢速切换
    public void RemoteManualMaterialPickupTiltSpeedSelectionEvent()//取料俯仰速度选择（0：慢速，1：快速）
    {
        try
        {
            if (fullVariables.RemoteManualMaterialPickupTiltSpeedSelection == false)
            {
                Debug.Log("取料俯仰速度慢速，切换为快速");
                SendCommand(6, 2, "ELEVATE_MODE", 1);//向服务器发送指令  指令内容为flase
                RemoteManualMaterialPickupTiltSpeedSelectionText.text = "取料俯仰速度：" + "快速";//取料俯仰速度文本框
                OperationLogs("取料俯仰速度切换为快速");//传入数据库记录操作历史记录
            }
            else if (fullVariables.RemoteManualMaterialPickupTiltSpeedSelection == true)
            {
                Debug.Log("取料俯仰速度快速，切换为慢速");
                SendCommand(6, 2, "ELEVATE_MODE", 0);//向服务器发送指令  指令内容为flase
                RemoteManualMaterialPickupTiltSpeedSelectionText.text = "取料俯仰速度：" + "慢速";//取料俯仰速度文本框
                OperationLogs("取料俯仰速度切换为慢速");//传入数据库记录操作历史记录
            }
            else
            {
                Debug.Log("取料俯仰速度故障");
                RemoteManualMaterialPickupTiltSpeedSelectionText.text = "取料俯仰速度：" + "";//取料俯仰速度文本框
            }
        }
        catch
        {
            Debug.Log("指令发送异常");
        }
    }
    public void RemoteManualMaterialPickupTurnLeftCommandDown()//取料回转左转
    {
        try
        {
            Debug.Log("取料回转左转按钮按下");
            SendCommand(6, 2, "REVERSE_LEFT");//向服务器发送指令  指令内容为flase
            OperationLogs("取料回转左转");//传入数据库记录操作历史记录
        }
        catch
        {
            Debug.Log("指令发送异常");
        }

    }
    public void RemoteManualMaterialPickupRotateStopCommandDown()//取料回转停止
    {
        try
        {
            Debug.Log("取料回转停止按钮按下");
            SendCommand(6, 2, "REVERSE_STOP");//向服务器发送指令  指令内容为flase
            OperationLogs("取料回转停止");//传入数据库记录操作历史记录
        }
        catch
        {
            Debug.Log("指令发送异常");
        }

    }
    public void RemoteManualMaterialPickupTurnRightCommandDown()//取料回转右转
    {
        try
        {
            Debug.Log("取料回转右转按钮按下");
            SendCommand(6, 2, "REVERSE_RIGHT");//向服务器发送指令  指令内容为flase
            OperationLogs("取料回转右转");//传入数据库记录操作历史记录
        }
        catch
        {
            Debug.Log("指令发送异常");
        }

    }
    //todo:堆料回转、取料回转、取料俯仰
    public void SetRemoteManualStackingRotateSpeed()  //YCQ, 20231229
    {
        Debug.Log("尝试发送堆料回转速度参数到服务器");
        if (RemoteManualStackingRotateSpeedInput.text != "")
        {
            int speed = int.Parse(RemoteManualStackingRotateSpeedInput.text);
            if (speed < 10 || speed > 25)//更改堆料回转速度 上限值 -ljz
            {
                Debug.Log("输入参数有误，请重新输入");
                WarningText.text = "堆料回转速度参数设置有误,\n请重新输入!";
                WarningPanel.SetActive(true);
            }
            else
            {
                Debug.Log("堆料回转速度参数发送至服务器");

                //开启2秒字体高光 -ljz
                if (myToggleHighLightTime2 == null)
                {
                    myToggleHighLightTime2 = StartCoroutine(ToggleHighLightTime2());
                }

                OperationLogs("设置堆料回转速度");//传入数据库记录操作历史记录
                SendCommand(6, 2, "STACKER_SPEED_SET_BUTTON", 1);//开启设置速度数值模式

                SendCommand(6, 2, "STACKER_SPEED_SET", speed);//向服务器发送自动堆料起始角度参数
            }
        }
        else
        {
            Debug.Log("输入参数有误，请重新输入");
            WarningText.text = "堆料回转速度参数设置有误,\n请重新输入!";
            WarningPanel.SetActive(true);
        }
    }
    public void SetRemoteManualMaterialPickupTurnSpeed() //YCQ, 20231229
    {

        Debug.Log("尝试发送取料回转速度参数到服务器");
        if (RemoteManualMaterialPickupRotateSpeedInput.text != "")
        {
            int speed = int.Parse(RemoteManualMaterialPickupRotateSpeedInput.text);
            if (speed < 10 || speed > 35)
            {
                Debug.Log("输入参数有误，请重新输入");
                WarningText.text = "取料回转速度参数设置有误,\n请重新输入!";
                WarningPanel.SetActive(true);
            }
            else
            {
                Debug.Log("取料回转速度参数发送至服务器");

                //开启2秒字体高光 -ljz
                if (myToggleHighLightTime3 == null)
                {
                    myToggleHighLightTime3 = StartCoroutine(ToggleHighLightTime3());
                }

                SendCommand(6, 2, "ROTATION_SPEED_SET_BUTTON", 1);//开启设置速度数值模式

                SendCommand(6, 2, "ROTATION_SPEED_SET", speed);//向服务器发送取料回转速度参数

                OperationLogs("设置取料回转速度");//传入数据库记录操作历史记录
            }
        }
        else
        {
            Debug.Log("输入参数有误，请重新输入");
            WarningText.text = "取料回转速度参数设置有误,\n请重新输入!";
            WarningPanel.SetActive(true);
        }

    }
    public void SetRemoteManualMaterialPickupTiltSpeed() //YCQ, 20231229
    {
        Debug.Log("尝试发送取料俯仰速度参数到服务器");
        if (RemoteManualMaterialPickupTiltSpeedInput.text != "")
        {
            int speed = int.Parse(RemoteManualMaterialPickupTiltSpeedInput.text);
            if (speed < 10 || speed > 35)
            {
                Debug.Log("输入参数有误，请重新输入");
                WarningText.text = "取料俯仰速度参数设置有误,\n请重新输入!";
                WarningPanel.SetActive(true);
            }
            else
            {
                Debug.Log("取料俯仰速度参数发送至服务器");

                //开启2秒字体高光 -ljz
                if (myToggleHighLightTime4 == null)
                {
                    myToggleHighLightTime4 = StartCoroutine(ToggleHighLightTime4());
                }

                SendCommand(6, 2, "PITCH_SPEED_SET_BUTTON", 1);//开启设置速度数值模式

                SendCommand(6, 2, "PITCH_SPEED_SET", speed);//向服务器发送取料俯仰速度参数

                OperationLogs("设置取料俯仰速度");//传入数据库记录操作历史记录
            }
        }
        else
        {
            Debug.Log("输入参数有误，请重新输入");
            WarningText.text = "取料俯仰速度参数设置有误,\n请重新输入!";
            WarningPanel.SetActive(true);
        }

    }

    public void SetScraperCurrentProtectionInterval()//设置刮板电流保护区间 -ljz
    {
        Debug.Log("尝试刮板电流保护区间到服务器");
        if (OverloadCurrent.text != "" && RestoreCurrent.text != "")
        {
            float AutoFeedingOverloadCurrent = float.Parse(OverloadCurrent.text);
            float AutoFeedingNormalCurrent = float.Parse(RestoreCurrent.text);
            if (AutoFeedingOverloadCurrent < 0 || AutoFeedingOverloadCurrent > 360 || AutoFeedingNormalCurrent < 0 || AutoFeedingNormalCurrent > 360)
            {
                Debug.Log("输入参数有误，请重新输入");
                WarningText.text = "刮板电流保护区间参数设置有误,\n请重新输入!";
                WarningPanel.SetActive(true);
            }
            else
            {
                Debug.Log("刮板电流保护区间参数发送至服务器");
                //开启2秒字体高光 -ljz
                if (myToggleHighLightTime12 == null)
                {
                    myToggleHighLightTime12 = StartCoroutine(ToggleHighLightTime12());
                }
                SendCommand(6, 2, "AUTO_MAX_CURRENT_SET", 1, AutoFeedingOverloadCurrent);//向服务器发送超载电流参数
                SendCommand(6, 2, "AUTO_NORMAL_CURRENT_SET", 1, AutoFeedingNormalCurrent);//向服务器发送恢复电流参数
                OperationLogs("设置刮板电流保护区间");//设置刮板电流保护区间

                StartCoroutine(RefreshSetCurrentParameters());//将设置后的参数显示上去
            }
        }
        else
        {
            Debug.Log("输入参数为空，请重新输入");
            WarningText.text = "刮板电流保护区间参数设置为空,\n请重新输入!";
            WarningPanel.SetActive(true);
        }
    }

    private IEnumerator RefreshSetCurrentParameters()
    {
        yield return new WaitForSeconds(2.0f);
        OverloadCurrent.text = "" + fullVariables.AutoFeedingOverloadCurrent.ToString("F2");     //超载电流
        RestoreCurrent.text = "" + fullVariables.AutoFeedingNormalCurrent.ToString("F2");     //恢复电流
    }

    public void RemoteManualMaterialPickupTiltUpCommandDown()//取料俯仰上仰
    {
        try
        {
            Debug.Log("取料俯仰上仰按钮按下");
            SendCommand(6, 2, "ELEVATE_UP");//向服务器发送指令  指令内容为flase
            OperationLogs("取料俯仰上仰");//传入数据库记录操作历史记录
        }
        catch
        {
            Debug.Log("指令发送异常");
        }

    }
    public void RemoteManualMaterialPickupTiltStopCommandDown()//取料俯仰停止
    {
        try
        {
            Debug.Log("取料俯仰停止按钮按下");
            SendCommand(6, 2, "ELEVATE_STOP");//向服务器发送指令  指令内容为flase
            OperationLogs("取料俯仰停止");//传入数据库记录操作历史记录
        }
        catch
        {
            Debug.Log("指令发送异常");
        }

    }
    public void RemoteManualMaterialPickupTiltDownCommandDown()//取料俯仰下俯
    {
        try
        {
            Debug.Log("取料俯仰下俯按钮按下");
            SendCommand(6, 2, "ELEVATE_DOWN");//向服务器发送指令  指令内容为flase
            OperationLogs("取料俯仰下俯");//传入数据库记录操作历史记录
        }
        catch
        {
            Debug.Log("指令发送异常");
        }

    }
    //设置自动取料起始终止角度
    public void SetAutoFeedingAngleEvent()//自动设置自动取料角度    不需要发送指令
    {
        Debug.Log("尝试发送起始终止角度参数到服务器");
        if (AutoFeedingStartAngle.text != "" && AutoFeedingEndAngle.text != "")
        {
            float StartAngle = float.Parse(AutoFeedingStartAngle.text);
            float EndAngle = float.Parse(AutoFeedingEndAngle.text);
            if (StartAngle < 0 || StartAngle > 360 || EndAngle < 0 || EndAngle > 360)
            {
                Debug.Log("输入参数有误，请重新输入");
                WarningText.text = "自动取料角度参数设置有误,\n请重新输入!";
                WarningPanel.SetActive(true);
            }
            else
            {
                Debug.Log("自动取料起终角度参数发送至服务器");
                SendCommand(6, 2, "PICKUPING_START_ANGLE", 1, StartAngle);//向服务器发送自动取料起始角度参数
                SendCommand(6, 2, "PICKUPING_STOP_ANGLE", 1, EndAngle);//向服务器发送指自动取料终止角度参数
                OperationLogs("设置自动取料起始与终止角度");//传入数据库记录操作历史记录
            }
        }
        else
        {
            Debug.Log("输入参数有误，请重新输入");
            WarningText.text = "自动取料角度参数设置有误,\n请重新输入!";
            WarningPanel.SetActive(true);
        }
    }
    //设置自动取料吃料深度
    public void SetScrapingDepthSettingEvent()//自动设置自动取料吃料深度    不需要发送指令
    {
        Debug.Log("尝试发送自动取料吃料深度参数到服务器");
        if (ScrapingDepthSetting.text != "")
        {
            float ScrapingDepth = float.Parse(ScrapingDepthSetting.text);
            if (ScrapingDepth < 0)
            {
                Debug.Log("输入参数有误，请重新输入");
                WarningText.text = "自动取料吃料深度参数设置有误,\n请重新输入!";
                WarningPanel.SetActive(true);
            }
            else
            {
                Debug.Log("自动取料吃料深度参数发送至服务器");
                SendCommand(6, 2, "PICKUPING_HEIGHT_SET", 1, ScrapingDepth);//向服务器发送自动堆料高度参数
                OperationLogs("设置自动取料吃料深度");//传入数据库记录操作历史记录
            }
        }
        else
        {
            Debug.Log("输入参数有误，请重新输入");
            WarningText.text = "自动取料吃料深度参数设置有误,\n请重新输入!";
            WarningPanel.SetActive(true);
        }
    }
    //设置自动取料切入点角度
    public void SetRotationEntryPointEvent()//自动设置自动取料切入点角度    不需要发送指令
    {
        Debug.Log("尝试发送自动取料切入点参数到服务器");
        string rotationEntryPoint = fullVariables.MaterialArmRotationAngle.ToString("F2");
        if (rotationEntryPoint != "")
        {
            float EntryPointAngle = float.Parse(rotationEntryPoint);
            if (EntryPointAngle < 0 || EntryPointAngle > 360)
            {
                Debug.Log("输入参数有误，请重新输入");
                WarningText.text = "自动取料切入点角度参数设置有误,\n请重新输入!";
                WarningPanel.SetActive(true);
            }
            else
            {
                Debug.Log("自动取料切入点参数发送至服务器");
                SendCommand(6, 2, "ROTATION_ENTRY", 1, EntryPointAngle);//向服务器发送自动堆料高度参数
                OperationLogs("设置自动取料切入点角度");//传入数据库记录操作历史记录
            }
        }
        else
        {
            Debug.Log("输入参数有误，请重新输入");
            WarningText.text = "自动取料切入点角度参数设置有误,\n请重新输入!";
            WarningPanel.SetActive(true);
        }
    }

    public void ConfirmScrapingClick()//刮平确认
    {
        try
        {
            // Debug.Log("刮平确认");
            //if (fullVariables.ConfirmScrapingButton)
            //{
            //    Debug.Log("当前为刮平运行状态");
            //    SendCommand(6, 2, "AUTO_SCRAPING_CONFIRM", 0);//向服务器发送指令  指令内容为false
            //    OperationLogs("刮平暂停");//传入数据库记录操作历史记录
            //}
            //else
            //{
            //    Debug.Log("当前为刮平运行状态");
            //    SendCommand(6, 2, "AUTO_SCRAPING_CONFIRM", 1);//向服务器发送指令  指令内容为true
            //    OperationLogs("刮平确认");//传入数据库记录操作历史记录
            //}


            Debug.Log("始终假定当前为刮平状态");
            SendCommand(6, 2, "AUTO_SCRAPING_CONFIRM", 1);//向服务器发送指令  指令内容为true
            OperationLogs("刮平确认");//传入数据库记录操作历史记录


        }
        catch
        {
            Debug.Log("指令发送异常");
        }
    }

    public void AutoFeedingPauseHMIClick() //自动取料暂停
    {
        try
        {
            Debug.Log("自动取料暂停");
            if (fullVariables.AutoFeedingPauseHMI)
            {
                Debug.Log("当前为自动取料暂停状态");
                SendCommand(6, 2, "AUTO_PICKUPING_PAUSE", 0);//向服务器发送指令  指令内容为false
                OperationLogs("自动取料继续");//传入数据库记录操作历史记录
            }
            else
            {
                Debug.Log("当前为自动取料运行状态");
                SendCommand(6, 2, "AUTO_PICKUPING_PAUSE", 1);//向服务器发送指令  指令内容为true
                OperationLogs("自动取料暂停");//传入数据库记录操作历史记录
            }
        }
        catch
        {
            Debug.Log("指令发送异常");
        }

    }
    public void IsManualRotationDown()//手动回转换向
    {
        try
        {
            Debug.Log("手动回转换向按钮按下");
            fullVariables.IsManualRotation = true;

            //开启2秒字体高光 -ljz
            if (myToggleHighLightTime1 == null)
            {
                myToggleHighLightTime1 = StartCoroutine(ToggleHighLightTime1());
            }

            SendCommand(6, 2, "MANUAL_ROTATION");//向服务器发送指令  指令内容为true

            OperationLogs("手动回转换向");//传入数据库记录操作历史记录
        }
        catch
        {
            Debug.Log("指令发送异常");
        }

    }
    public void ConfirmCurrentJobPointHMIDown()//作业点确认按钮按下
    {
        try
        {
            Debug.Log("作业点确认按钮按下");
            SendCommand(6, 2, "AUTO_JOBPOINT_CONFIRM");//向服务器发送指令  指令内容为true
            OperationLogs("作业点确认");//传入数据库记录操作历史记录
        }
        catch
        {
            Debug.Log("指令发送异常");
        }
    }
    #endregion

    #region 按钮按下 二次确认后 更改标识位
    //自动堆料启动抬起
    public void AutoStackingStartHMIDown()
    {
        try
        {
            Debug.Log("自动堆料启动按钮按下");
            SecondConfirmMode = 1;
            SecondConfirmText.text = "自动堆料确认启动?";
        }
        catch
        {
            Debug.Log("指令发送异常");
        }
    }

    public void AutoStackingStopHMIDown()
    {
        try
        {
            Debug.Log("自动堆料停止按钮按下");
            SecondConfirmMode = 2;
            SecondConfirmText.text = "自动堆料确认停止?";
        }
        catch
        {
            Debug.Log("指令发送异常");
        }
    }

    public void AutoFeedingStartHMIDown()
    {
        try
        {
            Debug.Log("自动取料启动按钮按下");
            SecondConfirmMode = 3;
            SecondConfirmText.text = "自动取料确认启动?";
        }
        catch
        {
            Debug.Log("指令发送异常");
        }
    }

    public void AutoFeedingStopHMIDown()
    {
        try
        {
            Debug.Log("自动取料停止按钮按下");
            SecondConfirmMode = 4;
            SecondConfirmText.text = "自动取料确认停止?";
        }
        catch
        {
            Debug.Log("指令发送异常");
        }
    }

    //堆料胶带启动    按下与松开
    public void RemoteManualStackingBeltStartCommandDown()
    {
        SecondConfirmMode = 5;
        SecondConfirmText.text = "堆料胶带启动?";
    }

    public void RemoteManualStackingBeltStopCommandDown()
    {
        SecondConfirmMode = 6;
        SecondConfirmText.text = "堆料胶带停止?";
    }
    //自动堆料与手动堆料
    public void RemoteAutoStackingModeSelectionCommandEvent()//自动堆料     与手动堆料按钮互斥
    {
        SecondConfirmMode = 7;
        SecondConfirmText.text = "启动自动堆料?";
    }
    public void RemoteManualStackingModeSelectionCommandEvent()//手动堆料   与自动堆料按钮互斥
    {
        SecondConfirmMode = 8;
        SecondConfirmText.text = "启动手动堆料?";
    }
    public void RemotePowerSupplyCloseCommandDown()
    {
        SecondConfirmMode = 9;
        SecondConfirmText.text = "动力电源合闸?";
    }

    public void RemotePowerSupplyOpenCommandDown()
    {
        SecondConfirmMode = 10;
        SecondConfirmText.text = "动力电源分闸?";
    }

    public void RemoteControlPowerSupplyCloseCommandDown()
    {
        SecondConfirmMode = 11;
        SecondConfirmText.text = "控制电源合闸?";
    }

    public void RemoteControlPowerSupplyOpenCommandDown()
    {
        SecondConfirmMode = 12;
        SecondConfirmText.text = "控制电源分闸?";
    }
    //与系统联锁与解锁功能
    public void RemoteInterlockLockCommandEvent()       //（0：解锁，1：连锁）
    {
        SecondConfirmMode = 13;
        SecondConfirmText.text = "与系统联锁?";
    }
    public void RemoteInterUnlockLockCommandEvent()     //（0：解锁，1：连锁）
    {
        SecondConfirmMode = 14;
        SecondConfirmText.text = "与系统解锁?";
    }

    public void RemoteBypassCommandDown()
    {
        SecondConfirmMode = 15;
        SecondConfirmText.text = "启用旁路功能?";
    }
    public void RemoteFaultResetCommandDown()
    {
        SecondConfirmMode = 16;
        SecondConfirmText.text = "启用故障复位?";
    }
    public void RemoteManualScraperStartStopCommandDown()
    {
        SecondConfirmMode = 17;
        SecondConfirmText.text = "刮板启动?";
    }
    public void RemoteManualScraperStopCommandDown()
    {
        SecondConfirmMode = 18;
        SecondConfirmText.text = "刮板停止?";
    }
    //自动与手动取料功能
    public void RemoteAutoMaterialPickupModeSelectionCommandEvent()//自动取料     与手动取料按钮互斥
    {
        SecondConfirmMode = 19;
        SecondConfirmText.text = "启动自动取料?";
    }
    public void RemoteManualMaterialPickupModeSelectionCommandEvent()//手动取料   与自动取料按钮互斥
    {
        SecondConfirmMode = 20;
        SecondConfirmText.text = "启动手动取料?";
    }
    public void StackingServiceCommandDown()///YCQ，20231229
    {
        SecondConfirmMode = 21;

        if (fullVariables.MaterialStackerMaintenanceButton)
            SecondConfirmText.text = "堆料投入?";
        else
            SecondConfirmText.text = "堆料检修?";
    }

    public void FeedingServiceCommandDown() ///YCQ，20231229
    {
        SecondConfirmMode = 22;
        if (fullVariables.MaterialFeederMaintenanceButton)
            SecondConfirmText.text = "取料投入?";
        else
            SecondConfirmText.text = "取料检修?";

    }
    public void RemoteEmergencyStopCommandDown()   //YCQ, 20231229
    {
        if (fullVariables.RemoteEmergencyStopCommand)
        {
            SecondConfirmMode = 23;
            SecondConfirmText.text = "是否取消急停?";
        }
        else
        {
            SecondConfirmMode = 23;
            SecondConfirmText.text = "是否启用急停?";
        }
    }

    public void PeekingValueReclaimingEncoderIsEnabled()
    {
        SecondConfirmMode = 24;
        //取料编码器手动预设值使能命令发送 -ljz
        try
        {
            Debug.Log("取料编码器手动预设值使能按钮按下");
            if (PeekingFetchingEncodeAngle.text != null)
            {
                float angle = float.Parse(PeekingFetchingEncodeAngle.text);
                SendCommand(6, 2, "FEEDER_ENCODE_ANGLE", 1, angle);
            }

            SendCommand(6, 2, "FEEDER_ENCODE_ENABLE_BUTTON");//向服务器发送指令  指令内容为true
            OperationLogs("取料编码器手动预设值使能");//传入数据库记录操作历史记录
        }
        catch
        {
            Debug.Log("指令发送异常");
        }

        SecondConfirmText.text = "是否手动校准取料编码器?";
    }

    public void MaterialStackingValueReclaimingEncoderIsEnabled()
    {
        SecondConfirmMode = 25;
        //堆料编码器手动预设值使能命令发送 -ljz
        try
        {
            Debug.Log("堆料编码器手动预设值使能按钮按下");
            if (MaterialFetchingEncodeAngle.text != null)
            {
                float angle = float.Parse(MaterialFetchingEncodeAngle.text);
                SendCommand(6, 2, "STACKER_ENCODE_ANGLE", 1, angle);
            }

            SendCommand(6, 2, "STACKER_ENCODE_ENABLE_BUTTON");//向服务器发送指令  指令内容为true
            OperationLogs("堆料编码器手动预设值使能");//传入数据库记录操作历史记录
        }
        catch
        {
            Debug.Log("指令发送异常");
        }

        SecondConfirmText.text = "是否手动校准堆料编码器?";
    }
    #endregion

    #region 设置脉冲信号的休眠时间 防止多段脉冲信号重合 -ljz
    public void process1() //自动堆料启动
    {
        //设置参数
        SetAutoStackingHeightEvent();
        SetAutoStackingAngleEvent();

        SendCommand(6, 2, "AUTO_STACKING_START");//向服务器发送指令  指令内容为flase
        Thread.Sleep(1000);
        OperationLogs("自动堆料启动");//传入数据库记录操作历史记录

        //自动堆料启动二次确认
        Debug.Log("自动堆料启动二次确认");

        SendCommand(6, 2, "AUTO_STACKING_CONFIRM");//向服务器发送指令  指令内容为true
        Thread.Sleep(1000);
        OperationLogs("自动堆料确认启动");//传入数据库记录操作历史记录
    }
    public void process2()//自动堆料停止
    {
        //开启2秒字体高光 -ljz
        if (myToggleHighLightTime5 == null)
        {
            myToggleHighLightTime5 = StartCoroutine(ToggleHighLightTime5());
        }

        SendCommand(6, 2, "AUTO_STACKING_STOP");//向服务器发送指令  指令内容为true
        Thread.Sleep(1000);

        OperationLogs("自动堆料停止");//传入数据库记录操作历史记录

        //自动堆料停止二次确认
        Debug.Log("自动堆料停止二次确认");

        SendCommand(6, 2, "AUTO_STACKING_STOP_CONFIRM");//向服务器发送指令  指令内容为true
        Thread.Sleep(1000);
        OperationLogs("自动堆料确认停止");//传入数据库记录操作历史记录
    }
    public void process3() //自动取料启动
    {
        SetAutoFeedingAngleEvent();//设置自动取料角度
        SetScrapingDepthSettingEvent();//设置自动取料吃料深度
        SetRotationEntryPointEvent();//自动设定切入点

        ConfirmCurrentJobPointHMIDown();//作业点确认按钮按下 -ljz
        Thread.Sleep(1000);

        ConfirmScrapingClick();//刮平确认

        Debug.Log("自动取料启动按钮按下");
        SendCommand(6, 2, "AUTO_PICKUPING_START");//向服务器发送指令  指令内容为true
        Thread.Sleep(1000);
        OperationLogs("自动取料启动");//传入数据库记录操作历史记录


        //自动取料启动二次确认
        Debug.Log("自动取料启动二次确认");

        SendCommand(6, 2, "AUTO_PICKUPING_CONFIRM");//向服务器发送指令  指令内容为true
        Thread.Sleep(1000);
        OperationLogs("自动取料确认启动");//传入数据库记录操作历史记录
    }
    public void process4()//自动取料停止
    {
        //开启2秒字体高光 -ljz
        if (myToggleHighLightTime6 == null)
        {
            myToggleHighLightTime6 = StartCoroutine(ToggleHighLightTime6());
        }

        SendCommand(6, 2, "AUTO_PICKUPING_STOP");//向服务器发送指令  指令内容为true

        OperationLogs("自动取料停止");//传入数据库记录操作历史记录

        //自动取料停止二次确认
        Debug.Log("自动取料停止二次确认");

        Thread.Sleep(1000);

        SendCommand(6, 2, "AUTO_PICKUPING_STOP_CONFIRM");//向服务器发送指令  指令内容为true
        OperationLogs("自动取料确认停止");//传入数据库记录操作历史记录
    }
    #endregion

    public void AutoStackingAndFeedingSecondConfirmDown()//二次确认后 根据标识位 对远程驱动发送指令 -ljz
    {
        try
        {
            if (SecondConfirmMode == 1)
            {
                Thread thread1 = new Thread(process1);//自动堆料启动二次确认

                thread1.Start();

                string IntroductoryTextValue1 = "自动堆料启动";//开启遮罩 -ljz
                string CountdownTextValue1 = "自动堆料启动中...";
                StartCoroutine(BlockOtherButtons(IntroductoryTextValue1, CountdownTextValue1));
            }
            else if (SecondConfirmMode == 2)
            {
                Thread thread2 = new Thread(process2);//自动堆料停止二次确认

                thread2.Start();
            }
            else if (SecondConfirmMode == 3)
            {
                Thread thread3 = new Thread(process3);//自动取料启动二次确认

                thread3.Start();

                string IntroductoryTextValue2 = "自动取料启动";//开启遮罩 -ljz
                string CountdownTextValue2 = "自动取料启动中...";
                StartCoroutine(BlockOtherButtons(IntroductoryTextValue2, CountdownTextValue2));
            }
            else if (SecondConfirmMode == 4)
            {
                Thread thread4 = new Thread(process4);//自动取料停止二次确认

                thread4.Start();
            }
            else if (SecondConfirmMode == 5)
            {
                //堆料胶带启动按钮二次确认
                try
                {
                    Debug.Log("堆料胶带启动按钮按下");
                    SendCommand(6, 2, "STACKING_START");//向服务器发送指令  指令内容为true
                    OperationLogs("堆料胶带启动");//传入数据库记录操作历史记录
                }
                catch
                {
                    Debug.Log("指令发送异常");
                }
            }
            else if (SecondConfirmMode == 6)
            {
                //堆料胶带停止按钮二次确认
                try
                {
                    Debug.Log("堆料胶带停止按钮按下");
                    SendCommand(6, 2, "STACKING_STOP");//向服务器发送指令  指令内容为flase
                    OperationLogs("堆料胶带停止");//传入数据库记录操作历史记录
                }
                catch
                {
                    Debug.Log("指令发送异常");
                }
            }
            else if (SecondConfirmMode == 7)
            {
                //启动自动堆料二次确认
                try
                {
                    Debug.Log("启动自动堆料");
                    //RemoteAutoStackingModeSelectionCommand.interactable = false;
                    //RemoteManualStackingModeSelectionCommand.interactable = true;
                    //SendCommand(6, 2, "STACKING_MODE_HM", 0);//向服务器发送指令  指令内容为flase
                    SendCommand(6, 2, "STACKING_MODE_AUTO", 1);//向服务器发送指令  指令内容为flase
                    OperationLogs("启动自动堆料");//传入数据库记录操作历史记录
                }
                catch
                {
                    Debug.Log("指令发送异常");
                }
            }
            else if (SecondConfirmMode == 8)
            {
                //启动手动堆料二次确认
                try
                {
                    Debug.Log("启动手动堆料");
                    //RemoteAutoStackingModeSelectionCommand.interactable = true;
                    //RemoteManualStackingModeSelectionCommand.interactable = false;
                    SendCommand(6, 2, "STACKING_MODE_HM", 1);//向服务器发送指令  指令内容为flase
                    //SendCommand(6, 2, "STACKING_MODE_AUTO", 0);//向服务器发送指令  指令内容为flase
                    OperationLogs("启动手动堆料");//传入数据库记录操作历史记录
                }
                catch
                {
                    Debug.Log("指令发送异常");
                }
            }
            else if (SecondConfirmMode == 9)
            {
                //动力电源合闸二次确认
                try
                {
                    Debug.Log("动力电源合闸按钮按下");
                    SendCommand(6, 2, "SUPPLYPOWER_ON");//向服务器发送指令  指令内容为flase
                    OperationLogs("动力电源合闸");//传入数据库记录操作历史记录
                }
                catch
                {
                    Debug.Log("指令发送异常");
                }

            }
            else if (SecondConfirmMode == 10)
            {
                //动力电源分闸二次确认
                try
                {
                    Debug.Log("动力电源分闸按钮按下");
                    SendCommand(6, 2, "SUPPLYPOWER_OFF");//向服务器发送指令  指令内容为flase
                    OperationLogs("动力电源分闸");//传入数据库记录操作历史记录
                }
                catch
                {
                    Debug.Log("指令发送异常");
                }

            }
            else if (SecondConfirmMode == 11)
            {
                //控制电源合闸二次确认
                try
                {
                    Debug.Log("控制电源合闸按钮按下");
                    SendCommand(6, 2, "CONTROLPOWER_ON");//向服务器发送指令  指令内容为flase
                    OperationLogs("控制电源合闸");//传入数据库记录操作历史记录
                }
                catch
                {
                    Debug.Log("指令发送异常");
                }

            }
            else if (SecondConfirmMode == 12)
            {
                //控制电源分闸二次确认
                try
                {
                    Debug.Log("控制电源分闸按钮按下");
                    SendCommand(6, 2, "CONTROLPOWER_OFF");//向服务器发送指令  指令内容为flase
                    OperationLogs("控制电源分闸");//传入数据库记录操作历史记录
                }
                catch
                {
                    Debug.Log("指令发送异常");
                }

            }
            else if (SecondConfirmMode == 13)
            {
                //与系统联锁二次确认
                try
                {
                    Debug.Log("与系统联锁");
                    //RemoteInterlockLockCommand.interactable = false;
                    //RemoteInterUnlockLockCommand.interactable = true;
                    SendCommand(6, 2, "SYSTEM_LOCK", 1);
                    OperationLogs("与系统联锁");//传入数据库记录操作历史记录
                }
                catch
                {
                    Debug.Log("指令发送异常");
                }

            }
            else if (SecondConfirmMode == 14)
            {
                //与系统解锁二次确认
                try
                {
                    //RemoteInterlockLockCommand.interactable = true;
                    //RemoteInterUnlockLockCommand.interactable = false;
                    Debug.Log("与系统解锁");
                    SendCommand(6, 2, "SYSTEM_UNLOCK", 1);
                    OperationLogs("与系统解锁");//传入数据库记录操作历史记录
                }
                catch
                {
                    Debug.Log("指令发送异常");
                }

            }
            else if (SecondConfirmMode == 15)
            {
                //旁路功能二次确认
                try
                {
                    Debug.Log("旁路功能按钮按下");
                    SendCommand(6, 2, "BYPASS_BUTTON");//向服务器发送指令  指令内容为flase
                    OperationLogs("旁路功能");//传入数据库记录操作历史记录
                }
                catch
                {
                    Debug.Log("指令发送异常");
                }

            }
            else if (SecondConfirmMode == 16)
            {
                //故障复位二次确认
                try
                {
                    Debug.Log("故障复位按钮按下");
                    //开启2秒字体高光 -ljz
                    if (myToggleHighLightTime9 == null)
                    {
                        myToggleHighLightTime9 = StartCoroutine(ToggleHighLightTime9());
                    }
                    SendCommand(6, 2, "FAULT_RESET");//向服务器发送指令  指令内容为flase
                    OperationLogs("故障复位");//传入数据库记录操作历史记录
                }
                catch
                {
                    Debug.Log("指令发送异常");
                }

            }
            else if (SecondConfirmMode == 17)
            {
                //刮板启动二次确认
                try
                {
                    Debug.Log("刮板启动按钮按下");
                    SendCommand(6, 2, "SCRAPER_CONTROL");//向服务器发送指令  指令内容为flase
                    OperationLogs("刮板启动");//传入数据库记录操作历史记录
                }
                catch
                {
                    Debug.Log("指令发送异常");
                }

            }
            else if (SecondConfirmMode == 18)
            {
                //刮板停止二次确认
                try
                {
                    Debug.Log("刮板停止按钮按下");
                    SendCommand(6, 2, "SCRAPER_STOP_BUTTON");//向服务器发送指令  指令内容为flase
                    OperationLogs("刮板停止");//传入数据库记录操作历史记录
                }
                catch
                {
                    Debug.Log("指令发送异常");
                }

            }
            else if (SecondConfirmMode == 19)
            {
                //启动自动取料二次确认
                try
                {
                    Debug.Log("启动自动取料");

                    //RemoteAutoMaterialPickupModeSelectionCommand.interactable = false;
                    //RemoteManualMaterialPickupModeSelectionCommand.interactable = true;
                    //SendCommand(6, 2, "PICKUPING_MODE_HM", 0);//向服务器发送指令  指令内容为flase
                    SendCommand(6, 2, "PICKUPING_MODE_AUTO", 1);//向服务器发送指令  指令内容为flase
                    OperationLogs("启动自动取料");//传入数据库记录操作历史记录
                }
                catch
                {
                    Debug.Log("指令发送异常");
                }

            }
            else if (SecondConfirmMode == 20)
            {
                //启动手动取料二次确认
                try
                {
                    Debug.Log("启动手动取料");

                    //RemoteAutoMaterialPickupModeSelectionCommand.interactable = true;
                    //RemoteManualMaterialPickupModeSelectionCommand.interactable = false;
                    SendCommand(6, 2, "PICKUPING_MODE_HM", 1);//向服务器发送指令  指令内容为flase
                    //SendCommand(6, 2, "PICKUPING_MODE_AUTO", 0);//向服务器发送指令  指令内容为flase
                    OperationLogs("启动手动取料");//传入数据库记录操作历史记录
                }
                catch
                {
                    Debug.Log("指令发送异常");
                }

            }
            //todo: 堆料检修、取料检修确认
            else if (SecondConfirmMode == 21)
            {

                StackingServiceCommandUp();

            }
            else if (SecondConfirmMode == 22)
            {
                FeedingServiceCommandUp();

            }
            else if (SecondConfirmMode == 23)
            {
                RemoteEmergencyStopCommandUp();

            }
            else if (SecondConfirmMode == 24)//取料编码器手动校准按钮按下 二次确认 ljz
            {
                try
                {
                    Debug.Log("取料编码器手动校准按钮按下");
                    //开启2秒字体高光 -ljz
                    if (myToggleHighLightTime13 == null)
                    {
                        myToggleHighLightTime13 = StartCoroutine(ToggleHighLightTime13());
                    }
                    SendCommand(6, 2, "FEEDER_ENCODE_ALIGN_BUTTON");//向服务器发送指令  指令内容为flase
                    // 重置取料编码器角度
                    PeekingFetchingEncodeAngle.text = fullVariables.ManuallyCalibratedPresetValueForFeederEncoderAngle.ToString();
                    OperationLogs("取料编码器手动校准");//传入数据库记录操作历史记录
                }
                catch
                {
                    Debug.Log("指令发送异常");
                }
            }
            else if (SecondConfirmMode == 25)//堆料编码器手动校准按钮按下 二次确认 ljz
            {
                try
                {
                    Debug.Log("堆料编码器手动校准按钮按下");
                    //开启2秒字体高光 -ljz
                    if (myToggleHighLightTime11 == null)
                    {
                        myToggleHighLightTime11 = StartCoroutine(ToggleHighLightTime11());
                    }
                    SendCommand(6, 2, "STACKER_ENCODE_ALIGN_BUTTON");//向服务器发送指令  指令内容为flase
                    //重置堆料编码器角度
                    MaterialFetchingEncodeAngle.text = fullVariables.ManuallyCalibratedPresetValueForStackerEncoderAngle.ToString();
                    OperationLogs("堆料编码器手动校准");//传入数据库记录操作历史记录
                }
                catch
                {
                    Debug.Log("指令发送异常");
                }
            }
            else if (SecondConfirmMode == 0)
            {
                SendCommandThreeD(3, 0);
                OperationLogs("综合监控手动关闭");
                if (mainSocketThreeD != null && mainSocketThreeD.IsOpen)
                {
                    mainSocketThreeD.Close();
                    antiInit();
                }

                if (mainSocketPlc != null && mainSocketPlc.IsOpen)
                {
                    mainSocketPlc.Close();
                    antiInit2();
                }

                //string processesName = "HuangtaiPowerPlantControlSystem";
                //System.Diagnostics.Process[] targetProcess = System.Diagnostics.Process.GetProcessesByName("HuangtaiPowerPlantControlSystem");
                //foreach (System.Diagnostics.Process process in targetProcess)
                //{
                //    if (processesName == process.ProcessName)
                //    {
                //        process.Kill();
                //    }
                //}
                //System.Diagnostics.Process.GetCurrentProcess().Kill();
                
                Application.Quit();//退出系统
            }
            else
            {
                //状态异常
                Debug.Log("状态异常");
            }
        }
        catch
        {
            Debug.Log("指令发送异常");
        }
    }
    #endregion

    #region 监听故障报警

    #region 定义监听事件
    //PLC1变量
    private EventListener MaterialStackingRotaryInverterFaultEvent = new EventListener();//堆料回转变频器故障
    private EventListener AmplitudeMotorOverheatProtectionEvent = new EventListener();//变幅电机过热保护
    private EventListener MaterialConveyorMotorProtectionEvent = new EventListener();//堆料胶带电机保护
    private EventListener ElectricalRoomEmergencyStopButtonEvent = new EventListener();//电气室急停按钮
    private EventListener DriverCabinEmergencyStopButtonEvent = new EventListener();//司机室急停按钮
    private EventListener DriverCabinSignalResetButtonEvent = new EventListener();//司机室信号复位按钮
    private EventListener FaultAlarmEvent = new EventListener();//故障报警
    private EventListener CircularStackFaultEvent = new EventListener();//圆堆故障
    private EventListener ScraperTensionLimitSwitchEvent = new EventListener();//刮板张紧限位
    private EventListener ScraperLooseLimitSwitchEvent = new EventListener();//刮板过松限位
    private EventListener MaterialFetchingRotaryInverterFaultEvent = new EventListener();//取料回转变频器故障
    private EventListener AmplitudeWeightLimiterOverloadAlarm1Event = new EventListener();//变幅重量限制器超载报警1
    private EventListener AmplitudeInverterFaultEvent = new EventListener();//变幅变频器故障
    private EventListener AmplitudeUpLimitSwitchEvent = new EventListener();//变幅上仰限位
    private EventListener AmplitudeUpExtremeLimitEvent = new EventListener();//变幅上仰极限
    private EventListener AmplitudeLowerLimitSwitchEvent = new EventListener();//变幅下俯限位
    private EventListener ElectricalRoomFireAlarmSignalEvent = new EventListener();//电气室火灾报警信号
    private EventListener ElectricalRoomSignalResetButtonEvent = new EventListener();//电气室信号复位按钮
    private EventListener DriverCabinFireAlarmSignalEvent = new EventListener();//司机室火灾报警信号
    private EventListener RemoteEmergencyStopControlEvent = new EventListener();//远程急停控制
    private EventListener EquipmentFaultEvent = new EventListener();//设备故障
    private EventListener ScraperHydraulicTensionerFaultEvent = new EventListener();//刮板液压张紧故障
    private EventListener ScraperMotorOverheatProtectionEvent = new EventListener();//刮板电机过热保护
    private EventListener MaterialFetchingRotaryBrakeResistorTemperatureAlarmEvent = new EventListener();//取料回转制动电阻温控报警
    private EventListener AmplitudeWeightLimiterOverloadAlarm2Event = new EventListener();//变幅重量限制器超载报警2
    //private EventListener MaterialStackingBeltPrimaryDeviationEvent = new EventListener();//堆料皮带一级跑偏
    private EventListener MaterialStackingBeltSecondaryDeviationEvent = new EventListener();//堆料皮带二级跑偏
    private EventListener MaterialStackingBeltRopeProtectionEvent = new EventListener();//堆料皮带拉绳保护
    private EventListener MaterialStackingBeltLongitudinalTearDetectionEvent = new EventListener();//堆料皮带纵向撕裂检测
    private EventListener SystemEmergencyStopCommandEvent = new EventListener();//系统急停命令
    private EventListener UnattendedEmergencyStopEvent = new EventListener();//无人值守急停
    private EventListener WaterPumpMotorOverloadEvent = new EventListener();//水泵电机过载
    private EventListener MaterialFetchingRotaryLeftTurnLimitEvent = new EventListener();//取料回转左转限位
    private EventListener MaterialFetchingRotaryLeftTurnExtremeLimitEvent = new EventListener();//取料回转左转极限限位
    private EventListener MaterialFetchingRotaryRightTurnLimitEvent = new EventListener();//取料回转右转限位
    private EventListener MaterialFetchingRotaryRightTurnExtremeLimitEvent = new EventListener();//取料回转右转极限
    private EventListener MaterialFetchingRotaryLeftCollisionSwitch1Event = new EventListener();//取料回转左转防撞开关1
    private EventListener MaterialFetchingRotaryLeftCollisionSwitch2Event = new EventListener();//取料回转左转防撞开关2
    private EventListener MaterialFetchingRotaryRightCollisionSwitch1Event = new EventListener();//取料回转右转防撞开关1
    private EventListener MaterialFetchingRotaryRightCollisionSwitch2Event = new EventListener();//取料回转右转防撞开关2
    private EventListener MaterialStackingRotaryMainCircuitBreakerEvent = new EventListener();//堆料回转主断路器
    private EventListener AmplitudeMotorMainCircuitBreakerEvent = new EventListener();//变幅电机主断路器
    private EventListener MaterialConveyorMotorMainCircuitBreakerEvent = new EventListener();//堆料胶带电机主断路器
    private EventListener ScraperMotorMainCircuitBreakerEvent = new EventListener();//刮板电机主断路器
    private EventListener MaterialFetchingRotaryMainCircuitBreakerEvent = new EventListener();//取料回转主断路器
    private EventListener MaterialFetchingRotaryMotorCircuitBreakerEvent = new EventListener();//取料回转电机断路器
    private EventListener MaterialFetchingRotaryBrakeCircuitBreakerEvent = new EventListener();//取料回转制动器断路器
    private EventListener AmplitudeBrakeOpeningLimitEvent = new EventListener();//变幅制动器打开限位
    private EventListener MaterialFetchingCoalBucketBlockageSwitchEvent = new EventListener();//取料落煤斗堵煤开关
    private EventListener MaterialStackingBeltSpeedDetectionEvent = new EventListener();//堆料皮带速度检测
    //PLC2变量
    private EventListener LeftFrontVerticalLevelMeterScrapingProtectionEvent = new EventListener();//左前垂直料位计禁止刮板下俯
    private EventListener RightFrontVerticalLevelMeterScrapingProtectionEvent = new EventListener();//右前垂直料位计禁止刮板下俯
    private EventListener LeftLevelMeterProtectionForbidLeftTurnEvent = new EventListener();//左侧斜向料位计禁止左转
    private EventListener RightLevelMeterProtectionForbidRightTurnEvent = new EventListener();//右侧斜向料位计禁止右转
    private EventListener SmallAngleScraperDownProtectionEvent = new EventListener();//倾角仪小角度禁止刮板下俯
    private EventListener LargeAngleScraperUpProtectionEvent = new EventListener();//倾角仪大角度禁止刮板上仰
    private EventListener SmallAngleScraperDownDecelerationZoneEvent = new EventListener();//倾角仪小角度刮板下俯减速
    private EventListener LargeAngleScraperUpDecelerationZoneEvent = new EventListener();//倾角仪大角度刮板上仰减速
    private EventListener ArmAngleScraperLeftTurnProhibitionEvent = new EventListener();//夹角保护刮板禁止左转
    private EventListener ArmAngleScraperRightTurnProhibitionEvent = new EventListener();//夹角保护刮板禁止右转
    private EventListener ArmAngleBucketLeftTurnProhibitionEvent = new EventListener();//夹角保护堆料臂禁止左转
    private EventListener ArmAngleBucketRightTurnProhibitionEvent = new EventListener();//夹角保护堆料臂禁止右转
    private EventListener ArmAngleScraperLeftTurnDecelerationZoneEvent = new EventListener();//夹角保护刮板左转减速区
    private EventListener ArmAngleScraperRightTurnDecelerationZoneEvent = new EventListener();//夹角保护刮板右转减速区
    private EventListener ArmAngleBucketLeftTurnDecelerationZoneEvent = new EventListener();//夹角保护堆料臂左转减速区
    private EventListener ArmAngleBucketRightTurnDecelerationZoneEvent = new EventListener();//夹角保护堆料臂右转减速区
    private EventListener VerticalLevelSensorFailureLeftFrontEvent = new EventListener();//左前垂直料位计异常
    private EventListener InclinedLevelSensorFailureLeftFrontEvent = new EventListener();//左前斜向料位计异常
    private EventListener InclinedLevelSensorFailureLeftRearEvent = new EventListener();//左后斜向料位计异常
    private EventListener VerticalLevelSensorFailureLeftRearEvent = new EventListener();//左后垂直料位计异常
    private EventListener VerticalLevelSensorFailureRightFrontEvent = new EventListener();//右前垂直料位计异常
    private EventListener InclinedLevelSensorFailureRightFrontEvent = new EventListener();//右前斜向料位计异常
    private EventListener InclinedLevelSensorFailureRightRearEvent = new EventListener();//右后斜向料位计异常
    private EventListener VerticalLevelSensorFailureRightRearEvent = new EventListener();//右后垂直料位计异常
    private EventListener UltrasonicLevelSensorFailureBottomEvent = new EventListener();//取料斗料位计异常
    private EventListener AngleSensorFailureEvent = new EventListener();//倾角仪异常
    private EventListener BucketLevelSensorFailureLeftEvent = new EventListener();//堆料臂左侧料位计异常
    private EventListener BucketLevelSensorFailureRightEvent = new EventListener();//堆料臂右侧料位计异常
    private EventListener ArmRotationEncoderFailureEvent = new EventListener();//取料臂回转编码器异常
    private EventListener BucketArmRotationEncoderFailureEvent = new EventListener();//堆料臂回转编码器异常
    private EventListener ScraperMotorHighCurrentAlarmEvent = new EventListener();//刮板电机电流过大
    #endregion

    #region 定义监听事件
    private void MaterialStackingRotaryInverterFaultEventFun()//堆料回转变频器故障
    {
        MaterialStackingRotaryInverterFaultEvent.Info = "堆料回转变频器故障!";
        MaterialStackingRotaryInverterFaultEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void AmplitudeMotorOverheatProtectionEventFun()//变幅电机过热保护
    {
        AmplitudeMotorOverheatProtectionEvent.Info = "变幅电机过热保护异常!";
        AmplitudeMotorOverheatProtectionEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void MaterialConveyorMotorProtectionEventFun()//堆料胶带电机保护
    {
        MaterialConveyorMotorProtectionEvent.Info = "堆料胶带电机保护异常!";
        MaterialConveyorMotorProtectionEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void ElectricalRoomEmergencyStopButtonEventFun()//电气室急停按钮
    {
        ElectricalRoomEmergencyStopButtonEvent.Info = "电气室急停按钮异常!";
        ElectricalRoomEmergencyStopButtonEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void DriverCabinEmergencyStopButtonEventFun()//司机室急停按钮
    {
        DriverCabinEmergencyStopButtonEvent.Info = "司机室急停按钮异常!";
        DriverCabinEmergencyStopButtonEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void DriverCabinSignalResetButtonEventFun()//司机室信号复位按钮
    {
        DriverCabinSignalResetButtonEvent.Info = "司机室信号复位按钮异常!";
        DriverCabinSignalResetButtonEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void FaultAlarmEventFun()//故障报警
    {
        FaultAlarmEvent.Info = "蜂鸣故障报警!";
        FaultAlarmEvent.OnVariableChange += AlarmTest;//绑定监听事件;
        FaultAlarmEvent.OnVariableChange += PlayFaultAlarmBuzzer;
    }
    private void CircularStackFaultEventFun()//圆堆故障
    {
        CircularStackFaultEvent.Info = "圆堆故障报警异常!";
        CircularStackFaultEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void ScraperTensionLimitSwitchEventFun()//刮板张紧限位
    {
        ScraperTensionLimitSwitchEvent.Info = "刮板张紧限位异常!";
        ScraperTensionLimitSwitchEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void ScraperLooseLimitSwitchEventFun()//刮板过松限位
    {
        ScraperLooseLimitSwitchEvent.Info = "刮板过松限位异常!";
        ScraperLooseLimitSwitchEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void MaterialFetchingRotaryInverterFaultEventFun()//取料回转变频器故障
    {
        MaterialFetchingRotaryInverterFaultEvent.Info = "取料回转变频器故障!";
        MaterialFetchingRotaryInverterFaultEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void AmplitudeWeightLimiterOverloadAlarm1EventFun()//变幅重量限制器超载报警1
    {
        AmplitudeWeightLimiterOverloadAlarm1Event.Info = "变幅重量限制器超载报警1异常!";
        AmplitudeWeightLimiterOverloadAlarm1Event.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void AmplitudeInverterFaultEventFun()//变幅变频器故障
    {
        AmplitudeInverterFaultEvent.Info = "变幅变频器故障!";
        AmplitudeInverterFaultEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void AmplitudeUpLimitSwitchEventFun()//变幅上仰限位
    {
        AmplitudeUpLimitSwitchEvent.Info = "变幅上仰限位异常!";
        AmplitudeUpLimitSwitchEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void AmplitudeUpExtremeLimitEventFun()//变幅上仰极限
    {
        AmplitudeUpExtremeLimitEvent.Info = "变幅上仰极限异常!";
        AmplitudeUpExtremeLimitEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void AmplitudeLowerLimitSwitchEventFun()//变幅下俯限位
    {
        AmplitudeLowerLimitSwitchEvent.Info = "变幅下俯限位异常!";
        AmplitudeLowerLimitSwitchEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void ElectricalRoomFireAlarmSignalEventFun()//电气室火灾报警信号
    {
        ElectricalRoomFireAlarmSignalEvent.Info = "电气室火灾报警信号异常!";
        ElectricalRoomFireAlarmSignalEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void ElectricalRoomSignalResetButtonEventFun()//电气室信号复位按钮
    {
        ElectricalRoomSignalResetButtonEvent.Info = "电气室信号复位按钮异常!";
        ElectricalRoomSignalResetButtonEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void DriverCabinFireAlarmSignalEventFun()//司机室火灾报警信号
    {
        DriverCabinFireAlarmSignalEvent.Info = "司机室火灾报警信号异常!";
        DriverCabinFireAlarmSignalEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void RemoteEmergencyStopControlEventFun()//远程急停控制
    {
        RemoteEmergencyStopControlEvent.Info = "远程急停控制异常!";
        RemoteEmergencyStopControlEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void EquipmentFaultEventFun()//设备故障
    {
        EquipmentFaultEvent.Info = "设备故障报警!";
        EquipmentFaultEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void ScraperHydraulicTensionerFaultEventFun()//刮板液压张紧故障
    {
        ScraperHydraulicTensionerFaultEvent.Info = "刮板液压张紧故障!";
        ScraperHydraulicTensionerFaultEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void ScraperMotorOverheatProtectionEventFun()//刮板电机过热保护
    {
        ScraperMotorOverheatProtectionEvent.Info = "刮板电机过热保护异常!";
        ScraperMotorOverheatProtectionEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void MaterialFetchingRotaryBrakeResistorTemperatureAlarmEventFun()//取料回转制动电阻温控报警
    {
        MaterialFetchingRotaryBrakeResistorTemperatureAlarmEvent.Info = "取料回转制动电阻温控报警!";
        MaterialFetchingRotaryBrakeResistorTemperatureAlarmEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void AmplitudeWeightLimiterOverloadAlarm2EventFun()//变幅重量限制器超载报警2
    {
        AmplitudeWeightLimiterOverloadAlarm2Event.Info = "变幅重量限制器超载报警2异常!";
        AmplitudeWeightLimiterOverloadAlarm2Event.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void MaterialStackingBeltSecondaryDeviationEventFun()//堆料皮带二级跑偏
    {
        MaterialStackingBeltSecondaryDeviationEvent.Info = "堆料皮带二级跑偏异常!";
        MaterialStackingBeltSecondaryDeviationEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void MaterialStackingBeltRopeProtectionEventFun()//堆料皮带拉绳保护
    {
        MaterialStackingBeltRopeProtectionEvent.Info = "堆料皮带拉绳保护异常!";
        MaterialStackingBeltRopeProtectionEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void MaterialStackingBeltLongitudinalTearDetectionEventFun()//堆料皮带纵向撕裂检测
    {
        MaterialStackingBeltLongitudinalTearDetectionEvent.Info = "堆料皮带纵向撕裂检测异常!";
        MaterialStackingBeltLongitudinalTearDetectionEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void SystemEmergencyStopCommandEventFun()//系统急停命令
    {
        SystemEmergencyStopCommandEvent.Info = "系统急停命令!";
        SystemEmergencyStopCommandEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void UnattendedEmergencyStopEventFun()//无人值守急停
    {
        UnattendedEmergencyStopEvent.Info = "无人值守急停!";
        UnattendedEmergencyStopEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void WaterPumpMotorOverloadEventFun()//水泵电机过载
    {
        WaterPumpMotorOverloadEvent.Info = "水泵电机过载!";
        WaterPumpMotorOverloadEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void MaterialFetchingRotaryLeftTurnLimitEventFun()//取料回转左转限位
    {
        MaterialFetchingRotaryLeftTurnLimitEvent.Info = "取料回转左转限位异常!";
        MaterialFetchingRotaryLeftTurnLimitEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void MaterialFetchingRotaryLeftTurnExtremeLimitEventFun()//取料回转左转极限限位
    {
        MaterialFetchingRotaryLeftTurnExtremeLimitEvent.Info = "取料回转左转极限限位异常!";
        MaterialFetchingRotaryLeftTurnExtremeLimitEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void MaterialFetchingRotaryRightTurnLimitEventFun()//取料回转右转限位
    {
        MaterialFetchingRotaryRightTurnLimitEvent.Info = "取料回转右转限位异常!";
        MaterialFetchingRotaryRightTurnLimitEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void MaterialFetchingRotaryRightTurnExtremeLimitEventFun()//取料回转右转极限
    {
        MaterialFetchingRotaryRightTurnExtremeLimitEvent.Info = "取料回转右转极限异常!";
        MaterialFetchingRotaryRightTurnExtremeLimitEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void MaterialFetchingRotaryLeftCollisionSwitch1EventFun()//取料回转左转防撞开关1
    {
        MaterialFetchingRotaryLeftCollisionSwitch1Event.Info = "取料回转左转防撞开关1异常!";
        MaterialFetchingRotaryLeftCollisionSwitch1Event.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void MaterialFetchingRotaryLeftCollisionSwitch2EventFun()//取料回转左转防撞开关2
    {
        MaterialFetchingRotaryLeftCollisionSwitch2Event.Info = "取料回转左转防撞开关2异常!";
        MaterialFetchingRotaryLeftCollisionSwitch2Event.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void MaterialFetchingRotaryRightCollisionSwitch1EventFun()//取料回转右转防撞开关1
    {
        MaterialFetchingRotaryRightCollisionSwitch1Event.Info = "取料回转右转防撞开关1异常!";
        MaterialFetchingRotaryRightCollisionSwitch1Event.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void MaterialFetchingRotaryRightCollisionSwitch2EventFun()//取料回转右转防撞开关2
    {
        MaterialFetchingRotaryRightCollisionSwitch2Event.Info = "取料回转右转防撞开关2异常!";
        MaterialFetchingRotaryRightCollisionSwitch2Event.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void MaterialStackingRotaryMainCircuitBreakerEventFun()//堆料回转主断路器
    {
        MaterialStackingRotaryMainCircuitBreakerEvent.Info = "堆料回转主断路器故障!";
        MaterialStackingRotaryMainCircuitBreakerEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void AmplitudeMotorMainCircuitBreakerEventFun()//变幅电机主断路器
    {
        AmplitudeMotorMainCircuitBreakerEvent.Info = "变幅电机主断路器故障!";
        AmplitudeMotorMainCircuitBreakerEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void MaterialConveyorMotorMainCircuitBreakerEventFun()//堆料胶带电机主断路器
    {
        MaterialConveyorMotorMainCircuitBreakerEvent.Info = "堆料胶带电机主断路器故障!";
        MaterialConveyorMotorMainCircuitBreakerEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void ScraperMotorMainCircuitBreakerEventFun()//刮板电机主断路器
    {
        ScraperMotorMainCircuitBreakerEvent.Info = "刮板电机主断路器故障!";
        ScraperMotorMainCircuitBreakerEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void MaterialFetchingRotaryMainCircuitBreakerEventFun()//取料回转主断路器
    {
        MaterialFetchingRotaryMainCircuitBreakerEvent.Info = "取料回转主断路器故障!";
        MaterialFetchingRotaryMainCircuitBreakerEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void MaterialFetchingRotaryMotorCircuitBreakerEventFun()//取料回转电机断路器
    {
        MaterialFetchingRotaryMotorCircuitBreakerEvent.Info = "取料回转电机断路器故障!";
        MaterialFetchingRotaryMotorCircuitBreakerEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void MaterialFetchingRotaryBrakeCircuitBreakerEventFun()//取料回转制动器断路器
    {
        MaterialFetchingRotaryBrakeCircuitBreakerEvent.Info = "取料回转制动器断路器故障!";
        MaterialFetchingRotaryBrakeCircuitBreakerEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void AmplitudeBrakeOpeningLimitEventFun()//变幅制动器打开限位
    {
        AmplitudeBrakeOpeningLimitEvent.Info = "变幅制动器打开限位异常!";
        AmplitudeBrakeOpeningLimitEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void MaterialFetchingCoalBucketBlockageSwitchEventFun()//取料落煤斗堵煤开关
    {
        MaterialFetchingCoalBucketBlockageSwitchEvent.Info = "取料落煤斗堵煤开关异常!";
        MaterialFetchingCoalBucketBlockageSwitchEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void MaterialStackingBeltSpeedDetectionEventFun()//堆料皮带速度检测
    {
        MaterialStackingBeltSpeedDetectionEvent.Info = "堆料皮带速度检测异常!";
        MaterialStackingBeltSpeedDetectionEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    //PLC2变量
    private void LeftFrontVerticalLevelMeterScrapingProtectionEventFun()//左前垂直料位计禁止刮板下俯 -ljz
    {
        LeftFrontVerticalLevelMeterScrapingProtectionEvent.Info = "左前垂直料位计禁止刮板下俯!";
        LeftFrontVerticalLevelMeterScrapingProtectionEvent.IsEnterSql = false;
        LeftFrontVerticalLevelMeterScrapingProtectionEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void RightFrontVerticalLevelMeterScrapingProtectionEventFun()//右前垂直料位计禁止刮板下俯
    {
        RightFrontVerticalLevelMeterScrapingProtectionEvent.Info = "右前垂直料位计禁止刮板下俯!";
        RightFrontVerticalLevelMeterScrapingProtectionEvent.IsEnterSql = false;
        RightFrontVerticalLevelMeterScrapingProtectionEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void LeftLevelMeterProtectionForbidLeftTurnEventFun()//左侧斜向料位计禁止左转 
    {
        LeftLevelMeterProtectionForbidLeftTurnEvent.Info = "左侧斜向料位计禁止左转!";
        LeftLevelMeterProtectionForbidLeftTurnEvent.IsEnterSql = false;
        LeftLevelMeterProtectionForbidLeftTurnEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void RightLevelMeterProtectionForbidRightTurnEventFun()//右侧斜向料位计禁止右转 
    {
        RightLevelMeterProtectionForbidRightTurnEvent.Info = "右侧斜向料位计禁止右转!";
        RightLevelMeterProtectionForbidRightTurnEvent.IsEnterSql = false;
        RightLevelMeterProtectionForbidRightTurnEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void SmallAngleScraperDownProtectionEventFun()//倾角仪小角度禁止刮板下俯 
    {
        SmallAngleScraperDownProtectionEvent.Info = "倾角仪小角度禁止刮板下俯!";
        SmallAngleScraperDownProtectionEvent.IsEnterSql = false;
        SmallAngleScraperDownProtectionEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void LargeAngleScraperUpProtectionEventFun()//倾角仪大角度禁止刮板上仰 
    {
        LargeAngleScraperUpProtectionEvent.Info = "倾角仪大角度禁止刮板上仰!";
        LargeAngleScraperUpProtectionEvent.IsEnterSql = false;
        LargeAngleScraperUpProtectionEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void SmallAngleScraperDownDecelerationZoneEventFun()//倾角仪小角度刮板下俯减速
    {
        SmallAngleScraperDownDecelerationZoneEvent.Info = "倾角仪小角度刮板下俯减速!";
        SmallAngleScraperDownDecelerationZoneEvent.IsEnterSql = false;
        SmallAngleScraperDownDecelerationZoneEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void LargeAngleScraperUpDecelerationZoneEventFun()//倾角仪大角度刮板上仰减速
    {
        LargeAngleScraperUpDecelerationZoneEvent.Info = "倾角仪大角度刮板上仰减速!";
        LargeAngleScraperUpDecelerationZoneEvent.IsEnterSql = false;
        LargeAngleScraperUpDecelerationZoneEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void ArmAngleScraperLeftTurnProhibitionEventFun()//夹角保护刮板禁止左转
    {
        ArmAngleScraperLeftTurnProhibitionEvent.Info = "夹角保护刮板禁止左转!";
        ArmAngleScraperLeftTurnProhibitionEvent.IsEnterSql = false;
        ArmAngleScraperLeftTurnProhibitionEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void ArmAngleScraperRightTurnProhibitionEventFun()//夹角保护刮板禁止右转
    {
        ArmAngleScraperRightTurnProhibitionEvent.Info = "夹角保护刮板禁止右转!";
        ArmAngleScraperRightTurnProhibitionEvent.IsEnterSql = false;
        ArmAngleScraperRightTurnProhibitionEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void ArmAngleBucketLeftTurnProhibitionEventFun()//夹角保护堆料臂禁止左转
    {
        ArmAngleBucketLeftTurnProhibitionEvent.Info = "夹角保护堆料臂禁止左转!";
        ArmAngleBucketLeftTurnProhibitionEvent.IsEnterSql = false;
        ArmAngleBucketLeftTurnProhibitionEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void ArmAngleBucketRightTurnProhibitionEventFun()//夹角保护堆料臂禁止右转
    {
        ArmAngleBucketRightTurnProhibitionEvent.Info = "夹角保护堆料臂禁止右转!";
        ArmAngleBucketRightTurnProhibitionEvent.IsEnterSql = false;
        ArmAngleBucketRightTurnProhibitionEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void ArmAngleScraperLeftTurnDecelerationZoneEventFun()//夹角保护刮板左转减速区
    {
        ArmAngleScraperLeftTurnDecelerationZoneEvent.Info = "夹角保护刮板左转减速区!";
        ArmAngleScraperLeftTurnDecelerationZoneEvent.IsEnterSql = false;
        ArmAngleScraperLeftTurnDecelerationZoneEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void ArmAngleScraperRightTurnDecelerationZoneEventFun()//夹角保护刮板右转减速区
    {
        ArmAngleScraperRightTurnDecelerationZoneEvent.Info = "夹角保护刮板右转减速区!";
        ArmAngleScraperRightTurnDecelerationZoneEvent.IsEnterSql = false;
        ArmAngleScraperRightTurnDecelerationZoneEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void ArmAngleBucketLeftTurnDecelerationZoneEventFun()//夹角保护堆料臂左转减速区
    {
        ArmAngleBucketLeftTurnDecelerationZoneEvent.Info = "夹角保护堆料臂左转减速区!";
        ArmAngleBucketLeftTurnDecelerationZoneEvent.IsEnterSql = false;
        ArmAngleBucketLeftTurnDecelerationZoneEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void ArmAngleBucketRightTurnDecelerationZoneEventFun()//夹角保护堆料臂右转减速区
    {
        ArmAngleBucketRightTurnDecelerationZoneEvent.Info = "夹角保护堆料臂右转减速区!";
        ArmAngleBucketRightTurnDecelerationZoneEvent.IsEnterSql = false;
        ArmAngleBucketRightTurnDecelerationZoneEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void VerticalLevelSensorFailureLeftFrontEventFun()//左前垂直料位计异常
    {
        VerticalLevelSensorFailureLeftFrontEvent.Info = "左前垂直料位计异常!";
        VerticalLevelSensorFailureLeftFrontEvent.IsEnterSql = false;
        VerticalLevelSensorFailureLeftFrontEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void InclinedLevelSensorFailureLeftFrontEventFun()//左前斜向料位计异常
    {
        InclinedLevelSensorFailureLeftFrontEvent.Info = "左前斜向料位计异常!";
        InclinedLevelSensorFailureLeftFrontEvent.IsEnterSql = false;
        InclinedLevelSensorFailureLeftFrontEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void InclinedLevelSensorFailureLeftRearEventFun()//左后斜向料位计异常
    {
        InclinedLevelSensorFailureLeftRearEvent.Info = "左后斜向料位计异常!";
        InclinedLevelSensorFailureLeftRearEvent.IsEnterSql = false;
        InclinedLevelSensorFailureLeftRearEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void VerticalLevelSensorFailureLeftRearEventFun()//左后垂直料位计异常
    {
        VerticalLevelSensorFailureLeftRearEvent.Info = "左后垂直料位计异常!";
        VerticalLevelSensorFailureLeftRearEvent.IsEnterSql = false;
        VerticalLevelSensorFailureLeftRearEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void VerticalLevelSensorFailureRightFrontEventFun()//右前垂直料位计异常
    {
        VerticalLevelSensorFailureRightFrontEvent.Info = "右前垂直料位计异常!";
        VerticalLevelSensorFailureRightFrontEvent.IsEnterSql = false;
        VerticalLevelSensorFailureRightFrontEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void InclinedLevelSensorFailureRightFrontEventFun()//右前斜向料位计异常
    {
        InclinedLevelSensorFailureRightFrontEvent.Info = "右前斜向料位计异常!";
        InclinedLevelSensorFailureRightFrontEvent.IsEnterSql = false;
        InclinedLevelSensorFailureRightFrontEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void InclinedLevelSensorFailureRightRearEventFun()//右后斜向料位计异常
    {
        InclinedLevelSensorFailureRightRearEvent.Info = "右后斜向料位计异常!";
        InclinedLevelSensorFailureRightRearEvent.IsEnterSql = false;
        InclinedLevelSensorFailureRightRearEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void VerticalLevelSensorFailureRightRearEventFun()//右后垂直料位计异常
    {
        VerticalLevelSensorFailureRightRearEvent.Info = "右后垂直料位计异常!";
        VerticalLevelSensorFailureRightRearEvent.IsEnterSql = false;
        VerticalLevelSensorFailureRightRearEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void UltrasonicLevelSensorFailureBottomEventFun()//取料斗料位计异常
    {
        UltrasonicLevelSensorFailureBottomEvent.Info = "取料斗料位计异常!";
        UltrasonicLevelSensorFailureBottomEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void AngleSensorFailureEventFun()//倾角仪异常
    {
        AngleSensorFailureEvent.Info = "倾角仪异常!";
        AngleSensorFailureEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void BucketLevelSensorFailureLeftEventFun()//堆料臂左侧料位计异常
    {
        BucketLevelSensorFailureLeftEvent.Info = "堆料臂左侧料位计异常!";
        BucketLevelSensorFailureLeftEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void BucketLevelSensorFailureRightEventFun()//堆料臂右侧料位计异常
    {
        BucketLevelSensorFailureRightEvent.Info = "堆料臂右侧料位计异常!";
        BucketLevelSensorFailureRightEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void ArmRotationEncoderFailureEventFun()//取料臂回转编码器异常
    {
        ArmRotationEncoderFailureEvent.Info = "取料臂回转编码器异常!";
        ArmRotationEncoderFailureEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    private void BucketArmRotationEncoderFailureEventFun() //堆料臂回转编码器异常
    {
        BucketArmRotationEncoderFailureEvent.Info = "堆料臂回转编码器异常!";
        BucketArmRotationEncoderFailureEvent.OnVariableChange -= AlarmTest;//绑定监听事件;
    }
    private void ScraperMotorHighCurrentAlarmEventFun()//刮板电机电流过大
    {
        ScraperMotorHighCurrentAlarmEvent.Info = "刮板电机电流过大!";
        ScraperMotorHighCurrentAlarmEvent.OnVariableChange += AlarmTest;//绑定监听事件;
    }
    #endregion

    public void AlarmStartListener()
    {
        //PLC1变量
        MaterialStackingRotaryInverterFaultEventFun();//堆料回转变频器故障
        AmplitudeMotorOverheatProtectionEventFun();//变幅电机过热保护
        MaterialConveyorMotorProtectionEventFun();//堆料胶带电机保护
        ElectricalRoomEmergencyStopButtonEventFun();//电气室急停按钮
        DriverCabinEmergencyStopButtonEventFun();//司机室急停按钮
        DriverCabinSignalResetButtonEventFun();//司机室信号复位按钮
        FaultAlarmEventFun();//故障报警
        CircularStackFaultEventFun();//圆堆故障
        ScraperTensionLimitSwitchEventFun();//刮板张紧限位
        ScraperLooseLimitSwitchEventFun();//刮板过松限位
        MaterialFetchingRotaryInverterFaultEventFun();//取料回转变频器故障
        AmplitudeWeightLimiterOverloadAlarm1EventFun();//变幅重量限制器超载报警1
        AmplitudeInverterFaultEventFun();//变幅变频器故障
        AmplitudeUpLimitSwitchEventFun();//变幅上仰限位
        AmplitudeUpExtremeLimitEventFun();//变幅上仰极限
        AmplitudeLowerLimitSwitchEventFun();//变幅下俯限位
        ElectricalRoomFireAlarmSignalEventFun();//电气室火灾报警信号
        ElectricalRoomSignalResetButtonEventFun();//电气室信号复位按钮
        DriverCabinFireAlarmSignalEventFun();//司机室火灾报警信号
        RemoteEmergencyStopControlEventFun();//远程急停控制
        EquipmentFaultEventFun();//设备故障
        ScraperHydraulicTensionerFaultEventFun();//刮板液压张紧故障
        ScraperMotorOverheatProtectionEventFun();//刮板电机过热保护
        MaterialFetchingRotaryBrakeResistorTemperatureAlarmEventFun();//取料回转制动电阻温控报警
        AmplitudeWeightLimiterOverloadAlarm2EventFun();//变幅重量限制器超载报警2
        MaterialStackingBeltSecondaryDeviationEventFun();//堆料皮带二级跑偏
        MaterialStackingBeltRopeProtectionEventFun();//堆料皮带拉绳保护
        MaterialStackingBeltLongitudinalTearDetectionEventFun();//堆料皮带纵向撕裂检测
        //SystemEmergencyStopCommandEventFun();//系统急停命令
        //UnattendedEmergencyStopEventFun();//无人值守急停
        //WaterPumpMotorOverloadEventFun();//水泵电机过载
        MaterialFetchingRotaryLeftTurnLimitEventFun();//取料回转左转限位
        MaterialFetchingRotaryLeftTurnExtremeLimitEventFun();//取料回转左转极限限位
        MaterialFetchingRotaryRightTurnLimitEventFun();//取料回转右转限位
        MaterialFetchingRotaryRightTurnExtremeLimitEventFun();//取料回转右转极限
        MaterialFetchingRotaryLeftCollisionSwitch1EventFun();//取料回转左转防撞开关1
        MaterialFetchingRotaryLeftCollisionSwitch2EventFun();//取料回转左转防撞开关2
        MaterialFetchingRotaryRightCollisionSwitch1EventFun();//取料回转右转防撞开关1
        MaterialFetchingRotaryRightCollisionSwitch2EventFun();//取料回转右转防撞开关2
        MaterialStackingRotaryMainCircuitBreakerEventFun();//堆料回转主断路器
        AmplitudeMotorMainCircuitBreakerEventFun();//变幅电机主断路器
        MaterialConveyorMotorMainCircuitBreakerEventFun();//堆料胶带电机主断路器
        ScraperMotorMainCircuitBreakerEventFun();//刮板电机主断路器
        MaterialFetchingRotaryMainCircuitBreakerEventFun();//取料回转主断路器
        MaterialFetchingRotaryMotorCircuitBreakerEventFun();//取料回转电机断路器
        MaterialFetchingRotaryBrakeCircuitBreakerEventFun();//取料回转制动器断路器
        AmplitudeBrakeOpeningLimitEventFun();//变幅制动器打开限位
        MaterialFetchingCoalBucketBlockageSwitchEventFun();//取料落煤斗堵煤开关
        MaterialStackingBeltSpeedDetectionEventFun();//堆料皮带速度检测

        //PLC2变量
        LeftFrontVerticalLevelMeterScrapingProtectionEventFun();//左前垂直料位计禁止刮板下俯
        RightFrontVerticalLevelMeterScrapingProtectionEventFun();//右前垂直料位计禁止刮板下俯
        LeftLevelMeterProtectionForbidLeftTurnEventFun();//左侧斜向料位计禁止左转
        RightLevelMeterProtectionForbidRightTurnEventFun();//右侧斜向料位计禁止右转
        SmallAngleScraperDownProtectionEventFun();//倾角仪小角度禁止刮板下俯
        LargeAngleScraperUpProtectionEventFun();//倾角仪大角度禁止刮板上仰
        SmallAngleScraperDownDecelerationZoneEventFun();//倾角仪小角度刮板下俯减速
        LargeAngleScraperUpDecelerationZoneEventFun();//倾角仪大角度刮板上仰减速
        ArmAngleScraperLeftTurnProhibitionEventFun();//夹角保护刮板禁止左转
        ArmAngleScraperRightTurnProhibitionEventFun();//夹角保护刮板禁止右转
        ArmAngleBucketLeftTurnProhibitionEventFun();//夹角保护堆料臂禁止左转
        ArmAngleBucketRightTurnProhibitionEventFun();//夹角保护堆料臂禁止右转
        ArmAngleScraperLeftTurnDecelerationZoneEventFun();//夹角保护刮板左转减速区
        ArmAngleScraperRightTurnDecelerationZoneEventFun();//夹角保护刮板右转减速区
        ArmAngleBucketLeftTurnDecelerationZoneEventFun();//夹角保护堆料臂左转减速区
        ArmAngleBucketRightTurnDecelerationZoneEventFun();//夹角保护堆料臂右转减速区
        VerticalLevelSensorFailureLeftFrontEventFun();//左前垂直料位计异常
        InclinedLevelSensorFailureLeftFrontEventFun();//左前斜向料位计异常
        InclinedLevelSensorFailureLeftRearEventFun();//左后斜向料位计异常
        VerticalLevelSensorFailureLeftRearEventFun();//左后垂直料位计异常
        VerticalLevelSensorFailureRightFrontEventFun();//右前垂直料位计异常
        InclinedLevelSensorFailureRightFrontEventFun();//右前斜向料位计异常
        InclinedLevelSensorFailureRightRearEventFun();//右后斜向料位计异常
        VerticalLevelSensorFailureRightRearEventFun();//右后垂直料位计异常
        UltrasonicLevelSensorFailureBottomEventFun();//取料斗料位计异常
        AngleSensorFailureEventFun();//倾角仪异常
        BucketLevelSensorFailureLeftEventFun();//堆料臂左侧料位计异常
        BucketLevelSensorFailureRightEventFun();//堆料臂右侧料位计异常
        ArmRotationEncoderFailureEventFun();//取料臂回转编码器异常
        BucketArmRotationEncoderFailureEventFun();//堆料臂回转编码器异常
        ScraperMotorHighCurrentAlarmEventFun();//刮板电机电流过大
    }

    public void AlarmUpdateBoolValueListener()
    {
        //PLC1变量
        MaterialStackingRotaryInverterFaultEvent.Boolean = !fullVariables.MaterialStackingRotaryInverterFault;//堆料回转变频器故障
        AmplitudeMotorOverheatProtectionEvent.Boolean = fullVariables.AmplitudeMotorOverheatProtection;//变幅电机过热保护
        MaterialConveyorMotorProtectionEvent.Boolean = fullVariables.MaterialConveyorMotorProtection;//堆料胶带电机保护
        ElectricalRoomEmergencyStopButtonEvent.Boolean = fullVariables.ElectricalRoomEmergencyStopButton;//电气室急停按钮
        DriverCabinEmergencyStopButtonEvent.Boolean = fullVariables.DriverCabinEmergencyStopButton;//司机室急停按钮
        DriverCabinSignalResetButtonEvent.Boolean = fullVariables.DriverCabinSignalResetButton;//司机室信号复位按钮
        FaultAlarmEvent.Boolean = fullVariables.FaultAlarm;//故障报警
        CircularStackFaultEvent.Boolean = fullVariables.CircularStackFault;//圆堆故障
        ScraperTensionLimitSwitchEvent.Boolean = !fullVariables.ScraperTensionLimitSwitch;//刮板张紧限位
        ScraperLooseLimitSwitchEvent.Boolean = !fullVariables.ScraperLooseLimitSwitch;//刮板过松限位
        MaterialFetchingRotaryInverterFaultEvent.Boolean = !fullVariables.MaterialFetchingRotaryInverterFault;//取料回转变频器故障
        AmplitudeWeightLimiterOverloadAlarm1Event.Boolean = !fullVariables.AmplitudeWeightLimiterOverloadAlarm1;//变幅重量限制器超载报警1
        AmplitudeInverterFaultEvent.Boolean = !fullVariables.AmplitudeInverterFault;//变幅变频器故障
        AmplitudeUpLimitSwitchEvent.Boolean = !fullVariables.AmplitudeUpLimitSwitch;//变幅上仰限位
        AmplitudeUpExtremeLimitEvent.Boolean = !fullVariables.AmplitudeUpExtremeLimit;//变幅上仰极限
        AmplitudeLowerLimitSwitchEvent.Boolean = !fullVariables.AmplitudeLowerLimitSwitch;//变幅下俯限位
        ElectricalRoomFireAlarmSignalEvent.Boolean = fullVariables.ElectricalRoomFireAlarmSignal;//电气室火灾报警信号
        ElectricalRoomSignalResetButtonEvent.Boolean = fullVariables.ElectricalRoomSignalResetButton;//电气室信号复位按钮
        DriverCabinFireAlarmSignalEvent.Boolean = fullVariables.DriverCabinFireAlarmSignal;//司机室火灾报警信号
        RemoteEmergencyStopControlEvent.Boolean = fullVariables.RemoteEmergencyStopControl;//远程急停控制
        EquipmentFaultEvent.Boolean = fullVariables.EquipmentFault;//设备故障
        ScraperHydraulicTensionerFaultEvent.Boolean = fullVariables.ScraperHydraulicTensionerFault;//刮板液压张紧故障
        ScraperMotorOverheatProtectionEvent.Boolean = fullVariables.ScraperMotorOverheatProtection;//刮板电机过热保护
        MaterialFetchingRotaryBrakeResistorTemperatureAlarmEvent.Boolean = !fullVariables.MaterialFetchingRotaryBrakeResistorTemperatureAlarm;//取料回转制动电阻温控报警
        AmplitudeWeightLimiterOverloadAlarm2Event.Boolean = !fullVariables.AmplitudeWeightLimiterOverloadAlarm2;//变幅重量限制器超载报警2
        MaterialStackingBeltSecondaryDeviationEvent.Boolean = !fullVariables.MaterialStackingBeltSecondaryDeviation;//堆料皮带二级跑偏
        MaterialStackingBeltRopeProtectionEvent.Boolean = !fullVariables.MaterialStackingBeltRopeProtection;//堆料皮带拉绳保护
        MaterialStackingBeltLongitudinalTearDetectionEvent.Boolean = !fullVariables.MaterialStackingBeltLongitudinalTearDetection;//堆料皮带纵向撕裂检测
        //SystemEmergencyStopCommandEvent.Boolean = fullVariables.SystemEmergencyStopCommand;//系统急停命令
        //UnattendedEmergencyStopEvent.Boolean = fullVariables.UnattendedEmergencyStop;//无人值守急停
        //WaterPumpMotorOverloadEvent.Boolean = fullVariables.WaterPumpMotorOverload;//水泵电机过载
        MaterialFetchingRotaryLeftTurnLimitEvent.Boolean = !fullVariables.MaterialFetchingRotaryLeftTurnLimit;//取料回转左转限位
        MaterialFetchingRotaryLeftTurnExtremeLimitEvent.Boolean = !fullVariables.MaterialFetchingRotaryLeftTurnExtremeLimit;//取料回转左转极限限位
        MaterialFetchingRotaryRightTurnLimitEvent.Boolean = !fullVariables.MaterialFetchingRotaryRightTurnLimit;//取料回转右转限位
        MaterialFetchingRotaryRightTurnExtremeLimitEvent.Boolean = !fullVariables.MaterialFetchingRotaryRightTurnExtremeLimit;//取料回转右转极限
        MaterialFetchingRotaryLeftCollisionSwitch1Event.Boolean = !fullVariables.MaterialFetchingRotaryLeftCollisionSwitch1;//取料回转左转防撞开关1
        MaterialFetchingRotaryLeftCollisionSwitch2Event.Boolean = !fullVariables.MaterialFetchingRotaryLeftCollisionSwitch2;//取料回转左转防撞开关2
        MaterialFetchingRotaryRightCollisionSwitch1Event.Boolean = !fullVariables.MaterialFetchingRotaryRightCollisionSwitch1;//取料回转右转防撞开关1
        MaterialFetchingRotaryRightCollisionSwitch2Event.Boolean = !fullVariables.MaterialFetchingRotaryRightCollisionSwitch2;//取料回转右转防撞开关2
        MaterialStackingRotaryMainCircuitBreakerEvent.Boolean = fullVariables.MaterialStackingRotaryMainCircuitBreaker;//堆料回转主断路器
        AmplitudeMotorMainCircuitBreakerEvent.Boolean = fullVariables.AmplitudeMotorMainCircuitBreaker;//变幅电机主断路器
        MaterialConveyorMotorMainCircuitBreakerEvent.Boolean = fullVariables.MaterialConveyorMotorMainCircuitBreaker;//堆料胶带电机主断路器
        ScraperMotorMainCircuitBreakerEvent.Boolean = fullVariables.ScraperMotorMainCircuitBreaker;//刮板电机主断路器
        MaterialFetchingRotaryMainCircuitBreakerEvent.Boolean = fullVariables.MaterialFetchingRotaryMainCircuitBreaker;//取料回转主断路器
        MaterialFetchingRotaryMotorCircuitBreakerEvent.Boolean = fullVariables.MaterialFetchingRotaryMotorCircuitBreaker;//取料回转电机断路器
        MaterialFetchingRotaryBrakeCircuitBreakerEvent.Boolean = fullVariables.MaterialFetchingRotaryBrakeCircuitBreaker;//取料回转制动器断路器
        AmplitudeBrakeOpeningLimitEvent.Boolean = !fullVariables.AmplitudeBrakeOpeningLimit;//变幅制动器打开限位
        MaterialFetchingCoalBucketBlockageSwitchEvent.Boolean = fullVariables.MaterialFetchingCoalBucketBlockageSwitch;//取料落煤斗堵煤开关
        MaterialStackingBeltSpeedDetectionEvent.Boolean = !fullVariables.MaterialStackingBeltSpeedDetection;//堆料皮带速度检测

        //PLC2变量
        LeftFrontVerticalLevelMeterScrapingProtectionEvent.Boolean = fullVariables.LeftFrontVerticalLevelMeterScrapingProtection;//左前垂直料位计禁止刮板下俯
        RightFrontVerticalLevelMeterScrapingProtectionEvent.Boolean = fullVariables.RightFrontVerticalLevelMeterScrapingProtection;//右前垂直料位计禁止刮板下俯
        LeftLevelMeterProtectionForbidLeftTurnEvent.Boolean = fullVariables.LeftLevelMeterProtectionForbidLeftTurn;//左侧斜向料位计禁止左转
        RightLevelMeterProtectionForbidRightTurnEvent.Boolean = fullVariables.RightLevelMeterProtectionForbidRightTurn;//右侧斜向料位计禁止右转
        SmallAngleScraperDownProtectionEvent.Boolean = fullVariables.SmallAngleScraperDownProtection;//倾角仪小角度禁止刮板下俯
        LargeAngleScraperUpProtectionEvent.Boolean = fullVariables.LargeAngleScraperUpProtection;//倾角仪大角度禁止刮板上仰
        SmallAngleScraperDownDecelerationZoneEvent.Boolean = fullVariables.SmallAngleScraperDownDecelerationZone;//倾角仪小角度刮板下俯减速
        LargeAngleScraperUpDecelerationZoneEvent.Boolean = fullVariables.LargeAngleScraperUpDecelerationZone;//倾角仪大角度刮板上仰减速
        ArmAngleScraperLeftTurnProhibitionEvent.Boolean = fullVariables.ArmAngleScraperLeftTurnProhibition;//夹角保护刮板禁止左转
        ArmAngleBucketLeftTurnProhibitionEvent.Boolean = fullVariables.ArmAngleBucketLeftTurnProhibition;//夹角保护堆料臂禁止左转
        ArmAngleBucketRightTurnProhibitionEvent.Boolean = fullVariables.ArmAngleBucketRightTurnProhibition;//夹角保护堆料臂禁止右转
        ArmAngleScraperLeftTurnDecelerationZoneEvent.Boolean = fullVariables.ArmAngleScraperLeftTurnDecelerationZone;//夹角保护刮板左转减速区
        ArmAngleScraperRightTurnDecelerationZoneEvent.Boolean = fullVariables.ArmAngleScraperRightTurnDecelerationZone;//夹角保护刮板右转减速区
        ArmAngleBucketLeftTurnDecelerationZoneEvent.Boolean = fullVariables.ArmAngleBucketLeftTurnDecelerationZone;//夹角保护堆料臂左转减速区
        ArmAngleBucketRightTurnDecelerationZoneEvent.Boolean = fullVariables.ArmAngleBucketRightTurnDecelerationZone;//夹角保护堆料臂右转减速区
        VerticalLevelSensorFailureLeftFrontEvent.Boolean = fullVariables.VerticalLevelSensorFailureLeftFront;//左前垂直料位计异常
        InclinedLevelSensorFailureLeftFrontEvent.Boolean = fullVariables.InclinedLevelSensorFailureLeftFront;//左前斜向料位计异常
        InclinedLevelSensorFailureLeftRearEvent.Boolean = fullVariables.InclinedLevelSensorFailureLeftRear;//左后斜向料位计异常
        VerticalLevelSensorFailureLeftRearEvent.Boolean = fullVariables.VerticalLevelSensorFailureLeftRear;//左后垂直料位计异常
        VerticalLevelSensorFailureRightFrontEvent.Boolean = fullVariables.VerticalLevelSensorFailureRightFront;//右前垂直料位计异常
        InclinedLevelSensorFailureRightFrontEvent.Boolean = fullVariables.InclinedLevelSensorFailureRightFront;//右前斜向料位计异常
        InclinedLevelSensorFailureRightRearEvent.Boolean = fullVariables.InclinedLevelSensorFailureRightRear;//右后斜向料位计异常
        VerticalLevelSensorFailureRightRearEvent.Boolean = fullVariables.VerticalLevelSensorFailureRightRear;//右后垂直料位计异常
        UltrasonicLevelSensorFailureBottomEvent.Boolean = fullVariables.UltrasonicLevelSensorFailureBottom;//取料斗料位计异常
        AngleSensorFailureEvent.Boolean = fullVariables.AngleSensorFailure;//倾角仪异常
        BucketLevelSensorFailureLeftEvent.Boolean = fullVariables.BucketLevelSensorFailureLeft;//堆料臂左侧料位计异常
        BucketLevelSensorFailureRightEvent.Boolean = fullVariables.BucketLevelSensorFailureRight;//堆料臂右侧料位计异常
        ArmRotationEncoderFailureEvent.Boolean = fullVariables.ArmRotationEncoderFailure;//取料臂回转编码器异常
        BucketArmRotationEncoderFailureEvent.Boolean = fullVariables.BucketArmRotationEncoderFailure;//堆料臂回转编码器异常
        ScraperMotorHighCurrentAlarmEvent.Boolean = fullVariables.ScraperMotorHighCurrentAlarm;//刮板电机电流过大

    }

    public void AlarmTest(bool value, int id, string info, string user, bool isEnterSql)//测试报警弹框记录操作 /*1.当前事件  2.报警内容  3.操作用户 4.是否入库*/
    {
        if (value)
        {
            //1.将报警信息写入数据库
            if (DataManager.Instance.CurrentAccount != null)  //YCQ，20231230
            {
                string login_user = DataManager.Instance.CurrentAccount.name;
                string sql = String.Format("insert into warning (time, info, operator) values('{0}', '{1}','{2}');", DateTime.Now, info, login_user, Encoding.UTF8, isEnterSql);
                //判断是否入库 -ljz
                if (isEnterSql)
                {
                    int ret = MySqlHelper.ExecuteSql(sql);
                }
                //StartCoroutine(CreateAlarmPanel(info));
                AddAlarmToTextbox(info);
            }

        }
    }

    IEnumerator CreateAlarmPanel(string _info)
    {
        //ShareRes.WaitMutex();
        //2.弹出警告框提示警告信息
        GameObject AlarmPanel = GameObject.Instantiate(AlarmPanelPrefab, GameObject.Find("Canvas").transform);//生成报警框并设置父对象
        AlarmPanel.GetComponent<RectTransform>().localPosition = Vector3.zero;//报警框位置归零
        AlarmPanel.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = _info;//"堆料回转变频器故障报警!";//设置报警框信息内容

        yield return new WaitForSeconds(5); //5秒后销毁Panel

        Destroy(AlarmPanel.gameObject);//销毁报警框
        //ShareRes.ReleaseMutex();
    }

    void AddAlarmToTextbox(string info)  //YCQ, 20231230
    {
        //将输入的数据处理为告警结构体 -ljz
        int count = recentAlarmsList.Count;
        recentTimeAlarms rta;
        rta.alarm = info;
        TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        rta.createTime = Convert.ToInt64(ts.TotalSeconds).ToString();

        switch (count)
        {
            case 0:
                {
                    recentAlarmsList.Add(rta);
                }
                break;
            case 1:
                {
                    recentTimeAlarms preInfo = recentAlarmsList[0];
                    recentAlarmsList.Clear();
                    recentAlarmsList.Add(rta);
                    recentAlarmsList.Add(preInfo);
                }
                break;
            case 2:
                {
                    recentTimeAlarms preInfo = recentAlarmsList[0];
                    recentTimeAlarms prepreInfo = recentAlarmsList[1];
                    recentAlarmsList.Clear();
                    recentAlarmsList.Add(rta);
                    recentAlarmsList.Add(preInfo);
                    recentAlarmsList.Add(prepreInfo);
                }
                break;

            case 3:
                {
                    recentTimeAlarms preInfo = recentAlarmsList[0];
                    recentTimeAlarms prepreInfo = recentAlarmsList[1];
                    recentAlarmsList.Clear();
                    recentAlarmsList.Add(rta);
                    recentAlarmsList.Add(preInfo);
                    recentAlarmsList.Add(prepreInfo);
                }
                break;
        }
    }

    private void UpdateAlarmData() //十秒后清除列表内数据 十秒内保存数据 -ljz
    {

        TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        string nowTime = Convert.ToInt64(ts.TotalSeconds).ToString();//获取当前时间搓

        if (recentAlarmsList.Count != 0)
        {
            for (int i = 0; i < recentAlarmsList.Count; i++)
            {
                int surplusTime = int.Parse(nowTime) - int.Parse(recentAlarmsList[i].createTime);//剩余时间时间挫
                //超过10s在列表销毁
                if (surplusTime >= 10)
                {
                    recentAlarmsList.Remove(recentAlarmsList[i]);
                }
                else//低于10s保存并显示
                {
                    string newText = recentAlarmsList[0].alarm;
                    for (int j = 1; j < recentAlarmsList.Count; j++)
                    {
                        newText += '\n' + recentAlarmsList[i].alarm;
                    }
                    alarmTextbox.text = newText;
                }
            }
        }
        else
        {
            //列表为空全部清空
            alarmTextbox.text = "";
        }

    }

    private void PlayFaultAlarmBuzzer(bool newVal, int id, string newInfo, string newUser)
    {
        if (newVal && DataManager.Instance.CurrentAccount != null)//判断用户是否登录 -ljz
        {
            faultAlarmAudioSource.Play();
        }
        else
        {
            faultAlarmAudioSource.time = 0;
            faultAlarmAudioSource.Stop();
        }
    }

    private void PlayFaultAlarmBuzzer(bool newVal, int id, string newInfo, string newUser, bool isEnterSql)
    {
        if (newVal && DataManager.Instance.CurrentAccount != null)//判断用户是否登录 -ljz
        {
            faultAlarmAudioSource.Play();
        }
        else
        {
            faultAlarmAudioSource.time = 0;
            faultAlarmAudioSource.Stop();
        }
    }
    #endregion

    #region 获取服务器数据并进行可视化

    #region 声明变量
    /*  变量定义
     *  hostIP:用于存放服务器（本机）IP地址
     *  port:服务器的网络终结点，即IP:端口号
     *  point:存放用户输入的端口号
     *  mainSocket:用于主要数据流传输的套接字
     */
    private WebSocket mainSocketPlc;// 远程驱动
    private WebSocket mainSocketThreeD;//三维扫描 -ljz

    private bool lockReconnect = false;
    private bool lockReconnect2 = false;
    private string strInfo = new string("");
    private string plcData = new string("");
    private bool firstStart = true;

    bool flag;
    public static bool changeThreeDModel = false;

    private FullVariables fullVariables = new FullVariables();  //YCQ, 20231230
    private Mutex mutexFull = new Mutex();
    
    public static CoalHeapDEM curHeapDEM = new CoalHeapDEM();//三维模型数据 ljz
    private Mutex mutexModel = new Mutex();
    #endregion

    delegate void SetTextCallback(string text);

    #region 实例化对象
    //实例化样机（以后有别的类型的机器就添加相应的类到解决方案中，这里是把斗轮堆取料机作为样机）
    private ServerCommand commandPlc = new ServerCommand();
    private ServerCommand commandThreeD = new ServerCommand();
    // 设置序列化选项
    JsonSerializerSettings settingsPlc = new JsonSerializerSettings
    {
        Formatting = Formatting.Indented // 设置缩进格式
    };
    JsonSerializerSettings settingsThreeD = new JsonSerializerSettings
    {
        Formatting = Formatting.Indented // 设置缩进格式
    };
    #endregion

    //模块：网络通信
    #region 网络通信

    #region 尝试连接按钮事件
    private void ConnectPlcServer()
    {
        mainSocketPlc = new WebSocket(new Uri(Address.taoUrl));
        init2();

        try
        {
            if (mainSocketPlc.IsOpen)
            {
                TaoIsConnect = true;
                
            }
            else
            {
                mainSocketPlc.Open();
            }
        }
        catch (Exception ey)
        {
            Debug.Log("远程驱动服务器无法连接\r\n" + ey.Message);
            return;
        }

    }
    private void ConnectThreeDServer()
    {
        mainSocketThreeD = new WebSocket(new Uri(Address.yuanUrl));
        init();
        
        try
        {
            if (mainSocketThreeD.IsOpen)
            {
                //OperationLogs("三维扫描系统已连接");
            }
            else
            {
                mainSocketThreeD.Open();
            }
        }
        catch (Exception ey)
        {
            Debug.Log("三维扫描服务器无法连接\r\n" + ey.Message);
            return;
        }
    }
    #endregion


    #region 网络通信进程
    private void ProccessPlc()
    {
        Thread.Sleep(3000);
        while (true)
        {
            if (mainSocketPlc!=null&&mainSocketPlc.IsOpen)
            {
                if (saveReconnectLogOnce) {
                    saveReconnectLogOnce = false;
                    saveStopLogOnce = true;
                    OperationLogs("远程驱动成功重连！");
                }
                TaoIsConnect = true;
                Debug.Log("发送获取一帧数据命令");
                SendCommand();
            }
            Thread.Sleep(1000); 
        }
    }
    private void ProcessThreeD()
    {
        Thread.Sleep(3000);
        Debug.Log("开始向三维扫描系统发送命令");

        while (true)
        {
            if (mainSocketThreeD != null && mainSocketThreeD.IsOpen) {
                if (firstStart)
                {
                    firstStart = false;
                    SendCommandThreeD(3, 1); //程序启动  给三维 发1
                }
                //更新模型
                Debug.Log("发送获取模型数据命令");
                SendCommandThreeD(3, 4);
            }
            else
            {
                //OperationLogs("三维扫描服务器断开!");
            }

            Thread.Sleep(updateThreeDModelTime);  //线程休息一分钟
        }
    }
    #endregion

    //发送指令到服务器获取JSON
    #region 套接字通信获取JSON
    /// <summary>
    /// 发送查询plc数据指令
    /// </summary>
    /// <param name="type1">数据类型</param>
    /// <param name="type2">命令类型</param>
    /// <param name="type3">命令名</param>
    /// <param name="type4">命令内容</param>
    /// <param name="type5"></param>
    private void SendCommand(int type1 = 6, int type2 = 1, string type3 = "", int type4 = 1, float type5 = 0)
    {
        ServerCommand cmd = SetCommandPlc(type1, type2, type3, type4, type5);
        string json = JsonConvert.SerializeObject(cmd, settingsPlc);

        try
        {
            mainSocketPlc.Send(json);
            Debug.Log("向远程发送的数据是" + json);
        }
        catch (Exception ex)
        {

            Debug.Log("远程驱动命令发送失败 ：" + ex.ToString());
        }

        if (type2 == 1)
        {
            GetPlcJson();
        }
    }
    private void SendCommandThreeD(int type1, int type2)
    {
        
        //查询三维扫描一帧数据
        ServerCommand cmd = SetCommandThreeD(type1, type2);
        string json = JsonConvert.SerializeObject(cmd, settingsThreeD);

        try
        {
            mainSocketThreeD.Send(json);
            Debug.Log("ws发送的数据是" + json);
        }
        catch (Exception ex)
        {
            Debug.Log("三维扫描命令发送失败 ：" + ex.ToString());
        }

        if (type2 == 4) 
        {
            GetThreeDJson();
        }

    }

    //方法：命令命名
    #region 命令命名
    private ServerCommand SetCommandPlc(int TYPE1, int TYPE2, string TYPE3, int TYPE4, float TYPE5)
    {
        ServerCommand command = new ServerCommand();
        command.QUERY_SYSTEM = "MC";
        command.DATA_TYPE = TYPE1;
        command.QUERY_TYPE = TYPE2;
        command.COMMAND_NAME = TYPE3;
        command.COMMAND_TYPE = TYPE4;
        command.COMMAND_DATA = TYPE5;
        return command;
    }
    private ServerCommand SetCommandThreeD(int TYPE1, int TYPE2)
    {
        ServerCommand command = new ServerCommand();
        command.QUERY_SYSTEM = "MC";
        command.DATA_TYPE = TYPE1;
        command.QUERY_TYPE = TYPE2;
        return command;
        
    }
    #endregion

    #region 发送命令到服务器

    private void GetPlcJson()
    {
        try
        {
            Debug.Log("开始获取新的PLC数据");

            if (plcData == null || plcData.Length < 10)
            {
                return;
            }
            string plcJson = Decompress(plcData);//解压数据
            SavePlcSqlData(plcJson);//数据入库
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }
    private void GetThreeDJson()
    {
        try
        {
            Debug.Log("开始获取新的模型数据");

            if (strInfo==null||strInfo.Length<100)
            {
                return;
            }

            //将接收到的“命令”JSON 字符串反序列化为对象
            mutexModel.WaitOne();
            try {
                curHeapDEM = JsonConvert.DeserializeObject<CoalHeapDEM>(Decompress(strInfo));//解压数据
                strInfo = "";

                Debug.Log("获取新的模型数据.......................................................................");

                changeThreeDModel = true;//开始更新
            }
            finally {
                mutexModel.ReleaseMutex(); 
            }
            
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }
    
    //plc数据入库
    private bool SavePlcSqlData(string text)
    {
        mutexFull.WaitOne();//请求互斥锁
        try
        {
            Debug.Log("获取新的PLC数据");
            fullVariables = JsonConvert.DeserializeObject<FullVariables>(text);    //用 Newtonsoft.Json 解析 JSON 数据
            plcData = "";
        }
        finally
        {
            mutexFull.ReleaseMutex(); // 当完成时释放Mutex。
        }

        ++_currentSaveCriticalParameterCounter;
        try
        {
            if (_currentSaveCriticalParameterCounter > 1000000 - 1)
                _currentSaveCriticalParameterCounter = 0;
            if (_currentSaveCriticalParameterCounter % _saveCriticalParameterEveryXRead == 0)
            {
                if (fullVariables.TimeStamp > new DateTime(2023, 1, 1))  //YCQ, 20231230
                {
                    string sql = $"INSERT INTO `criticalparameter` (`time`,`M_ScraperMotor1Current`,`M_ScraperMotor2Current`,`M_MaterialFetchingAmplitudeMotorCurrent`,`M_MaterialFetchingRotaryMotorCurrent`,`M_MaterialStackingBeltMotorCurrent`,`M_MaterialStackingRotaryMotorCurrent`) " +
                       $"VALUES ('{fullVariables.TimeStamp}',{fullVariables.M_ScraperMotor1Current},{fullVariables.M_ScraperMotor2Current},{fullVariables.M_MaterialFetchingAmplitudeMotorCurrent},{fullVariables.M_MaterialFetchingRotaryMotorCurrent},{fullVariables.M_MaterialStackingBeltMotorCurrent},{fullVariables.M_MaterialStackingRotaryMotorCurrent})";
                    //System.Random r = new System.Random();
                    //string sql = $"INSERT INTO `criticalparameter` (`time`,`M_ScraperMotor1Current`,`M_ScraperMotor2Current`,`M_MaterialFetchingAmplitudeMotorCurrent`,`M_MaterialFetchingRotaryMotorCurrent`,`M_MaterialStackingBeltMotorCurrent`,`M_MaterialStackingRotaryMotorCurrent`) " +
                    //    $"VALUES ('{DateTime.Now}',{r.Next() % 71 + 80},{r.Next() % 71 + 80},{r.Next() % 71 + 80},{r.Next() % 71 + 80},{r.Next() % 71 + 80},{r.Next() % 71 + 80})";
                    MySqlHelper.ExecuteSql(sql);
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return false;
        }
    }
    #endregion
    #endregion

    #endregion
    #endregion

    void OnApplicationQuit()//退出程序 关闭远程驱动系统 关闭三维扫描系统
    {
        SecondConfirmMode = 0;
        AutoStackingAndFeedingSecondConfirmDown();

        //SecondConfirmText.text = "确认关闭系统?";
        //SecondConfirmPanel.SetActive(true);

        //Application.CancelQuit();//暂停退出
    }

    private void ResetSpeedByDelayTime() //给Slider的onValueChanged事件添加监听器 -ljz
    {

        PeekingRotateSpeedSliderChangeByDelayTime.GetComponent<Slider>().onValueChanged.AddListener
            ((float value) => PeekingRotateSpeedSliderOnDataChanged(value));

        PeekingTiltSpeedSliderChangeByDelayTime.GetComponent<Slider>().onValueChanged.AddListener
            ((float value) => PeekingTiltSpeedSliderOnDataChanged(value));

        RemoteManualStackingRotateSpeedSliderChangeByDelayTime.GetComponent<Slider>().
            onValueChanged.AddListener((float value) => RemoteManualStackingRotateSpeedSliderOnDataChanged(value));
    }

    #region 取料回转速度 输入 延时重置 -ljz
    public void PeekingRotateSpeedSliderOnDataChanged(float text)
    {
        if (isChangeByCode1)
        {
            // 代码更改文本，则重置标志位并返回
            isChangeByCode1 = false;
            return;
        }

        //确定人为更改并判定协程是否运行 运行则重启 不运行则开启
        if (PeekingRotateSpeedSliderTimerCoroutine != null)
        {
            StopCoroutine(PeekingRotateSpeedSliderTimerCoroutine);
        }

        PeekingRotateSpeedSliderTimerCoroutine = StartCoroutine(PeekingRotateSpeedSliderResetDataByDealyTime());
    }
    //将状态更改为 代码修改 同时执行重置
    private IEnumerator PeekingRotateSpeedSliderResetDataByDealyTime()
    {
        yield return new WaitForSeconds(resetThreeSpeedDataDelayTime); //程序在这里等待delay长的时间后，再去往下执行
        try
        {
            // 设置标志位，表示下面的更改是由代码引起的
            isChangeByCode1 = true;
            //重置设置值
            RemoteManualMaterialPickupRotateSpeedInput.text = "" + fullVariables.MaterialFeederRotationSpeed.ToString();  //从全局变量中获取取料回转速度 ， YCQ， 20231229
            PeekingRotateSpeedSlider.value = fullVariables.MaterialFeederRotationSpeed;// 取料回转速度滑条，YCQ， 20231230

            PeekingRotateSpeedSliderTimerCoroutine = null;
        }
        catch (Exception)
        {
            Debug.Log("20s后重置数据失败");
        }

    }
    #endregion 

    #region 取料俯仰速度 输入 延时重置 -ljz
    public void PeekingTiltSpeedSliderOnDataChanged(float text)
    {
        if (isChangeByCode2)
        {
            // 如果是代码更改文本，则重置标志位并返回
            isChangeByCode2 = false;
            return;
        }

        //确定人为更改并判定协程是否运行 运行则重启 不运行则开启
        if (PeekingTiltSpeedSliderTimerCoroutine != null)
        {
            StopCoroutine(PeekingTiltSpeedSliderTimerCoroutine);
        }

        PeekingTiltSpeedSliderTimerCoroutine = StartCoroutine(PeekingTiltSpeedSliderResetDataByDealyTimeIE());
    }
    //将状态更改为 代码修改 同时执行重置
    private IEnumerator PeekingTiltSpeedSliderResetDataByDealyTimeIE()
    {

        yield return new WaitForSeconds(resetThreeSpeedDataDelayTime); //程序在这里等待delay长的时间后，再去往下执行
        try
        {
            // 设置标志位，表示下面的更改是由代码引起的
            isChangeByCode2 = true;
            //重置设置值
            RemoteManualMaterialPickupTiltSpeedInput.text = "" + fullVariables.MaterialFeederPitchSpeed.ToString();  //从全局变量中获取取料俯仰速度 ， YCQ， 20231229
            PeekingTiltSpeedSlider.value = fullVariables.MaterialFeederPitchSpeed; //取料俯仰速度滑条，YCQ， 20231230

            PeekingTiltSpeedSliderTimerCoroutine = null;
        }
        catch (Exception)
        {
            Debug.Log("20s后重置数据失败");
        }

    }
    #endregion

    #region 堆料回转速度 输入 延时重置 -ljz
    public void RemoteManualStackingRotateSpeedSliderOnDataChanged(float text)
    {
        if (isChangeByCode3)
        {
            // 如果是代码更改文本，则重置标志位并返回
            isChangeByCode3 = false;
            return;
        }

        //确定人为更改并判定协程是否运行 运行则重启 不运行则开启
        if (RemoteManualStackingRotateSpeedSliderTimerCoroutine != null)
        {
            StopCoroutine(RemoteManualStackingRotateSpeedSliderTimerCoroutine);
        }

        RemoteManualStackingRotateSpeedSliderTimerCoroutine = StartCoroutine(RemoteManualStackingRotateSpeedSliderResetDataByDealyTimeIE());
    }
    //将状态更改为 代码修改 同时执行重置
    private IEnumerator RemoteManualStackingRotateSpeedSliderResetDataByDealyTimeIE()
    {
        yield return new WaitForSeconds(resetThreeSpeedDataDelayTime); //程序在这里等待delay长的时间后，再去往下执行
        try
        {
            // 设置标志位，表示下面的更改是由代码引起的
            isChangeByCode3 = true;
            //重置设置值
            RemoteManualStackingRotateSpeedInput.text = "" + fullVariables.MaterialStackerRotationSpeed.ToString();  //从全局变量中获取堆料回转速度 ， YCQ， 20231229
            RemoteManualStackingRotateSpeedSlider.value = fullVariables.MaterialStackerRotationSpeed;// 堆料回转速度滑条，YCQ， 20231230

            RemoteManualStackingRotateSpeedSliderTimerCoroutine = null;
        }
        catch (Exception)
        {
            Debug.Log("20s后重置数据失败");
        }

    }
    #endregion

    void AutoButtonHighLightChange(int typeNum) //主要按钮互斥检测 状态 -ljz
    {
        switch (typeNum)
        {
            //case 0:
            //    //手动取料 （手动取料显示 自动取料不显示）
            //    MaterialControlManualFetching.isOn = true;
            //    MaterialControlAutoFetching.isOn = false;
            //    break;
            //case 1:
            //    //自动取料 （手动取料不显示 自动取料显示）
            //    MaterialControlManualFetching.isOn = false;
            //    MaterialControlAutoFetching.isOn = true;
            //    break;
            //case 2:
            //    //手动堆料 （手动堆料显示 自动堆料不显示）
            //    RemoteManualStackingModeSelectionCommand.isOn = true;
            //    RemoteAutoStackingModeSelectionCommand.isOn = false;
            //    break;
            //case 3:
            //    //自动堆料 （手动堆料不显示 自动堆料显示）
            //    RemoteManualStackingModeSelectionCommand.isOn = false;
            //    RemoteAutoStackingModeSelectionCommand.isOn = true;
            //    break;
            case 4:
                //自动取料启动二次确认 （自动取料启动显示 自动取料停止不显示）
                AutoFeedingStartHMI.isOn = true;
                AutoFeedingStopHMI.isOn = false;
                break;
            case 5:
                //手动取料启动二次确认/自动取料停止  （自动取料启动不显示 自动取料停止显示）
                AutoFeedingStartHMI.isOn = false;
                AutoFeedingStopHMI.isOn = true;
                break;
            case 6:
                //自动堆料启动二次确认 （自动堆料启动显示 自动堆料停止不显示）
                AutoStackingStartHMI.isOn = true;
                AutoStackingStopHMI.isOn = false;
                break;
            case 7:
                //手动堆料启动二次确认/自动堆料停止 （自动堆料启动不显示 自动堆料停止显示）
                AutoStackingStartHMI.isOn = false;
                AutoStackingStopHMI.isOn = true;
                break;
            default:
                break;
        }
    }

    public void changeBtnColorByState() {

        bool autoRetrieveOperationStartedState = fullVariables.AutoRetrieveOperationStarted;
        bool autoStackingOperationStartedState = fullVariables.AutoStackingOperationStarted;

        if (autoRetrieveOperationStartedState == true && autoStackingOperationStartedState == true)
        {
            AutoButtonHighLightChange(4);
            AutoButtonHighLightChange(6);
        }
        else if (autoRetrieveOperationStartedState == true && autoStackingOperationStartedState == false)
        {
            AutoButtonHighLightChange(4);
            AutoButtonHighLightChange(7);
        }
        else if (autoRetrieveOperationStartedState == false && autoStackingOperationStartedState == true)
        {
            AutoButtonHighLightChange(5);
            AutoButtonHighLightChange(6);
        }
        else
        {
            AutoButtonHighLightChange(5);
            AutoButtonHighLightChange(7);
        }
    }

    #region 高光保持两秒协程 -ljz

    private IEnumerator ToggleHighLightTime1()//回转转向
    {
        myToggleHighLightTimeText1.color = Color.red;
        yield return new WaitForSeconds(highLightTime);
        myToggleHighLightTimeText1.color = Color.white;
        myToggleHighLightTime1 = null;
    }

    private IEnumerator ToggleHighLightTime2()//堆料回转速度 设置
    {
        myToggleHighLightTimeText2.color = Color.red;
        yield return new WaitForSeconds(highLightTime);
        myToggleHighLightTimeText2.color = Color.white;
        myToggleHighLightTime2 = null;
    }
    private IEnumerator ToggleHighLightTime3()//取料回转速度 设置
    {
        myToggleHighLightTimeText3.color = Color.red;
        yield return new WaitForSeconds(highLightTime);
        myToggleHighLightTimeText3.color = Color.white;
        myToggleHighLightTime3 = null;
    }
    private IEnumerator ToggleHighLightTime4()//取料俯仰速度 设置
    {
        myToggleHighLightTimeText4.color = Color.red;
        yield return new WaitForSeconds(highLightTime);
        myToggleHighLightTimeText4.color = Color.white;
        myToggleHighLightTime4 = null;
    }
    private IEnumerator ToggleHighLightTime5()//自动堆料停止 按钮
    {
        myToggleHighLightTimeText5.color = Color.red;
        yield return new WaitForSeconds(highLightTime);
        myToggleHighLightTimeText5.color = Color.white;
        myToggleHighLightTime5 = null;
    }
    private IEnumerator ToggleHighLightTime6()//自动取料停止 按钮
    {
        myToggleHighLightTimeText6.color = Color.red;
        yield return new WaitForSeconds(highLightTime);
        myToggleHighLightTimeText6.color = Color.white;
        myToggleHighLightTime6 = null;
    }
    private IEnumerator ToggleHighLightTime7()//堆料参数更新 按钮
    {
        myToggleHighLightTimeText7.color = UnityEngine.Color.red;
        yield return new WaitForSeconds(highLightTime);
        myToggleHighLightTimeText7.color = Color.white;
        myToggleHighLightTime7 = null;
    }
    private IEnumerator ToggleHighLightTime8()//取料参数更新 按钮
    {
        myToggleHighLightTimeText8.color = Color.red;
        yield return new WaitForSeconds(highLightTime);
        myToggleHighLightTimeText8.color = Color.white;
        myToggleHighLightTime8 = null;
    }
    private IEnumerator ToggleHighLightTime9()//复位更新 按钮
    {
        myToggleHighLightTimeText9.color = Color.red;
        yield return new WaitForSeconds(highLightTime);
        myToggleHighLightTimeText9.color = Color.white;
        myToggleHighLightTime9 = null;
    }
    private IEnumerator ToggleHighLightTime10()//启车报警 按钮
    {
        myToggleHighLightTimeText10.color = Color.red;
        yield return new WaitForSeconds(highLightTime);
        myToggleHighLightTimeText10.color = Color.white;
        myToggleHighLightTime10 = null;
    }

    private IEnumerator ToggleHighLightTime11()
    {
        myToggleHighLightTimeText11.color = Color.red;
        yield return new WaitForSeconds(highLightTime);
        myToggleHighLightTimeText11.color = Color.white;
        myToggleHighLightTime11 = null;
    }
    private IEnumerator ToggleHighLightTime12()
    {
        myToggleHighLightTimeText12.color = Color.red;
        yield return new WaitForSeconds(highLightTime);
        myToggleHighLightTimeText12.color = Color.white;
        myToggleHighLightTime12 = null;
    }
    private IEnumerator ToggleHighLightTime13()
    {
        myToggleHighLightTimeText13.color = Color.red;
        yield return new WaitForSeconds(highLightTime);
        myToggleHighLightTimeText13.color = Color.white;
        myToggleHighLightTime13 = null;
    }
    #endregion

    #region 进入页面刷新一次数据 -ljz
    private void UpdateTwoAutoData()
    {
        if (DataManager.Instance.CurrentAccount == null)//未进入用户界面 
        {
            return;
        }
        if (IsChange)
        {
            AutoDataButton1.onClick.Invoke();//更新两个数据
            AutoDataButton2.onClick.Invoke();
            //更新刮板电流参数                      
            changeOverloadDataAndRestoreCurrentData();

            DeleteThreeMonthData();//清空三个月前的数据
            IsChange = false;//已经更新
        }
    }
    #endregion

    private IEnumerator BlockOtherButtons(string IntroductoryTextValue, string CountdownTextValue)
    {
        WorkScreenPanel.SetActive(true);//显示面板

        IntroductoryText.GetComponent<TextMeshProUGUI>().text = IntroductoryTextValue;//更改文本内容
        IntroductoryText2.GetComponent<TextMeshProUGUI>().text = CountdownTextValue;

        do
        {
            CountdownText.GetComponent<TextMeshProUGUI>().text = ((int)SecondTime).ToString();
            yield return new WaitForSeconds(1.0f);//在每帧update之后进行等待一秒
            SecondTime -= 1;
        } while (SecondTime > 0);

        SecondTime = 3.0f;
        WorkScreenPanel.SetActive(false);//关闭面板
    }

    public void StartModelGet() {
        modelBtnIsOnClick = !modelBtnIsOnClick;
        if (modelBtnIsOnClick) {
            SendCommandThreeD(3, 1);
            OperationLogs("开启全局扫描");
        }
        else
        {
            SendCommandThreeD(3, 0);
            OperationLogs("关闭全局扫描");
        }
        modelBtnTog.isOn = modelBtnIsOnClick;
        
    }


    private void UpdateModel()
    {
        Thread.Sleep(2000);
        //更新请求状态
        int newUpdateState = getThreeDCommandType();
        if (newUpdateState!=0)
        {
            SendCommandThreeD(3, newUpdateState);
        }
        SendState = true;
    }

    private int getThreeDCommandType()
    {
        
        if ((ManualFeedingRemote.isOn || AutoFeedingRemote.isOn) && (!AutoStackingRemote.isOn || !ManualStackingRemote.isOn)) //自动或者手动取料状态
        {
            return 2;
        }
        else if ((AutoStackingRemote.isOn || ManualStackingRemote.isOn) && (ManualFeedingRemote.isOn || AutoFeedingRemote.isOn))//自动和手动取料状态
        {
            return 3;
        }
        else
        {
            return 0;
        }
    }

    private void changeOverloadDataAndRestoreCurrentData()
    {
        //清空数据
        OverloadCurrent.text = "";
        RestoreCurrent.text = "";
        //赋值
        OverloadCurrent.text = "" + fullVariables.AutoFeedingOverloadCurrent.ToString("F2");     //超载电流
        RestoreCurrent.text = "" + fullVariables.AutoFeedingNormalCurrent.ToString("F2");     //恢复电流
    }

    //数据压缩处理
    public static string Decompress(string compressedText) { 
        byte[] compressedBuffer = Convert.FromBase64String(compressedText); 
        using (MemoryStream compressedStream = new MemoryStream(compressedBuffer)) { 
            using (GZipStream gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress)) {
                using (MemoryStream resultStream = new MemoryStream()) { gzipStream.CopyTo(resultStream); 
                    return Encoding.UTF8.GetString(resultStream.ToArray()); 
                } 
            }
        } 
    }

    #region WebSocket Event Handlers

    /// <summary>
    /// Called when the web socket is open, and we are ready to send and receive data
    /// </summary>
    void OnOpen(WebSocket ws)
    {
        Debug.Log("三维服务建立连接");
    }
    void OnOpen2(WebSocket ws)
    {
        Debug.Log("远程驱动建立连接");
    }

    /// <summary>
    /// Called when we received a text message from the server
    /// </summary>
    void OnMessageReceived(WebSocket ws, string message)
    {
        strInfo = message;
        Debug.Log("三维服务ws获得数据" + strInfo);
    }

    void OnMessageReceived2(WebSocket ws, string message)
    {
        plcData = message;
        Debug.Log("远程驱动ws获得数据" + plcData);
    }

    /// <summary>
    /// Called when the web socket closed
    /// </summary>
    void OnClosed(WebSocket ws, UInt16 code, string message)
    {
        Debug.Log("三维服务ws关闭"+message);
        antiInit();
        init();
    }
    void OnClosed2(WebSocket ws, UInt16 code, string message)
    {
        Debug.Log("远程驱动ws关闭" + message);
        antiInit2();
        init2();
    }

    /// <summary>
    /// Called when an error occured on client side
    /// </summary>
    void OnError(WebSocket ws, string reason)
    {
        if (reason != null)
        {
            Debug.Log("websocket 三维服务连接异常:" + reason);
        }
            
        antiInit();
        ReConnect();
    }
    void OnError2(WebSocket ws, string reason)
    {
        if (reason != null)
        {
            TaoIsConnect = false;
            Debug.Log("websocket 远程驱动连接异常:" + reason);
        }
        if (saveStopLogOnce) {
            saveStopLogOnce = false;
            OperationLogs("远程驱动服务器断开连接!");
        }
        
        antiInit2();
        ReConnect2();
    }

    #endregion
    private void antiInit()
    {
        mainSocketThreeD.OnOpen = null;
        mainSocketThreeD.OnMessage = null;
        mainSocketThreeD.OnError = null;
        mainSocketThreeD.OnClosed = null;
        mainSocketThreeD = null;
    }

    private void antiInit2()
    {
        mainSocketPlc.OnOpen = null;
        mainSocketPlc.OnMessage = null;
        mainSocketPlc.OnError = null;
        mainSocketPlc.OnClosed = null;
        mainSocketPlc = null;
    }

    private void init()
    {
        mainSocketThreeD.OnOpen += OnOpen;
        mainSocketThreeD.OnMessage += OnMessageReceived;
        mainSocketThreeD.OnError += OnError;
        mainSocketThreeD.OnClosed += OnClosed;
    }

    private void init2()
    {
        mainSocketPlc.OnOpen += OnOpen2;
        mainSocketPlc.OnMessage += OnMessageReceived2;
        mainSocketPlc.OnError += OnError2;
        mainSocketPlc.OnClosed += OnClosed2;
    }
    void ReConnect()
    {
        if (this.lockReconnect)
            return;
        this.lockReconnect = true;
        StartCoroutine(SetReConnect());
    }
    private IEnumerator SetReConnect()
    {
        Debug.Log("正在重连三维服务websocket");
        yield return new WaitForSeconds(0.5f);
        ConnectThreeDServer();//断线重连
        lockReconnect = false;
    }

    void ReConnect2()
    {
        if (this.lockReconnect2)
            return;
        this.lockReconnect2 = true;
        StartCoroutine(SetReConnect2());
    }
    private IEnumerator SetReConnect2()
    {
        Debug.Log("正在重连远程驱动websocket");
        saveReconnectLogOnce = true;
        yield return new WaitForSeconds(0.5f);
        ConnectPlcServer();//断线重连
        lockReconnect2 = false;
    }

    //料位计防撞与切除 -ljz
    public void LeftLowerCollisionPreventionClick() {
        LeftLowerCollisionPreventionOnClick = !LeftLowerCollisionPreventionOnClick;
        PreventionClick(LeftLowerCollisionPreventionOnClick, "LEFT_LOWER_PREVENTION", LeftLowerCollisionTog, "左侧下俯防撞");
    }

    public void RightLowerCollisionPreventionClick()
    {
        RightLowerCollisionPreventionOnClick =!RightLowerCollisionPreventionOnClick;
        PreventionClick(RightLowerCollisionPreventionOnClick, "RIGHT_LOWER_PREVENTION", RightLowerCollisionTog, "右侧下俯防撞");
    }

    public void LeftRotateCollisionPreventionClick()
    {
        LeftRotateCollisionPreventionOnClick = ! LeftRotateCollisionPreventionOnClick;
        PreventionClick(LeftRotateCollisionPreventionOnClick, "LEFT_ROTATE_PREVENTION", LeftRotateCollisionTog, "左侧回转防撞");
    }
    public void RightRotateCollisionPreventionClick()
    {
        RightRotateCollisionPreventionOnClick = ! RightRotateCollisionPreventionOnClick;
        PreventionClick(RightRotateCollisionPreventionOnClick, "RIGHT_ROTATE_PREVENTION", RightRotateCollisionTog, "右侧回转防撞");
    }

    private void PreventionClick(bool isOnClick, string operate, Toggle tog,string operateLog) {
        if (isOnClick)
        {
            SendCommand(6, 2, operate, 1);//向服务器发送指令  指令内容为false
            OperationLogs(operateLog + "切除");
        }
        else
        {
            SendCommand(6, 2, operate, 0);//向服务器发送指令  指令内容为false
            OperationLogs(operateLog + "投入");
        }
        tog.isOn = isOnClick;
    }

    private void changeTogColorByTime()
    {
        if (startColorChange)
        {
            // 更新计时器
            timer += Time.deltaTime;

            // 如果计时器超过了切换间隔
            if (timer >= colorChangeInterval)
            {
                // 重置计时器
                timer = 0.0f;

                // 切换颜色
                PlcIsConnectTog.isOn = !PlcIsConnectTog.isOn;
            }
        }
    }

    private void plcIsStopWork()
    {
        PlcBG.SetActive(false);
        PlcIsConnect.SetActive(true);
        startColorChange = true;

        if (oncePlcError)
        {
            oncePlcError = false;
            string login_user = "数据无效";
            if (DataManager.Instance.CurrentAccount != null) 
            {
                login_user = DataManager.Instance.CurrentAccount.name;
            }
            string sql = String.Format("insert into warning (time, info, operator) values('{0}', '{1}','{2}');", DateTime.Now, "远程驱动断开", login_user, Encoding.UTF8);
            MySqlHelper.ExecuteSql(sql);
            AddAlarmToTextbox("远程驱动断开！");
        }
    }
    private void plcIsStopWork2()
    {
        PlcBG.SetActive(false);
        PlcIsConnect.SetActive(true);
        startColorChange = true;

        if (oncePlcError)
        {
            oncePlcError = false;
            string login_user = "数据无效";
            if (DataManager.Instance.CurrentAccount != null)
            {
                login_user = DataManager.Instance.CurrentAccount.name;
            }
            string sql = String.Format("insert into warning (time, info, operator) values('{0}', '{1}','{2}');", DateTime.Now, "远程驱动与plc断开", login_user, Encoding.UTF8);
            MySqlHelper.ExecuteSql(sql);
            AddAlarmToTextbox("远程驱动与plc断开！");
        }
    }

    //刮板电机同时启动
    public void ScraperMotorStartsSameTimeBtnOnClick() {
        SendCommand(6, 2, "SET_SCRAPER_MOTOR", 1);
        OperationLogs("刮板电机同时启动");
    }
    //刮板电机轮流启动
    public void ScraperMotorStartsInTurnTimeBtnOnClick()
    {
        SendCommand(6, 2, "SET_SCRAPER_MOTOR", 0);
        OperationLogs("刮板电机轮流启动");
    }
}
