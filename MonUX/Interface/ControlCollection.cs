using System;
using System.Collections;
using System.Collections.Generic;

namespace MonUX.Interface
{
    public class ControlCollection : IEnumerable<Control>
    {
        private List<Control> myInternalCollection;

        public Control this[int index]
        {
            get { return myInternalCollection[index]; }
            set { myInternalCollection[index] = value; }
        }

        public int Length
        {
            get { return myInternalCollection.Count; }
        }

        public ControlCollection()
        {
            myInternalCollection = new List<Control>();
        }

        public void Add(Control control)
        {
            myInternalCollection.Add(control);
        }

        public bool Remove(Control control)
        {
            return myInternalCollection.Remove(control);
        }

        public void RemoveAt(int index)
        {
            myInternalCollection.RemoveAt(index);
        }

        public IEnumerator<Control> GetEnumerator()
        {
            return myInternalCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return myInternalCollection.GetEnumerator();
        }
    }
}