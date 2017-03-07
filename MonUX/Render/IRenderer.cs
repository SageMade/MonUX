using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonUX.Interface;

namespace MonUX.Render
{
    public interface IRenderer
    {
        void Init();
        
        void PushViewport(Rectangle bounds);

        Rectangle PopViewport();

        Rectangle PeekViewport();

        void PushClip(Rectangle bounds);

        Rectangle PopClip();

        Rectangle PeekClip();

        void FillBounds(Rectangle bounds, Color color);

        void DrawBounds(Rectangle bounds, Color color, BorderStyle borderStyle);

        void FillRoundedRect(Rectangle rectangle, Color color, float cornerRadius, int numSamples = 4);

        void DrawRoundedRect(Rectangle rectangle, Color color, float cornerRadius, bool inner, float thickness = 1, int numSamples = 4);

        void DrawText(SpriteFont font, string text, Vector2 position, Color color);
        

        void StartFrame();
        void EndFrame();

        void Flush();

        void PushOffset(Vector2 offset);

        void PushOffset(float x, float y);

        Vector2 PopOffset();

        Vector2 PeekOfset();
    }
}
