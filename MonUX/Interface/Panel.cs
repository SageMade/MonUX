using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonUX.Interface
{
    public class Panel : Container
    {
        public Panel(Rectangle bounds)
        {
            Bounds = bounds;
            BackgroundColor = Color.LightGray;
        }

        public Panel(int x, int y, int width, int height) : this(new Rectangle(x, y, width, height)) { }
    }
}
