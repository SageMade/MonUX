using Microsoft.Xna.Framework;
using MonUX.Input;
using MonUX.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonUX
{
    public static class Utility
    {
        private static IRenderer myRenderer;
        private static MonuxServiceProvider myServiceProvider;

        internal static MonuxServiceProvider MonuxServices
        {
            get { return myServiceProvider; }
        }
        public static IRenderer Renderer
        {
            get { return myRenderer; }
        }

        public static void InitMonUX(Game game)
        {
            InitMonUX(game, new MonoRenderer());
        }

        public static void InitMonUX(Game game, IRenderer renderer)
        {
            myServiceProvider = new MonuxServiceProvider(game);
            myRenderer = renderer;

            EmbeddedContent.Init();
            myRenderer.Init();
            MouseManager.Init();
            KeyboardManager.Init();

            game.Components.Add(new MonuxComponent(game));
        }

        internal class MonuxComponent : GameComponent
        {
            public MonuxComponent(Game game) : base(game)
            {
            }

            public override void Initialize()
            {

            }

            public override void Update(GameTime gameTime)
            {
                MouseManager.Poll(gameTime);
                KeyboardManager.Poll(gameTime);
            }

            protected override void OnEnabledChanged(object sender, EventArgs args)
            {
                base.OnEnabledChanged(sender, args);
            }
        }
    }
}
