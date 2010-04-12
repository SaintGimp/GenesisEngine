using System;
using System.Text;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Rhino.Mocks;
using Machine.Specifications;

namespace GenesisEngine.Specs.CameraSpecs
{
    [Subject(typeof(Camera))]
    public class when_view_parameters_are_set_by_yaw_pitch_roll : CameraContext
    {
        Because of = () =>
            _camera.SetViewParameters(new DoubleVector3(1, 2, 3), 4, 5, 6);

        It should_go_to_the_requested_location = () =>
            _camera.Location.ShouldEqual(new DoubleVector3(1, 2, 3));

        It should_have_the_requested_yaw = () =>
            _camera.Yaw.ShouldEqual(4f);

        It should_have_the_requested_pitch = () =>
            _camera.Pitch.ShouldEqual(5f);

        It should_have_the_requested_roll = () =>
            _camera.Roll.ShouldEqual(6f);

        It should_generate_a_view_transformation_for_the_current_state = () =>
            _camera.OriginBasedViewTransformation.ShouldEqual(GenerateOriginBasedViewMatrix(_camera.Location, _camera.Yaw, _camera.Pitch, _camera.Roll));
    }

    [Subject(typeof(Camera))]
    public class when_view_parameters_are_set_by_look_at : CameraContext
    {
        Because of = () =>
            _camera.SetViewParameters(new DoubleVector3(0, 1, 1), DoubleVector3.Zero);

        It should_go_to_the_requested_location = () =>
            _camera.Location.ShouldEqual(new DoubleVector3(0, 1, 1));

        It should_set_the_yaw_to_face_toward_the_look_at_point = () =>
            _camera.Yaw.ShouldEqual(0f);

        It should_set_the_pitch_to_face_toward_the_look_at_point = () =>
            _camera.Pitch.ShouldEqual(-MathHelper.Pi / 4);

        It should_set_the_roll_to_face_toward_the_look_at_point = () =>
            _camera.Roll.ShouldEqual(0f);

        It should_generate_a_view_transformation_for_the_current_state = () =>
            _camera.OriginBasedViewTransformation.ShouldEqual(GenerateOriginBasedViewMatrix(_camera.Location, _camera.Yaw, _camera.Pitch, _camera.Roll));
    }

    [Subject(typeof(Camera))]
    public class when_view_parameters_are_set_to_look_straight_up : CameraContext
    {
        Because of = () =>
            _camera.SetViewParameters(DoubleVector3.Zero, DoubleVector3.Up);

        It should_have_no_yaw = () =>
            _camera.Yaw.ShouldEqual(0f);

        It should_have_the_maximum_allowable_pitch = () =>
            _camera.Pitch.ShouldEqual(Camera.MaximumPitch);

        It should_have_no_roll = () =>
            _camera.Roll.ShouldEqual(0f);

        It should_generate_a_view_transformation_for_the_current_state = () =>
            _camera.OriginBasedViewTransformation.ShouldEqual(GenerateOriginBasedViewMatrix(_camera.Location, _camera.Yaw, _camera.Pitch, _camera.Roll));
    }

    [Subject(typeof(Camera))]
    public class when_view_parameters_are_set_to_look_straight_down : CameraContext
    {
        Because of = () =>
            _camera.SetViewParameters(DoubleVector3.Zero, DoubleVector3.Down);

        It should_have_no_yaw = () =>
            _camera.Yaw.ShouldEqual(0f);

        It should_set_the_pitch_to_the_minimum_allowable_pitch = () =>
            _camera.Pitch.ShouldEqual(Camera.MinimumPitch);

        It should_have_no_roll = () =>
            _camera.Roll.ShouldEqual(0f);

        It should_generate_a_view_transformation_for_the_current_state = () =>
            _camera.OriginBasedViewTransformation.ShouldEqual(GenerateOriginBasedViewMatrix(_camera.Location, _camera.Yaw, _camera.Pitch, _camera.Roll));
    }

    [Subject(typeof(Camera))]
    public class when_projection_parameters_are_set : CameraContext
    {
        Because of = () =>
            _camera.SetProjectionParameters(1f, 1f, 1f, 1f, 2f);

        It should_do_something_awesome;
            // TODO
    }

    [Subject(typeof(Camera))]
    public class when_the_clipping_planes_are_set : CameraContext
    {
        Because of = () =>
            _camera.SetClippingPlanes(123, 456);

        It should_set_the_near_plane_of_the_view_frustum = () =>
            _camera.OriginBasedViewFrustum.Near.D.ShouldEqual(123f);

        It should_set_the_far_plane_of_the_view_frustum = () =>
            _camera.OriginBasedViewFrustum.Far.D.ShouldBeCloseTo(-456, 0.001f);
    }

    [Subject(typeof(Camera))]
    public class when_the_yaw_is_set : CameraContext
    {
        Because of = () =>
            _camera.Yaw = 1;

        It should_generate_a_view_transformation_for_the_current_state = () =>
            _camera.OriginBasedViewTransformation.ShouldEqual(GenerateOriginBasedViewMatrix(_camera.Location, 1, _camera.Pitch, _camera.Roll));
    }

    [Subject(typeof(Camera))]
    public class when_the_pitch_is_set : CameraContext
    {
        Because of = () =>
            _camera.Pitch = 1;

        It should_generate_a_view_transformation_for_the_current_state = () =>
            _camera.OriginBasedViewTransformation.ShouldEqual(GenerateOriginBasedViewMatrix(_camera.Location, _camera.Yaw, 1, _camera.Roll));
    }

    [Subject(typeof(Camera))]
    public class when_the_pitch_is_set_to_straight_up_or_more : CameraContext
    {
        Because of = () =>
            _camera.Pitch = MathHelper.Pi / 2;

        It should_limit_the_pitch_to_the_maximum_allowable = () =>
            _camera.Pitch.ShouldEqual(Camera.MaximumPitch);
    }

    [Subject(typeof(Camera))]
    public class when_the_pitch_is_set_to_straight_down_or_more : CameraContext
    {
        Because of = () =>
            _camera.Pitch = -MathHelper.Pi / 2;

        It should_limit_the_pitch_to_the_minimum_allowable = () =>
            _camera.Pitch.ShouldEqual(Camera.MinimumPitch);
    }

    [Subject(typeof(Camera))]
    public class when_the_roll_is_set : CameraContext
    {
        Because of = () =>
            _camera.Roll = 1;

        It should_generate_a_view_transformation_for_the_current_state = () =>
            _camera.OriginBasedViewTransformation.ShouldEqual(GenerateOriginBasedViewMatrix(_camera.Location, _camera.Yaw, _camera.Pitch, 1));
    }

    [Subject(typeof(Camera))]
    public class when_the_location_is_set : CameraContext
    {
        Because of = () =>
            _camera.Location = new DoubleVector3(1, 2, 3);

        It should_generate_a_view_transformation_for_the_current_state = () =>
            _camera.OriginBasedViewTransformation.ShouldEqual(GenerateOriginBasedViewMatrix(new DoubleVector3(1, 2, 3), _camera.Yaw, _camera.Pitch, _camera.Roll));
    }

    [Subject(typeof(Camera))]
    public class when_it_is_reset_and_the_default_location_and_look_at_are_the_same : CameraContext
    {
        Establish context = () =>
        {
            _settings.CameraStartingLocation = DoubleVector3.Up;
            _settings.CameraStartingLookAt = DoubleVector3.Up;
        };

        Because of = () =>
            _camera.Reset();

        It should_go_to_the_default_location = () =>
            _camera.Location.ShouldEqual(_settings.CameraStartingLocation);

        It should_have_no_roll = () =>
            _camera.Roll.ShouldEqual(0);

        It should_have_no_yaw = () =>
            _camera.Yaw.ShouldEqual(0);

        It should_have_no_pitch = () =>
            _camera.Pitch.ShouldEqual(0);

        It should_generate_a_view_transformation_for_the_current_state = () =>
            _camera.OriginBasedViewTransformation.ShouldEqual(GenerateOriginBasedViewMatrix(DoubleVector3.Up, 0, 0, 0));
    }

    [Subject(typeof(Camera))]
    public class when_it_is_reset_and_the_default_location_and_look_at_are_different : CameraContext
    {
        Establish context = () =>
        {
            _settings.CameraStartingLocation = DoubleVector3.Right;
            _settings.CameraStartingLookAt = DoubleVector3.Up;
        };

        Because of = () =>
            _camera.Reset();

        It should_go_to_the_default_location = () =>
            _camera.Location.ShouldEqual(_settings.CameraStartingLocation);

        It should_have_no_roll = () =>
            _camera.Roll.ShouldEqual(0);

        It should_set_the_yaw_to_face_toward_the_look_at_point = () =>
            _camera.Yaw.ShouldEqual(MathHelper.Pi / 2);

        It should_set_the_pitch_to_face_toward_the_look_at_point = () =>
            _camera.Pitch.ShouldEqual(MathHelper.Pi / 4);

        It should_generate_a_view_transformation_for_the_current_state = () =>
            _camera.OriginBasedViewTransformation.ShouldEqual(GenerateOriginBasedViewMatrix(DoubleVector3.Right, MathHelper.Pi / 2, MathHelper.Pi / 4, 0));
    }

    [Subject(typeof(Camera))]
    public class when_the_yaw_is_changed : CameraContext
    {
        Establish context = () =>
            _camera.Yaw = 1;

        Because of = () =>
            _camera.ChangeYaw(-0.5f);

        It should_change_the_yaw_relative_to_the_current_yaw = () =>
            _camera.Yaw.ShouldEqual(0.5f);

        It should_generate_a_view_transformation_for_the_current_state = () =>
            _camera.OriginBasedViewTransformation.ShouldEqual(GenerateOriginBasedViewMatrix(DoubleVector3.Zero, 0.5f, 0, 0));
    }

    [Subject(typeof(Camera))]
    public class when_the_pitch_is_changed : CameraContext
    {
        Establish context = () =>
            _camera.Pitch = 1;

        Because of = () =>
            _camera.ChangePitch(-0.5f);

        It should_change_the_pitch_relative_to_the_current_pitch = () =>
            _camera.Pitch.ShouldEqual(0.5f);

        It should_generate_a_view_transformation_for_the_current_state = () =>
            _camera.OriginBasedViewTransformation.ShouldEqual(GenerateOriginBasedViewMatrix(DoubleVector3.Zero, 0, 0.5f, 0));
    }

    [Subject(typeof(Camera))]
    public class when_the_pitch_is_changed_to_point_straight_up_or_more : CameraContext
    {
        Because of = () =>
            _camera.ChangePitch(MathHelper.Pi / 2);

        It should_limit_the_pitch_to_the_maximum_allowable = () =>
            _camera.Pitch.ShouldEqual(Camera.MaximumPitch);
    }

    [Subject(typeof(Camera))]
    public class when_the_pitch_is_changed_to_point_straight_down_or_more : CameraContext
    {
        Because of = () =>
            _camera.ChangePitch(-MathHelper.Pi / 2);

        It should_limit_the_pitch_to_the_minimum_allowable = () =>
            _camera.Pitch.ShouldEqual(Camera.MinimumPitch);
    }

    [Subject(typeof(Camera))]
    public class when_moved_forward_horizontally : CameraContext
    {
        Establish context = () =>
        {
            _camera.ChangeYaw(MathHelper.Pi / 4);
            _camera.ChangePitch(1);
        };

        Because of = () =>
            _camera.MoveForwardHorizontally(1);

        It should_move_horizontally_in_the_direction_the_camera_is_facing_regardless_of_pitch = () =>
            _camera.Location.ShouldBeCloseTo(new DoubleVector3(-Math.Sqrt(.5f), 0, -Math.Sqrt(.5f)));

        It should_generate_a_view_transformation_for_the_current_state = () =>
            _camera.OriginBasedViewTransformation.ShouldEqual(GenerateOriginBasedViewMatrix(_camera.Location, _camera.Yaw, _camera.Pitch, _camera.Roll));
    }

    [Subject(typeof(Camera))]
    public class when_moved_backward_horizontally : CameraContext
    {
        Establish context = () =>
        {
            _camera.ChangeYaw(MathHelper.Pi / 4);
            _camera.ChangePitch(1);
        };

        Because of = () =>
            _camera.MoveBackwardHorizontally(1);

        It should_move_horizontally_opposite_the_direction_the_camera_is_facing_regardless_of_pitch = () =>
            _camera.Location.ShouldBeCloseTo(new DoubleVector3(Math.Sqrt(.5f), 0, Math.Sqrt(.5f)));

        It should_generate_a_view_transformation_for_the_current_state = () =>
            _camera.OriginBasedViewTransformation.ShouldEqual(GenerateOriginBasedViewMatrix(_camera.Location, _camera.Yaw, _camera.Pitch, _camera.Roll));
    }

    [Subject(typeof(Camera))]
    public class when_moved_left : CameraContext
    {
        Establish context = () =>
        {
            _camera.ChangeYaw(MathHelper.Pi / 4);
            _camera.ChangePitch(1);
        };

        Because of = () =>
            _camera.MoveLeft(1);

        It should_move_left = () =>
            _camera.Location.ShouldBeCloseTo(new DoubleVector3(-Math.Sqrt(.5f), 0, Math.Sqrt(.5f)));

        It should_generate_a_view_transformation_for_the_current_state = () =>
            _camera.OriginBasedViewTransformation.ShouldEqual(GenerateOriginBasedViewMatrix(_camera.Location, _camera.Yaw, _camera.Pitch, _camera.Roll));
    }

    [Subject(typeof(Camera))]
    public class when_moved_right : CameraContext
    {
        Establish context = () =>
        {
            _camera.ChangeYaw(MathHelper.Pi / 4);
            _camera.ChangePitch(1);
        };

        Because of = () =>
            _camera.MoveRight(1);

        It should_move_right = () =>
            _camera.Location.ShouldBeCloseTo(new DoubleVector3(Math.Sqrt(.5f), 0, -Math.Sqrt(.5f)));

        It should_generate_a_view_transformation_for_the_current_state = () =>
            _camera.OriginBasedViewTransformation.ShouldEqual(GenerateOriginBasedViewMatrix(_camera.Location, _camera.Yaw, _camera.Pitch, _camera.Roll));
    }

    [Subject(typeof(Camera))]
    public class when_moved_up : CameraContext
    {
        Establish context = () =>
        {
            _camera.ChangeYaw(MathHelper.Pi / 4);
            _camera.ChangePitch(1);
        };

        Because of = () =>
            _camera.MoveUp(1);

        It should_move_straight_up_regardless_of_orientation = () =>
            _camera.Location.ShouldBeCloseTo(DoubleVector3.Up);

        It should_generate_a_view_transformation_for_the_current_state = () =>
            _camera.OriginBasedViewTransformation.ShouldEqual(GenerateOriginBasedViewMatrix(_camera.Location, _camera.Yaw, _camera.Pitch, _camera.Roll));
    }

    [Subject(typeof(Camera))]
    public class when_moved_down : CameraContext
    {
        Establish context = () =>
        {
            _camera.ChangeYaw(MathHelper.Pi / 4);
            _camera.ChangePitch(1);
        };

        Because of = () =>
            _camera.MoveDown(1);

        It should_move_straight_down_regardless_of_orientation = () =>
            _camera.Location.ShouldBeCloseTo(DoubleVector3.Down);

        It should_generate_a_view_transformation_for_the_current_state = () =>
            _camera.OriginBasedViewTransformation.ShouldEqual(GenerateOriginBasedViewMatrix(_camera.Location, _camera.Yaw, _camera.Pitch, _camera.Roll));
    }

    // TODO: other methods need coverage
     
    public class CameraContext
    {
        public static ISettings _settings;
        public static Camera _camera;

        Establish context = () =>
        {
            _settings = MockRepository.GenerateStub<ISettings>();
            _camera = new Camera(_settings);
        };

        public static Matrix GenerateOriginBasedViewMatrix(DoubleVector3 location, float yaw, float pitch, float roll)
        {
            Matrix yawMatrix = Matrix.CreateRotationY(yaw);
            Matrix pitchMatrix = Matrix.CreateRotationX(pitch);
            Matrix rollMatrix = Matrix.CreateRotationZ(roll);

            Vector3 rotation = Vector3.Transform(Vector3.Forward, pitchMatrix);
            rotation = Vector3.Transform(rotation, yawMatrix);
            Vector3 lookAt = rotation;
            Vector3 up = Vector3.Transform(Vector3.UnitY, rollMatrix);

            return Matrix.CreateLookAt(Vector3.Zero, lookAt, up);
        }
    }
}
