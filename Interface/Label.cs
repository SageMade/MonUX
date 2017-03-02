using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonUX.Interface
{
    public class Label : TextControl
    {

        public Label(string text) : base()
        {
            Text = text;
            BackgroundColor = Color.LightGray;
            TextColor = Color.Black;
            Vector2 size = Font?.MeasureString(text) ?? Vector2.Zero;
            Bounds = new Rectangle(0, 0, (int)size.X, (int)size.Y);
        }
    }
}
