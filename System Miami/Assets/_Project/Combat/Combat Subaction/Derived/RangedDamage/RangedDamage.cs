using UnityEngine;
using SystemMiami.CombatRefactor;
using SystemMiami.CombatSystem;
using SystemMiami;

[CreateAssetMenu(menuName = "Combat Subaction/Ranged Damage")]
public class RangedDamageSubactionSO : CombatSubactionSO
{
    [SerializeField] private float damageToDeal;
    [SerializeField] private GameObject projectilePrefab; // Projectile prefab
    [SerializeField] private float projectileSpeed = 5f; // Projectile speed
    private bool perTurn;
    private int durationTurns;

    public override ISubactionCommand GenerateCommand(ITargetable target, CombatAction action)
    {
        float power = 0;
        if (action is AbilityPhysical)
        {
            power = action.User.Stats.GetStat(StatType.PHYSICAL_PWR);
        }
        else if (action is AbilityMagical)
        {
            power = action.User.Stats.GetStat(StatType.MAGICAL_PWR);
        }

        float finalDamage = damageToDeal + power;
        return new RangedDamageCommand(target, action, projectilePrefab, projectileSpeed, finalDamage, perTurn, durationTurns);
    }
}


public class RangedDamageCommand : ISubactionCommand
{
    private ITargetable target;
    private CombatAction action;
    private GameObject projectilePrefab;
    private float speed;
    private float damage;
    private bool perTurn;
    private int durationTurns;

    public RangedDamageCommand(ITargetable target, CombatAction action, GameObject projectilePrefab, float speed, float damage, bool perTurn, int durationTurns)
    {
        this.target = target;
        this.action = action;
        this.projectilePrefab = projectilePrefab;
        this.speed = speed;
        this.damage = damage;
        this.perTurn = perTurn;
        this.durationTurns = durationTurns;
    }

    public void Execute()
    {
        if (projectilePrefab == null) return;

        // Get target position
        Vector3 targetPosition = Vector3.zero;
        if (target is MonoBehaviour targetMb)
        {
            targetPosition = targetMb.transform.position;
        }

        // Instantiate projectile
        GameObject projectile = GameObject.Instantiate(projectilePrefab, action.User.transform.position, Quaternion.identity);
        Projectile projectileScript = projectile.GetComponent<Projectile>();

        // Move towards the target and apply damage on hit
        if (projectileScript != null)
        {
            projectileScript.Launch(targetPosition, speed, () =>
            {
                if (target as Combatant == action.User)
                {
                    return;
                }
                target.GetDamageInterface()?.ReceiveDamage(damage, perTurn, durationTurns);
                Debug.Log(target);
            });
        }
    }

    public void Preview()
    {
        if (target as Combatant == action.User)
        {
            return;
        }
        target.GetDamageInterface()?.PreviewDamage(damage, perTurn, durationTurns);
    }
}
