using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Machine.Specifications;
using StructureMap;

namespace GenesisEngine.Specs.InfrastructureSpecs
{
    [Subject(typeof(Bootstrapper))]
    public class when_structuremap_is_bootstrapped
    {
        Because of = () =>
            Bootstrapper.BootstrapStructureMap();

        It should_create_a_valid_container_configuration = () =>
            ObjectFactory.AssertConfigurationIsValid();
    }
}
