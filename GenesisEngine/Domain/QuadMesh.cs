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
        static readonly short _gridSize = 65;

        DoubleVector3 _locationRelativeToPlanet;
        double _planetRadius;
        DoubleVector3 _uVector;
        DoubleVector3 _vVector;
        DoubleVector3 _planeNormalVector;
        protected QuadNodeExtents _extents;
        double _meshStride;

        IHeightfieldGenerator _generator;

        VertexPositionNormalColor[] _vertices;
        static short[] _indices;
        DoubleVector3[] _vertexSamples;

        IQuadMeshRenderer _renderer;
        readonly ISettings _settings;

        static QuadMesh()
        {
            GenerateIndices();    
        }

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

            _meshStride = _extents.Width / (_gridSize - 1);

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

            // Move them back into planet-relative space
            for (int x = 0; x < _vertexSamples.Length; x++)
            {
                _vertexSamples[x] += _locationRelativeToPlanet;
            }
        }

        void GenerateMeshVertices()
        {
            _vertices = new VertexPositionNormalColor[_gridSize * _gridSize];

            for (int row = 0; row < _gridSize; row++)
            {
                for (int column = 0; column < _gridSize; column++)
                {
                    _vertices[row * _gridSize + column] = GetVertexInMeshSpace(column, row);
                }
            }

            GenerateNormals();
        }

        VertexPositionNormalColor GetVertexInMeshSpace(int column, int row)
        {
            // Check out "Textures and Modelling - A Procedural Approach" by Ken Musgaves 

            // We want to build a mesh where
            // the center of the mesh is at 0,0 but the vertices are sphere-projected as though they were out in their
            // correct place in the sphere.  Then we want to keep track of the mesh's real-world location so we can do a
            // camera-relative translation for rendering.

            // We have several different reference frames to deal with here:
            //   "heightfield" coordinates which is the column and row index of the vertex in the mesh grid
            //   "Unit plane" coordinates which is the coordinates of the vertex on the unit plane of this quadtree
            //   "Unit sphere" coordinates which is the coordinates of the vertex on the unit sphere arc of this quadtree
            //   "Planet space" coordinates which is the coordinates of the vertex in real units relative to the planet center
            //   "Mesh space" coordinates which is the coordinates of the vertex in real units after the center point of the mesh
            //   has been translated to the center of the planet

            // We start with quad-relative coordinates.  We first convert the quad coordinates into a unit plane vector that
            // points to the equivalent point on the quadtree's plane.  Then we project the unit plane to its equivalent unit
            // sphere vector.  We then calculate the terrain height for the vertex and use that information to extend the unit
            // sphere vector to the proper length for the real-space size of our planet.  Finally we translate the vector
            // "downward" by the radius so that a zero-height point in the exact middle of the mesh surface would be at the origin.

            var unitPlaneVector = ConvertToUnitPlaneVector(column, row);
            var unitSphereVector = unitPlaneVector.ProjectUnitPlaneToUnitSphere();

            var terrainHeight = _generator.GetHeight(unitSphereVector, Level, 8000);

            var planetSpaceVector = ConvertToPlanetSpace(unitSphereVector, terrainHeight);
            var meshSpaceVector = ConvertToMeshSpace(planetSpaceVector);

            var vertexColor = GetVertexColor(terrainHeight, column, row, _gridSize, _extents);

            return CreateVertex(meshSpaceVector, vertexColor);
        }

        DoubleVector3 ConvertToUnitPlaneVector(int column, int row)
        {
            var uDelta = _uVector * (_extents.North + (row * _meshStride));
            var vDelta = _vVector * (_extents.West + (column * _meshStride));
            var convertedVector = _planeNormalVector + uDelta + vDelta;

            return convertedVector;
        }

        Color GetVertexColor(double terrainHeight, int column, int row, int gridSize, QuadNodeExtents extents)
        {
            var color = GetTerrainColor(terrainHeight);

            if (_settings.ShowQuadBoundaries)
            {
                color = AddBoundaryColor(color, column, row, gridSize, extents);
            }

            return color;
        }

        Color GetTerrainColor(double terrainHeight)
        {
            return terrainHeight <= 0 ? Color.Blue : Color.White;
        }

        Color AddBoundaryColor(Color terrainColor, int column, int row, int gridSize, QuadNodeExtents extents)
        {
            var color = terrainColor;

            if (row == 0)
            {
                color = extents.North == -1 ? Color.Green : Color.Red;
            }
            else if (row == gridSize - 1)
            {
                color = extents.South == 1 ? Color.Green : Color.Red;
            }
            else if (column == 0)
            {
                color = extents.West == -1 ? Color.Green : Color.Red;
            }
            else if (column == gridSize - 1)
            {
                color = extents.East == 1 ? Color.Green : Color.Red;
            }

            return color;
        }

        DoubleVector3 ConvertToPlanetSpace(DoubleVector3 sphereUnitVector, double terrainHeight)
        {
            return sphereUnitVector * (_planetRadius + terrainHeight);
        }

        DoubleVector3 ConvertToMeshSpace(DoubleVector3 planetSpaceVector)
        {
            return planetSpaceVector - _locationRelativeToPlanet;
        }

        VertexPositionNormalColor CreateVertex(DoubleVector3 meshVector, Color terrainColor)
        {
            return new VertexPositionNormalColor { Position = meshVector, Color = terrainColor };
        }

        static void GenerateIndices()
        {
            // We can generate the indices once and share it for all
            // instances since it never changes.  In the future we'll want to deal
            // with adjacent nodes at different levels by constructing special
            // index sets that blend them at the edge.

            // TODO: Right now our indices must be 16 bits because we have to target
            // the XNA 4.0 Reach profile until RTM is released.  After RTM we should
            // be able to push this up to 32 bits if we want to.

            _indices = new short[(_gridSize - 1) * (_gridSize - 1) * 6];
            int counter = 0;
            for (var x = 0; x < _gridSize - 1; x++)
            {
                for (var y = 0; y < _gridSize - 1; y++)
                {
                    var topLeft = (short)(x * _gridSize + y);
                    var lowerLeft = (short)((x + 1) * _gridSize + y);
                    var topRight = (short)(x * _gridSize + (y + 1));
                    var lowerRight = (short)((x + 1) * _gridSize + (y + 1));

                    _indices[counter++] = topLeft;
                    _indices[counter++] = lowerRight;
                    _indices[counter++] = lowerLeft;

                    _indices[counter++] = topLeft;
                    _indices[counter++] = topRight;
                    _indices[counter++] = lowerRight;
                }
            }
        }

        void GenerateNormals()
        {
            CalculateNormals();
            NormalizeNormals();
        }

        void CalculateNormals()
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

        void NormalizeNormals()
        {
            for (int i = 0; i < _vertices.Length; i++)
                _vertices[i].Normal.Normalize();
        }

        public void Update(TimeSpan elapsedTime, DoubleVector3 cameraLocation, DoubleVector3 planetLocation, ClippingPlanes clippingPlanes)
        {
            // TODO: I don't like this class's public member design.  In order to get properties like IsVisibleToCamera,
            // you must first call Update with appropriate information.  Internally, a lot of stuff is also
            // order-dependent.  However, this seems to be the most performant design at the moment.

            var meshDistance = GetDistanceFrom(cameraLocation);
            var distanceFromCamera = meshDistance.ClosestDistance;
            
            WidthToCameraDistanceRatio = distanceFromCamera / WidthInRealSpaceUnits();

            IsVisibleToCamera = CalculateVisibility(cameraLocation, planetLocation, meshDistance.ClosestVertex);

            SetClippingPlanes(meshDistance, clippingPlanes);
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
                if (distanceSquared > furthestDistanceSquared)
                {
                    furthestDistanceSquared = distanceSquared;
                    furthestVertex = vertex;
                }
            }

            // TODO: We're spamming the garbage collector with this.  Allocate one instance per mesh and reuse.
            return new MeshDistance()
            {
                ClosestDistance = Math.Sqrt(closestDistanceSquared),
                ClosestVertex = closestVertex,
                FurthestDistance = Math.Sqrt(furthestDistanceSquared),
                FurthestVertex = furthestVertex
            };
        }

        double WidthInRealSpaceUnits()
        {
            return _extents.Width * _planetRadius;
        }

        bool CalculateVisibility(DoubleVector3 cameraLocation, DoubleVector3 planetLocation, DoubleVector3 closestVertex)
        {
            // TODO: this isn't quite right yet.  At high altitude you can see some popping of distant meshes on
            // the horizon

            var planetToCamera = DoubleVector3.Normalize(cameraLocation - planetLocation);
            var planetToMesh = DoubleVector3.Normalize(closestVertex - planetLocation);

            var horizonAngle = Math.Acos(_planetRadius * 0.99 / DoubleVector3.Distance(planetLocation, cameraLocation));
            var angleToMesh = Math.Acos(DoubleVector3.Dot(planetToCamera, planetToMesh));

            return horizonAngle > angleToMesh;
        }

        void SetClippingPlanes(MeshDistance meshDistance, ClippingPlanes clippingPlanes)
        {
            if (IsVisibleToCamera)
            {
                if (clippingPlanes.Near > meshDistance.ClosestDistance)
                {
                    clippingPlanes.Near = meshDistance.ClosestDistance;
                }
                if (clippingPlanes.Far < meshDistance.FurthestDistance)
                {
                    clippingPlanes.Far = meshDistance.FurthestDistance;
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

