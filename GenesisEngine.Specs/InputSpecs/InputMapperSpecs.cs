using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Microsoft.Xna.Framework.Input;
using Rhino.Mocks;

namespace GenesisEngine.Specs.InputSpecs
{
    [Subject(typeof(InputMapper))]
    public class when_a_mapped_key_is_pressed : InputMapperContext
    {
        Establish context = () =>
        {
            _inputMapper.AddKeyPressMessage<DoSomething>(Keys.K);
            _input.Stub(x => x.IsKeyPressed(Keys.K)).Return(true);
        };

        Because of = () =>
            _inputMapper.HandleInput(_input);

        It should_send_a_keypress_message = () =>
            _eventAggregator.AssertWasCalled(x => x.SendMessage(Arg<DoSomething>.Is.Anything));

        It should_send_a_message_containing_the_current_input_state = () =>
            _eventAggregator.AssertWasCalled(x => x.SendMessage(Arg<DoSomething>.Matches(p => p.InputState == _input)));
    }

    [Subject(typeof(InputMapper))]
    public class when_an_unmapped_key_is_pressed : InputMapperContext
    {
        Establish context = () =>
        {
            _inputMapper.AddKeyPressMessage<DoSomething>(Keys.K);
            _input.Stub(x => x.IsKeyPressed(Keys.J)).Return(true);
        };

        Because of = () =>
            _inputMapper.HandleInput(_input);

        It should_not_send_a_keypress_message = () =>
            _eventAggregator.AssertWasNotCalled(x => x.SendMessage(Arg<DoSomething>.Is.Anything));
    }

    [Subject(typeof(InputMapper))]
    public class when_no_keys_are_pressed : InputMapperContext
    {
        Establish context = () =>
            _inputMapper.AddKeyPressMessage<DoSomething>(Keys.K);

        Because of = () =>
            _inputMapper.HandleInput(_input);

        It should_not_send_a_keypress_message = () =>
            _eventAggregator.AssertWasNotCalled(x => x.SendMessage(Arg<DoSomething>.Is.Anything));
    }

    [Subject(typeof(InputMapper))]
    public class when_a_mapped_key_is_down : InputMapperContext
    {
        Establish context = () =>
        {
            _inputMapper.AddKeyDownMessage<DoSomething>(Keys.K);
            _input.Stub(x => x.IsKeyDown(Keys.K)).Return(true);
        };

        Because of = () =>
            _inputMapper.HandleInput(_input);

        It should_send_a_keydown_message = () =>
            _eventAggregator.AssertWasCalled(x => x.SendMessage(Arg<DoSomething>.Is.Anything));

        It should_send_a_message_containing_the_current_input_state = () =>
            _eventAggregator.AssertWasCalled(x => x.SendMessage(Arg<DoSomething>.Matches(p => p.InputState == _input)));
    }

    [Subject(typeof(InputMapper))]
    public class when_an_unmapped_key_is_down : InputMapperContext
    {
        Establish context = () =>
        {
            _inputMapper.AddKeyDownMessage<DoSomething>(Keys.K);
            _input.Stub(x => x.IsKeyDown(Keys.J)).Return(true);
        };

        Because of = () =>
            _inputMapper.HandleInput(_input);

        It should_not_send_a_keydown_message = () =>
            _eventAggregator.AssertWasNotCalled(x => x.SendMessage(Arg<DoSomething>.Is.Anything));
    }

    // TODO: how best to handle context name collisions with others in the same namespace?
    [Subject(typeof(InputMapper))]
    public class when_no_keys_are_down_ : InputMapperContext
    {
        Because of = () =>
            _inputMapper.HandleInput(_input);

        It should_not_send_a_keydown_message = () =>
            _eventAggregator.AssertWasNotCalled(x => x.SendMessage(Arg<DoSomething>.Is.Anything));
    }

    [Subject(typeof(InputMapper))]
    public class when_the_mouse_right_button_is_down_and_is_moved_horizontally : InputMapperContext
    {
        Establish context = () =>
        {
            _inputMapper.AddMouseMoveMessage<DoSomething>();
            _input.Stub(x => x.MouseDeltaX).Return(10);
            _input.Stub(x => x.IsRightMouseButtonDown).Return(true);
        };

        Because of = () =>
            _inputMapper.HandleInput(_input);

        It should_send_a_mousemove_message = () =>
            _eventAggregator.AssertWasCalled(x => x.SendMessage(Arg<DoSomething>.Is.Anything));
    }

    [Subject(typeof(InputMapper))]
    public class when_the_mouse_right_button_is_down_and_is_moved_vertically : InputMapperContext
    {
        Establish context = () =>
        {
            _inputMapper.AddMouseMoveMessage<DoSomething>();
            _input.Stub(x => x.MouseDeltaY).Return(10);
            _input.Stub(x => x.IsRightMouseButtonDown).Return(true);
        };

        Because of = () =>
            _inputMapper.HandleInput(_input);

        It should_send_a_mousemove_message = () =>
            _eventAggregator.AssertWasCalled(x => x.SendMessage(Arg<DoSomething>.Is.Anything));
    }

    [Subject(typeof(InputMapper))]
    public class when_the_mouse_right_button_is_not_down_and_is_moved : InputMapperContext
    {
        Establish context = () =>
        {
            _inputMapper.AddMouseMoveMessage<DoSomething>();
            _input.Stub(x => x.MouseDeltaX).Return(10);
            _input.Stub(x => x.MouseDeltaY).Return(10);
        };

        Because of = () =>
            _inputMapper.HandleInput(_input);

        It should_not_send_a_mousemove_message = () =>
            _eventAggregator.AssertWasNotCalled(x => x.SendMessage(Arg<DoSomething>.Is.Anything));
    }

    public class InputMapperContext
    {
        static public IInputState _input;
        public static IEventAggregator _eventAggregator;
        static public InputMapper _inputMapper;

        Establish context = () =>
        {
            _input = MockRepository.GenerateStub<IInputState>();
            _eventAggregator = MockRepository.GenerateStub<IEventAggregator>();
            _inputMapper = new InputMapper(_eventAggregator);
        };
    }

    public class DoSomething : InputMessage
    {
    }
}
