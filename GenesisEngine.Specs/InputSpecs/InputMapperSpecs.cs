using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Microsoft.Xna.Framework.Input;
using NSubstitute;

namespace GenesisEngine.Specs.InputSpecs
{
    [Subject(typeof(InputMapper))]
    public class when_a_mapped_key_is_pressed : InputMapperContext
    {
        Establish context = () =>
        {
            _inputMapper.AddKeyPressMessage<DoSomething>(Keys.K);
            _input.IsKeyPressed(Keys.K).Returns(true);
        };

        Because of = () =>
            _inputMapper.HandleInput(_input);

        It should_send_a_keypress_message = () =>
            _eventAggregator.Received().SendMessage(Arg.Any<DoSomething>());

        It should_send_a_message_containing_the_current_input_state = () =>
            _eventAggregator.Received().SendMessage(Arg.Is<DoSomething>(p => p.InputState == _input));
    }

    [Subject(typeof(InputMapper))]
    public class when_a_multimapped_key_is_pressed : InputMapperContext
    {
        Establish context = () =>
        {
            _inputMapper.AddKeyPressMessage<DoSomething>(Keys.K);
            _inputMapper.AddKeyPressMessage<DoSomethingElse>(Keys.K);
            _input.IsKeyPressed(Keys.K).Returns(true);
        };

        Because of = () =>
            _inputMapper.HandleInput(_input);

        It should_send_the_first_keypress_message = () =>
            _eventAggregator.Received().SendMessage(Arg.Any<DoSomething>());

        It should_send_the_second_keypress_message = () =>
            _eventAggregator.Received().SendMessage(Arg.Any<DoSomethingElse>());
    }

    [Subject(typeof(InputMapper))]
    public class when_an_unmapped_key_is_pressed : InputMapperContext
    {
        Establish context = () =>
        {
            _inputMapper.AddKeyPressMessage<DoSomething>(Keys.K);
            _input.IsKeyPressed(Keys.J).Returns(true);
        };

        Because of = () =>
            _inputMapper.HandleInput(_input);

        It should_not_send_a_keypress_message = () =>
            _eventAggregator.DidNotReceive().SendMessage(Arg.Any<DoSomething>());
    }

    [Subject(typeof(InputMapper))]
    public class when_no_keys_are_pressed : InputMapperContext
    {
        Establish context = () =>
            _inputMapper.AddKeyPressMessage<DoSomething>(Keys.K);

        Because of = () =>
            _inputMapper.HandleInput(_input);

        It should_not_send_a_keypress_message = () =>
            _eventAggregator.DidNotReceive().SendMessage(Arg.Any<DoSomething>());
    }

    [Subject(typeof(InputMapper))]
    public class when_a_mapped_key_is_down : InputMapperContext
    {
        Establish context = () =>
        {
            _inputMapper.AddKeyDownMessage<DoSomething>(Keys.K);
            _input.IsKeyDown(Keys.K).Returns(true);
        };

        Because of = () =>
            _inputMapper.HandleInput(_input);

        It should_send_a_keydown_message = () =>
            _eventAggregator.Received().SendMessage(Arg.Any<DoSomething>());

        It should_send_a_message_containing_the_current_input_state = () =>
            _eventAggregator.Received().SendMessage(Arg.Is<DoSomething>(p => p.InputState == _input));
    }

    [Subject(typeof(InputMapper))]
    public class when_a_multimapped_mapped_key_is_down : InputMapperContext
    {
        Establish context = () =>
        {
            _inputMapper.AddKeyDownMessage<DoSomething>(Keys.K);
            _inputMapper.AddKeyDownMessage<DoSomethingElse>(Keys.K);
            _input.IsKeyDown(Keys.K).Returns(true);
        };

        Because of = () =>
            _inputMapper.HandleInput(_input);

        It should_send_the_first_keydown_message = () =>
            _eventAggregator.Received().SendMessage(Arg.Any<DoSomething>());

        It should_send_the_second_keydown_message = () =>
        _eventAggregator.Received().SendMessage(Arg.Any<DoSomethingElse>());
    }

    [Subject(typeof(InputMapper))]
    public class when_an_unmapped_key_is_down : InputMapperContext
    {
        Establish context = () =>
        {
            _inputMapper.AddKeyDownMessage<DoSomething>(Keys.K);
            _input.IsKeyDown(Keys.J).Returns(true);
        };

        Because of = () =>
            _inputMapper.HandleInput(_input);

        It should_not_send_a_keydown_message = () =>
            _eventAggregator.DidNotReceive().SendMessage(Arg.Any<DoSomething>());
    }

    // TODO: how best to handle context name collisions with others in the same namespace?
    [Subject(typeof(InputMapper))]
    public class when_no_keys_are_down_ : InputMapperContext
    {
        Because of = () =>
            _inputMapper.HandleInput(_input);

        It should_not_send_a_keydown_message = () =>
            _eventAggregator.DidNotReceive().SendMessage(Arg.Any<DoSomething>());
    }

    [Subject(typeof(InputMapper))]
    public class when_the_mouse_right_button_is_down_and_is_moved_horizontally : InputMapperContext
    {
        Establish context = () =>
        {
            _inputMapper.AddMouseMoveMessage<DoSomething>();
            _input.MouseDeltaX.Returns(10);
            _input.IsRightMouseButtonDown.Returns(true);
        };

        Because of = () =>
            _inputMapper.HandleInput(_input);

        It should_send_a_mousemove_message = () =>
            _eventAggregator.Received().SendMessage(Arg.Any<DoSomething>());
    }

    [Subject(typeof(InputMapper))]
    public class when_the_mouse_right_button_is_down_and_is_moved_vertically : InputMapperContext
    {
        Establish context = () =>
        {
            _inputMapper.AddMouseMoveMessage<DoSomething>();
            _input.MouseDeltaY.Returns(10);
            _input.IsRightMouseButtonDown.Returns(true);
        };

        Because of = () =>
            _inputMapper.HandleInput(_input);

        It should_send_a_mousemove_message = () =>
            _eventAggregator.Received().SendMessage(Arg.Any<DoSomething>());
    }

    [Subject(typeof(InputMapper))]
    public class when_the_mouse_right_button_is_not_down_and_is_moved : InputMapperContext
    {
        Establish context = () =>
        {
            _inputMapper.AddMouseMoveMessage<DoSomething>();
            _input.MouseDeltaX.Returns(10);
            _input.MouseDeltaY.Returns(10);
        };

        Because of = () =>
            _inputMapper.HandleInput(_input);

        It should_not_send_a_mousemove_message = () =>
            _eventAggregator.DidNotReceive().SendMessage(Arg.Any<DoSomething>());
    }

    public class InputMapperContext
    {
        static public IInputState _input;
        public static IEventAggregator _eventAggregator;
        static public InputMapper _inputMapper;

        Establish context = () =>
        {
            _input = Substitute.For<IInputState>();
            _eventAggregator = Substitute.For<IEventAggregator>();
            _inputMapper = new InputMapper(_eventAggregator);
        };
    }

    public class DoSomething : InputMessage
    {
    }

    public class DoSomethingElse : InputMessage
    {
    }
}
