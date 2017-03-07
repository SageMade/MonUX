using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonUX.Interface;
using System.Collections.Generic;
using static MonUX.Utility;

namespace MonUX.Render
{
    public class MonoRenderer : IRenderer
    {
        private static SpriteBatch mySpriteBatch;
        private static PrimitiveBatch myPrimitiveBatch;
        private static Viewport myViewport;
        private static Viewport myDefaultViewport;

        private static Stack<Rectangle> myViewportStack;
        private static Stack<Rectangle> myClipStack;
        private static Stack<Vector2> myOffsetStack;

        private static Vector2 myOffset = Vector2.Zero;

        private GraphicsDevice Graphics
        {
            get { return MonuxServices.GraphicsDevice; }
        }

        public void Init()
        {
            mySpriteBatch = new SpriteBatch(MonuxServices.GraphicsDevice);
            myPrimitiveBatch = new PrimitiveBatch(MonuxServices.GraphicsDevice);

            myViewport = new Viewport(0, 0, Graphics.PresentationParameters.BackBufferWidth, Graphics.PresentationParameters.BackBufferHeight, 0, 1);
            myDefaultViewport = myViewport;

            myViewportStack = new Stack<Rectangle>();
            myClipStack = new Stack<Rectangle>();
            myOffsetStack = new Stack<Vector2>();

            myViewportStack.Push(myViewport.Bounds);
            myClipStack.Push(myViewport.Bounds);
            myOffsetStack.Push(Vector2.Zero);
        }
        
        public void FillBounds(Rectangle bounds, Color color)
        {
            bounds.X += (int)myOffset.X;
            bounds.Y += (int)myOffset.Y;

            myPrimitiveBatch.DrawQuad(bounds, color);
        }

        public void DrawBounds(Rectangle bounds, Color color, BorderStyle borderStyle)
        {
            bounds.X += (int)myOffset.X;
            bounds.Y += (int)myOffset.Y;

            if ((borderStyle & BorderStyle.Visisble) != 0)
                myPrimitiveBatch.DrawOutline(bounds, color, (borderStyle & BorderStyle.Inner) != 0, (borderStyle & BorderStyle.Thick) == 0 ? 1 : 2.0f);
        }

        public void FillRoundedRect(Rectangle rectangle, Color color, float cornerRadius, int numSamples = 4)
        {
            rectangle.X += (int)myOffset.X;
            rectangle.Y += (int)myOffset.Y;

            myPrimitiveBatch.DrawRoundedQuad(rectangle, color, cornerRadius, numSamples);
        }

        public void DrawRoundedRect(Rectangle rectangle, Color color, float cornerRadius, bool inner, float thickness = 1, int numSamples = 4)
        {
            rectangle.X += (int)myOffset.X;
            rectangle.Y += (int)myOffset.Y;

            myPrimitiveBatch.DrawRoundedOutline(rectangle, color, cornerRadius, inner, thickness, numSamples);
        }

        public void DrawText(SpriteFont font, string text, Vector2 position, Color color)
        {
            position += myOffset;
            mySpriteBatch.DrawString(font, text, position, color);
        }
        
        public void StartFrame()
        {
            mySpriteBatch.Begin();
            myPrimitiveBatch.Begin();
        }

        public void EndFrame()
        {
            myPrimitiveBatch.End();
            mySpriteBatch.End();
        }

        public void Flush()
        {
            myPrimitiveBatch.Flush();
            mySpriteBatch.End();
            mySpriteBatch.Begin();
        }

        public void PushOffset(float x, float y)
        {
            PushOffset(new Vector2(x, y));
        }

        public void PushOffset(Vector2 offset)
        {
            myOffsetStack.Push(myOffset);
            myOffset = offset;
        }

        public Vector2 PopOffset()
        {
            Vector2 result;
            if (myOffsetStack.Count > 1)
                result = myOffsetStack.Pop();
            else
                result = myOffsetStack.Peek();

            myOffset = result;

            return result;
        }

        public Vector2 PeekOfset()
        {
            return myOffsetStack.Peek();
        }

        public void PushViewport(Rectangle bounds)
        {
            bounds.X += myViewport.X;
            bounds.Y += myViewport.Y;

            myViewportStack.Push(myViewport.Bounds);
            myViewport.Bounds = bounds;
            Graphics.Viewport = myViewport;
        }

        public Rectangle PopViewport()
        {
            Rectangle result;

            if (myViewportStack.Count > 1)
                result = myViewportStack.Pop();
            else
                result = myViewportStack.Peek();

            myViewport.Bounds = result;
            Graphics.Viewport = myViewport;

            return result;
        }

        public Rectangle PeekViewport()
        {
            return myViewportStack.Peek();
        }

        public void PushClip(Rectangle bounds)
        {
            bounds.X += myViewport.X;
            bounds.Y += myViewport.Y;

            myClipStack.Push(Graphics.ScissorRectangle);
            Graphics.ScissorRectangle = bounds;
        }

        public Rectangle PopClip()
        {
            Rectangle result;

            if (myClipStack.Count > 1)
                result = myClipStack.Pop();
            else
                result = myClipStack.Peek();

            Graphics.ScissorRectangle = result;

            return result;
        }

        public Rectangle PeekClip()
        {
            return myClipStack.Peek();
        }

    }
}
