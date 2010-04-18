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
        private VertexDeclaration _vertexDeclaration;
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        private int _numberOfVertices;
        private int _numberOfIndices;
        private Effect _effect;
        private ISettings _settings;

        public QuadMeshRenderer(ContentManager contentManager, GraphicsDevice graphicsDevice, ISettings settings)
        {
            _contentManager = contentManager;
            _graphicsDevice = graphicsDevice;
            _settings = settings;
        }

        public void Initialize(VertexPositionNormalColored[] vertices, int[] indices)
        {
            _vertexDeclaration = new VertexDeclaration(_graphicsDevice, VertexPositionNormalColored.VertexElements);

            _effect = _contentManager.Load<Effect>("effects");

            CreateVertexBuffer(vertices);
            CreateIndexBuffer(indices);
        }

        private void CreateVertexBuffer(VertexPositionNormalColored[] vertices)
        {
            _numberOfVertices = vertices.Length;
            _vertexBuffer = new VertexBuffer(_graphicsDevice, _numberOfVertices * VertexPositionNormalColored.SizeInBytes, BufferUsage.WriteOnly);
            _vertexBuffer.SetData(vertices);
        }

        private void CreateIndexBuffer(int[] indices)
        {
            // TODO: according to the _Interactive Visualization_ paper, we should be able to pre-generate
            // all possible index buffers once (tweaked on the edges to match lower LOD neighbors) and reuse
            // them for all nodes.

            _numberOfIndices = indices.Length;
            _indexBuffer = new IndexBuffer(_graphicsDevice, typeof(int), _numberOfIndices, BufferUsage.WriteOnly);
            _indexBuffer.SetData(indices);
        }

        public void Draw(DoubleVector3 location, DoubleVector3 cameraLocation, Matrix originBasedViewMatrix, Matrix projectionMatrix)
        {
            // TODO: Calculate a dynamic scaling factor based on the distance of the object from the camera?  See
            // the Interactive Visualization paper, page 24.

            Matrix translationMatrix = Matrix.CreateTranslation(location - cameraLocation);
            Matrix worldMatrix = translationMatrix;

            _effect.CurrentTechnique = _effect.Techniques["Colored"];
            _effect.Parameters["xView"].SetValue(originBasedViewMatrix);
            _effect.Parameters["xProjection"].SetValue(projectionMatrix);
            _effect.Parameters["xWorld"].SetValue(worldMatrix);

            _effect.Parameters["xEnableLighting"].SetValue(true);
            Vector3 lightDirection = new Vector3(1.0f, -1.0f, -1.0f);
            lightDirection.Normalize();
            _effect.Parameters["xLightDirection"].SetValue(lightDirection);
            _effect.Parameters["xAmbient"].SetValue(0.1f);

            if (_settings.ShouldDrawWireframe)
            {
                _graphicsDevice.RenderState.FillMode = FillMode.WireFrame;
            }
            else
            {
                _graphicsDevice.RenderState.FillMode = FillMode.Solid;
            }

            _effect.Begin();
            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Begin();

                _graphicsDevice.VertexDeclaration = _vertexDeclaration;
                _graphicsDevice.Indices = _indexBuffer;
                _graphicsDevice.Vertices[0].SetSource(_vertexBuffer, 0, VertexPositionNormalColored.SizeInBytes);
                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _numberOfVertices, 0, _numberOfIndices / 3);

                pass.End();
            }
            _effect.End();
        }

        public void Dispose()
        {
            _vertexDeclaration.Dispose();
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();

            // Don't dispose _effect here because the ContentManager gives us a shared instance
        }
    }
}
