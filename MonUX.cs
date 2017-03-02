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
        private static MonuxServiceProvider myServiceProvider;

        internal static MonuxServiceProvider MonuxServices
        {
            get { return myServiceProvider; }
        }

        public static void InitMonUX(Game game)
        {
            myServiceProvider = new MonuxServiceProvider(game);

            EmbeddedContent.Init();
            Renderer.Init();
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
