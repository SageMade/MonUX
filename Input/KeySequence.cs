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
    /// Represents a sequence of keys that can be pressed in order to perform an action
    /// </summary>
    public class KeySequence
    {
        private List<Keys> myKeyList;
        private TimeSpan myLastKeyPressTime;
        private TimeSpan myMaxSequenceTime;

        private int myCurrentKey;

        public event EventHandler OnInvoked;

        public KeySequence(TimeSpan minTiming, params Keys[] keys)
        {
            if (keys.Length == 0)
                throw new ArgumentException("keys");

            myMaxSequenceTime = minTiming;

            myKeyList = new List<Keys>(keys);
        }
        public KeySequence(params Keys[] keys) : this(new TimeSpan(0, 0, 0, 1, 00), keys) { }

        public void CheckKeys(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - myLastKeyPressTime > myMaxSequenceTime)
            {
                myLastKeyPressTime = gameTime.TotalGameTime;
                myCurrentKey = 0;
            }

            if (KeyboardManager.GetDelta(myKeyList[myCurrentKey]) == ButtonDelta.Pressed)
            {
                myCurrentKey++;
                if (myCurrentKey >= myKeyList.Count)
                {
                    OnInvoked?.Invoke(this, EventArgs.Empty);
                    myCurrentKey = 0;
                }
            }
        }
    }
}
