using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static MonUX.Utility;

namespace MonUX
{
    public static class EmbeddedContent
    {
        private static ResourceContentManager myResourceManager;

        public static Effect PrimitiveEffect
        {
            get;
            private set;
        }
        public static SpriteFont DebugFont
        {
            get;
            private set;
        }
        public static SpriteFont DefaultFont
        {
            get;
            private set;
        }
        public static Texture2D DefaultCursor
        {
            get;
            private set;
        }

        internal static void Init()
        {
            myResourceManager = new ResourceContentManager(MonuxServices, Resources.ResourceManager);

            PrimitiveEffect = myResourceManager.Load<Effect>    ("WINDX_PrimitiveEffect");
            DebugFont       = myResourceManager.Load<SpriteFont>("WINDX_FONT_Debug");
            DefaultFont     = myResourceManager.Load<SpriteFont>("WINDX_FONT_Default");
            DefaultCursor   = myResourceManager.Load<Texture2D> ("WINDX_TEX_DefaultCursor");
        }
    }
}
