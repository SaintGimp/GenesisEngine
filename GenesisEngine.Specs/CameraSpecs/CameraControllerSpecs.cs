using System;
using System.Text;
using System.Collections.Generic;

using NSubstitute;
using Machine.Specifications;

namespace GenesisEngine.Specs.CameraSpecs
{
    [Subject(typeof(CameraController))]
    public class when_a_move_forward_message_is_received : CameraControllerContext
    {
        Because of = () =>
            _controller.Handle(new MoveForward() { InputState = _input });

        It should_move_the_camera_forward_the_correct_distance = () =>
            _camera.Received().MoveForwardHorizontally(_expectedDistance);
    }

    [Subject(typeof(CameraController))]
    public class when_a_move_backward_message_is_received : CameraControllerContext
    {
        Because of = () =>
            _controller.Handle(new MoveBackward() { InputState = _input });

        It should_move_the_camera_backward_the_correct_distance = () =>
            _camera.Received().MoveBackwardHorizontally(_expectedDistance);
    }

    [Subject(typeof(CameraController))]
    public class when_a_move_left_message_is_received : CameraControllerContext
    {
        Because of = () =>
            _controller.Handle(new MoveLeft() { InputState = _input });

        It should_move_the_camera_left_the_correct_distance = () =>
            _camera.Received().MoveLeft(_expectedDistance);
    }

    [Subject(typeof(CameraController))]
    public class when_a_move_right_message_is_received : CameraControllerContext
    {
        Because of = () =>
            _controller.Handle(new MoveRight() { InputState = _input });

        It should_move_the_camera_right_the_correct_distance = () =>
            _camera.Received().MoveRight(_expectedDistance);
    }

    [Subject(typeof(CameraController))]
    public class when_a_move_up_message_is_received : CameraControllerContext
    {
        Because of = () =>
            _controller.Handle(new MoveUp() { InputState = _input });

        It should_move_the_camera_up_the_correct_distance = () =>
            _camera.Received().MoveUp(_expectedDistance);
    }

    [Subject(typeof(CameraController))]
    public class when_a_move_down_message_is_received : CameraControllerContext
    {
        Because of = () =>
            _controller.Handle(new MoveDown() { InputState = _input });

        It should_move_the_camera_down_the_correct_distance = () =>
            _camera.Received().MoveDown(_expectedDistance);
    }

    [Subject(typeof(CameraController))]
    public class when_a_mouse_look_message_is_received : CameraControllerContext
    {
        Because of = () =>
            _controller.Handle(new MouseLook() { InputState = _input });

        It should_pitch_the_camera = () =>
            _camera.Received().ChangePitch(_expectedPitch);

        It should_yaw_the_camera = () =>
        _camera.Received().ChangeYaw(_expectedYaw);
    }

    [Subject(typeof(CameraController))]
    public class when_a_go_to_ground_message_is_received : CameraControllerContext
    {
        Establish context = () =>
        {
            _camera.Location = DoubleVector3.Up;

            _planet = Substitute.For<IPlanet>();
            _planet.GetGroundHeight(_camera.Location).Returns(123);
            _controller.AttachToPlanet(_planet);
        };

        Because of = () =>
            _controller.Handle(new GoToGround() { InputState = _input });

        It should_move_the_camera_to_ground_level = () =>
            _camera.Location.ShouldEqual(DoubleVector3.Up * 125);
    }

    [Subject(typeof(CameraController))]
    public class when_a_zoom_in_message_is_received : CameraControllerContext
    {
        Establish context = () =>
            _camera.ZoomLevel = 3f;

        Because of = () =>
            _controller.Handle(new ZoomIn());

        It should_increase_the_zoom_level_of_the_camera = () =>
            _camera.ZoomLevel.ShouldBeGreaterThan(3f);
    }

    [Subject(typeof(CameraController))]
    public class when_a_zoom_out_message_is_received : CameraControllerContext
    {
        Establish context = () =>
            _camera.ZoomLevel = 3f;

        Because of = () =>
            _controller.Handle(new ZoomOut());

        It should_decrease_the_zoom_level_of_the_camera = () =>
            _camera.ZoomLevel.ShouldBeLessThan(3f);
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
            _camera = Substitute.For<ICamera>();

            _settings = Substitute.For<ISettings>();
            _settings.CameraMoveSpeedPerSecond = 10;
            _settings.CameraMouseLookDamping = 300f;

            _input = Substitute.For<IInputState>();
            _input.ElapsedTime.Returns(new TimeSpan(0, 0, 0, 0, 500));
            _input.MouseDeltaX.Returns(100);
            _input.MouseDeltaY.Returns(150);

            _expectedDistance = 10 / 2;
            _expectedYaw = -100 / 300.0f;
            _expectedPitch = -150 / 300.0f;

            _controller = new CameraController(_camera, _settings);
        };
    }
}
