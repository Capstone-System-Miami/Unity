using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using SystemMiami.CombatSystem;

namespace SystemMiami
{
    public class EnemiesPanel : MonoBehaviour
    {
        [SerializeField] private List<HealthDevPanel> _enemyHealthDevPanels = new List<HealthDevPanel>();

        private void Start()
        {
            StartCoroutine(waitThenFill());
        }

        private IEnumerator waitThenFill()
        {
            yield return new WaitUntil(() => TurnManager.MGR.enemyCharacters.Count > 0);
            setEnemyPanels();
        }

        private void setEnemyPanels()
        {
            List<Combatant> enemies = TurnManager.MGR.enemyCharacters;

            for (int i = 0; i < _enemyHealthDevPanels.Count; i++)
            {
                if (i >= enemies.Count) { print("breaking");break; }

                _enemyHealthDevPanels[i].SetCombatant(enemies[i], i + 1);
            }
        }
    }
}
