using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace GenesisEngine
{
    public class CameraController : ICameraController,
                                    IListener<MoveForward>,
                                    IListener<MoveBackward>,
                                    IListener<MoveLeft>,
                                    IListener<MoveRight>,
                                    IListener<MoveUp>,
                                    IListener<MoveDown>,
                                    IListener<MouseLook>,
                                    IListener<GoToGround>
    {
        private ICamera _camera;
        readonly IPlanet _planet;
        ISettings _settings;

        public CameraController(ICamera camera, IPlanet planet, ISettings settings)
        {
            _camera = camera;
            _planet = planet;
            _settings = settings;
        }

        public void Handle(MoveForward message)
        {
            _camera.MoveForwardHorizontally(_settings.CameraMoveSpeedPerSecond * message.InputState.ElapsedTime.TotalSeconds);
        }

        public void Handle(MoveBackward message)
        {
            _camera.MoveBackwardHorizontally(_settings.CameraMoveSpeedPerSecond * message.InputState.ElapsedTime.TotalSeconds);
        }

        public void Handle(MoveLeft message)
        {
            _camera.MoveLeft(_settings.CameraMoveSpeedPerSecond * message.InputState.ElapsedTime.TotalSeconds);
        }

        public void Handle(MoveRight message)
        {
            _camera.MoveRight(_settings.CameraMoveSpeedPerSecond * message.InputState.ElapsedTime.TotalSeconds);
        }

        public void Handle(MoveUp message)
        {
            _camera.MoveUp(_settings.CameraMoveSpeedPerSecond * message.InputState.ElapsedTime.TotalSeconds);
        }

        public void Handle(MoveDown message)
        {
            _camera.MoveDown(_settings.CameraMoveSpeedPerSecond * message.InputState.ElapsedTime.TotalSeconds);
        }

        public void Handle(MouseLook message)
        {
            if (message.InputState.MouseDeltaX != 0)
            {
                float changeInYaw = -(float)(message.InputState.MouseDeltaX) / _settings.CameraMouseLookDamping;
                _camera.ChangeYaw(changeInYaw);
            }

            if (message.InputState.MouseDeltaY != 0)
            {
                float changeInPitch = -(float)(message.InputState.MouseDeltaY) / _settings.CameraMouseLookDamping;
                _camera.ChangePitch(changeInPitch);
            }
        }

        public void Handle(GoToGround message)
        {
            // TODO: assumes planet is at origin
            var planetUnitVector = DoubleVector3.Normalize(_camera.Location);
            var height = _planet.GetGroundHeight(_camera.Location);
            _camera.Location = planetUnitVector * (height + 2);
        }
    }
}
