using System.Collections;
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami
{
    public interface IPerTurn : ISubactionCommand
    {
      public bool perTurn { get; }
      public int RemainingTurns { get; }
    }
}
