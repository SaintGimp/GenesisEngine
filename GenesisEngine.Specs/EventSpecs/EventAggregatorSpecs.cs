using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using NSubstitute;

namespace GenesisEngine.Specs.EventSpecs
{
    [Subject(typeof(EventAggregator))]
    public class when_a_message_is_sent_and_the_aggregator_has_no_listeners : EventAggregatorContext
    {
        Because of = () =>
            _eventAggregator.SendMessage(_helloMessage);

        It should_not_send_any_messages = () =>
        {
            // Don't crash
        };            
    }

    [Subject(typeof(EventAggregator))]
    public class when_a_message_with_no_eligible_listeners_is_sent : EventAggregatorContext
    {
        Establish context = () =>
            _eventAggregator.AddListener(_helloListener1);

        Because of = () =>
            _eventAggregator.SendMessage(_goodbyeMessage);

        It should_not_send_any_messages = () =>
            _helloListener1.DidNotReceive().Handle(Arg.Any<Hello>());
    }

    [Subject(typeof(EventAggregator))]
    public class when_a_message_with_one_eligible_listener_is_sent : EventAggregatorContext
    {
        Establish context = () =>
            _eventAggregator.AddListener(_helloListener1);

        Because of = () =>
            _eventAggregator.SendMessage(_helloMessage);

        It should_send_the_message_to_the_listener = () =>
            _helloListener1.Received().Handle(_helloMessage);
    }

    [Subject(typeof(EventAggregator))]
    public class when_a_message_with_multiple_eligible_listeners_is_sent : EventAggregatorContext
    {
        Establish context = () =>
        {
            _eventAggregator.AddListener(_helloListener1);
            _eventAggregator.AddListener(_helloListener2);
        };

        Because of = () =>
            _eventAggregator.SendMessage(_helloMessage);

        It should_send_the_message_to_all_eligible_listeners = () =>
        {
            _helloListener1.Received().Handle(_helloMessage);
            _helloListener2.Received().Handle(_helloMessage);
        };
    }

    [Subject(typeof(EventAggregator))]
    public class when_a_listener_is_added_multiple_times : EventAggregatorContext
    {
        static int _handleCounter;

        Establish context = () =>
        {
            _helloListener1.When(x => x.Handle(_helloMessage)).Do(x => _handleCounter++);

            _eventAggregator.AddListener(_helloListener1);
            _eventAggregator.AddListener(_helloListener1);
        };

        Because of = () =>
            _eventAggregator.SendMessage(_helloMessage);

        It should_send_the_message_to_the_listener_only_once = () =>
            _handleCounter++;
    }

    [Subject(typeof(EventAggregator))]
    public class when_a_listener_wants_to_modify_the_aggregator_in_response_to_a_message : EventAggregatorContext
    {
        Establish context = () =>
        {
            _helloListener1.When(x => x.Handle(_helloMessage)).Do(x => _eventAggregator.AddListener(_helloListener2));
            
            _eventAggregator.AddListener(_helloListener1);
        };

        Because of = () =>
            _eventAggregator.SendMessage(_helloMessage);

        It should_allow_the_modification = () =>
            _eventAggregator.HasListener(_helloListener2);
    }

    [Subject(typeof(EventAggregator))]
    public class when_a_listener_loses_all_strong_references : EventAggregatorContext
    {
        Establish context = () =>
        {
            var disposableListener = Substitute.For<IListener<Hello>>();
            _eventAggregator.AddListener(disposableListener);
        };

        Because of = () =>
            GC.Collect();

        It should_remove_the_listener_from_the_aggregator = () =>
            _eventAggregator.HasListener(_helloListener1).ShouldBeFalse();
    }

    public class EventAggregatorContext
    {
        public static Hello _helloMessage;
        public static Goodbye _goodbyeMessage;
        public static IListener<Hello> _helloListener1;
        public static IListener<Hello> _helloListener2;
        public static IListener<Goodbye> _goodbyeListener;
        public static TestableEventAggregator _eventAggregator;

        Establish context = () =>
        {
            _helloMessage = new Hello();
            _goodbyeMessage = new Goodbye();
            _helloListener1 = Substitute.For<IListener<Hello>>();
            _helloListener2 = Substitute.For<IListener<Hello>>();
            _goodbyeListener = Substitute.For<IListener<Goodbye>>();
            _eventAggregator = new TestableEventAggregator();
        };
    }

    public class TestableEventAggregator : EventAggregator
    {
        public new bool HasListener(object listener)
        {
            return base.HasListener(listener);
        }
    }

    public class Hello
    {
    }

    public class Goodbye
    {
    }
}
