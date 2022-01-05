using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySlot
{
    public EnemyManager enemyManager;

    public int index;
    public float angle;

    //vfx spawn position
    public Vector3 vfxSpawnPosition;
    //enemy model spawn position
    public Vector3 enemySpawnPosition;

    public Enemy enemy;

    #region Constructors

    public EnemySlot()
    {
        //Default constructor
    }

    public EnemySlot(EnemyManager manager, int index, float angle, Vector3 vfxSpawnPosition, Vector3 enemySpawnPosition)
    {
        this.enemyManager = manager;
        this.index = index;
        this.angle = angle;
        this.vfxSpawnPosition = vfxSpawnPosition;
        this.enemySpawnPosition = enemySpawnPosition;
    }

    #endregion

    public void AssignEnemyToSlot(Enemy enemy)
    {
        if (this.enemy != null)
            RemoveEnemyFromSlot(true);

        this.enemy = enemy;
        this.enemy.slot = this;
        this.enemy.AssignedToSlot();
    }

    public void RemoveEnemyFromSlot(bool destroyEnemy)
    {
        if (this.enemy == null)
            return;

        this.enemy.slot = null;

        if (destroyEnemy)
        {
            enemyManager.enemies.Remove(enemy);
            enemy.DestroySelf();
        }

        this.enemy = null;
    }
}
