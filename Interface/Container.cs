using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonUX.Input;

using static MonUX.Utility;

namespace MonUX.Interface
{
    public abstract class Container : Control
    {
        protected ControlCollection myChildControls;

        protected Container()
        {
            myChildControls = new ControlCollection();
        }

        public virtual void Add(Control control)
        {
            myChildControls.Add(control);
        }

        protected override void RenderSelf()
        {
            for (int index = 0; index < myChildControls.Length; index++)
                myChildControls[index].Render();
        }

        public override void HandleMouse(DeltaMouseState mouseState)
        {
            base.HandleMouse(mouseState);

            for (int index = 0; index < myChildControls.Length; index++)
                if (myChildControls[index].Bounds.Contains(mouseState.Location))
                    myChildControls[index].HandleMouse(mouseState);
        }

        protected override void __InitRender()
        {
            base.__InitRender();
        }

        protected override void __EndRender()
        {
            base.__EndRender();
        }
    }
}
