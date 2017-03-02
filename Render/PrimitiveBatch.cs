using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MonUX.Render
{
    public class PrimitiveBatch
    {
        private const int NUM_CACHED_LINES = 512;
        private const int NUM_CACHED_TRIS = 512;
        private const int NUM_CACHED_TEX_TRIS = 512;

        private int myLineCount;
        private int myTriCount;
        private int myTexTriCount;

        private VertexPositionColorTexture[] myLineVerts;
        private VertexPositionColorTexture[] myTriVerts;
        private VertexPositionColorTexture[] myTextTriVerts;

        private VertexBuffer myLineVertexBuffer;
        private VertexBuffer myTriVertexBuffer;
        private VertexBuffer myTexTriVertexBuffer;

        private GraphicsDevice myGraphics;


        private PrimitiveEffect myPrimitiveEffect;

        public PrimitiveBatch(GraphicsDevice graphics)
        {
            myGraphics = graphics;

            myLineVerts = new VertexPositionColorTexture[NUM_CACHED_LINES * 2];
            myTriVerts = new VertexPositionColorTexture[NUM_CACHED_TRIS * 3];
            myTextTriVerts = new VertexPositionColorTexture[NUM_CACHED_TEX_TRIS * 3];

            myLineVertexBuffer = new VertexBuffer(myGraphics, VertexPositionColorTexture.VertexDeclaration, myLineVerts.Length, BufferUsage.WriteOnly);
            myTriVertexBuffer = new VertexBuffer(myGraphics, VertexPositionColorTexture.VertexDeclaration, myTriVerts.Length, BufferUsage.WriteOnly);
            myTexTriVertexBuffer = new VertexBuffer(myGraphics, VertexPositionColorTexture.VertexDeclaration, myTextTriVerts.Length, BufferUsage.WriteOnly);

            myPrimitiveEffect = new PrimitiveEffect();
        }

        public void DrawQuad(Rectangle bounds, Color color)
        {
            __AddTri(bounds.Left, bounds.Top, bounds.Right, bounds.Top, bounds.Left, bounds.Bottom, color);
            __AddTri(bounds.Right, bounds.Top, bounds.Right, bounds.Bottom, bounds.Left, bounds.Bottom, color);
        }

        protected void __AddTri(float x1, float y1, float x2, float y2, float x3, float y3, Color color)
        {
            myTriVerts[myTriCount * 3 + 0].Position.X = x1;
            myTriVerts[myTriCount * 3 + 0].Position.Y = y1;
            myTriVerts[myTriCount * 3 + 0].Color = color;

            myTriVerts[myTriCount * 3 + 1].Position.X = x2;
            myTriVerts[myTriCount * 3 + 1].Position.Y = y2;
            myTriVerts[myTriCount * 3 + 1].Color = color;

            myTriVerts[myTriCount * 3 + 2].Position.X = x3;
            myTriVerts[myTriCount * 3 + 2].Position.Y = y3;
            myTriVerts[myTriCount * 3 + 2].Color = color;

            myTriCount++;

            if (myTriCount * 3 == myTriVerts.Length)
                __FlushTris();
        }

        public void Begin()
        {
            myLineCount = 0;
            myTriCount = 0;
            myTexTriCount = 0;
        }

        public void End()
        {
            __FlushAll();
        }

        private void __FlushAll()
        {
            __FlushLines();
            __FlushTris();
            __FlushTexTris();
        }

        private void __FlushLines()
        {
            if (myLineCount > 0)
            {
                myPrimitiveEffect.TextureEnabled = false;
                myPrimitiveEffect.CurrentTechnique.Passes[0].Apply();

                myGraphics.BlendState = BlendState.AlphaBlend;

                myLineVertexBuffer.SetData(myLineVerts);
                myGraphics.SetVertexBuffer(myLineVertexBuffer);
                myGraphics.DrawPrimitives(PrimitiveType.LineList, 0, myLineCount);
                myLineCount = 0;
            }
        }

        private void __FlushTris()
        {
            if (myTriCount > 0)
            {
                myPrimitiveEffect.TextureEnabled = false;
                myPrimitiveEffect.CurrentTechnique.Passes[0].Apply();

                myGraphics.BlendState = BlendState.AlphaBlend;

                myTriVertexBuffer.SetData(myTriVerts);
                myGraphics.SetVertexBuffer(myTriVertexBuffer);
                myGraphics.DrawPrimitives(PrimitiveType.TriangleList, 0, myTriCount);
                myTriCount = 0;
            }
        }

        private void __FlushTexTris()
        {
            if (myTexTriCount > 0)
            {
                myPrimitiveEffect.TextureEnabled = true;
                myPrimitiveEffect.CurrentTechnique.Passes[0].Apply();

                myGraphics.BlendState = BlendState.AlphaBlend;

                myTexTriVertexBuffer.SetData(myTextTriVerts);
                myGraphics.SetVertexBuffer(myTexTriVertexBuffer);
                myGraphics.DrawPrimitives(PrimitiveType.TriangleList, 0, myTexTriCount);
                myTexTriCount = 0;
            }
        }
    }
}
