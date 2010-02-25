using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Machine.Specifications;
using Microsoft.Xna.Framework.Input;

namespace GenesisEngine.Specs.InputSpecs
{
    [Subject(typeof(XnaInputState))]
    public class when_it_is_updated : XnaInputStateContext
    {
        Because of = () =>
            _inputState.Update(_elapsedTime, _currentKeyboardState, _currentMouseState);

        It should_remember_the_elapsed_time = () =>
            _inputState.ElapsedTime.ShouldEqual(_elapsedTime);
    }

    [Subject(typeof(XnaInputState))]
    public class when_no_keys_are_down : XnaInputStateContext
    {
        Because of = () =>
            _inputState.Update(_elapsedTime, _currentKeyboardState, _currentMouseState);

        It should_not_report_that_any_keys_are_down = () =>
            _inputState.IsKeyDown(Keys.A).ShouldBeFalse();

        It should_not_report_that_any_keys_are_pressed = () =>
            _inputState.IsKeyPressed(Keys.A).ShouldBeFalse();
    }

    [Subject(typeof(XnaInputState))]
    public class when_a_key_is_down_for_the_first_time : XnaInputStateContext
    {
        Establish context = () =>
            _currentKeyboardState = SetKey(_currentKeyboardState, Keys.A);

        Because of = () =>
            _inputState.Update(_elapsedTime, _currentKeyboardState, _currentMouseState);

        It should_report_that_the_key_is_down = () =>
            _inputState.IsKeyDown(Keys.A).ShouldBeTrue();

        It should_report_that_the_key_is_pressed = () =>
            _inputState.IsKeyPressed(Keys.A).ShouldBeTrue();

        It should_not_report_that_other_keys_are_down = () =>
            _inputState.IsKeyDown(Keys.B).ShouldBeFalse();

        It should_not_report_that_other_keys_are_pressed = () =>
            _inputState.IsKeyPressed(Keys.B).ShouldBeFalse();
    }

    [Subject(typeof(XnaInputState))]
    public class when_a_key_is_down_for_the_second_time : XnaInputStateContext
    {
        Establish context = () =>
        {
            _previousKeyboardState = SetKey(_previousKeyboardState, Keys.A);
            _inputState.Update(_elapsedTime, _previousKeyboardState, _previousMouseState);
            _currentKeyboardState = SetKey(_currentKeyboardState, Keys.A);
        };

        Because of = () =>
            _inputState.Update(_elapsedTime, _currentKeyboardState, _currentMouseState);

        It should_report_that_the_key_is_down = () =>
            _inputState.IsKeyDown(Keys.A).ShouldBeTrue();

        It should_not_report_that_the_key_is_pressed = () =>
            _inputState.IsKeyPressed(Keys.A).ShouldBeFalse();

        It should_not_report_that_other_keys_are_down = () =>
            _inputState.IsKeyDown(Keys.B).ShouldBeFalse();

        It should_not_report_that_other_keys_are_pressed = () =>
            _inputState.IsKeyPressed(Keys.B).ShouldBeFalse();
    }

    [Subject(typeof(XnaInputState))]
    public class when_the_right_mouse_button_is_not_down : XnaInputStateContext
    {
        Because of = () =>
            _inputState.Update(_elapsedTime, _currentKeyboardState, _currentMouseState);

        It should_not_report_that_the_right_mouse_button_is_down = () =>
            _inputState.IsRightMouseButtonDown.ShouldBeFalse();
    }

    [Subject(typeof(XnaInputState))]
    public class when_the_right_mouse_button_is_down : XnaInputStateContext
    {
        Establish context = () =>
            _currentMouseState = SetMouseRightButton(_currentMouseState, ButtonState.Pressed);

        Because of = () =>
            _inputState.Update(_elapsedTime, _currentKeyboardState, _currentMouseState);

        It should_report_that_the_right_mouse_button_is_down = () =>
            _inputState.IsRightMouseButtonDown.ShouldBeTrue();
    }

    [Subject(typeof(XnaInputState))]
    public class when_the_mouse_is_not_moved : XnaInputStateContext
    {
        Because of = () =>
            _inputState.Update(_elapsedTime, _currentKeyboardState, _currentMouseState);

        It should_not_report_that_the_mouse_moved_horizontally = () =>
            _inputState.MouseDeltaX.ShouldEqual(0);

        It should_not_report_that_the_mouse_moved_vertically = () =>
            _inputState.MouseDeltaY.ShouldEqual(0);
    }

    [Subject(typeof(XnaInputState))]
    public class when_the_mouse_is_moved : XnaInputStateContext
    {
        Because of = () =>
        {
            _previousMouseState = SetMouseXY(_previousMouseState, 3, 15);
            _inputState.Update(_elapsedTime, _previousKeyboardState, _previousMouseState);
            _currentMouseState = SetMouseXY(_currentMouseState, 10, 13);
            _inputState.Update(_elapsedTime, _currentKeyboardState, _currentMouseState);
        };

        It should_report_that_the_mouse_moved_horizontally = () =>
            _inputState.MouseDeltaX.ShouldEqual(7);

        It should_not_report_that_the_mouse_moved_vertically = () =>
            _inputState.MouseDeltaY.ShouldEqual(-2);
    }

    public class XnaInputStateContext
    {
        static public KeyboardState _previousKeyboardState;
        static public KeyboardState _currentKeyboardState;
        static public MouseState _previousMouseState;
        static public MouseState _currentMouseState;
        static public TimeSpan _elapsedTime;
        public static XnaInputState _inputState;

        Establish context = () =>
        {
            _previousKeyboardState = new KeyboardState();
            _currentKeyboardState = new KeyboardState();
            _previousMouseState = new MouseState();
            _currentMouseState = new MouseState();
            _elapsedTime = new TimeSpan(0, 0, 1);
            _inputState = new XnaInputState();
        };

        static public MouseState SetMouseXY(MouseState state, int x, int y)
        {
            ValueType boxedState = state;

            FieldInfo field = typeof(MouseState).GetField("x", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
            field.SetValue(boxedState, x);
            field = typeof(MouseState).GetField("y", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
            field.SetValue(boxedState, y);
            
            return (MouseState)boxedState;
        }

        static public MouseState SetMouseRightButton(MouseState state, ButtonState buttonState)
        {
            ValueType boxedState = state;
            
            FieldInfo field = typeof(MouseState).GetField("rightButton", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
            field.SetValue(boxedState, buttonState);
            
            return (MouseState)boxedState;
        }

        static public KeyboardState SetKey(KeyboardState state, Keys key)
        {
            // it would probably be better to use .GetPressedKeys and the constructor that takes
            // a KeyState array to do this work.

            ValueType boxedState = state;

            MethodInfo addPressedKeyMethod = typeof(KeyboardState).GetMethod("AddPressedKey", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
            object[] argumentArray = { key };
            addPressedKeyMethod.Invoke(boxedState, argumentArray);

            return (KeyboardState)boxedState;
        }
    }
}
