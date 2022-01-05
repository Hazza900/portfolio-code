using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [HideInInspector] public EnemySlot slot;

    [SerializeField] protected GameObject model;
    [SerializeField] protected ParticleSystem particle;
    [SerializeField] protected Animator animator;

    [SerializeField] protected float damage;
    [SerializeField] float minSpeed;
    [SerializeField] float maxSpeed;
    protected float speed;

    public bool attackBlocked;
    public bool isAttacking;

    #region initilization

    public virtual void Init()
    {

    }

    public virtual void Init(float aggression)
    {
        particle.Emit(1);

        speed = Mathf.Lerp(minSpeed, maxSpeed, aggression);
        StartCoroutine(Attacking());
    }

    #endregion

    public void AssignedToSlot()
    {
        //Update position and facing of enemy gameobject
        transform.position = slot.vfxSpawnPosition;
        transform.LookAt(Vector3.zero);
        
        //Update position of model
        model.transform.position = slot.enemySpawnPosition;
    }

    public virtual IEnumerator Attacking()
    {
        yield return new WaitForSeconds(0.5f);

        isAttacking = true;

        while (isAttacking)
        {
            //Move enemy towards player until within 2m range
            if (Vector3.Distance(Vector3.zero, model.transform.position) > 2.5)
            {
                model.transform.position = Vector3.MoveTowards(model.transform.position, Vector3.up, Time.deltaTime * speed);

                //Wait for next frame
                yield return 0;
            }
            else
            {
                isAttacking = false;

                if (attackBlocked)
                {
                    AttackBlocked();
                }
                else
                {
                    //Enemy has reached player
                    EnemyReachedPlayer();
                }
                

                //Stop coroutine
                yield break;
            }
        }
    }

    public virtual void EnemyReachedPlayer()
    {
        DealDamage();

        if (slot != null)
        {
            //Remove enemy from slot and destroy enemy
            slot.RemoveEnemyFromSlot(true);
        }
        else
        {
            DestroySelf();
        }
    }

    public virtual void DealDamage()
    {
        RitualController.instance.TakeDamage(damage);
    }

    public void CheckIfAttackBlocked()
    {
        if (Vector3.Distance(Vector3.zero, model.transform.position) > 9)
        {
            Debug.Log("block was too early");
        }
        else
        {
            attackBlocked = true;
        }
    }

    public virtual bool AttackBlocked()
    {
        StopCoroutine(Attacking());
        return true;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
