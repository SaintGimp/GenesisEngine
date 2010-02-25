using System;
using StructureMap;
using StructureMap.Interceptors;
using StructureMap.TypeRules;

namespace GenesisEngine
{
    public class EventAggregatorTypeInterceptor : TypeInterceptor
    {
        public object Process(object target, IContext context)
        {
            context.GetInstance<IEventAggregator>().AddListener(target);
            return target;
        }

        public bool MatchesType(Type type)
        {
            return type.ImplementsInterfaceTemplate(typeof(IListener<>));
        }
    }
}