using UnityEngine;

public class QuestTracker : MonoBehaviour
{
    public Quest quest;  // Reference to the current quest

    // Assuming this method is called when a monster dies
    void OnMonsterDeath(GameObject monster)
    {
        if (quest != null)
        {
            quest.RegisterKill(monster);  // Pass the killed monster GameObject
        }
    }
}
