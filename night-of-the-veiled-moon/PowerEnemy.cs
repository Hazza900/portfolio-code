using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PowerEnemy : Enemy
{
    bool isRetreating;
    [SerializeField] DampedTransform[] dampers;

    //Animations
    int lunge;
    int snap;
    int recoil;
    int retreat;

    public override void Init(float aggression)
    {
        base.Init(aggression);

        lunge = Animator.StringToHash("SK_Enemy@Lunge");
        snap = Animator.StringToHash("SK_Enemy@Snap");
        recoil = Animator.StringToHash("SK_Enemy@Recoil");
        retreat = Animator.StringToHash("SK_Enemy@Retreat");

        animator.Play(lunge);
    }

    public override void EnemyReachedPlayer()
    {
        StartCoroutine(SnapAnimation());
    }

    public IEnumerator SnapAnimation()
    {
        animator.Play(snap);

        while (true)
        {
            //continue to move forward
            transform.position += transform.forward * Time.deltaTime * 7;
            yield return null;
        }
    }

    public override bool AttackBlocked()
    {
        if (base.AttackBlocked())
        {
            StartCoroutine(Retreat());
            return true;
        }

        return false;
    }

    IEnumerator Retreat()
    {
        isRetreating = true;

        animator.Play(recoil);
        yield return new WaitForSeconds(1.25f);
        animator.Play(retreat);

        //Move back to spawn position
        while (isRetreating)
        {
            if (Vector3.Distance(Vector3.zero, model.transform.position) < 20)
            {
                //Move back to edge of circle
                model.transform.position = Vector3.MoveTowards(model.transform.position, slot.enemySpawnPosition, Time.deltaTime * 10);

                //Wait for next frame
                yield return 0;
            }
            else
            {
                //delete enemy once out of view
                slot.RemoveEnemyFromSlot(true);
                yield break;
            }
        }
    }

    void SetDampers(float weight)
    {
        foreach (DampedTransform damper in dampers)
        {
            damper.weight = Mathf.Clamp01(weight);
        }
    }
}
