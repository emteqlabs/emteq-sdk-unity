using System;
using System.Collections.Generic;
using UnityEngine;

namespace EmteqLabs
{
    public class UnityThreadRelay
    {
        private List<Action> pending = new List<Action>();

        /** Post call to the pending queue */
        public void Invoke(Action fn)
        {
            lock (this.pending)
            {
                this.pending.Add(fn);
            }
        }

        private void InvokePending()
        {
            lock (this.pending)
            {
                foreach (Action action in this.pending)
                {
                    action();
                }

                this.pending.Clear();
            }
        }

        /** Call from destination context */
        public void Update()
        {
            this.InvokePending();
        }
    }
}