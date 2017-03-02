using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static MonUX.Utility;
using Microsoft.Xna.Framework;
using MonUX.Interface;

namespace MonUX.Render
{
    public static class Renderer
    {
        private static SpriteBatch mySpriteBatch;
        private static PrimitiveBatch myPrimitiveBatch;

        public static void Init()
        {
            mySpriteBatch = new SpriteBatch(MonuxServices.GraphicsDevice);
            myPrimitiveBatch = new PrimitiveBatch(MonuxServices.GraphicsDevice);
        }

        public static void SetClip(Rectangle bounds)
        {

        }

        public static void FillBounds(Rectangle bounds, Color color)
        {
            myPrimitiveBatch.DrawQuad(bounds, color);
        }

        public static void DrawBounds(Rectangle bounds, Color color, BorderStyle borderStyle)
        {

        }

        internal static void DrawText(SpriteFont font, string text, Vector2 position, Color color)
        {
            mySpriteBatch.DrawString(font, text, position, color);
        }

        public static void UnsetClip()
        {

        }

        public static void StartFrame()
        {
            mySpriteBatch.Begin();
            myPrimitiveBatch.Begin();
        }

        public static void EndFrame()
        {
            myPrimitiveBatch.End();
            mySpriteBatch.End();
        }
    }
}
