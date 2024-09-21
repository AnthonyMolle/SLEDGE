using UnityEngine;
using static UnityEditor.Rendering.CameraUI;
using UnityEngine.ProBuilder;

public class Shoot_Action : ActionNode
{
    public float bulletSpeed;
    public float bulletLifetime;
    public Blackboard.ObjectOptions shootTarget;
    public Blackboard.PrefabOptions bulletType;
    GameObject target;
    GameObject bulletTypeSet;
    Transform gunPoint;

    protected override void OnStart()
    {
        target = blackboard.getObject(shootTarget);
        bulletTypeSet = blackboard.getPrefabLink(bulletType);

        EnemyStatsController stats = blackboard.getCurrentRunner().GetComponent<EnemyStatsController>();
        if(stats == null)
        {
            Debug.LogError("Trying to run a Shoot Action but missing EnemyStatsController.");
        }

        Transform x = stats.bulletSource.transform;
        if (x == null)
        {
            Debug.LogError("Trying to run a Shoot Action on a entity that has no Gun Endpoint object in Stats Controller.");
        }

        gunPoint = x.gameObject.transform;
    }

    protected override void OnStop()
    {
    }
    protected override State OnUpdate()
    {
        GameObject projectile = Instantiate(bulletTypeSet, gunPoint.position, Quaternion.identity);
        //projectiles.Add(projectile);
        projectile.GetComponent<Projectile>().initializeProjectile(target.transform.position, bulletSpeed, bulletLifetime, false, null);
        
        //Instantiate(ShootSound, gameObject.transform.position, Quaternion.identity);

        if (child != null)
        {
            child.state = State.Running;
            child.Update();
        }
        return State.Success;
    }
}
