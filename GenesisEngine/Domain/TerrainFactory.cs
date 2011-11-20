using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GenesisEngine
{
    public class TerrainFactory : ITerrainFactory
    {
        IQuadNodeFactory _quadNodeFactory;
        static readonly IDictionary<Vector3, Vector3[]> _faceOrientations;

        static TerrainFactory()
        {
            // TODO: replace array with anonymous type
            // face orientations is a map between the face normal vectors and their
            // UV vectors.  UV is row-major so they should be face-local south then
            // face-local east vectors.
            _faceOrientations = new Dictionary<Vector3, Vector3[]>
            {
                {Vector3.Up, new[] {Vector3.Backward, Vector3.Right}},
                {Vector3.Left, new[] {Vector3.Down, Vector3.Backward}},
                {Vector3.Right, new[] {Vector3.Down, Vector3.Forward}},
                {Vector3.Forward, new[] {Vector3.Down, Vector3.Left}},
                {Vector3.Backward, new[] {Vector3.Down, Vector3.Right}},
                {Vector3.Down, new[] {Vector3.Forward, Vector3.Right}}
            };
        }

        public TerrainFactory(IQuadNodeFactory quadNodeFactory)
        {
            _quadNodeFactory = quadNodeFactory;
        }

        public ITerrain Create(double planetRadius)
        {
            var faces = new List<IQuadNode>();
            var faceNormals = new [] { Vector3.Up, Vector3.Left, Vector3.Right, Vector3.Forward, Vector3.Backward, Vector3.Down };

            for (int x = 0; x < 6; x++)
            {
                var face = CreateFace(planetRadius, faceNormals[x]);
                faces.Add(face);
            }

            return new Terrain(faces);
        }

        private IQuadNode CreateFace(double planetRadius, Vector3 normalVector)
        {
            var face = _quadNodeFactory.Create();
            
            var orientationVectors = _faceOrientations[normalVector];
            var u = orientationVectors[0];
            var v = orientationVectors[1];
            face.Initialize(planetRadius, normalVector, u, v, new QuadNodeExtents(-1.0, 1.0, -1.0, 1.0), 0);

            return face;
        }
    }
}
