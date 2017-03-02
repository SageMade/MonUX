using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonUX.Interface
{
    public enum Alignment
    {
        Left   = 1,
        Center = 2,
        Right  = 4,

        Top    = 8,
        Middle = 16,
        Bottom = 32,

        TopLeft      = Left   | Top,
        TopCenter    = Center | Top,
        TopRight     = Right  | Top,

        MiddleLeft   = Left   | Middle,
        MiddleCenter = Center | Middle,
        MiddleRight  = Right  | Middle,

        BottomLeft   = Left   | Bottom,
        BottomCenter = Center | Bottom,
        BottomRight  = Right  | Bottom,
    }
}
