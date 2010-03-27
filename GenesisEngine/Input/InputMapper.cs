using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace GenesisEngine
{
    // TODO: It would be interesting to use the Reactive Framework here

    public class InputMapper : IInputMapper
    {
        readonly IEventAggregator _eventAggregator;
        readonly List<KeyEvent> _keyPressEvents = new List<KeyEvent>();
        readonly List<KeyEvent> _keyDownEvents = new List<KeyEvent>();
        readonly List<MouseMoveEvent> _mouseMoveEvents = new List<MouseMoveEvent>();

        public InputMapper(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void HandleInput(IInputState inputState)
        {
            SendKeyPressMessages(inputState);
            SendKeyDownMessages(inputState);
            SendMouseMoveMessages(inputState);
        }

        private void SendKeyPressMessages(IInputState inputState)
        {
            foreach (var keyEvent in _keyPressEvents.Where(keyEvent => inputState.IsKeyPressed(keyEvent.Key)))
            {
                keyEvent.Send(inputState);
            }
        }

        private void SendKeyDownMessages(IInputState inputState)
        {
            foreach (var keyEvent in _keyDownEvents.Where(keyEvent => inputState.IsKeyDown(keyEvent.Key)))
            {
                keyEvent.Send(inputState);
            }
        }

        private void SendMouseMoveMessages(IInputState inputState)
        {
            // TODO: we have the "right mouse button down" requirement hardcoded here for now
            if (inputState.IsRightMouseButtonDown && (inputState.MouseDeltaX != 0 || inputState.MouseDeltaY != 0))
            {
                foreach (var moveEvent in _mouseMoveEvents)
                {
                    moveEvent.Send(inputState);
                }
            }
        }

        // We put the key/action pairs into a list rather than a dictionary because
        // we want to be able to support multiple actions per key

        public void AddKeyPressMessage<T>(Keys key) where T : InputMessage, new()
        {
            _keyPressEvents.Add(new KeyEvent { Key = key, Send = x => _eventAggregator.SendMessage(new T { InputState = x}) });
        }

        public void AddKeyDownMessage<T>(Keys key) where T : InputMessage, new()
        {
            _keyDownEvents.Add(new KeyEvent { Key = key, Send = x => _eventAggregator.SendMessage(new T { InputState = x }) });
        }

        public void AddMouseMoveMessage<T>() where T : InputMessage, new()
        {
            _mouseMoveEvents.Add(new MouseMoveEvent { Send = x => _eventAggregator.SendMessage(new T { InputState = x }) });
        }

        private class KeyEvent
        {
            public Keys Key;
            public Action<IInputState> Send;
        }

        private class MouseMoveEvent
        {
            public Action<IInputState> Send;
        }
    }
}
