using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static MonUX.Utility;

namespace MonUX.Input
{
    /// <summary>
    /// The shared interface for accessing the keyboard input
    /// </summary>
    public static class KeyboardManager
    {
        /// <summary>
        /// The previous state of the keyboard
        /// </summary>
        private static KeyboardState myPrevState;
        /// <summary>
        /// The current state of the keyboard
        /// </summary>
        private static KeyboardState myCurrentState;

        /// <summary>
        /// Stores the keyboard listeners indexed by the key they listen to
        /// </summary>
        private static Dictionary<Keys, KeyListener> myWatches;

        private static List<KeySequence> myKeySequences;

        public static event EventHandler<TextInputEventArgs> TextInput;
        
        internal static void Init()
        {
            myPrevState = Keyboard.GetState();
            myCurrentState = Keyboard.GetState();

            myWatches = new Dictionary<Keys, KeyListener>();
            myKeySequences = new List<KeySequence>();

            MonuxServices.Game.Window.TextInput += __GameWindowTextInput;
        }

        private static void __GameWindowTextInput(object sender, TextInputEventArgs e)
        {
            TextInput?.Invoke(sender, e);
        }

        public static void AddSequence(KeySequence sequence)
        {
            myKeySequences.Add(sequence);
        }

        /// <summary>
        /// Polls the keyboard for changes, this is the meat of the keyboard manager and will be invoked
        /// by the core game instance
        /// </summary>
        public static void Poll(GameTime gameTime)
        {
            if (MonuxServices.Game?.IsActive ?? false)
            {
                myPrevState = myCurrentState;
                myCurrentState = Keyboard.GetState();

                foreach (KeyValuePair<Keys, KeyListener> l in myWatches)
                    l.Value.Watch(myCurrentState, myPrevState);

                for (int index = 0; index < myKeySequences.Count; index++)
                    myKeySequences[index].CheckKeys(gameTime);
            }
        }

        /// <summary>
        /// Gets the current change in state for the specified button
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>The buttons's delta state</returns>
        public static ButtonDelta GetDelta(Keys key)
        {
            bool wasPressed = myPrevState.IsKeyDown(key);
            bool isPressed = myCurrentState.IsKeyDown(key);

            return
                 isPressed != wasPressed ?
                 isPressed ? ButtonDelta.Pressed : ButtonDelta.Released :
                 isPressed ? ButtonDelta.Down : ButtonDelta.Up;
        }

        public static bool WasPressed(Keys key)
        {
            return myPrevState.IsKeyUp(key) && myCurrentState.IsKeyDown(key);
        }

        public static bool WasReleased(Keys key)
        {
            return myPrevState.IsKeyDown(key) && myCurrentState.IsKeyUp(key);
        }

        public static bool IsDown(Keys key)
        {
            return myCurrentState.IsKeyDown(key);
        }

        public static bool IsUp(Keys key)
        {
            return myCurrentState.IsKeyUp(key);
        }

        /// <summary>
        /// Adds a new watch to the given key
        /// </summary>
        /// <param name="key">The key to add the watch to</param>
        /// <param name="listener">The general-purpose listener, which will listen to any key change events</param>
        public static void AddWatch(Keys key, EventHandler<KeyPressEventArgs> listener)
        {
            if (!myWatches.ContainsKey(key))
                myWatches.Add(key, new KeyListener(key));

            myWatches[key] += listener;
        }

        /// <summary>
        /// Adds a new watch to the given key, only listening for the specified change
        /// </summary>
        /// <param name="key">The key to add the watch to</param>
        /// <param name="listenState">The specific state change to listen to</param>
        /// <param name="listener">The event listener that will listen for events</param>
        public static void AddWatch(Keys key, ButtonDelta listenState, EventHandler<KeyPressEventArgs> listener)
        {
            if (!myWatches.ContainsKey(key))
                myWatches.Add(key, new KeyListener(key));

            myWatches[key][listenState] += listener;
        }

        /// <summary>
        /// Adds a new watch to the given key, only listening for the specified state
        /// </summary>
        /// <param name="key">The key to add the watch to</param>
        /// <param name="listenState">The specific state change to listen to</param>
        /// <param name="listener">The event listener that will listen for events</param>
        public static void AddWatch(Keys key, ButtonState listenState, EventHandler<KeyPressEventArgs> listener)
        {
            if (!myWatches.ContainsKey(key))
                myWatches.Add(key, new KeyListener(key));

            myWatches[key][(ButtonDelta)listenState] += listener;
        }

        /// <summary>
        /// Utility class for tying event listeners to keyboard keys
        /// </summary>
        private class KeyListener
        {
            private Keys myKey;

            /// <summary>
            /// Gets the key that this listener is tied to
            /// </summary>
            public Keys Key
            {
                get { return myKey; }
            }

            /// <summary>
            /// The event to invoke when the key has been pressed
            /// </summary>
            public event EventHandler<KeyPressEventArgs> OnPressed;
            /// <summary>
            /// The event to invoke when the key has been released
            /// </summary>
            public event EventHandler<KeyPressEventArgs> OnReleased;
            /// <summary>
            /// The event to invoke when the key is down
            /// </summary>
            public event EventHandler<KeyPressEventArgs> OnDown;
            /// <summary>
            /// The event to invoke when the key is up
            /// </summary>
            public event EventHandler<KeyPressEventArgs> OnUp;
            /// <summary>
            /// The event to invoke on any key event
            /// </summary>
            public event EventHandler<KeyPressEventArgs> OnEvent;

            /// <summary>
            /// Gets the event handler for the given button state
            /// </summary>
            /// <param name="button">The button state of the the event to get</param>
            /// <returns>The event handler tied to the given button delta</returns>
            public EventHandler<KeyPressEventArgs> this[ButtonDelta button]
            {
                get
                {
                    switch (button)
                    {
                        case ButtonDelta.Pressed:
                            return OnPressed;
                        case ButtonDelta.Released:
                            return OnReleased;
                        case ButtonDelta.Down:
                            return OnDown;
                        case ButtonDelta.Up:
                            return OnUp;
                        default:
                            return OnEvent;
                    }
                }
                set
                {
                    switch (button)
                    {
                        case ButtonDelta.Pressed:
                            OnPressed = value;
                            break;
                        case ButtonDelta.Released:
                            OnReleased = value;
                            break;
                        case ButtonDelta.Down:
                            OnDown = value;
                            break;
                        case ButtonDelta.Up:
                            OnUp = value;
                            break;
                        default:
                            OnEvent = value;
                            break;
                    }
                }
            }

            /// <summary>
            /// Creates a new key listener
            /// </summary>
            /// <param name="key">The key to listen to</param>
            public KeyListener(Keys key)
            {
                myKey = key;
            }

            /// <summary>
            /// Handles watching the given key for
            /// </summary>
            /// <param name="myCurrentState"></param>
            /// <param name="myPrevState"></param>
            /// <param name="key"></param>
            public void Watch(KeyboardState myCurrentState, KeyboardState myPrevState)
            {
                bool wasPressed = myPrevState.IsKeyDown(myKey);
                bool isPressed = myCurrentState.IsKeyDown(myKey);

                ButtonDelta deltaState =
                    isPressed != wasPressed ?
                    isPressed ? ButtonDelta.Pressed : ButtonDelta.Released :
                    isPressed ? ButtonDelta.Down : ButtonDelta.Up;

                if (deltaState == ButtonDelta.Pressed)
                    OnPressed?.Invoke(this, new KeyPressEventArgs(myKey, deltaState));

                if (deltaState == ButtonDelta.Released)
                    OnReleased?.Invoke(this, new KeyPressEventArgs(myKey, deltaState));

                if (deltaState == ButtonDelta.Down)
                    OnDown?.Invoke(this, new KeyPressEventArgs(myKey, deltaState));

                if (deltaState == ButtonDelta.Up)
                    OnUp?.Invoke(this, new KeyPressEventArgs(myKey, deltaState));

                OnEvent?.Invoke(this, new KeyPressEventArgs(myKey, deltaState));
            }

            public static KeyListener operator +(KeyListener left, EventHandler<KeyPressEventArgs> right)
            {
                left.OnEvent += right;

                return left;
            }

            public static KeyListener operator -(KeyListener left, EventHandler<KeyPressEventArgs> right)
            {
                left.OnEvent -= right;

                return left;
            }
        }
    }
}
