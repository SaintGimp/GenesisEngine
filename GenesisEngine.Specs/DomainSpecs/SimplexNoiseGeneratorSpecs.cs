// ReSharper disable CheckNamespace

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

// TODO: better fix for spec name collisions
namespace GenesisEngine.Specs.DomainSpecs.SimplexNoiseGeneratorSpecs
{
    [Subject(typeof(SimplexNoiseGenerator))]
    public class when_noise_is_generated_for_the_same_location_twice : SimplexNoiseGeneratorContext
    {
        It should_generate_the_same_noise = () =>
            _generator.GetNoise(new DoubleVector3(1.2, 3.4, 5.6)).ShouldEqual(_generator.GetNoise(new DoubleVector3(1.2, 3.4, 5.6)));
    }

    [Subject(typeof(SimplexNoiseGenerator))]
    public class when_noise_is_generated_for_different_locations : SimplexNoiseGeneratorContext
    {
        It should_generate_different_noise = () =>
            _generator.GetNoise(new DoubleVector3(1.2, 3.4, 5.6)).ShouldNotEqual(_generator.GetNoise(new DoubleVector3(6.5, 4.3, 2.1)));
    }

    [Subject(typeof(SimplexNoiseGenerator))]
    public class when_two_different_generators_are_used : SimplexNoiseGeneratorContext
    {
        public static SimplexNoiseGenerator _anotherGenerator;

        Establish context = () =>
        {
            _anotherGenerator = new SimplexNoiseGenerator();
        };

        It should_generate_the_same_noise = () =>
            _generator.GetNoise(new DoubleVector3(1.2, 3.4, 5.6)).ShouldEqual(_anotherGenerator.GetNoise(new DoubleVector3(1.2, 3.4, 5.6)));
    }

    // TODO: bounds?

    public class SimplexNoiseGeneratorContext
    {
        public static SimplexNoiseGenerator _generator;

        Establish context = () =>
        {
            _generator = new SimplexNoiseGenerator();
            string path = Environment.CurrentDirectory;
        };
    }
}
