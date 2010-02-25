using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace GenesisEngine
{
    // TODO: this isn't tied strictly to a camera controller, so it could
    // arguably be broken out into its own library.  It'll probably be used
    // in conjunction with cameras, though, so we'll leave it here for now.

    public interface IInputState
    {
        bool IsKeyDown(Keys key);
        
        bool IsKeyPressed(Keys key);
        
        bool IsRightMouseButtonDown { get; }
        
        int MouseDeltaX { get; }
        
        int MouseDeltaY { get; }
        
        TimeSpan ElapsedTime { get; }

        void Update(TimeSpan elapsedTime, KeyboardState currentkeyboardState, MouseState currentMouseState);
    }
}
