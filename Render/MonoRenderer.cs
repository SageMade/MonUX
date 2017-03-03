using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonUX.Interface;
using static MonUX.Utility;

namespace MonUX.Render
{
    public class MonoRenderer : IRenderer
    {
        private static SpriteBatch mySpriteBatch;
        private static PrimitiveBatch myPrimitiveBatch;
        private static Viewport myViewport;
        private static Viewport myDefaultViewport;

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
        }

        public void SetViewport(Rectangle bounds)
        {
            myViewport.Bounds = bounds;
            Graphics.Viewport = myViewport;
        }

        public void UnsetViewport()
        {
            myViewport = myDefaultViewport;
            Graphics.Viewport = myDefaultViewport;
        }

        public void SetClip(Rectangle bounds)
        {

        }

        public void FillBounds(Rectangle bounds, Color color)
        {
            myPrimitiveBatch.DrawQuad(bounds, color);
        }

        public void DrawBounds(Rectangle bounds, Color color, BorderStyle borderStyle)
        {            
            if ((borderStyle & BorderStyle.Visisble) != 0)
                myPrimitiveBatch.DrawOutline(bounds, color, (borderStyle & BorderStyle.Inner) != 0, (borderStyle & BorderStyle.Thick) == 0 ? 1 : 2.0f);
        }

        public void FillRoundedRect(Rectangle rectangle, Color color, float cornerRadius, int numSamples = 4)
        {
            myPrimitiveBatch.DrawRoundedQuad(rectangle, color, cornerRadius, numSamples);
        }
        public void DrawRoundedRect(Rectangle rectangle, Color color, float cornerRadius, bool inner, float thickness = 1, int numSamples = 4)
        {
            myPrimitiveBatch.DrawRoundedOutline(rectangle, color, cornerRadius, inner, thickness, numSamples);
        }

        public void DrawText(SpriteFont font, string text, Vector2 position, Color color)
        {
            mySpriteBatch.DrawString(font, text, position, color);
        }

        public void UnsetClip()
        {
            Graphics.Viewport = myDefaultViewport;
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

    }
}
