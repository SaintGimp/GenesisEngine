using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Machine.Specifications;
using NSubstitute;
using Microsoft.Xna.Framework;

namespace GenesisEngine.Specs.DomainSpecs
{
    [Subject(typeof (Terrain))]
    public class when_the_terrain_is_drawn : TerrainContext
    {
        public static DoubleVector3 _cameraLocation;
        public static BoundingFrustum _viewFrustum;
        public static Matrix _viewMatrix;
        public static Matrix _projectionMatrix;

        Establish context = () =>
        {
            _cameraLocation = DoubleVector3.Up;
            _viewFrustum = new BoundingFrustum(Matrix.Identity);
            _viewMatrix = Matrix.Identity;
            _projectionMatrix = Matrix.Identity;
        };

        Because of = () =>
            _terrain.Draw(_cameraLocation, _viewFrustum, _viewMatrix, _projectionMatrix);

        It should_draw_the_faces = () =>
            _face.Received().Draw(_cameraLocation, _viewFrustum, _viewMatrix, _projectionMatrix);
    }

    [Subject(typeof(Terrain))]
    public class when_the_terrain_is_updated : TerrainContext
    {
        Because of = () =>
            _terrain.Update(DoubleVector3.Zero, DoubleVector3.Zero);

        It should_update_the_faces = () =>
            _face.Received().Update(DoubleVector3.Zero, DoubleVector3.Zero);
    }
    
    public class TerrainContext
    {
        public static IQuadNode _face;
        public static ITerrain _terrain;

        Establish context = () =>
        {
            _face = Substitute.For<IQuadNode>();
            var faces = new List<IQuadNode>() {_face};

            _terrain = new Terrain(faces);
        };
    }
}
