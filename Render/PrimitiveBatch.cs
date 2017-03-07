using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonUX.Interface;

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

        public void DrawOutline(Rectangle bounds, Color color, bool inner, float thickness = 1)
        {
            if (thickness <= 1)
            {
                if (inner)
                {
                    __AddLine(bounds.Left - 1,  bounds.Top,    bounds.Right, bounds.Top,    color);
                    __AddLine(bounds.Left,      bounds.Bottom, bounds.Right, bounds.Bottom, color);
                    __AddLine(bounds.Left,      bounds.Top,    bounds.Left,  bounds.Bottom, color);
                    __AddLine(bounds.Right,     bounds.Top,    bounds.Right, bounds.Bottom, color);
                }
                else
                {
                    __AddLine(bounds.Left  - 2, bounds.Top    - 1, bounds.Right + 1, bounds.Top    - 1, color);
                    __AddLine(bounds.Left  - 1, bounds.Bottom + 1, bounds.Right + 1, bounds.Bottom + 1, color);
                    __AddLine(bounds.Left  - 1, bounds.Top    - 1, bounds.Left  - 1, bounds.Bottom + 1, color);
                    __AddLine(bounds.Right + 1, bounds.Top    - 1, bounds.Right + 1, bounds.Bottom + 1, color);
                }
            }
            else
            {
                float leftOuter, leftInner, rightOuter, rightInner;
                float topOuter, topInner, bottomOuter, bottomInner;

                if (inner)
                {
                    leftOuter   = bounds.Left;
                    leftInner   = bounds.Left + thickness;

                    rightInner  = bounds.Right - thickness;
                    rightOuter  = bounds.Right;

                    topOuter    = bounds.Top;
                    topInner    = bounds.Top + thickness;

                    bottomOuter = bounds.Bottom;
                    bottomInner = bounds.Bottom - thickness;
                }
                else
                {
                    leftOuter = bounds.Left;
                    leftInner = bounds.Left + thickness;

                    rightInner = bounds.Right - thickness;
                    rightOuter = bounds.Right;

                    topOuter = bounds.Top;
                    topInner = bounds.Top + thickness;

                    bottomOuter = bounds.Bottom;
                    bottomInner = bounds.Bottom - thickness;
                }

                __AddTri(leftOuter, topOuter,    leftInner, topOuter, leftOuter, bottomOuter, color);
                __AddTri(leftOuter, bottomOuter, leftInner, topOuter, leftInner, bottomOuter, color);

                __AddTri(rightInner, topOuter,    rightOuter, topOuter, rightInner, bottomOuter, color);
                __AddTri(rightInner, bottomOuter, rightOuter, topOuter, rightOuter, bottomOuter, color);

                __AddTri(leftInner, topOuter, rightInner, topOuter, leftInner, topInner, color);
                __AddTri(leftInner, topInner, rightInner, topOuter, rightInner, topInner, color);

                __AddTri(leftInner, bottomInner, rightInner, bottomInner, leftInner,  bottomOuter, color);
                __AddTri(leftInner, bottomOuter, rightInner, bottomInner, rightInner, bottomOuter, color);
            }
        }

        public void DrawQuad(Rectangle bounds, Color color)
        {
            __AddTri(bounds.Left, bounds.Top, bounds.Right, bounds.Top, bounds.Left, bounds.Bottom, color);
            __AddTri(bounds.Right, bounds.Top, bounds.Right, bounds.Bottom, bounds.Left, bounds.Bottom, color);
        }

        public void DrawRoundedOutline(Rectangle bounds, Color color, float cornerRadius, bool inner, float thickness = 1, int sampleCount = 4)
        {
            if (cornerRadius * 2 > bounds.Width || cornerRadius * 2 > bounds.Height)
                throw new ArgumentException("Corner diameter cannot exceed size of bounds edge lengths");

            float leftOuter, rightOuter, topOuter, bottomOuter;
            float leftInner, rightInner, topInner, bottomInner;

            leftInner = bounds.Left + cornerRadius;
            rightInner = bounds.Right - cornerRadius;
            topInner = bounds.Top + cornerRadius;
            bottomInner = bounds.Bottom - cornerRadius;

            leftOuter = bounds.Left;
            rightOuter = bounds.Right;
            topOuter = bounds.Top;
            bottomOuter = bounds.Bottom;

            if (!inner)
            {
                leftInner -= thickness;
                rightInner += thickness;
                topInner -= thickness;
                bottomInner += thickness;

                leftOuter -= thickness;
                rightOuter += thickness;
                topOuter -= thickness;
                bottomOuter += thickness;
            }

            if (thickness <= 1)
            {
                __AddLine(leftOuter, bottomInner, leftOuter, topInner, color);
                __AddLine(rightOuter, bottomInner, rightOuter, topInner, color);
                __AddLine(leftInner, topOuter, rightInner, topOuter, color);
                __AddLine(leftInner, bottomOuter, rightInner, bottomOuter, color);

                float theta = MathHelper.PiOver2 / sampleCount;

                for (int index = 0; index < sampleCount; index++)
                {
                    float lx = (float)Math.Cos(theta * index) * cornerRadius;
                    float ly = (float)Math.Sin(theta * index) * cornerRadius;
                    float rx = (float)Math.Cos(theta * (index + 1)) * cornerRadius;
                    float ry = (float)Math.Sin(theta * (index + 1)) * cornerRadius;

                    __AddLine(leftInner - lx, topInner - ly, leftInner - rx, topInner - ry, color);
                    __AddLine(rightInner + rx, topInner - ry, rightInner + lx, topInner - ly, color);

                    __AddLine(leftInner - rx, bottomInner + ry, leftInner - lx, bottomInner + ly, color);
                    __AddLine(rightInner + lx, bottomInner + ly, rightInner + rx, bottomInner + ry, color);
                }
            }
            else
            {
                __AddTri(leftOuter, topInner,    leftOuter + thickness, topInner, leftOuter,             bottomInner, color);
                __AddTri(leftOuter, bottomInner, leftOuter + thickness, topInner, leftOuter + thickness, bottomInner, color);

                __AddTri(rightOuter - thickness, topInner,    rightOuter, topInner, rightOuter - thickness, bottomInner, color);
                __AddTri(rightOuter - thickness, bottomInner, rightOuter, topInner, rightOuter,             bottomInner, color);

                __AddTri(leftInner, topOuter,             rightInner, topOuter, leftInner, topOuter + thickness, color);
                __AddTri(leftInner, topOuter + thickness, rightInner, topOuter, rightInner, topOuter + thickness, color);

                __AddTri(leftInner, bottomOuter - thickness, rightInner, bottomOuter - thickness, leftInner,  bottomOuter, color);
                __AddTri(leftInner, bottomOuter,             rightInner, bottomOuter - thickness, rightInner, bottomOuter, color);

                float theta = MathHelper.PiOver2 / sampleCount;

                for (int index = 0; index < sampleCount; index++)
                {
                    float lx = (float)Math.Cos(theta * index) * cornerRadius;
                    float ly = (float)Math.Sin(theta * index) * cornerRadius;
                    float rx = (float)Math.Cos(theta * (index + 1)) * cornerRadius;
                    float ry = (float)Math.Sin(theta * (index + 1)) * cornerRadius;

                    float lx2 = (float)Math.Cos(theta * index) * (cornerRadius - thickness);
                    float ly2 = (float)Math.Sin(theta * index) * (cornerRadius - thickness);
                    float rx2 = (float)Math.Cos(theta * (index + 1)) * (cornerRadius - thickness);
                    float ry2 = (float)Math.Sin(theta * (index + 1)) * (cornerRadius - thickness);

                    __AddTri(leftInner - lx2, topInner - ly2, leftInner - lx, topInner - ly, leftInner - rx2, topInner - ry2, color);
                    __AddTri(leftInner - rx2, topInner - ry2, leftInner - lx, topInner - ly, leftInner - rx, topInner - ry, color);
                    __AddTri(rightInner + lx2, topInner - ly2, rightInner + rx2, topInner - ry2, rightInner + lx, topInner - ly, color);
                    __AddTri(rightInner + rx2, topInner - ry2, rightInner + rx, topInner - ry, rightInner + lx, topInner - ly, color);

                    __AddTri(leftInner - lx2, bottomInner + ly2, leftInner - rx2, bottomInner + ry2, leftInner - lx, bottomInner + ly, color);
                    __AddTri(leftInner - rx2, bottomInner + ry2, leftInner - rx, bottomInner + ry, leftInner - lx, bottomInner + ly, color);
                    __AddTri(rightInner + lx2, bottomInner + ly2, rightInner + lx, bottomInner + ly, rightInner + rx2, bottomInner + ry2, color);
                    __AddTri(rightInner + rx2, bottomInner + ry2, rightInner + lx, bottomInner + ly, rightInner + rx, bottomInner + ry, color);
                }
            }

        }

        public void DrawRoundedQuad(Rectangle bounds, Color color, float cornerRadius, int sampleCount = 4)
        {
            if (cornerRadius * 2 > bounds.Width || cornerRadius * 2 > bounds.Height)
                throw new ArgumentException("Corner diameter cannot exceed size of bounds edge lengths");

            float leftOuter, rightOuter, topOuter, bottomOuter;
            float leftInner, rightInner, topInner, bottomInner;

            leftInner = bounds.Left + cornerRadius;
            rightInner = bounds.Right - cornerRadius;
            topInner = bounds.Top + cornerRadius;
            bottomInner = bounds.Bottom - cornerRadius;

            leftOuter = bounds.Left;
            rightOuter = bounds.Right;
            topOuter = bounds.Top;
            bottomOuter = bounds.Bottom;

            __AddTri(leftOuter,  topInner, rightOuter, topInner,    leftOuter, bottomInner, color);
            __AddTri(rightOuter, topInner, rightOuter, bottomInner, leftOuter, bottomInner, color);

            __AddTri(leftInner, topOuter, rightInner, topOuter, leftInner, topInner,    color);
            __AddTri(leftInner, topInner, rightInner, topOuter, rightInner, topInner, color);

            __AddTri(leftInner, bottomInner, rightInner, bottomInner, leftInner, bottomOuter, color);
            __AddTri(leftInner, bottomOuter, rightInner, bottomInner, rightInner, bottomOuter, color);

            float theta = MathHelper.PiOver2 / sampleCount;

            for (int index = 0; index < sampleCount; index++)
            {
                float lx = (float)Math.Cos(theta * index) * cornerRadius;
                float ly = (float)Math.Sin(theta * index) * cornerRadius;
                float rx = (float)Math.Cos(theta * (index + 1)) * cornerRadius;
                float ry = (float)Math.Sin(theta * (index + 1)) * cornerRadius;

                __AddTri(leftInner, topInner, leftInner - lx, topInner - ly, leftInner - rx, topInner - ry, color);
                __AddTri(rightInner, topInner, rightInner + rx, topInner - ry, rightInner + lx, topInner - ly, color);

                __AddTri(leftInner,  bottomInner, leftInner - rx, bottomInner + ry, leftInner - lx, bottomInner + ly, color);
                __AddTri(rightInner, bottomInner, rightInner + lx, bottomInner + ly, rightInner + rx, bottomInner + ry, color);
            }

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

        protected void __AddLine(float x, float y, float x2, float y2, Color color)
        {
            myLineVerts[myLineCount * 2 + 0].Position.X = x;
            myLineVerts[myLineCount * 2 + 0].Position.Y = y;
            myLineVerts[myLineCount * 2 + 0].Color = color;

            myLineVerts[myLineCount * 2 + 1].Position.X = x2;
            myLineVerts[myLineCount * 2 + 1].Position.Y = y2;
            myLineVerts[myLineCount * 2 + 1].Color = color;

            myLineCount++;

            if (myLineCount * 2 == myLineVerts.Length)
                __FlushLines();
        }

        public void Begin()
        {
            myLineCount = 0;
            myTriCount = 0;
            myTexTriCount = 0;
        }

        public void End()
        {
            Flush();
        }

        public void Flush()
        {
            __FlushTris();
            __FlushTexTris();
            __FlushLines();
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
