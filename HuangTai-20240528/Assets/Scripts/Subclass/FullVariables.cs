using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuangtaiPowerPlantControlSystem
{
    public class FullVariables
    {
        //PLC1
        public DateTime TimeStamp { get; set; }

        public bool MaterialStackingRotaryInverterOperationMonitoring { get; set; }//变频器投入运行（暂定） 20 I10.1
        public bool MaterialStackingRotaryMainCircuitBreaker { get; set; }
        public bool MaterialStackingRotaryBrakeOperationMonitoring { get; set; }//电机制动器运行（暂定） 22 I10.2
        public bool MaterialStackingRotaryInverterFault { get; set; }
        public bool AmplitudeInverterOperation { get; set; }
        public bool AmplitudePrimaryBrakeOperation { get; set; }
        public bool AmplitudeBackup { get; set; }
        public bool AmplitudeSecondaryBrakeOperationMonitoring { get; set; }
        public bool AmplitudePrimaryBrakeOperationMonitoring { get; set; }
        public bool AmplitudeMotorOverheatProtection { get; set; }
        public bool AmplitudeMotorMainCircuitBreaker { get; set; }
        public bool MaterialConveyorMotorMainCircuitBreaker { get; set; }
        public bool MaterialConveyorMotorProtection { get; set; }
        public bool MaterialConveyorMaterialOperationMonitoring { get; set; }
        public bool MaterialStackingRotaryBrakeOperation { get; set; }
        public bool MaterialStackingRotaryInverterOperation { get; set; }
        public bool MaterialStackingRotaryTurnLeft { get; set; }//堆料左转运行 36 Q3.3
        public bool MaterialStackingRotaryTurnRight { get; set; }//堆料左转运行 37 Q3.4
        public bool MaterialStackingRotaryInverterFaultReset { get; set; }
        public bool MaterialBeltOperation { get; set; }
        public bool RemoteControlPowerDisconnect { get; set; }//控制电源分闸 40 Q7.1
        public bool DustSuppressionStartupSwitch { get; set; }
        public bool AmplitudeLower { get; set; }
        public bool LowVoltageControlPowerClosure { get; set; }//低压控制电源合闸 43 I0.0
        public bool LowVoltagePowerClosure { get; set; }//低压动力电源合闸 44 I0.1
        public bool ElectricalRoomEmergencyStopButton { get; set; }//电气室急停按钮 45 I0.2
        public bool EmergencyStopRelay { get; set; }
        public bool LightingPowerClosure { get; set; }
        public bool ScraperMotorMainCircuitBreaker { get; set; }
        public bool DriverCabinEmergencyStopButton { get; set; }//司机室急停按钮 49 I16.0
        public bool DriverCabinSignalResetButton { get; set; }
        public bool MaterialControlAutoStacking { get; set; }
        public bool MaterialControlManualFetching { get; set; }
        public bool MaterialControlAutoFetching { get; set; }
        public bool AutoStackingStartupSwitch { get; set; }
        public bool FaultAlarm { get; set; }
        public bool BypassButton { get; set; }
        public bool EquipmentFetchingOperation { get; set; }
        public bool CircularStackFault { get; set; }
        public bool ScraperTensionLimitSwitch { get; set; }
        public bool ScraperLooseLimitSwitch { get; set; }
        public bool ScraperReducerOilPressureSwitch1 { get; set; }
        public bool ScraperReducerOilPressureSwitch2 { get; set; }
        public bool ScraperFluidCouplingTemperatureSwitch2 { get; set; }
        public bool ScraperMotor2Operation { get; set; }
        public bool ReducerLubricationOilPumpOperation { get; set; }
        public bool MaterialFetchingRotaryMainCircuitBreaker { get; set; }
        public bool MaterialFetchingRotaryMotorCircuitBreaker { get; set; }
        public bool MaterialFetchingRotaryBrakeCircuitBreaker { get; set; }
        public bool MaterialFetchingRotaryInverterOperationMonitoring { get; set; }
        public bool MaterialFetchingRotaryBrakeOperationMonitoring { get; set; }
        public bool MaterialFetchingRotaryInverterFault { get; set; }
        public bool MaterialFetchingRotaryInverterOperation { get; set; }
        public bool MaterialFetchingRotaryBrakeOperation { get; set; }
        public bool MaterialFetchingRotaryPositionCalibrationLimitSwitch { get; set; }
        public bool AmplitudeBrakeResistorOverheatSwitch { get; set; }
        public bool AmplitudeWeightLimiterOverloadAlarm1 { get; set; }
        public bool MaterialFetchingRotaryTurnLeft { get; set; }
        public bool MaterialFetchingRotaryTurnRight { get; set; }
        public bool MaterialFetchingRotaryInverterFaultReset { get; set; }
        public bool AmplitudeAirCooledMotorOperationMonitoring { get; set; }
        public bool AmplitudeInverterFault { get; set; }
        public bool AmplitudeBrakeOperationSignal { get; set; }
        public bool AmplitudeUpLimitSwitch { get; set; }
        public bool AmplitudeUpExtremeLimit { get; set; }
        public bool AmplitudeLowerLimitSwitch { get; set; }
        public bool ManualControl { get; set; }
        public bool RemotePowerClosure { get; set; }//动力电源合闸 87 Q7.2
        public bool RemotePowerDisconnect { get; set; }//动力电源分闸 88 Q7.3
        public bool RemoteLightingPowerClosure { get; set; }
        public bool AmplitudeInverterOperationMonitoring { get; set; }
        public bool AmplitudeSecondaryBrakeOperation { get; set; }
        public bool AmplitudeInverterFaultReset { get; set; }
        public bool AmplitudeAirCooledMotorOperation { get; set; }
        public ushort DB23_W0 { get; set; }
        public ushort DB26_W0 { get; set; }
        public ushort DB27_W0 { get; set; }
        public ushort DB25_W0 { get; set; }
        public ushort DB24_W0 { get; set; }
        public ushort DB22_W0 { get; set; }
        public ushort DB21_W0 { get; set; }
        public ushort HMI_a_W0 { get; set; }
        public bool ElectricalRoomFireAlarmSignal { get; set; }
        public bool ElectricalRoomSignalResetButton { get; set; }
        public bool ControlModeLocalControl { get; set; }//操作模式-本地控制 104 I16.2
        public bool ControlModeRemoteControl { get; set; }//操作模式-远程控制 105 I16.3
        public bool MaterialControlManualStacking { get; set; }//操作模式-手动堆料 106 I16.4
        public bool MaterialFetchingRotaryTurnLeftSwitch { get; set; }
        public bool MaterialFetchingRotaryTurnRightSwitch { get; set; }
        public bool InterlockSwitchWithSystem { get; set; }
        public bool StartupAlarmSwitch { get; set; }
        public bool MaterialBeltStartupRunSwitch { get; set; }
        public bool MaterialFetchingRotarySpeedSelectionSwitch { get; set; }
        public bool AmplitudeUpSwitch { get; set; }
        public bool MaterialStackingRotaryTurnLeftSwitch { get; set; }
        public bool MaterialStackingRotarySpeedSelectionSwitch { get; set; }
        public bool AmplitudeUp { get; set; }
        public bool AmplitudeLowerSwitch { get; set; }
        public bool AmplitudeSpeedSelectionSwitch { get; set; }
        public bool ElectricalRoomPLCModulePower { get; set; }
        public bool DriverCabinFireAlarmSignal { get; set; }
        public bool AutoFetchingStartupSwitch { get; set; }
        public bool AmplitudeLowerExtreme { get; set; }
        public bool MaterialMechanismAudioVisualAlarm { get; set; }
        public bool EquipmentStackingOperation { get; set; }
        public bool ScraperMotor1Operation { get; set; }
        public bool RemoteEmergencyStopControl { get; set; }
        public bool AutoControl { get; set; }
        public bool EquipmentFault { get; set; }
        public long M_ScraperMotor1Current { get; set; }//刮板电机1电流 129 DB28.DBD0
        public long M_ScraperMotor2Current { get; set; }//刮板电机2电流 130 DB28.D4
        public long M_MaterialFetchingAmplitudeMotorCurrent { get; set; }
        public long M_MaterialFetchingRotaryMotorCurrent { get; set; }
        public long M_MaterialStackingBeltMotorCurrent { get; set; }
        public long M_MaterialStackingRotaryMotorCurrent { get; set; }
        public long MaterialFetchingRotaryAngle { get; set; }
        public long MaterialFetchingPitchAngle { get; set; }
        public long MaterialStackingRotaryAngle { get; set; }
        public bool AutoStackingBeltOperation { get; set; }
        public bool AutoStackingForceStop { get; set; }
        public bool AutoStackingTouchScreenStartup { get; set; }
        public bool AutoStackingTouchScreenStop { get; set; }
        public bool AutoStackingTouchScreenTurnLeftSetting { get; set; }
        public bool AutoStackingTouchScreenTurnRightSetting { get; set; }
        public bool AutoStackingLeftTurnSignal { get; set; }
        public bool AutoStackingRightTurnSignal { get; set; }
        public bool AutoStackingLeftTurn { get; set; }
        public bool AutoStackingRightTurn { get; set; }
        public long M_AutoStackingRangeSetting { get; set; }
        public bool ManualIntervention { get; set; }
        public bool InterventionStop { get; set; }
        public bool AutoStackingIntervention { get; set; }
        public bool AutoFetchingIntervention { get; set; }
        public bool AutoEdgeFetching { get; set; }
        public bool AutoFetchingTouchScreenStartup { get; set; }
        public bool AutoFetchingTouchScreenStop { get; set; }
        public bool TouchScreenEdgeFetching { get; set; }
        public bool TouchScreenMiddleFetching { get; set; }
        public bool AutoFetchingTouchScreenTurnLeftSetting { get; set; }
        public bool AutoFetchingTouchScreenTurnRightSetting { get; set; }
        public bool AutoFetchingLeftTurnSignal { get; set; }
        public bool AutoFetchingRightTurnSignal { get; set; }
        public bool EdgeFetchingLeftTurn { get; set; }
        public bool EdgeFetchingRightTurn { get; set; }
        public bool AutoMiddleFetching { get; set; }
        public bool MiddleFetchingLeftTurn { get; set; }
        public bool MiddleFetchingRightTurn { get; set; }
        public bool AutoFetchingScraperOperation { get; set; }
        public bool AutoFetchingRotaryInPlace { get; set; }
        public bool AutoFetchingScraperDescend { get; set; }
        public long M_AutoEdgeFetchingAngleSetting { get; set; }
        public long M_AutoMiddleFetchingAngleSetting { get; set; }
        public bool ScraperLubricationButton { get; set; }
        public bool ScraperFrameLubricationButton { get; set; }
        public bool MaterialStackingUpperLubricationButton { get; set; }
        public bool MaterialStackingMiddleLubricationButton { get; set; }
        public bool MaterialFetchingWireRopeLubricationButton { get; set; }
        public bool MaterialFetchingRotaryLubricationButton { get; set; }
        public bool AmplitudeBrakeOpeningLimit { get; set; }
        public bool ScraperHydraulicTensionerFault { get; set; }
        public bool MaterialStackingBeltBrakeOperationMonitoring { get; set; }
        public bool MaterialFetchingBearingLubricationButton { get; set; }
        public bool MaterialFetchingRotaryBackup { get; set; }
        public bool MaterialStackingRotaryBackup { get; set; }
        public bool ScraperTemperatureSwitch1 { get; set; }
        public bool TouchScreenTensionManualStartupButton { get; set; }
        public bool TouchScreenTensionManualStopButton { get; set; }
        public bool RemoteControlPowerClosure { get; set; }//控制电源合闸 187 Q7.0
        public bool ScraperMotorOverheatProtection { get; set; }
        public bool ScraperMotor1OperationMonitoring { get; set; }
        public bool ScraperMotor2OperationMonitoring { get; set; }
        public bool ScraperReducerLubricationOilPumpMonitoring { get; set; }
        public bool MaterialFetchingCoalBucketBlockageSwitch { get; set; }
        public bool MaterialFetchingRotaryBrakeResistorTemperatureAlarm { get; set; }
        public bool AmplitudeWeightLimiterOverloadAlarm2 { get; set; }
        public bool MaterialStackingRotaryTurnRightSwitch { get; set; }
        public bool MaterialStackingBeltPrimaryDeviation { get; set; }
        public bool MaterialStackingBeltSecondaryDeviation { get; set; }
        public bool MaterialStackingBeltRopeProtection { get; set; }
        public bool MaterialStackingBeltSpeedDetection { get; set; }
        public bool MaterialStackingBeltMaterialFlowDetection { get; set; }
        public bool MaterialStackingBeltLongitudinalTearDetection { get; set; }
        public bool MaterialStackingArmLeftCollisionSwitch { get; set; }
        public bool MaterialStackingArmRightCollisionSwitch { get; set; }
        public bool MaterialStackingCoalBucketBlockageSwitch { get; set; }
        public bool SystemAllowStackingCommand { get; set; }
        public bool SystemAllowFetchingCommand { get; set; }
        public bool SystemEmergencyStopCommand { get; set; }
        public bool WaterPumpMotorOperation { get; set; }
        public bool MaterialStackingWaterSpraySolenoidValveOperation { get; set; }
        public bool MaterialStackingBeltBrakeOperation { get; set; }
        public ushort DB32_W0 { get; set; }
        public bool UnattendedEmergencyStop { get; set; }
        public bool MaterialFetchingBearingLubricationStopButton { get; set; }
        public bool ScraperFrameLubricationStopButton { get; set; }
        public bool WaterPumpMotorMainCircuitBreaker { get; set; }
        public bool WaterPumpMotorOverload { get; set; }
        public bool WaterPumpMotorContactor { get; set; }
        public bool MaterialFetchingRotaryLeftTurnLimit { get; set; }
        public bool MaterialFetchingRotaryLeftTurnExtremeLimit { get; set; }
        public bool MaterialFetchingRotaryRightTurnLimit { get; set; }
        public bool MaterialFetchingRotaryRightTurnExtremeLimit { get; set; }
        public bool MaterialFetchingRotaryLeftCollisionSwitch1 { get; set; }
        public bool MaterialFetchingRotaryLeftCollisionSwitch2 { get; set; }
        public bool MaterialFetchingRotaryRightCollisionSwitch1 { get; set; }
        public bool MaterialFetchingRotaryRightCollisionSwitch2 { get; set; }
        public bool MaterialStackingMiddleLubricationOperation { get; set; }
        public bool MaterialStackingUpperLubricationOperation { get; set; }
        public bool MaterialFetchingRotaryLubricationOperation { get; set; }
        public bool ScraperLubricationOperation { get; set; }
        public bool MaterialRotationLubricationOperation { get; set; }
        public bool ScraperFrameLubricationOperation { get; set; }
        public bool MaterialBearingLubricationOperation { get; set; }
        public bool FluidCouplingTemperatureSwitch { get; set; }
        public bool MaterialPileMiddleLubricationStopButton { get; set; }
        public bool MaterialPileUpperLubricationStopButton { get; set; }
        public bool MaterialWireRopeLubricationStopButton { get; set; }
        public bool MaterialRotationLubricationStopButton { get; set; }
        public bool ScraperLubricationStopButton { get; set; }
        public bool StackingRotationLeftAntiCollisionLimit { get; set; }
        public bool StackingRotationRightAntiCollisionLimit { get; set; }
        public bool ScraperStartSwitch { get; set; }

        //PLC2
        //ID300
        public bool LeftFrontVerticalLevelMeterScrapingProtection { get; set; }
        public bool RightFrontVerticalLevelMeterScrapingProtection { get; set; }
        public bool LeftLevelMeterProtectionForbidLeftTurn { get; set; }
        public bool RightLevelMeterProtectionForbidRightTurn { get; set; }
        public bool RemotePowerSupplyCloseCommand { get; set; }
        public bool RemotePowerSupplyOpenCommand { get; set; }
        public bool RemoteControlPowerSupplyCloseCommand { get; set; }
        public bool RemoteControlPowerSupplyOpenCommand { get; set; }
        public bool RemoteInterlockUnlockCommand { get; set; }
        public bool RemoteInterlockLockCommand { get; set; }
        public bool RemoteManualStackingRotateLeftCommand { get; set; }
        public bool RemoteManualStackingRotateRightCommand { get; set; }
        public bool RemoteManualStackingRotateStopCommand { get; set; }
        public bool RemoteManualStackingBeltStartCommand { get; set; }
        public bool RemoteManualStartBellCommand { get; set; }
        public bool RemoteManualMaterialPickupTurnLeftCommand { get; set; }
        public bool RemoteManualMaterialPickupTurnRightCommand { get; set; }
        public bool RemoteManualMaterialPickupRotateStopCommand { get; set; }
        public bool RemoteManualMaterialPickupRotateSpeedSelection { get; set; }
        public bool RemoteManualMaterialPickupTiltUpCommand { get; set; }
        public bool RemoteManualMaterialPickupTiltDownCommand { get; set; }
        public bool RemoteManualMaterialPickupTiltStopCommand { get; set; }
        public bool RemoteManualMaterialPickupTiltSpeedSelection { get; set; }
        public bool RemoteManualScraperStartStopCommand { get; set; }
        public bool RemoteIlluminationCloseCommand { get; set; }
        public bool RemoteIlluminationOpenCommand { get; set; }
        public bool RemoteFaultResetCommand { get; set; }
        public bool RemoteEmergencyStopCommand { get; set; }
        public bool RemoteBypassCommand { get; set; }
        public bool RemoteManualStackingRotateSpeedSelection { get; set; }
        public bool RemoteManualScraperStopCommand { get; set; }
        public bool RemoteManualStackingBeltStopCommand { get; set; }
        public bool RemoteManualStackingModeSelectionCommand { get; set; }
        public bool RemoteAutoStackingModeSelectionCommand { get; set; }
        public bool RemoteManualMaterialPickupModeSelectionCommand { get; set; }
        public bool RemoteAutoMaterialPickupModeSelectionCommand { get; set; }
        public bool RemoteMode { get; set; }
        public bool StackingMiddleLubricationButton { get; set; }
        public bool StackingUpperLubricationButton { get; set; }
        public bool MaterialPickupWireRopeLubricationButton { get; set; }
        public bool MaterialPickupRotateLubricationButton { get; set; }
        public bool ScraperLubriButton { get; set; }
        public bool ScraperFrameLubriButton { get; set; }
        public bool MaterialPickupBearingLubricationButton { get; set; }
        public bool ScreenTensionManualStartButton { get; set; }
        public bool ScreenTensionManualStopButton { get; set; }
        public bool StackingMiddleLubricationStopButton { get; set; }
        public bool StackingUpperLubricationStopButton { get; set; }
        public bool MaterialPickupWireRopeLubricationStopButton { get; set; }
        public bool MaterialPickupRotateLubricationStopButton { get; set; }
        public bool ScraperLubriStopButton { get; set; }
        public bool ScraperFrameLubriStopButton { get; set; }
        public bool MaterialPickupBearingLubricationStopButton { get; set; }

        //ID353
        public float LeftFrontVerticalMaterialLevel { get; set; }
        public float LeftFrontInclinedMaterialLevel { get; set; }
        public float LeftRearInclinedMaterialLevel { get; set; }
        public float LeftRearVerticalMaterialLevel { get; set; }
        public float RightFrontVerticalMaterialLevel { get; set; }
        public float RightFrontInclinedMaterialLevel { get; set; }
        public float RightRearInclinedMaterialLevel { get; set; }
        public float RightRearVerticalMaterialLevel { get; set; }
        public float CenterCylinderBottomUltrasonicLevel { get; set; }
        public float Inclinometer { get; set; }
        public float LeftStackerMaterialLevel { get; set; }
        public float RightStackerMaterialLevel { get; set; }
        public float MaterialArmRotationAngle { get; set; }
        public float StackerArmRotationAngle { get; set; }

        //ID367
        public bool AutoStackingStartHMI { get; set; }
        public bool AutoStackingPauseHMI { get; set; }
        public bool AutoStackingStopHMI { get; set; }
        public bool AutoFeedingStartHMI { get; set; }
        public bool AutoFeedingPauseHMI { get; set; }
        public bool AutoFeedingStopHMI { get; set; }
        public bool ConfirmCurrentJobPointHMI { get; set; }
        public bool ConfirmScrapingButton { get; set; }
        public float AutoFeedingStartAngle { get; set; }
        public float AutoFeedingEndAngle { get; set; }
        public float AutoStackingStartAngle { get; set; }
        public float AutoStackingEndAngle { get; set; }
        public bool AutoFeedingStartConfirmation { get; set; }
        public bool AutoFeedingStopConfirmation { get; set; }
        public bool AutoStackingStartConfirmation { get; set; }
        public bool AutoStackingStopConfirmation { get; set; }

        //ID383
        public bool SmallAngleScraperDownProtection { get; set; }
        public bool LargeAngleScraperUpProtection { get; set; }
        public bool SmallAngleScraperDownDecelerationZone { get; set; }
        public bool LargeAngleScraperUpDecelerationZone { get; set; }
        public bool ArmAngleScraperLeftTurnProhibition { get; set; }
        public bool ArmAngleScraperRightTurnProhibition { get; set; }
        public bool ArmAngleBucketLeftTurnProhibition { get; set; }
        public bool ArmAngleBucketRightTurnProhibition { get; set; }
        public bool ArmAngleScraperLeftTurnDecelerationZone { get; set; }
        public bool ArmAngleScraperRightTurnDecelerationZone { get; set; }
        public bool ArmAngleBucketLeftTurnDecelerationZone { get; set; }
        public bool ArmAngleBucketRightTurnDecelerationZone { get; set; }
        public bool VerticalLevelSensorFailureLeftFront { get; set; }
        public bool InclinedLevelSensorFailureLeftFront { get; set; }
        public bool InclinedLevelSensorFailureLeftRear { get; set; }
        public bool VerticalLevelSensorFailureLeftRear { get; set; }
        public bool VerticalLevelSensorFailureRightFront { get; set; }
        public bool InclinedLevelSensorFailureRightFront { get; set; }
        public bool InclinedLevelSensorFailureRightRear { get; set; }
        public bool VerticalLevelSensorFailureRightRear { get; set; }
        public bool UltrasonicLevelSensorFailureBottom { get; set; }
        public bool AngleSensorFailure { get; set; }
        public bool BucketLevelSensorFailureLeft { get; set; }
        public bool BucketLevelSensorFailureRight { get; set; }
        public bool ArmRotationEncoderFailure { get; set; }
        public bool BucketArmRotationEncoderFailure { get; set; }
        public bool ScraperMotorHighCurrentAlarm { get; set; }

        public bool AutoStackingRemote { get; set; }
        public bool ManualStackingRemote { get; set; }
        public bool AutoFeedingRemote { get; set; }
        public bool ManualFeedingRemote { get; set; }

        //414
        public float AutoStackingHeightSetting { get; set; }
        public bool IsManualRotation { get; set; }
        public float ScrapingDepthSetting { get; set; }
        public float RotationEntryPoint { get; set; }

        //418
        public bool MaterialFeederMaintenanceButton { get; set; }
        public bool MaterialStackerMaintenanceButton { get; set; }

        //242 (PLC1)
        public bool MaterialStackerRotationSpeedButton { get; set; }
        public bool MaterialFeederRotationSpeedButton { get; set; }
        public bool MaterialFeederPitchSpeedButton { get; set; }
        public int MaterialStackerRotationSpeed { get; set; }
        public int MaterialFeederRotationSpeed { get; set; }
        public int MaterialFeederPitchSpeed { get; set; }

        //420
        public float AutoFeedingOverloadCurrent { get; set; }
        public float AutoFeedingNormalCurrent { get; set; }

        //422
        public float ManuallyCalibratedPresetValueForFeederEncoderAngle { get; set; }
        public float ManuallyCalibratedPresetValueForStackerEncoderAngle { get; set; }
        public bool EnableButtonForManuallySetPresetValueOfFeederEncoder { get; set; }
        public bool EnableButtonForManuallySetPresetValueOfStackerEncoder { get; set; }
        public bool ManualCalibrationButtonForFeederEncoder { get; set; }
        public bool ManualCalibrationButtonForStackerEncoder { get; set; }

        //428
        public bool AutoRetrieveOperationStarted { get; set; }
        public bool AutoStackingOperationStarted { get; set; }
        public bool LeftLowerCollisionPrevention { get; set; }
        public bool RightLowerCollisionPrevention { get; set; }
        public bool LeftRotateCollisionPrevention { get; set; }
        public bool RightRotateCollisionPrevention { get; set; }

        public bool CommunicationWithPLCSuccessful { get; set; }

        //435 2024/3/29
        public bool AutomaticMaterialScraperStopTriggered { get; set; }
        public bool EquipmentMalfunction { get; set; }
        public bool MaterialScraperMalfunction { get; set; }//0 空 1 黄

        //438
        public bool AutomaticFeedStartTriggered { get; set; }
        public bool RemoteScraperStart { get; set; }
        public bool ScraperRunningSignal { get; set; }

        //441
        public bool PLCSignal { get; set; }

        //248 (PLC1)
        public bool ManualSetScraperMotorStartButton { get; set; }
    }


}
