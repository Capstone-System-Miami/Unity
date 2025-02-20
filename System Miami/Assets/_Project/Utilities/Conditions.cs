using System;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami.Utilities
{
    public class Conditions
    {
        private List<Func<bool>> conditions = new();

        public Conditions()
        {
            conditions = new();
        }

        public Conditions(List<Func<bool>> conditions)
        {
            this.conditions = new(conditions);
        }

        public void Add(Func<bool> condition)
        {
            conditions.Add(condition);
        }

        public void Remove(Func<bool> condition)
        {
            conditions.Remove(condition);
        }

        public bool AllMet()
        {
            foreach (Func<bool> condition in conditions)
            {
                if (condition == null) { continue; }
                if (!condition())
                {
                    return false;
                }
            }
            return true;
        }

        public bool AnyMet()
        {
            foreach (Func<bool> condition in conditions)
            {
                if (condition == null) { continue; }
                if (condition())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
