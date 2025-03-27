using UnityEngine;
using SystemMiami.CombatRefactor;
using SystemMiami.CombatSystem;
using SystemMiami;
using FunkyCode.LightingSettings;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(menuName = "Combat Subaction/Ranged Damage")]
public class RangedDamageSubactionSO : CombatSubactionSO
{
    [SerializeField] private float damageToDeal;
    [SerializeField] private GameObject projectilePrefab; // Projectile prefab
    [SerializeField] private float projectileSpeed = 5f; // Projectile speed

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
        return new RangedDamageCommand(target, action, projectilePrefab, projectileSpeed, finalDamage);
    }
}


public class RangedDamageCommand : ISubactionCommand
{
    private ITargetable target;
    private CombatAction action;
    private GameObject projectilePrefab;
    private float speed;
    private float damage;

    public RangedDamageCommand(ITargetable target, CombatAction action, GameObject projectilePrefab, float speed, float damage)
    {
        this.target = target;
        this.action = action;
        this.projectilePrefab = projectilePrefab;
        this.speed = speed;
        this.damage = damage;
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
                target.GetDamageInterface()?.ReceiveDamage(damage);
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
        target.GetDamageInterface()?.PreviewDamage(damage);
    }
}
