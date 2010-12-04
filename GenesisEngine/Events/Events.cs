using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    public abstract class InputMessage
    {
        public IInputState InputState;
    }

    public class MoveForward : InputMessage
    {
    }

    public class MoveBackward : InputMessage
    {
    }

    public class MoveLeft : InputMessage
    {
    }

    public class MoveRight : InputMessage
    {
    }

    public class MoveUp : InputMessage
    {
    }

    public class MoveDown : InputMessage
    {
    }

    public class MouseLook : InputMessage
    {
    }

    public class ToggleDrawWireframeSetting : InputMessage
    {
    }

    public class ToggleUpdateSetting : InputMessage
    {
    }

    public class ToggleSingleStepSetting : InputMessage
    {
    }

    public class IncreaseCameraSpeed : InputMessage
    {
    }

    public class DecreaseCameraSpeed : InputMessage
    {
    }

    public class GoToGround : InputMessage
    {
    }

    public class ZoomIn : InputMessage
    {
    }

    public class ZoomOut : InputMessage
    {
    }

    public class SettingsChanged
    {
    }

    public class GarbageCollect : InputMessage
    {
    }

    public class ExitApplication : InputMessage
    {
    }
}
