using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace GenesisEngine
{
    public class XnaInputState : IInputState
    {
        private KeyboardState _previousKeyboardState;
        private KeyboardState _currentKeyboardState;
        private MouseState _previousMouseState;
        private MouseState _currentMouseState;
        private TimeSpan _elapsedTime;

        public bool IsKeyDown(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key);
        }

        public bool IsKeyPressed(Keys key)
        {
            return (!_previousKeyboardState.IsKeyDown(key) && _currentKeyboardState.IsKeyDown(key));
        }

        public bool IsRightMouseButtonDown
        {
            get { return _currentMouseState.RightButton == ButtonState.Pressed; }
        }

        public int MouseDeltaX
        {
            get { return (_currentMouseState.X - _previousMouseState.X); }
        }

        public int MouseDeltaY
        {
            get { return (_currentMouseState.Y - _previousMouseState.Y); }
        }

        public TimeSpan ElapsedTime
        {
            get { return _elapsedTime; }
        }

        public void Update(TimeSpan elapsedTime, KeyboardState currentkeyboardState, MouseState currentMouseState)
        {
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = currentkeyboardState;
            _previousMouseState = _currentMouseState;
            _currentMouseState = currentMouseState;
            _elapsedTime = elapsedTime;
        }
    }
}
