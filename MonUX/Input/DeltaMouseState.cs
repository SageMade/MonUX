using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonUX.Input
{
    /// <summary>
    /// Represents the changes to a mouse state between polls
    /// </summary>
    public struct DeltaMouseState
    {
        /// <summary>
        /// Gets the current X coordinate of the mouse
        /// </summary>
        public int X;
        /// <summary>
        /// Gets the current Y coordinate of the mouse
        /// </summary>
        public int Y;

        /// <summary>
        /// Gets the change in X between frames
        /// </summary>
        public int DeltaX;
        /// <summary>
        /// Gets the change in Y between frames
        /// </summary>
        public int DeltaY;
        /// <summary>
        /// Gets the change in the scroll wheel between frames
        /// </summary>
        public int DeltaScroll;

        public Vector2 SmoothedPosition;
        public Vector2 SmoothDeltaPosition;

        /// <summary>
        /// Gets the absolute location of the mouse
        /// </summary>
        public Vector2 Location
        {
            get { return new Vector2(X, Y); }
        }
        /// <summary>
        /// Gets the absolute delta in the mouse position since the previous frame
        /// </summary>
        public Vector2 Delta
        {
            get { return new Vector2(DeltaX, DeltaY); }
        }

        /// <summary>
        /// Gets whether this state is a double click state
        /// </summary>
        public bool DoubleClicked;

        /// <summary>
        /// Gets the changes in the left mouse button
        /// </summary>
        public ButtonDelta LeftButton;
        /// <summary>
        /// Gets the changes in the right mouse button
        /// </summary>
        public ButtonDelta RightButton;
        /// <summary>
        /// Gets the changes in the middle mouse button
        /// </summary>
        public ButtonDelta MiddleButton;
        /// <summary>
        /// Gets the changes in XButon1, this is the forward button on most mice
        /// </summary>
        public ButtonDelta XButton1;
        /// <summary>
        /// Gets the changes in XButon2, this is the forward button on most mice
        /// </summary>
        public ButtonDelta XButton2;

        /// <summary>
        /// Gets the mouse button delta for the given mouse button
        /// </summary>
        /// <param name="button">The mouse button to check</param>
        /// <returns>The button delta for the given mouse button</returns>
        public ButtonDelta this[MouseButton button]
        {
            get
            {
                switch (button)
                {
                    case MouseButton.Left:
                        return LeftButton;
                    case MouseButton.Right:
                        return RightButton;
                    case MouseButton.Middle:
                        return MiddleButton;
                    case MouseButton.XButton1:
                        return XButton1;
                    case MouseButton.XButton2:
                        return XButton2;
                    default:
                        return ButtonDelta.Released;
                }
            }
        }

        /// <summary>
        /// Creates a new mouse delta state from two mouse states
        /// </summary>
        /// <param name="currentState">The mouse state of the most current frame</param>
        /// <param name="prevState">The mouse state of the previous frame</param>
        public DeltaMouseState(MouseState currentState, MouseState prevState, Vector2 smoothedPos, Vector2 smoothDelta)
        {
            X = currentState.X;
            Y = currentState.Y;

            DeltaX = currentState.X - prevState.X;
            DeltaY = currentState.Y - prevState.Y;

            SmoothedPosition = smoothedPos;
            SmoothDeltaPosition = smoothDelta;

            DoubleClicked = false;

            LeftButton = currentState.LeftButton != prevState.LeftButton ? (ButtonDelta)currentState.LeftButton : currentState.LeftButton == ButtonState.Pressed ? ButtonDelta.Down : ButtonDelta.Up;
            RightButton = currentState.RightButton != prevState.RightButton ? (ButtonDelta)currentState.RightButton : currentState.RightButton == ButtonState.Pressed ? ButtonDelta.Down : ButtonDelta.Up;
            MiddleButton = currentState.MiddleButton != prevState.MiddleButton ? (ButtonDelta)currentState.MiddleButton : currentState.MiddleButton == ButtonState.Pressed ? ButtonDelta.Down : ButtonDelta.Up;

            XButton1 = currentState.XButton1 != prevState.XButton1 ? (ButtonDelta)currentState.XButton1 : currentState.XButton1 == ButtonState.Pressed ? ButtonDelta.Down : ButtonDelta.Up;
            XButton2 = currentState.XButton2 != prevState.XButton2 ? (ButtonDelta)currentState.XButton2 : currentState.XButton2 == ButtonState.Pressed ? ButtonDelta.Down : ButtonDelta.Up;

            DeltaScroll = currentState.ScrollWheelValue - prevState.ScrollWheelValue;
        }
    }
}
