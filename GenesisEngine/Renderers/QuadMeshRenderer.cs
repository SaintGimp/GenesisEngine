using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GenesisEngine
{
    public class QuadMeshRenderer : IQuadMeshRenderer, IDisposable
    {
        readonly GraphicsDevice _graphicsDevice;
        readonly BasicEffect _effect;
        readonly ISettings _settings;
        readonly Statistics _statistics;

        VertexBuffer _vertexBuffer;
        static IndexBuffer _indexBuffer;
        int _numberOfVertices;
        static int _numberOfIndices;
        BoundingBox _boundingBox;

        public QuadMeshRenderer(GraphicsDevice graphicsDevice, BasicEffect effect, ISettings settings, Statistics statistics)
        {
            _graphicsDevice = graphicsDevice;
            _effect = effect;
            _settings = settings;
            _statistics = statistics;
        }

        public void Initialize(VertexPositionNormalColor[] vertices, short[] indices, BoundingBox boundingBox)
        {
            CreateVertexBuffer(vertices);
            CreateIndexBuffer(indices);
            _boundingBox = boundingBox;
        }

        private void CreateVertexBuffer(VertexPositionNormalColor[] vertices)
        {
            _numberOfVertices = vertices.Length;
            _vertexBuffer = new VertexBuffer(_graphicsDevice, typeof(VertexPositionNormalColor), _numberOfVertices, BufferUsage.WriteOnly);
            _vertexBuffer.SetData(vertices);
        }

        private void CreateIndexBuffer(short[] indices)
        {
            // Right now all of our index buffers are exactly identical so we'll create just one to be shared across
            // all instances
            if (_indexBuffer == null)
            {
                _numberOfIndices = indices.Length;
                _indexBuffer = new IndexBuffer(_graphicsDevice, IndexElementSize.SixteenBits, _numberOfIndices, BufferUsage.WriteOnly);
                _indexBuffer.SetData(indices);
            }
        }

        public void Draw(DoubleVector3 location, DoubleVector3 cameraLocation, Matrix originBasedViewMatrix, Matrix projectionMatrix)
        {
            _effect.View = originBasedViewMatrix;
            _effect.Projection = projectionMatrix;
            _effect.World = GetWorldMatrix(location, cameraLocation);

            // TODO: we shouldn't have to set all of these every time - could be optimized
            SetLightingEffects();
            SetFogEffects();
            SetFillMode();

            DrawMesh();
            if (_settings.ShouldDrawMeshBoundingBoxes)
            {
                DrawBoundingBox();
            }

            _statistics.NumberOfQuadMeshesRendered++;
        }

        Matrix GetWorldMatrix(DoubleVector3 location, DoubleVector3 cameraLocation)
        {
            // The mesh stored in the vertex buffer is centered at the origin in order to take it easy on
            // the float number system.  The camera view matrix is also generated as though the camera were
            // at the origin.  In order to correctly render the mesh we translate it away from the origin
            // by the same vector that the mesh (in double space) is displaced from the camera (in double space).

            // We also translate and scale distant meshes to bring them inside the far clipping plane.  For
            // every mesh that's further than the start of the scaled space, we calcuate a new distance
            // using an exponential downscale function to make it fall in the view frustum.  We also scale
            // it down proportionally so that it appears perspective-wise to be identical to the original
            // location.  See the Interactive Visualization paper, page 24.

            Matrix scaleMatrix;
            Matrix translationMatrix;

            var locationRelativeToCamera = location - cameraLocation;
            var distanceFromCamera = locationRelativeToCamera.Length();
            var unscaledViewSpace = _settings.FarClippingPlaneDistance * 0.25;

            if (distanceFromCamera > unscaledViewSpace)
            {
                var scaledViewSpace = _settings.FarClippingPlaneDistance - unscaledViewSpace;
                double scaledDistanceFromCamera = unscaledViewSpace + (scaledViewSpace * (1.0 - Math.Exp((scaledViewSpace - distanceFromCamera) / 1000000000)));
                var scaledLocationRelativeToCamera = DoubleVector3.Normalize(locationRelativeToCamera) * scaledDistanceFromCamera;
                
                scaleMatrix = Matrix.CreateScale((float)(scaledDistanceFromCamera / distanceFromCamera));
                translationMatrix = Matrix.CreateTranslation(scaledLocationRelativeToCamera);
            }
            else
            {
                scaleMatrix = Matrix.Identity;
                translationMatrix = Matrix.CreateTranslation(locationRelativeToCamera);
            }
            
            return scaleMatrix * translationMatrix;
        }

        void SetLightingEffects()
        {
            _effect.VertexColorEnabled = true;
            _effect.PreferPerPixelLighting = true;

            _effect.DirectionalLight0.DiffuseColor = Vector3.One;
            _effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1.0f, -1.0f, -1.0f));
            _effect.AmbientLightColor = new Vector3(0.1f, 0.1f, 0.1f);
            _effect.LightingEnabled = true;
        }

        void SetFogEffects()
        {
            _effect.FogEnabled = false;
            _effect.FogColor = new Vector3(0.5f, 0.5f, 0.6f);
            _effect.FogStart = 0f;
            _effect.FogEnd = 300000f;
        }

        void SetFillMode()
        {
            if (_settings.ShouldDrawWireframe && _graphicsDevice.RasterizerState.FillMode != FillMode.WireFrame)
            {
                _graphicsDevice.RasterizerState = new RasterizerState() { FillMode = FillMode.WireFrame };
            }
            else if (!_settings.ShouldDrawWireframe && _graphicsDevice.RasterizerState.FillMode != FillMode.Solid)
            {
                _graphicsDevice.RasterizerState = new RasterizerState() { FillMode = FillMode.Solid };
            }
        }

        void DrawMesh()
        {
            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                _graphicsDevice.Indices = _indexBuffer;
                _graphicsDevice.SetVertexBuffer(_vertexBuffer);
                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _numberOfVertices, 0, _numberOfIndices / 3);
            }
        }

        void DrawBoundingBox()
        {
            // TODO: set this stuff up once
            var vertices = new VertexPositionNormalColor[8];
            var indices = new short[]
            {
                0, 1,
                1, 2,
                2, 3,
                3, 0,
                0, 4,
                1, 5,
                2, 6,
                3, 7,
                4, 5,
                5, 6,
                6, 7,
                7, 4,
            };

            Vector3[] corners = _boundingBox.GetCorners();
            for (int i = 0; i < 8; i++)
            {
                vertices[i].Position = corners[i];
                vertices[i].Color = Color.DarkGreen;
                vertices[i].Normal = Vector3.Up;
            }

            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                _graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineList, vertices, 0, 8, indices, 0, indices.Length / 2);
            }
        }

        public void Dispose()
        {
            _vertexBuffer.Dispose();
        }
    }
}
