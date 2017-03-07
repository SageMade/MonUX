using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static MonUX.Utility;
using MonUX.Input;
using Microsoft.Xna.Framework.Input;

namespace MonUX.Interface
{
    public class ScrollablePanel : Panel
    {
        protected float myScrollX;
        protected float myScrollY;

        protected float myInternalWidth;
        protected float myInternalHeight;

        protected float myScrollableWidth;
        protected float myScrollableHeight;

        protected float myScrollBarSize = 12;

        protected bool isHorizontalScrollVisible;
        protected bool isVerticalScrollVisible;

        protected bool isHoldingVerticalScroll;
        protected bool isHoldingHorizontalScroll;

        protected Rectangle myHorizontalScrollerBounds;
        protected Rectangle myVerticalScrollerBounds;

        public ScrollStyle ScrollStyle
        {
            get;
            set;
        }

        public ScrollablePanel(Rectangle bounds) : base(bounds)
        {
            __InternalBoundsChanged();
            __CalculateScrollerBounds();
        }

        public ScrollablePanel(int x, int y, int width, int height) : this(new Rectangle(x, y, width, height)) { }

        public override void HandleMouse(DeltaMouseState mouseState)
        {
            base.HandleMouse(mouseState);

            if (mouseState.LeftButton == ButtonDelta.Released)
            {
                isHoldingHorizontalScroll = false;
                isHoldingVerticalScroll = false;
            }

            if (isHoldingVerticalScroll)
            {
                float deltaY = mouseState.DeltaY;
                if (deltaY != 0)
                    myScrollY += (deltaY / myScrollableHeight);
                myScrollY = MathHelper.Clamp(myScrollY, 0, 1);
                __CalculateScrollerBounds();
            }
            else if (isHoldingHorizontalScroll)
            {
                float deltaX = mouseState.DeltaX;
                if (deltaX != 0)
                    myScrollX += (deltaX / myScrollableWidth);
                myScrollX = MathHelper.Clamp(myScrollX, 0, 1);
                __CalculateScrollerBounds();
            }

            if (Bounds.Contains(mouseState.Location))
            {
                if (mouseState.LeftButton == ButtonDelta.Pressed)
                {
                    if (isHorizontalScrollVisible && myHorizontalScrollerBounds.Contains(mouseState.Location - Location))
                        isHoldingHorizontalScroll = true;
                    else if (isVerticalScrollVisible && myVerticalScrollerBounds.Contains(mouseState.Location - Location))
                        isHoldingVerticalScroll = true;
                }

                if (mouseState.DeltaScroll != 0)
                {
                    if (!KeyboardManager.IsDown(Keys.LeftShift))
                    {
                        myScrollY -= ((float)mouseState.DeltaScroll / myInternalHeight) * (Bounds.Height / myInternalHeight);
                        myScrollY = MathHelper.Clamp(myScrollY, 0, 1);
                    }
                    else
                    {
                        myScrollX += ((float)mouseState.DeltaScroll / myInternalWidth) * (Bounds.Width / myInternalWidth);
                        myScrollX = MathHelper.Clamp(myScrollX, 0, 1);
                    }
                    __CalculateScrollerBounds();
                }
            }
        }

        protected override void OnBoundsChanged(Rectangle bounds)
        {
            base.OnBoundsChanged(bounds);

            if (bounds.Width > myInternalWidth)
                myInternalWidth = bounds.Width;
            if (bounds.Height > myInternalHeight)
                myInternalHeight = bounds.Height;

            __InternalBoundsChanged();
        }

        protected void __CalculateScrollerBounds()
        {
            if (isHorizontalScrollVisible)
            {
                int width = (int)(isVerticalScrollVisible ? Bounds.Width - myScrollBarSize : Bounds.Width);
                int top = (int)(Bounds.Height - myScrollBarSize);
                int height = (int)(myScrollBarSize);
                float scale = (width / myInternalWidth) * width;
                float offset = (myScrollX * (myScrollableWidth + (isVerticalScrollVisible ? myScrollBarSize : 0))) / myInternalWidth * width;

                myHorizontalScrollerBounds = new Rectangle((int)offset, top, (int)scale, height);
            }
            if (isVerticalScrollVisible)
            {
                int left = (int)(Bounds.Width - myScrollBarSize);
                int width = (int)(myScrollBarSize);                
                float scale = (Bounds.Height / myInternalHeight) * Bounds.Height;
                float offset = (myScrollY * myScrollableHeight) / myInternalHeight * Bounds.Height;

                myVerticalScrollerBounds = new Rectangle(left, (int)offset, width, (int)scale);
            }
        }

        public override void Add(Control control)
        {
            base.Add(control);

            if (control.Bounds.Right > myInternalWidth)
                myInternalWidth = control.Bounds.Right;
            if (control.Bounds.Bottom > myInternalHeight)
                myInternalHeight = control.Bounds.Bottom;

            __InternalBoundsChanged();
        }

        protected virtual void __InternalBoundsChanged()
        {
            isVerticalScrollVisible = myInternalHeight > Bounds.Height;
            isHorizontalScrollVisible = myInternalWidth > Bounds.Width;

            myInternalHeight += isVerticalScrollVisible ? myScrollBarSize : 0;
            myInternalWidth += isHorizontalScrollVisible ? myScrollBarSize : 0;

            myScrollableWidth = myInternalWidth - Bounds.Width + 1;
            myScrollableHeight = myInternalHeight - Bounds.Height + 1;

            __CalculateScrollerBounds();
        }

        protected override void __InitRender()
        {
            base.__InitRender();
            Renderer.PushOffset(-myScrollX * myScrollableWidth, -myScrollY * myScrollableHeight);
        }
                
        protected override void __EndRender()
        {
            Renderer.PopOffset();
                        
            if (isVerticalScrollVisible)
                __DrawVerticalScrollBar();
            if (isHorizontalScrollVisible)
                __DrawHorizontalScrollBar();

            base.__EndRender();
        }

        protected virtual void __DrawHorizontalScrollBar()
        {
            int width = (int)(isVerticalScrollVisible ? Bounds.Width - myScrollBarSize : Bounds.Width);

            int top = (int)(Bounds.Height - myScrollBarSize);
            int height = (int)(myScrollBarSize);

            if (ScrollStyle == ScrollStyle.Rounded)
            {
                Renderer.FillRoundedRect(new Rectangle(0, top, width, height), Color.LightGray, 4, 4);
                Renderer.DrawRoundedRect(new Rectangle(0, top, width, height), BorderColor, 4, true, 1, 4);
                Renderer.FillRoundedRect(myHorizontalScrollerBounds, Color.DarkGray, 4, 4);
                Renderer.DrawRoundedRect(myHorizontalScrollerBounds, BorderColor, 4, true, 1, 4);
            }
            else if (ScrollStyle == ScrollStyle.Square)
            {
                Renderer.FillBounds(new Rectangle(0, top, width, height), Color.LightGray);
                Renderer.DrawBounds(new Rectangle(0, top, width, height), BorderColor, BorderStyle.Visisble | BorderStyle.Inner);
                Renderer.FillBounds(myHorizontalScrollerBounds, Color.DarkGray);
                Renderer.DrawBounds(myHorizontalScrollerBounds, BorderColor, BorderStyle.Visisble | BorderStyle.Inner);
            }

            Renderer.Flush();
        }

        protected virtual void __DrawVerticalScrollBar()
        {
            int left = (int)(Bounds.Width - myScrollBarSize);
            int width = (int)(myScrollBarSize);

            if (ScrollStyle == ScrollStyle.Rounded)
            {
                Renderer.FillRoundedRect(new Rectangle(left, 0, width, Bounds.Height), Color.LightGray, 4, 4);
                Renderer.DrawRoundedRect(new Rectangle(left, 0, width, Bounds.Height), BorderColor, 4, true, 1, 4);
                Renderer.FillRoundedRect(myVerticalScrollerBounds, Color.DarkGray, 4, 4);
                Renderer.DrawRoundedRect(myVerticalScrollerBounds, BorderColor, 4, true, 1, 4);
            }
            else if (ScrollStyle == ScrollStyle.Square)
            {
                Renderer.FillBounds(new Rectangle(left, 0, width, Bounds.Height), Color.LightGray);
                Renderer.DrawBounds(new Rectangle(left, 0, width, Bounds.Height), BorderColor, BorderStyle.Visisble | BorderStyle.Inner);
                Renderer.FillBounds(myVerticalScrollerBounds, Color.DarkGray);
                Renderer.DrawBounds(myVerticalScrollerBounds, BorderColor, BorderStyle.Visisble | BorderStyle.Inner);
            }

                Renderer.Flush();
        }
    }

    public enum ScrollStyle
    {
        Rounded,
        Square
    }
}
