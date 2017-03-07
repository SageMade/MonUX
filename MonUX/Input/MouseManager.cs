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
    public static class MouseManager
    {
        private const int NUM_SMOOTHING_SAMPLES = 8;
        private static readonly TimeSpan DOUBLE_CLICK_TIME = new TimeSpan(0, 0, 0, 0, 500);

        private static MouseState myPrevState;
        private static MouseState myCurrentState;

        public static event EventHandler<int> OnWheelChanged;
        public static event EventHandler<MouseState> OnPositionChanged;

        private static Dictionary<MouseButton, MouseListener> myWatches;

        private static DeltaMouseState myMouseDelta;

        private static Vector2 myCurrentSmoothPosition;
        private static Vector2 myPrevSmoothPosition;
        private static Vector2[] myMouseSmoothQueue;
        private static int myMouseSmoothQueuePos;

        private static TimeSpan myLastLeftClickTime;

        /// <summary>
        /// Gets the current state of the mouse
        /// </summary>
        public static MouseState CurrentState
        {
            get { return myCurrentState; }
        }
        /// <summary>
        /// Gets the state of the mouse when it was last polled
        /// </summary>
        public static MouseState PrevState
        {
            get { return myPrevState; }
        }
        /// <summary>
        /// Gets the difference in state between CurrentState and PrevState
        /// </summary>
        public static DeltaMouseState DeltaState
        {
            get { return myMouseDelta; }
        }

        /// <summary>
        /// Gets or sets whether the mouse is currently visible
        /// </summary>
        public static bool IsMouseVisible
        {
            get { return MonuxServices.Game != null && MonuxServices.Game.IsMouseVisible; }
            set { if (MonuxServices.Game != null) MonuxServices.Game.IsMouseVisible = value; }
        }
        
        internal static void Init()
        {
            myPrevState = Mouse.GetState();
            myCurrentState = Mouse.GetState();

            myWatches = new Dictionary<MouseButton, MouseListener>();

            myMouseSmoothQueue = new Vector2[NUM_SMOOTHING_SAMPLES];
        }
        
        /// <summary>
        /// Polls the mouse for changes, this is the meat of the mouse manager and will be invoked
        /// by the core game instance
        /// </summary>
        public static void Poll(GameTime gameTime)
        {
            if (MonuxServices.Game?.IsActive ?? false)
            {
                myPrevState = myCurrentState;
                myCurrentState = Mouse.GetState();

                myMouseSmoothQueue[myMouseSmoothQueuePos] = new Vector2(myCurrentState.X, myCurrentState.Y);
                myMouseSmoothQueuePos = myMouseSmoothQueuePos == NUM_SMOOTHING_SAMPLES - 1 ? 0 : myMouseSmoothQueuePos + 1;

                myCurrentSmoothPosition = Vector2.Zero;
                for (int index = 0; index < NUM_SMOOTHING_SAMPLES; index++)
                    myCurrentSmoothPosition += myMouseSmoothQueue[index];
                myCurrentSmoothPosition /= NUM_SMOOTHING_SAMPLES;               

                myMouseDelta = new DeltaMouseState(myCurrentState, myPrevState, myCurrentSmoothPosition, myCurrentSmoothPosition - myPrevSmoothPosition);

                myPrevSmoothPosition = myCurrentSmoothPosition;

                if (myMouseDelta.LeftButton == ButtonDelta.Released)
                {
                    if (gameTime.TotalGameTime - myLastLeftClickTime < DOUBLE_CLICK_TIME)
                        myMouseDelta.DoubleClicked = true;

                    myLastLeftClickTime = gameTime.TotalGameTime;
                }

                if (myCurrentState.ScrollWheelValue != myPrevState.ScrollWheelValue)
                    OnWheelChanged?.Invoke(null, myCurrentState.ScrollWheelValue - myPrevState.ScrollWheelValue);

                if (myCurrentState.Position != myPrevState.Position)
                    OnPositionChanged?.Invoke(null, myCurrentState);

                foreach (KeyValuePair<MouseButton, MouseListener> m in myWatches)
                    m.Value.Watch(myMouseDelta);
            }
        }
        
        /// <summary>
        /// Sets the position of the cursor on the screen, in screen coordinates
        /// </summary>
        /// <param name="x">The new X coordinate for the mouse</param>
        /// <param name="y">The new Y coordinate for the mouse</param>
        public static void SetPos(int x, int y)
        {
            Mouse.SetPosition(x, y);
            myPrevState = Mouse.GetState();
        }

        /// <summary>
        /// Sets the position of the cursor on the screen, in screen coordinates
        /// </summary>
        /// <param name="pos">The point on the screen to move the mouse to</param>
        public static void SetPos(Point pos)
        {
            SetPos(pos.X, pos.Y);
        }

        /// <summary>
        /// Adds a new event listener to the given mouse button
        /// </summary>
        /// <param name="button">The mouse button to listen to</param>
        /// <param name="listener">The event handler to handle events for this mouse button. This will receive any event tied to the button</param>
        public static void AddWatch(MouseButton button, EventHandler<DeltaMouseState> listener)
        {
            if (!myWatches.ContainsKey(button))
                myWatches.Add(button, new MouseListener(button));

            myWatches[button] += listener;
        }

        /// <summary>
        /// Adds a new event listener to the given mouse button, only listening for the given state
        /// </summary>
        /// <param name="button">The mouse button to listen to</param>
        /// <param name="listenState">The specific state change to listen to</param>
        /// <param name="listener">The event handler to handle events for this mouse button</param>
        public static void AddWatch(MouseButton button, ButtonDelta listenState, EventHandler<DeltaMouseState> listener)
        {
            if (!myWatches.ContainsKey(button))
                myWatches.Add(button, new MouseListener(button));

            myWatches[button][listenState] += listener;
        }

        /// <summary>
        /// Adds a new event listener to the given mouse button, only listening for the given state
        /// </summary>
        /// <param name="button">The mouse button to listen to</param>
        /// <param name="listenState">The specific state to listen for</param>
        /// <param name="listener">The event handler to handle events for this mouse button</param>
        public static void AddWatch(MouseButton button, ButtonState listenState, EventHandler<DeltaMouseState> listener)
        {
            if (!myWatches.ContainsKey(button))
                myWatches.Add(button, new MouseListener(button));

            myWatches[button][(ButtonDelta)listenState] += listener;
        }

        /// <summary>
        /// Flushes the current frame and re-polls the mouse
        /// </summary>
        public static void FlushFrame()
        {
            myPrevState = Mouse.GetState();
            myCurrentState = Mouse.GetState();

            myMouseDelta = new DeltaMouseState(myPrevState, myCurrentState, Vector2.Zero, Vector2.Zero);
        }

        /// <summary>
        /// Utility class for listening to mouse button events
        /// </summary>
        private class MouseListener
        {
            public event EventHandler<DeltaMouseState> OnPressed;
            public event EventHandler<DeltaMouseState> OnReleased;
            public event EventHandler<DeltaMouseState> OnDown;
            public event EventHandler<DeltaMouseState> OnUp;
            public event EventHandler<DeltaMouseState> OnEvent;

            public EventHandler<DeltaMouseState> this[ButtonDelta button]
            {
                get
                {
                    switch (button)
                    {
                        case ButtonDelta.Down:
                            return OnDown;
                        case ButtonDelta.Up:
                            return OnUp;
                        case ButtonDelta.Pressed:
                            return OnPressed;
                        case ButtonDelta.Released:
                            return OnReleased;
                        default:
                            return OnEvent;
                    }
                }
                set
                {
                    switch (button)
                    {
                        case ButtonDelta.Down:
                            OnDown = value;
                            break;
                        case ButtonDelta.Up:
                            OnUp = value;
                            break;
                        case ButtonDelta.Pressed:
                            OnPressed = value;
                            break;
                        case ButtonDelta.Released:
                            OnReleased = value;
                            break;
                        default:
                            OnEvent = value;
                            break;
                    }
                }
            }

            private MouseButton myButton;

            public MouseListener(MouseButton button)
            {
                myButton = button;
            }

            public void Watch(DeltaMouseState change)
            {
                ButtonDelta deltaState = ButtonDelta.Invalid;

                switch (myButton)
                {
                    case MouseButton.Left:
                        deltaState = change.LeftButton;
                        break;
                    case MouseButton.Right:
                        deltaState = change.RightButton;
                        break;
                    case MouseButton.Middle:
                        deltaState = change.MiddleButton;
                        break;
                    case MouseButton.XButton1:
                        deltaState = change.XButton1;
                        break;
                    case MouseButton.XButton2:
                        deltaState = change.XButton2;
                        break;
                }

                if (deltaState != ButtonDelta.Invalid)
                {
                    if (deltaState == ButtonDelta.Pressed)
                        OnPressed?.Invoke(this, change);

                    if (deltaState == ButtonDelta.Released)
                        OnReleased?.Invoke(this, change);

                    if (deltaState == ButtonDelta.Down)
                        OnDown?.Invoke(this, change);

                    if (deltaState == ButtonDelta.Up)
                        OnUp?.Invoke(this, change);

                    OnEvent?.Invoke(this, change);
                }
            }

            public static MouseListener operator +(MouseListener left, EventHandler<DeltaMouseState> right)
            {
                left.OnEvent += right;

                return left;
            }

            public static MouseListener operator -(MouseListener left, EventHandler<DeltaMouseState> right)
            {
                left.OnEvent -= right;

                return left;
            }
        }
    }
}
