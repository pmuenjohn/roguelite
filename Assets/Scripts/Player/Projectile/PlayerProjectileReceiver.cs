using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerProjectileReceiver : MonoBehaviour
{
    public int projectileCount = 0;
    public GameObject pickup;

    public List<ProjectileEffectSO> effectSOs = new List<ProjectileEffectSO>();
    
    public void Start()
    {
        BasicEnemy enemy = GetComponent<BasicEnemy>();
        if(enemy)
            enemy.OnEnemyDied.AddListener(DropProjectiles);
    }

    public void AddProjectile(ProjectileEffectSO projectileEffect)
    {
        projectileCount++;
        effectSOs.Add(projectileEffect);
        projectileEffect.Initialise(this.gameObject);
    }

    public void DropProjectiles(BasicEnemy enemy)
    {
        Debug.Log("DropProjectiles" + projectileCount);
        if (projectileCount > 0)
        {
            for (int i = 0; i < projectileCount; i++)
            {
                Instantiate(pickup, transform.position, Quaternion.identity);
            }
        }
    }
}