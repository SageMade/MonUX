using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonUX.Render
{
    public class PrimitiveEffect : Effect
    {
        private const bool NEEDS_HALF_PIXEL_OFFSET = true;
        private bool isDirty;
        
        private EffectParameter myMatrixParam;
        private EffectParameter myTextureParam;

        private Matrix myWorld;

        public Matrix World
        {
            get { return myWorld; }
            set
            {
                myWorld = value;
                isDirty = true;
            }
        }

        public Texture2D DiffuseTexture
        {
            get { return myTextureParam?.GetValueTexture2D(); }
            set { myTextureParam?.SetValue(value); }
        }

        public bool TextureEnabled
        {
            get { return CurrentTechnique.Name == "SpriteDrawing"; }
            set { CurrentTechnique = Techniques[value ? "SpriteDrawing" : "ColorDrawing"]; }
        }

        public PrimitiveEffect() : base(EmbeddedContent.PrimitiveEffect)
        {
            myMatrixParam = Parameters["MatrixTransform"];
            myTextureParam = Parameters["SpriteTexture"];
        }

        protected override bool OnApply()
        {
            var viewport = GraphicsDevice.Viewport;

            var projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, 1);
            var halfPixelOffset = Matrix.CreateTranslation(0, 0, 0);

            if (NEEDS_HALF_PIXEL_OFFSET)
            {
                halfPixelOffset += Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            }

            myMatrixParam.SetValue(halfPixelOffset * projection);    

            return base.OnApply();
        }
    }
}
