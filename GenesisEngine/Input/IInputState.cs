using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace GenesisEngine
{
    public interface IInputState
    {
        bool IsKeyDown(Keys key);
        
        bool IsKeyPressed(Keys key);
        
        bool IsRightMouseButtonDown { get; }
        
        int MouseDeltaX { get; }
        
        int MouseDeltaY { get; }
        
        TimeSpan ElapsedTime { get; }

        void Update(TimeSpan elapsedTime, KeyboardState newKeyboardState, MouseState newMouseState);
    }
}
