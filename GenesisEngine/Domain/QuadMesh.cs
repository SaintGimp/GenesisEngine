using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

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
        protected QuadNodeExtents _extents;

        static short[] _indices;
        DoubleVector3[] _planetSpaceVertexSamples;
        BoundingBox _boundingBox;

        readonly IHeightmapGenerator _generator;
        readonly ITerrainColorizer _terrainColorizer;
        readonly IQuadMeshRenderer _renderer;
        readonly ISettings _settings;

        public QuadMesh(IHeightmapGenerator generator, ITerrainColorizer terrainColorizer, IQuadMeshRenderer renderer, ISettings settings)
        {
            _generator = generator;
            _terrainColorizer = terrainColorizer;
            _renderer = renderer;
            _settings = settings;
        }

        public bool IsAboveHorizonToCamera { get; private set; }

        public double CameraDistanceToWidthRatio { get; private set; }

        // TODO: push this data in through the constructor, probably in a QuadMeshDefintion class, and make
        // this method private.  Except that would do real work in construction.  Hmmm.
        public void Initialize(double planetRadius, DoubleVector3 planeNormalVector, DoubleVector3 uVector, DoubleVector3 vVector, QuadNodeExtents extents, int level)
        {
            _planetRadius = planetRadius;
            _extents = extents;

            // TODO: get this from the QuadNode instead
            _locationRelativeToPlanet = (planeNormalVector) + (uVector * (_extents.North + (_extents.Width / 2.0))) + (vVector * (_extents.West + (_extents.Width / 2.0)));
            _locationRelativeToPlanet = _locationRelativeToPlanet.ProjectUnitPlaneToUnitSphere() * _planetRadius;

            // TODO: cover this in specs
            _boundingBox.Min = new Vector3(float.MaxValue);
            _boundingBox.Max = new Vector3(float.MinValue);

            GenerateIndices();
            var heightmap = _generator.GenerateHeightmapSamples(new HeightmapDefinition()
            {
                GridSize = _gridSize,
                Stride = extents.Width / (_gridSize - 1),
                PlaneNormalVector = planeNormalVector,
                UVector = uVector,
                VVector = vVector,
                Extents = _extents,
                QuadLevel = level,
                PlanetRadius = planetRadius
            });
            var vertices = GenerateMeshVertices(heightmap);
            CollectHeightmapSamples(heightmap);

            _renderer.Initialize(vertices, _indices, _boundingBox);
        }

        VertexPositionNormalColor[] GenerateMeshVertices(HeightmapSample[] heightmapSamples)
        {
            var vertices = new VertexPositionNormalColor[_gridSize * _gridSize];

            for (int row = 0; row < _gridSize; row++)
            {
                for (int column = 0; column < _gridSize; column++)
                {
                    var vertex = GetVertexInMeshSpace(heightmapSamples[row * _gridSize + column], column, row);
                    AdjustBoundingBoxToInclude(vertex.Position);
                    vertices[row * _gridSize + column] = vertex;
                }
            }

            GenerateNormals(vertices);

            return vertices;
        }

        void AdjustBoundingBoxToInclude(Vector3 vertex)
        {
            Vector3.Min(ref vertex, ref _boundingBox.Min, out _boundingBox.Min);
            Vector3.Max(ref vertex, ref _boundingBox.Max, out _boundingBox.Max);
        }

        void CollectHeightmapSamples(HeightmapSample[] heightmapSamples)
        {
            // We just take a few samples for now
            _planetSpaceVertexSamples = new []
            {
                heightmapSamples[0].Vector,  // Upper left
                heightmapSamples[_gridSize / 2].Vector,  // Upper middle
                heightmapSamples[_gridSize - 1].Vector,  // Upper right
                heightmapSamples[(_gridSize / 2) * _gridSize].Vector,  // Middle left
                heightmapSamples[heightmapSamples.Length / 2].Vector, // Middle
                heightmapSamples[(_gridSize / 2) * _gridSize + _gridSize - 1].Vector,  // Middle right
                heightmapSamples[_gridSize * (_gridSize - 1)].Vector, // Lower left
                heightmapSamples[_gridSize * (_gridSize - 1) + _gridSize / 2].Vector, // Lower middle
                heightmapSamples[_gridSize * _gridSize - 1].Vector  // Lower right
            };
        }

        VertexPositionNormalColor GetVertexInMeshSpace(HeightmapSample heightmapSample, int column, int row)
        {
            // We have two different reference frames to deal with here:
            //   "Planet space" coordinates which is the coordinates of the vertex in real units relative to the planet center
            //   "Mesh space" coordinates which is the coordinates of the vertex in real units after the center point of the mesh
            //   has been translated to the center of the planet

            // We want to build a mesh where the center of the mesh is at 0,0 but the vertices are sphere-projected as though
            // they were out in their correct place in the sphere.  Then we want to keep track of the mesh's real-world location
            // so we can do a camera-relative translation for rendering.

            // We start with a heightmap height and vector which is in planet space.  We translate the vector "downward" by the
            // radius of the planet so that a zero-height point in the exact middle of the mesh surface would be at the origin.

            var meshSpaceVector = ConvertToMeshSpace(heightmapSample.Vector);
            var vertexColor = _terrainColorizer.GetColor(heightmapSample.Height, column, row, _gridSize, _extents);

            return new VertexPositionNormalColor { Position = meshSpaceVector, Color = vertexColor };
        }

        DoubleVector3 ConvertToMeshSpace(DoubleVector3 planetSpaceVector)
        {
            return planetSpaceVector - _locationRelativeToPlanet;
        }

        static void GenerateIndices()
        {
            // TODO: Right now our indices must be 16 bits because we have to target
            // the XNA 4.0 Reach profile until RTM is released.  After RTM we should
            // be able to push this up to 32 bits if we want to.

            // We can generate the indices once and share it for all
            // instances since it never changes.
            // TODO: In the future we'll want to deal
            // with adjacent nodes at different levels by constructing special
            // index sets that blend them at the edge as in the _Interactive Visualization_ paper

            if (_indices == null)
            {
                _indices = new short[(_gridSize - 1) * (_gridSize - 1) * 6];
                int counter = 0;
                for (var x = 0; x < _gridSize - 1; x++)
                {
                    for (var y = 0; y < _gridSize - 1; y++)
                    {
                        var topLeft = (short) (x * _gridSize + y);
                        var lowerLeft = (short) ((x + 1) * _gridSize + y);
                        var topRight = (short) (x * _gridSize + (y + 1));
                        var lowerRight = (short) ((x + 1) * _gridSize + (y + 1));

                        _indices[counter++] = topLeft;
                        _indices[counter++] = lowerRight;
                        _indices[counter++] = lowerLeft;

                        _indices[counter++] = topLeft;
                        _indices[counter++] = topRight;
                        _indices[counter++] = lowerRight;
                    }
                }
            }
        }

        void GenerateNormals(VertexPositionNormalColor[] vertices)
        {
            CalculateNormals(vertices);
            NormalizeNormals(vertices);
        }

        void CalculateNormals(VertexPositionNormalColor[] vertices)
        {
            // Iterate through each indexed vertex and gradually
            // accumulate the normals in the vertices as we go.

            for (int i = 0; i < _indices.Length / 3; i++)
            {
                int index1 = _indices[i * 3];
                int index2 = _indices[i * 3 + 1];
                int index3 = _indices[i * 3 + 2];

                Vector3 side1 = vertices[index1].Position - vertices[index3].Position;
                Vector3 side2 = vertices[index1].Position - vertices[index2].Position;
                Vector3 normal = Vector3.Cross(side1, side2);

                vertices[index1].Normal += normal;
                vertices[index2].Normal += normal;
                vertices[index3].Normal += normal;
            }
        }

        void NormalizeNormals(VertexPositionNormalColor[] vertices)
        {
            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal.Normalize();
        }

        public void Update(DoubleVector3 cameraLocation, DoubleVector3 planetLocation)
        {
            // TODO: I don't like this class's public member design.  In order to get properties like IsVisibleToCamera,
            // you must first call Update with appropriate information.  Internally, a lot of stuff is also
            // order-dependent.  However, this seems to be the most performant design at the moment.

            var meshDistance = GetDistanceFrom(cameraLocation);
            var distanceFromCamera = meshDistance.ClosestDistance;
            
            CameraDistanceToWidthRatio = distanceFromCamera / WidthInRealSpaceUnits();

            IsAboveHorizonToCamera = CalculateIsAboveHorizonToCamera(cameraLocation, planetLocation, meshDistance.ClosestVertex);
        }

        MeshDistance GetDistanceFrom(DoubleVector3 location)
        {
            // TODO: This method contributes a significant portion of the CPU cost of an update sweep.
            // Can we simplify it somehow, maybe by calculating distance based on the center of the
            // bounding box or something?  For calculating horizon visibility, it might be both faster
            // and better to pre-calc the vertex with the highest altitude and use that, rather than the
            // closest vertex.

            double closestDistanceSquared = double.MaxValue;
            DoubleVector3 closestVertex = _planetSpaceVertexSamples[0];

            foreach (var vertex in _planetSpaceVertexSamples)
            {
                var distanceSquared = DoubleVector3.DistanceSquared(location, vertex);
                if (distanceSquared < closestDistanceSquared)
                {
                    closestDistanceSquared = distanceSquared;
                    closestVertex = vertex;
                }
            }

            // TODO: We're spamming the garbage collector with this.  Allocate one instance per mesh and reuse.
            return new MeshDistance
            {
                ClosestDistance = Math.Sqrt(closestDistanceSquared),
                ClosestVertex = closestVertex,
            };
        }

        double WidthInRealSpaceUnits()
        {
            return _extents.Width * _planetRadius;
        }

        bool CalculateIsAboveHorizonToCamera(DoubleVector3 cameraLocation, DoubleVector3 planetLocation, DoubleVector3 closestVertex)
        {
            // Taken from http://www.crappycoding.com/2009/04/

            // TODO: This algorithm is poor.  Implement this algorithm instead:
            // http://www.gamedev.net/community/forums/mod/journal/journal.asp?jn=263350&reply_id=3173799

            // TODO: sometimes the mesh is so large and we're so close the surface that none of the sampled
            // vertices are above the horizon.  That causes problems when we want to do early termination
            // when we do a draw walk on the QuadNode tree (see comments there).  Can we add a test here to
            // see if we're inside the mesh's bounding box?

            var planetToCamera = DoubleVector3.Normalize(cameraLocation - planetLocation);
            var planetToMesh = DoubleVector3.Normalize(closestVertex - planetLocation);

            var horizonAngle = Math.Acos(_planetRadius * 0.99 / DoubleVector3.Distance(planetLocation, cameraLocation));
            var angleToMesh = Math.Acos(DoubleVector3.Dot(planetToCamera, planetToMesh));

            return horizonAngle > angleToMesh;
        }

        public void Draw(DoubleVector3 cameraLocation, BoundingFrustum originBasedViewFrustum, Matrix originBasedViewMatrix, Matrix projectionMatrix)
        {
            // TODO: better spec coverage here
            if (IsAboveHorizonToCamera && IsVisibleToCamera(cameraLocation, originBasedViewFrustum))
            {
                _renderer.Draw(_locationRelativeToPlanet, cameraLocation, originBasedViewMatrix, projectionMatrix);
            }
        }

        bool IsVisibleToCamera(DoubleVector3 cameraLocation, BoundingFrustum originBasedViewFrustum)
        {
            var locationRelativeToCamera = _locationRelativeToPlanet - cameraLocation;
            var translatedBoundingBox = new BoundingBox(_boundingBox.Min + (Vector3)locationRelativeToCamera,
                                                        _boundingBox.Max + (Vector3)locationRelativeToCamera);

            return IsInViewFrustumWithNoFarClipping(translatedBoundingBox, originBasedViewFrustum);
        }

        bool IsInViewFrustumWithNoFarClipping(BoundingBox box, BoundingFrustum viewFrustum)
        {
            // TODO: this causes a lot of allocations - maybe use a extension method with custom iterator?
            // Actually, the lambda below also generates allocations.  Maybe unroll this?  Not sure what
            // perf improvement that would have, if any
            var planes = new [] { viewFrustum.Near, viewFrustum.Left, viewFrustum.Right, viewFrustum.Top, viewFrustum.Bottom};

            return planes.All(plane => box.Intersects(plane) != PlaneIntersectionType.Front);
        }

        public void Dispose()
        {
            ((IDisposable)_renderer).Dispose();
        }

        private struct MeshDistance
        {
            public DoubleVector3 ClosestVertex { get; set; }
            public double ClosestDistance { get; set; }
        }
    }
}

