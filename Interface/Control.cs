using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonUX.Input;
using MonUX.Render;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonUX.Interface
{
    /// <summary>
    /// Base class for all Monux controls
    /// </summary>
    public abstract class Control
    {
        private Rectangle myBounds;
        private BorderStyle myBorderStyle;
        private Color myBackgroundColor;
        private Color myBorderColor;

        /// <summary>
        /// Gets or sets the bounds of this control
        /// </summary>
        public virtual Rectangle Bounds
        {
            get { return myBounds; }
            set
            {
                myBounds = value;
                OnBoundsChanged(value);
            }
        }
        /// <summary>
        /// Gets or sets the background color for this control
        /// </summary>
        public virtual Color BackgroundColor
        {
            get { return myBackgroundColor; }
            set { myBackgroundColor = value; OnPropertyChanged("BackgroundColor", value); }
        }
        /// <summary>
        /// Gets or sets the border style for this control
        /// </summary>
        public virtual BorderStyle BorderStyle
        {
            get { return myBorderStyle; }
            set { myBorderStyle = value; OnPropertyChanged("BorderStyle", value); }
        }
        /// <summary>
        /// Gets or sets the border color for this control
        /// </summary>
        public virtual Color BorderColor
        {
            get { return myBorderColor; }
            set { myBorderColor = value; OnPropertyChanged("BorderColor", value); }
        }

        /// <summary>
        /// Gets or sets the location (top left) of this control
        /// </summary>
        public virtual Vector2 Location
        {
            get { return new Vector2(myBounds.X, myBounds.Y); }
            set
            {
                myBounds.X = (int)value.X;
                myBounds.Y = (int)value.Y;
                OnBoundsChanged(myBounds);
            }
        }

        /// <summary>
        /// Invoked when a property of the control has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Invoked when the control's bounds have changed
        /// </summary>
        public event BoundsChangedEventHandler BoundsChanged;

        public event EventHandler<DeltaMouseState> MouseDoubleClick;
        public event EventHandler<DeltaMouseState> MousePressed;
        public event EventHandler<DeltaMouseState> MouseDown;
        public event EventHandler<DeltaMouseState> MouseReleased;

        public event EventHandler<DeltaMouseState> RightMousePressed;
        public event EventHandler<DeltaMouseState> RightMouseDown;
        public event EventHandler<DeltaMouseState> RightMouseReleased;

        /// <summary>
        /// Handles when th control has been resized
        /// </summary>
        /// <param name="bounds">The new bounds for the control</param>
        protected virtual void OnBoundsChanged(Rectangle bounds)
        {
            BoundsChanged?.Invoke(this, bounds);
        }

        /// <summary>
        /// Handles whenever a property of the control has changed
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        /// <param name="newValue">The new value of the property</param>
        protected virtual void OnPropertyChanged(string propertyName, object newValue)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Renders this control
        /// </summary>
        public void Render()
        {
            __InitRender();
            RenderSelf();
            __EndRender();
        }

        public virtual void HandleMouse(DeltaMouseState mouseState)
        {
            if (Bounds.Contains(mouseState.Location))
            {
                if (mouseState.DoubleClicked)
                    OnMouseDoubleClicked(mouseState);
                if (mouseState.LeftButton == ButtonDelta.Pressed)
                    OnMousePressed(mouseState);
                if (mouseState.LeftButton == ButtonDelta.Down)
                    OnMouseDown(mouseState);
                if (mouseState.LeftButton == ButtonDelta.Released)
                    OnMouseUp(mouseState);

                if (mouseState.RightButton == ButtonDelta.Pressed)
                    OnRightMousePressed(mouseState);
                if (mouseState.RightButton == ButtonDelta.Down)
                    OnRightMouseDown(mouseState);
                if (mouseState.RightButton == ButtonDelta.Released)
                    OnRightMouseUp(mouseState);
            }
        }

        protected virtual void OnMouseDoubleClicked(DeltaMouseState mouseState)
        {
            MouseDoubleClick?.Invoke(this, mouseState);
        }
        protected virtual void OnMousePressed(DeltaMouseState mouseState)
        {
            MousePressed?.Invoke(this, mouseState);
        }
        protected virtual void OnMouseDown(DeltaMouseState mouseState)
        {
            MouseDown?.Invoke(this, mouseState);
        }
        protected virtual void OnMouseUp(DeltaMouseState mouseState)
        {
            MouseReleased?.Invoke(this, mouseState);
        }

        protected virtual void OnRightMousePressed(DeltaMouseState mouseState)
        {
            RightMousePressed?.Invoke(this, mouseState);
        }
        protected virtual void OnRightMouseDown(DeltaMouseState mouseState)
        {
            RightMouseDown?.Invoke(this, mouseState);
        }
        protected virtual void OnRightMouseUp(DeltaMouseState mouseState)
        {
            RightMouseReleased?.Invoke(this, mouseState);
        }

        /// <summary>
        /// Handles performing control-specific rendering
        /// </summary>
        protected abstract void RenderSelf();

        /// <summary>
        /// Sets up the rendering for the control, should not be overloaded in most controls
        /// </summary>
        protected virtual void __InitRender()
        {
            Renderer.FillBounds(Bounds, BackgroundColor);
            Renderer.DrawBounds(Bounds, BorderColor, BorderStyle);
            Renderer.SetClip(Bounds);
        }

        /// <summary>
        /// Handles finishing the render of this control
        /// </summary>
        protected virtual void __EndRender()
        {

        }
    }
}
