using System;

namespace SurvivalTemplatePro.CameraSystem
{
    [Serializable]
    public class CameraMotionState
    {
        public Sprind.Settings PositionSpring = Sprind.Settings.Default;
        public Sprind.Settings RotationSpring = Sprind.Settings.Default;

        public CameraBob Headbob;
        public NoiseMotionModule Noise;

        [BHeader("State Change Forces")]

        public SpringForce EnterForce;
        public SpringForce ExitForce;
    }
}
