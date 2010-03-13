using System;
using System.Text;
using System.Collections.Generic;

using Rhino.Mocks;
using Machine.Specifications;

namespace GenesisEngine.Specs.CameraSpecs
{
    [Subject(typeof(CameraController))]
    public class when_a_move_forward_message_is_received : CameraControllerContext
    {
        Because of = () =>
            _controller.Handle(new MoveForward() { InputState = _input });

        It should_move_the_camera_forward_the_correct_distance = () =>
            _camera.AssertWasCalled(x => x.MoveForwardHorizontally(_expectedDistance));
    }

    [Subject(typeof(CameraController))]
    public class when_a_move_backward_message_is_received : CameraControllerContext
    {
        Because of = () =>
            _controller.Handle(new MoveBackward() { InputState = _input });

        It should_move_the_camera_backward_the_correct_distance = () =>
            _camera.AssertWasCalled(x => x.MoveBackwardHorizontally(_expectedDistance));
    }

    [Subject(typeof(CameraController))]
    public class when_a_move_left_message_is_received : CameraControllerContext
    {
        Because of = () =>
            _controller.Handle(new MoveLeft() { InputState = _input });

        It should_move_the_camera_left_the_correct_distance = () =>
            _camera.AssertWasCalled(x => x.MoveLeft(_expectedDistance));
    }

    [Subject(typeof(CameraController))]
    public class when_a_move_right_message_is_received : CameraControllerContext
    {
        Because of = () =>
            _controller.Handle(new MoveRight() { InputState = _input });

        It should_move_the_camera_right_the_correct_distance = () =>
            _camera.AssertWasCalled(x => x.MoveRight(_expectedDistance));
    }

    [Subject(typeof(CameraController))]
    public class when_a_move_up_message_is_received : CameraControllerContext
    {
        Because of = () =>
            _controller.Handle(new MoveUp() { InputState = _input });

        It should_move_the_camera_up_the_correct_distance = () =>
            _camera.AssertWasCalled(x => x.MoveUp(_expectedDistance));
    }

    [Subject(typeof(CameraController))]
    public class when_a_move_down_message_is_received : CameraControllerContext
    {
        Because of = () =>
            _controller.Handle(new MoveDown() { InputState = _input });

        It should_move_the_camera_down_the_correct_distance = () =>
            _camera.AssertWasCalled(x => x.MoveDown(_expectedDistance));
    }

    [Subject(typeof(CameraController))]
    public class when_a_mouse_look_message_is_received : CameraControllerContext
    {
        Because of = () =>
            _controller.Handle(new MouseLook() { InputState = _input });

        It should_pitch_the_camera = () =>
            _camera.AssertWasCalled(x => x.ChangePitch(_expectedPitch));

        It should_yaw_the_camera = () =>
        _camera.AssertWasCalled(x => x.ChangeYaw(_expectedYaw));
    }

    [Subject(typeof(CameraController))]
    public class when_a_go_to_ground_message_is_received : CameraControllerContext
    {
        Establish context = () =>
        {
            _camera.Location = DoubleVector3.Up;

            _planet = MockRepository.GenerateStub<IPlanet>();
            _planet.Stub(x => x.GetGroundHeight(_camera.Location)).Return(123);
            _controller.AttachToPlanet(_planet);
        };

        Because of = () =>
            _controller.Handle(new GoToGround() { InputState = _input });

        It should_go_to_ground_level = () =>
            _camera.Location.ShouldEqual(DoubleVector3.Up * 125);
    }

    public class CameraControllerContext
    {
        public static ICamera _camera;
        public static IPlanet _planet;

        public static ISettings _settings;
        
        public static IInputState _input;
        public static double _expectedDistance;
        public static float _expectedPitch;
        public static float _expectedYaw;
        public static CameraController _controller;

        Establish context = () =>
        {
            _camera = MockRepository.GenerateStub<ICamera>();

            _settings = MockRepository.GenerateStub<ISettings>();
            _settings.CameraMoveSpeedPerSecond = 10;
            _settings.CameraMouseLookDamping = 300f;

            _input = MockRepository.GenerateStub<IInputState>();
            _input.Stub(x => x.ElapsedTime).Return(new TimeSpan(0, 0, 0, 0, 500));
            _input.Stub(x => x.MouseDeltaX).Return(100);
            _input.Stub(x => x.MouseDeltaY).Return(150);

            _expectedDistance = 10 / 2;
            _expectedYaw = -100 / 300.0f;
            _expectedPitch = -150 / 300.0f;

            _controller = new CameraController(_camera, _settings);
        };
    }
}
