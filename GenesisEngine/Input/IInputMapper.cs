using Microsoft.Xna.Framework.Input;

namespace GenesisEngine
{
    public interface IInputMapper
    {
        void AddKeyPressMessage<T>(Keys key) where T : InputMessage, new();

        void AddKeyDownMessage<T>(Keys key) where T : InputMessage, new();

        void AddMouseMoveMessage<T>() where T : InputMessage, new();

        void HandleInput(IInputState inputState);
    }
}