using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GenesisEngine
{
    public class QuadMeshRenderer : IQuadMeshRenderer, IDisposable
    {
        private ContentManager _contentManager;
        private GraphicsDevice _graphicsDevice;
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        private int _numberOfVertices;
        private int _numberOfIndices;
        private BasicEffect _effect;
        private ISettings _settings;

        public QuadMeshRenderer(ContentManager contentManager, GraphicsDevice graphicsDevice, ISettings settings)
        {
            _contentManager = contentManager;
            _graphicsDevice = graphicsDevice;
            _settings = settings;
        }

        public void Initialize(VertexPositionNormalColor[] vertices, short[] indices)
        {
            _effect = new BasicEffect(_graphicsDevice);

            CreateVertexBuffer(vertices);
            CreateIndexBuffer(indices);
        }

        private void CreateVertexBuffer(VertexPositionNormalColor[] vertices)
        {
            _numberOfVertices = vertices.Length;
            _vertexBuffer = new VertexBuffer(_graphicsDevice, typeof(VertexPositionNormalColor), _numberOfVertices, BufferUsage.WriteOnly);
            _vertexBuffer.SetData(vertices);
        }

        private void CreateIndexBuffer(short[] indices)
        {
            // TODO: according to the _Interactive Visualization_ paper, we should be able to pre-generate
            // all possible index buffers once (tweaked on the edges to match lower LOD neighbors) and reuse
            // them for all nodes.

            _numberOfIndices = indices.Length;
            _indexBuffer = new IndexBuffer(_graphicsDevice, IndexElementSize.SixteenBits, _numberOfIndices, BufferUsage.WriteOnly);
            _indexBuffer.SetData(indices);
        }

        public void Draw(DoubleVector3 location, DoubleVector3 cameraLocation, Matrix originBasedViewMatrix, Matrix projectionMatrix)
        {
            // TODO: Calculate a dynamic scaling factor based on the distance of the object from the camera?  See
            // the Interactive Visualization paper, page 24.

            Matrix translationMatrix = Matrix.CreateTranslation(location - cameraLocation);
            Matrix worldMatrix = translationMatrix;

            _effect.View = originBasedViewMatrix;
            _effect.Projection = projectionMatrix;
            _effect.World = worldMatrix;

            _effect.VertexColorEnabled = true;
            _effect.PreferPerPixelLighting = true;

            _effect.FogEnabled = false;
            _effect.FogColor = new Vector3(0.5f, 0.5f, 0.6f);
            _effect.FogStart = 0f;
            _effect.FogEnd = 300000f;

            _effect.DirectionalLight0.DiffuseColor = Vector3.One;
            _effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1.0f, -1.0f, -1.0f));
            _effect.AmbientLightColor = new Vector3(0.1f, 0.1f, 0.1f);
            _effect.LightingEnabled = true;

            if (_settings.ShouldDrawWireframe)
            {
                if (_graphicsDevice.RasterizerState.FillMode != FillMode.WireFrame)
                {
                    _graphicsDevice.RasterizerState = new RasterizerState() { FillMode = FillMode.WireFrame };
                }
            }
            else
            {
                if (_graphicsDevice.RasterizerState.FillMode != FillMode.Solid)
                {
                    _graphicsDevice.RasterizerState = new RasterizerState() { FillMode = FillMode.Solid };
                }
            }

            _effect.CurrentTechnique.Passes[0].Apply(); 
            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                _graphicsDevice.Indices = _indexBuffer;
                _graphicsDevice.SetVertexBuffer(_vertexBuffer);
                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _numberOfVertices, 0, _numberOfIndices / 3);
            }
        }

        public void Dispose()
        {
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();

            // Don't dispose _effect here because the ContentManager gives us a shared instance
        }
    }
}
