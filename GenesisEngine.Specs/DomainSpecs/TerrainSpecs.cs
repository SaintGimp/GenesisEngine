using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Machine.Specifications;
using Rhino.Mocks;
using Microsoft.Xna.Framework;

namespace GenesisEngine.Specs.DomainSpecs
{
    [Subject(typeof (Terrain))]
    public class when_the_terrain_is_drawn : TerrainContext
    {
        public static DoubleVector3 _cameraLocation;
        public static Matrix _viewMatrix;
        public static Matrix _projectionMatrix;

        Establish context = () =>
        {
            _cameraLocation = DoubleVector3.Up;
            _viewMatrix = Matrix.Identity;
            _projectionMatrix = Matrix.Identity;
        };

        Because of = () =>
            _terrain.Draw(_cameraLocation, _viewMatrix, _projectionMatrix);

        It should_draw_the_faces = () =>
            _face.AssertWasCalled(x => x.Draw(_cameraLocation, _viewMatrix, _projectionMatrix));
    }

    [Subject(typeof(Terrain))]
    public class when_the_terrain_is_updated : TerrainContext
    {
        public static ClippingPlanes _clippingPlanes;

        Because of = () =>
            _terrain.Update(new TimeSpan(), DoubleVector3.Zero, DoubleVector3.Zero, _clippingPlanes);

        It should_update_the_faces = () =>
            _face.AssertWasCalled(x => x.Update(new TimeSpan(), DoubleVector3.Zero, DoubleVector3.Zero, _clippingPlanes));
    }
    
    public class TerrainContext
    {
        public static IQuadNode _face;
        public static ITerrain _terrain;

        Establish context = () =>
        {
            _face = MockRepository.GenerateStub<IQuadNode>();
            var faces = new List<IQuadNode>() {_face};

            _terrain = new Terrain(faces);
        };
    }
}
