using Microsoft.Xna.Framework.Input;
using System;

namespace MonUX.Input
{
    /// <summary>
    /// Represents the event arguments for a keyboard button change
    /// </summary>
    public class KeyPressEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the Key that has changed
        /// </summary>
        public Keys Key
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets the delta state of the button
        /// </summary>
        public ButtonDelta DeltaState
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates an empty key press event argument
        /// </summary>
        public KeyPressEventArgs()
        {
            Key = Keys.None;
            DeltaState = ButtonDelta.Released;
        }

        /// <summary>
        /// Creates a new key press event argument
        /// </summary>
        /// <param name="key">The keyboard key that the event is for</param>
        /// <param name="newState">The current state of the button</param>
        /// <param name="oldState">The state of the button at the last polling event</param>
        public KeyPressEventArgs(Keys key, ButtonState newState, ButtonState oldState)
        {
            Key = key;
            DeltaState = newState != oldState ? (ButtonDelta)newState : newState == ButtonState.Pressed ? ButtonDelta.Down : ButtonDelta.Up;
        }

        /// <summary>
        /// Creates a new key press event argument
        /// </summary>
        /// <param name="key">The keyboard key that the event is for</param>
        /// <param name="deltaState">The delta state of the button</param>
        public KeyPressEventArgs(Keys key, ButtonDelta deltaState)
        {
            Key = key;
            DeltaState = deltaState;
        }
    }
}
