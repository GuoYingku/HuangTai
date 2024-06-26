using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuangtaiPowerPlantControlSystem
{
    public class PLC2Variables
    {
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
    }
}
