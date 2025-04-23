using System.Collections;
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami
{
    public interface IPerTurn : ISubactionCommand
    {
      public int RemainingTurns { get; }
    }
}
