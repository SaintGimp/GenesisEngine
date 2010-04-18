using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace GenesisEngine
{
    public class QuadMesh : IQuadMesh, IDisposable
    {
        // XNA is a right-handed system so the positive z axis points out of the screen
        // and the winding order is clockwise (counter-clockwise faces are culled).

        // TODO: make sure we're accessing 2D arrays in row-major order as per http://msdn.microsoft.com/en-us/magazine/cc872851.aspx

        // This should be 2^n+1
        readonly int _gridSize = 65;

        DoubleVector3 _locationRelativeToPlanet;
        double _planetRadius;
        DoubleVector3 _uVector;
        DoubleVector3 _vVector;
        DoubleVector3 _planeNormalVector;
        protected QuadNodeExtents _extents;

        IHeightfieldGenerator _generator;

        VertexPositionNormalColored[] _vertices;
        int[] _indices;
        DoubleVector3[] _vertexSamples;

        IQuadMeshRenderer _renderer;
        readonly ISettings _settings;

        public QuadMesh(IHeightfieldGenerator generator, IQuadMeshRenderer renderer, ISettings settings)
        {
            _generator = generator;
            _renderer = renderer;
            _settings = settings;
        }

        public int Level { get; private set; }

        public bool IsVisibleToCamera { get; private set; }

        public double WidthToCameraDistanceRatio { get; private set; }

        // TODO: push this data in through the constructor, probably in a QuadMeshDefintion class, and make
        // this method private.  Except that would do real work in construction.  Hmmm.  When we explode this class
        // into separate responsibilites, this problem may go away.
        public void Initialize(double planetRadius, DoubleVector3 planeNormalVector, DoubleVector3 uVector, DoubleVector3 vVector, QuadNodeExtents extents, int level)
        {
            _planetRadius = planetRadius;
            _planeNormalVector = planeNormalVector;
            _uVector = uVector;
            _vVector = vVector;
            _extents = extents;
            Level = level;

            _locationRelativeToPlanet = (_planeNormalVector) + (_uVector * ((_extents.West + (_extents.Width / 2.0)))) + (_vVector * ((_extents.North + (_extents.Width / 2.0))));
            _locationRelativeToPlanet = _locationRelativeToPlanet.ProjectUnitPlaneToUnitSphere() * _planetRadius;

            GenerateMeshVertices();
            CollectMeshSamples();

            _renderer.Initialize(_vertices, _indices);
        }

        void CollectMeshSamples()
        {
            // We just sample the corners and the middle for now
            _vertexSamples = new DoubleVector3[]
            {
                _vertices[0].Position,
                _vertices[_gridSize - 1].Position,
                _vertices[_vertices.Length / 2].Position,
                _vertices[_gridSize * (_gridSize - 1)].Position,
                _vertices[_gridSize * _gridSize - 1].Position
            };

            // Move them back into real space
            for (int x = 0; x < _vertexSamples.Length; x++)
            {
                _vertexSamples[x] += _locationRelativeToPlanet;
            }
        }

        private void GenerateMeshVertices()
        {
            _vertices = new VertexPositionNormalColored[_gridSize * _gridSize];

            for (int u = 0; u < _gridSize; u++)
            {
                for (int v = 0; v < _gridSize; v++)
                {
                    // TODO: pass in the vertex to be modified
                    _vertices[u * _gridSize + v] = GetVertexInMeshSpace(u, v);
                }
            }

            GenerateIndices();
            GenerateNormals();

            if (_settings.ShowQuadBoundaries)
            {
                MarkQuadBoundaries();
            }
        }

        void MarkQuadBoundaries()
        {
            MarkNorthBoundary(_extents.North == -1 ? Color.Green : Color.Red);
            MarkSouthBoundary(_extents.South == 1 ? Color.Green : Color.Red);
            MarkWestBoundary(_extents.West == -1 ? Color.Green : Color.Red);
            MarkEastBoundary(_extents.East == 1 ? Color.Green : Color.Red);
        }

        void MarkNorthBoundary(Color color)
        {
            for (int x = 0; x < _gridSize; x++)
            {
                _vertices[x].Color = color;
            }
        }

        void MarkSouthBoundary(Color color)
        {
            for (int x = 0; x < _gridSize; x++)
            {
                _vertices[_vertices.Length - x - 1].Color = color;
            }
        }

        void MarkWestBoundary(Color color)
        {
            for (int x = 0; x < _gridSize; x++)
            {
                _vertices[_gridSize * x].Color = color;
            }
        }

        void MarkEastBoundary(Color color)
        {
            for (int x = 0; x < _gridSize; x++)
            {
                _vertices[_gridSize * x + _gridSize - 1].Color = color;
            }
        }

        private VertexPositionNormalColored GetVertexInMeshSpace(int u, int v)
        {
            // Check out "Textures and Modelling - A Procedural Approach" by Ken Musgaves 

            // Managing data in different reference frames (zero-based mesh, real-world-based mesh) is going
            // to be tricky.  We want to build a mesh where
            // the center of the mesh is at 0,0 but the vertices are sphere-projected as though they were out in their
            // correct place in the sphere.  Then we want to keep track of the mesh's real-world location so we can do a
            // camera-relative translation for rendering.

            // We start with a sea-level-based height and quad-relative coordinates.  We first convert the quad
            // coordinates into a planet-space vector that points to the equivalent point on the mesh plane.  Then we project
            // to a sphere by normalizing the vector and extending it by the planet radius plus heightfield height, which gives
            // us the correct spherical distance from the center.  Finally we translate the vector "downward" by the radius so
            // that a zero-height point in the exact middle of the mesh surface would be at the origin.

            var sphereUnitVector = ProjectOntoSphere(u, v);
            var terrainHeight = _generator.GetHeight(sphereUnitVector, Level, 8000);

            // TODO: Temporary water adjustment, needs to live somewhere else
            if (terrainHeight < 0)
            {
                terrainHeight = 0;
            }

            var realVector = ConvertToRealSpace(sphereUnitVector, terrainHeight);
            var meshVector = ConvertToMeshSpace(realVector);

            return CreateVertex(meshVector, terrainHeight); ;
        }

        VertexPositionNormalColored CreateVertex(DoubleVector3 meshVector, double terrainHeight)
        {
            var vertex = new VertexPositionNormalColored { Position = meshVector };

            if (terrainHeight <= 0)
            {
                vertex.Color = Color.Blue;
            }
            else
            {
                vertex.Color = Color.White;
            }

            return vertex;
        }

        private DoubleVector3 ProjectOntoSphere(int u, int v)
        {
            var planeUnitVector = ConvertToUnitPlaneVector(u, v);
            var sphereUnitVector = planeUnitVector.ProjectUnitPlaneToUnitSphere();

            return sphereUnitVector;
        }

        private DoubleVector3 ConvertToUnitPlaneVector(int u, int v)
        {
            // TODO: promote to class member
            var stride = _extents.Width / (_gridSize - 1);

            var uDelta = _uVector * (_extents.North + (u * stride));
            var vDelta = _vVector * (_extents.West + (v * stride));
            var convertedVector = _planeNormalVector + uDelta + vDelta;

            return convertedVector;
        }

        private DoubleVector3 ConvertToRealSpace(DoubleVector3 sphereUnitVector, double terrainHeight)
        {
            return sphereUnitVector * (_planetRadius + terrainHeight);
        }

        private DoubleVector3 ConvertToMeshSpace(DoubleVector3 planetSpaceVector)
        {
            return planetSpaceVector - _locationRelativeToPlanet;
        }

        private void GenerateIndices()
        {
            // TODO: I think we can generate the indices once and share it for all
            // instances since it never changes.  In the future we'll want to deal
            // with adjacent nodes at different levels by constructing special
            // index sets that blend them at the edge.

            _indices = new int[(_gridSize - 1) * (_gridSize - 1) * 6];
            int counter = 0;
            for (int x = 0; x < _gridSize - 1; x++)
            {
                for (int y = 0; y < _gridSize - 1; y++)
                {
                    int topLeft = x * _gridSize + y;
                    int lowerLeft = (x + 1) * _gridSize + y;
                    int topRight = x * _gridSize + (y + 1);
                    int lowerRight = (x + 1) * _gridSize + (y + 1);

                    _indices[counter++] = topLeft;
                    _indices[counter++] = lowerRight;
                    _indices[counter++] = lowerLeft;

                    _indices[counter++] = topLeft;
                    _indices[counter++] = topRight;
                    _indices[counter++] = lowerRight;
                }
            }
        }

        private void GenerateNormals()
        {
            InitializeNormals();
            CalculateNormals();
            NormalizeNormals();
        }

        private void InitializeNormals()
        {
            for (int x = 0; x < _vertices.Length; x++)
            {
                _vertices[x].Normal = new Vector3(0, 0, 0);
            }
        }

        private void CalculateNormals()
        {
            // Iterate through each indexed vertex and gradually
            // accumulate the normals in the vertices as we go.

            for (int i = 0; i < _indices.Length / 3; i++)
            {
                int index1 = _indices[i * 3];
                int index2 = _indices[i * 3 + 1];
                int index3 = _indices[i * 3 + 2];

                Vector3 side1 = _vertices[index1].Position - _vertices[index3].Position;
                Vector3 side2 = _vertices[index1].Position - _vertices[index2].Position;
                Vector3 normal = Vector3.Cross(side1, side2);

                _vertices[index1].Normal += normal;
                _vertices[index2].Normal += normal;
                _vertices[index3].Normal += normal;
            }
        }

        private void NormalizeNormals()
        {
            for (int i = 0; i < _vertices.Length; i++)
                _vertices[i].Normal.Normalize();
        }

        public void Update(TimeSpan elapsedTime, DoubleVector3 cameraLocation, DoubleVector3 planetLocation, ClippingPlanes clippingPlanes)
        {
            var cameraRelationship = GetDistanceFrom(cameraLocation);
            var distanceFromCamera = cameraRelationship.ClosestDistance;
            WidthToCameraDistanceRatio = distanceFromCamera / WidthInRealSpaceUnits();

            DetermineVisibility(cameraLocation, planetLocation, cameraRelationship.ClosestVertex);

            SetClippingPlanes(cameraRelationship, clippingPlanes);
        }

        MeshDistance GetDistanceFrom(DoubleVector3 location)
        {
            double closestDistanceSquared = double.MaxValue;
            double furthestDistanceSquared = double.MinValue;
            DoubleVector3 closestVertex = _vertexSamples[0];
            DoubleVector3 furthestVertex = _vertexSamples[0];

            foreach (var vertex in _vertexSamples)
            {
                var distanceSquared = DoubleVector3.DistanceSquared(location, vertex);
                if (distanceSquared < closestDistanceSquared)
                {
                    closestDistanceSquared = distanceSquared;
                    closestVertex = vertex;
                }
                else if (distanceSquared > furthestDistanceSquared)
                {
                    furthestDistanceSquared = distanceSquared;
                    furthestVertex = vertex;
                }
            }

            return new MeshDistance()
            {
                ClosestDistance = Math.Sqrt(closestDistanceSquared),
                ClosestVertex = closestVertex,
                FurthestDistance = Math.Sqrt(closestDistanceSquared),
                FurthestVertex = furthestVertex
            };
        }

        double WidthInRealSpaceUnits()
        {
            return _extents.Width * _planetRadius;
        }

        void DetermineVisibility(DoubleVector3 cameraLocation, DoubleVector3 planetLocation, DoubleVector3 closestVertex)
        {
            var cameraDirection = DoubleVector3.Normalize(cameraLocation - planetLocation);
            var nodeDirection = DoubleVector3.Normalize(closestVertex - planetLocation);

            var horizonAngle = Math.Acos(_planetRadius * 0.997 / DoubleVector3.Distance(planetLocation, cameraLocation));
            var angleToNode = Math.Acos(DoubleVector3.Dot(cameraDirection, nodeDirection));

            IsVisibleToCamera = (horizonAngle > angleToNode);
        }

        void SetClippingPlanes(MeshDistance cameraRelationship, ClippingPlanes clippingPlanes)
        {
            if (IsVisibleToCamera)
            {
                if (clippingPlanes.Near > cameraRelationship.ClosestDistance)
                {
                    clippingPlanes.Near = cameraRelationship.ClosestDistance;
                }
                if (clippingPlanes.Far < cameraRelationship.FurthestDistance)
                {
                    clippingPlanes.Far = cameraRelationship.FurthestDistance;
                }
            }
        }

        public void Draw(DoubleVector3 cameraLocation, Matrix originBasedViewMatrix, Matrix projectionMatrix)
        {
            _renderer.Draw(_locationRelativeToPlanet, cameraLocation, originBasedViewMatrix, projectionMatrix);
        }

        public void Dispose()
        {
            ((IDisposable)_renderer).Dispose();
        }

        private class MeshDistance
        {
            public DoubleVector3 ClosestVertex { get; set; }
            public DoubleVector3 FurthestVertex { get; set; }
            public double ClosestDistance { get; set; }
            public double FurthestDistance { get; set; }
        }
    }
}

