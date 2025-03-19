using FunkyCode.Utilities;
using SystemMiami;
using UnityEngine;
using SystemMiami.CombatRefactor;
using SystemMiami.CombatSystem;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Combat Subaction/SpawnVFXSubaction")]
public class SpawnVFXSubactionSO : CombatSubactionSO
{ 
    [SerializeField] private GameObject VFXPrefab;
    
    
    
    public override ISubactionCommand GenerateCommand(ITargetable target, CombatAction action)
    {
        // Return a new command that knows which prefab to spawn
        // and which target to spawn it on.
        
        return new SpawnVFXCommand(target, action, VFXPrefab);
    }
}

public class SpawnVFXCommand : ISubactionCommand
{
    private ITargetable target;
    private CombatAction _action;
    private GameObject VFXPrefab;

    public SpawnVFXCommand(ITargetable target, CombatAction action, GameObject vfxPrefab)
    {
        this.target = target;
        _action = action;
        VFXPrefab = vfxPrefab;
    }

    
    public void Preview()
    {
        
    }

    public void Execute()
    {
        Debug.Log("SpawnVFXCommand");
        if (VFXPrefab == null) return;
        if (target is Combatant combatant)
        {
            Vector3 spawnPosition = combatant.transform.position;
            Quaternion spawnRotation = combatant.transform.rotation;

            GameObject.Instantiate(VFXPrefab, spawnPosition, spawnRotation);
        }
        
         if (target is OverlayTile tile)
         {
            Vector3 spawnPos = tile.transform.position;
            GameObject.Instantiate(VFXPrefab, spawnPos, Quaternion.identity);
         }
         else if (target is Obstacle obstacle)
         {
             Vector3 spawnPos = obstacle.transform.position;
             GameObject.Instantiate(VFXPrefab, spawnPos, Quaternion.identity);
         }
         
    }
}
