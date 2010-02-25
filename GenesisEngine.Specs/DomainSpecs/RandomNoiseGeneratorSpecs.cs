using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace GenesisEngine.Specs.DomainSpecs
{
    [Subject(typeof(RandomNoiseGenerator))]
    public class when_noise_is_generated_for_the_same_location_twice : RandomNoiseGeneratorContext
    {
        It should_generate_the_same_noise = () =>
            _generator.GetNoise(new DoubleVector3(1.0, 2.0, 3.0)).ShouldEqual(_generator.GetNoise(new DoubleVector3(1.0, 2.0, 3.0)));
    }

    [Subject(typeof(RandomNoiseGenerator))]
    public class when_noise_is_generated_for_different_locations : RandomNoiseGeneratorContext
    {
        It should_generate_different_noise = () =>
            _generator.GetNoise(new DoubleVector3(1.0, 2.0, 3.0)).ShouldNotEqual(_generator.GetNoise(new DoubleVector3(3.0, 2.0, 1.0)));
    }

    [Subject(typeof(RandomNoiseGenerator))]
    public class when_two_different_generators_are_used : RandomNoiseGeneratorContext
    {
        public static RandomNoiseGenerator _anotherGenerator;

        Establish context = () =>
        {
            _anotherGenerator = new RandomNoiseGenerator();
        };

        It should_generate_the_same_noise = () =>
            _generator.GetNoise(new DoubleVector3(1.0, 2.0, 3.0)).ShouldEqual(_anotherGenerator.GetNoise(new DoubleVector3(1.0, 2.0, 3.0)));
    }

    // TODO: bounds?

    public class RandomNoiseGeneratorContext
    {
        public static RandomNoiseGenerator _generator;

        Establish context = () =>
        {
            _generator = new RandomNoiseGenerator();
        };
    }
}
