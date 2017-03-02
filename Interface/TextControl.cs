using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonUX.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonUX.Interface
{
    public abstract class TextControl : Control
    {
        private SpriteFont mySpriteFont;
        private Color myTextColor;
        private Alignment myAlignment;

        private Vector2 myTextPos;
        private Vector2 myTextSize;

        private string myText;

        public SpriteFont Font
        {
            get { return mySpriteFont; }
            set
            {
                mySpriteFont = value;
                myTextSize = mySpriteFont?.MeasureString(myText ?? "") ?? Vector2.Zero;
                __CalculateTextPos();
                OnPropertyChanged("Font", value);
            }
        }
        public Color TextColor
        {
            get { return myTextColor; }
            set { myTextColor = value; OnPropertyChanged("TextColor", value); }
        }
        public Alignment TextAlignment
        {
            get { return myAlignment; }
            set
            {
                myAlignment = value;
                __CalculateTextPos();
                OnPropertyChanged("TextAlignment", value);
            }
        }
        public string Text
        {
            get { return myText; }
            set
            {
                myText = value;
                myTextSize = mySpriteFont?.MeasureString(myText ?? "") ?? Vector2.Zero;
                __CalculateTextPos();
                OnPropertyChanged("Text", value);
            }
        }

        protected TextControl()
        {
            Font = EmbeddedContent.DefaultFont;
            TextColor = Color.Black;
            BackgroundColor = Color.LightGray;
        }
        
        protected void __CalculateTextPos()
        {
            myTextPos = Vector2.Zero;
            myTextPos.X = (int)Math.Round(myAlignment.HasFlag(Alignment.Center) ? Bounds.Center.X - (myTextSize.X / 2.0f) : myAlignment.HasFlag(Alignment.Right) ? Bounds.Right - myTextSize.X : Bounds.Left);
            myTextPos.Y = (int)Math.Round(myAlignment.HasFlag(Alignment.Middle) ? Bounds.Center.Y - (myTextSize.Y / 2.0f) : myAlignment.HasFlag(Alignment.Bottom) ? Bounds.Bottom - myTextSize.Y : Bounds.Top);
        }

        protected override void OnBoundsChanged(Rectangle bounds)
        {
            base.OnBoundsChanged(bounds);
            __CalculateTextPos();
        }

        protected override void RenderSelf()
        {
            Renderer.DrawText(Font, Text, myTextPos, myTextColor);
        }
    }
}
