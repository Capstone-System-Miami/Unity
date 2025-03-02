using UnityEngine;
using System;
using System.Collections.Generic;

namespace SystemMiami.ui
{
    public class DialogueEventArgs : EventArgs
    {
        public readonly object client;
        public readonly bool wrapStart;
        public readonly bool wrapEnd;
        public readonly bool allowCloseEarly;
        public readonly string header;
        public readonly string[] messages;

        public DialogueEventArgs(
            object client,
            bool wrapStart,
            bool wrapEnd,
            bool allowCloseEarly,
            string header,
            string[] messages)
        {
            this.client = client;
            this.wrapStart = wrapStart;
            this.wrapEnd = wrapEnd;
            this.allowCloseEarly = allowCloseEarly;
            this.header = header;
            this.messages = messages;
        }
    }
}
