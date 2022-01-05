using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyCombatController : MonoBehaviour
{
    RitualController ritualController;
    EnemyManager enemyManager;

    [SerializeField] [Range(0, 1)] float maxBossAggressionPoint = 0.8f;
    [SerializeField] float bossAggression;

    bool canSpecialAttack;
    bool specialAttacking;

    [Header("Attack Cooldowns")]

    [SerializeField] float minBasicAttackCooldown;
    [SerializeField] float maxBasicAttackCooldown;
    float basicAttackCooldown;

    [SerializeField] float minPowerAttackCooldown;
    [SerializeField] float maxPowerAttackCooldown;
    float powerAttackCooldown;

    [SerializeField] float minSpecialAttackCooldown;
    [SerializeField] float maxSpecialAttackCooldown;
    float specialAttackCooldown;

    [SerializeField] Transform specialRotationPoint;

    // Start is called before the first frame update
    void Start()
    {
        ritualController = GetComponent<RitualController>();
        enemyManager = GetComponent<EnemyManager>();

        //Calculate initial cooldowns for basic and power attacks
        basicAttackCooldown = CalculateAttackCooldown(minBasicAttackCooldown, maxBasicAttackCooldown);
        powerAttackCooldown = CalculateAttackCooldown(minPowerAttackCooldown, maxPowerAttackCooldown);
    }

    void Update()
    {
        //Calculate boss aggression levels
        bossAggression = ritualController.ritualProgress / (ritualController.ritualLength * maxBossAggressionPoint);
        specialRotationPoint.RotateAround(transform.position, transform.up, Time.deltaTime * 20f);

        if (bossAggression > 0.25 && !canSpecialAttack)
        {
            canSpecialAttack = true;
            //StartCoroutine(BossSpecialAttacks());
        }
            
    }

    float CalculateAttackCooldown(float min, float max)
    {
        //+15%/-15% range of variance on cooldowns 
        float variance = Random.Range(-0.15f, 0.15f);
       
        //Get lerped cooldown value based upon boss aggression
        float cooldown = Mathf.Lerp(max, min, (bossAggression + variance));

        return cooldown;
    }

    public IEnumerator AmbientAttacks()
    {
        //While ritual is active
        while (ritualController.ritualActive)
        {
            while (specialAttacking) yield return null;

            powerAttackCooldown -= Time.deltaTime;
            basicAttackCooldown -= Time.deltaTime;
            
            if (powerAttackCooldown <= 0)
            {
                Enemy enemy = enemyManager.SpawnEnemy(EnemyManager.EnemyType.Boss, -1);
                enemy.Init(bossAggression);

                powerAttackCooldown = CalculateAttackCooldown(minPowerAttackCooldown, maxPowerAttackCooldown);
            }

            if (basicAttackCooldown <= 0)
            {
                int numberOfBasicEnemies = 1;

                //Determine if any power enemies about to attack or are currently attacking
                if (!enemyManager.enemies.OfType<PowerEnemy>().Any() && powerAttackCooldown < 1f)
                {
                    //if none, roll chance to spawn two basic enemies at same time
                    if (Random.Range(0f, 1f) <= bossAggression)
                    {
                        numberOfBasicEnemies = 2;
                    }

                    //Reset cooldown if about to spawn (preventing 3 simultaneous enemies)
                    if (powerAttackCooldown < 2f)
                    {
                        powerAttackCooldown = 2f;
                    }
                }

                for (int i = 0; i < numberOfBasicEnemies; i++)
                {
                    Enemy enemy = enemyManager.SpawnEnemy(EnemyManager.EnemyType.Basic, -1);
                    enemy.Init(bossAggression);
                }

                basicAttackCooldown = CalculateAttackCooldown(minBasicAttackCooldown, maxBasicAttackCooldown);
            }

            

            yield return null;
        }
    }

    public IEnumerator BossSpecialAttacks()
    {
        while (ritualController.ritualActive)
        {
            //Wait for next frame whilst special active
            while (specialAttacking)
                yield return 0;
                
            specialAttackCooldown -= Time.deltaTime;

            if (specialAttackCooldown <= 0)
            {
                specialAttacking = true;

                //Wait for remaining ambient attacks to be completed
                while (enemyManager.enemies.Count > 0)
                    yield return null;

                StartCoroutine(RotatingEnemiesSpecial());
            }

            yield return null;
        }
    }

    public IEnumerator RotatingEnemiesSpecial()
    {
        int numberOfEnemies = 8;

        //calculate anglular division
        float angleSeperation = 360 / numberOfEnemies;

        List<Enemy> enemiesToAttack = new List<Enemy>();

        //Create enemies equally spaced around perimeter
        for (int i=0; i < numberOfEnemies; i++)
        {
            float angleRadians = (angleSeperation * i) * Mathf.Deg2Rad;

            float x = Mathf.Sin(angleRadians);
            float y = Mathf.Cos(angleRadians);

            Vector3 position = new Vector3(x, 0, y);
            position = position * enemyManager.radius;
            Vector3 enemyPosition = position * (enemyManager.radius * 3);

            SpecialEnemy enemy = (SpecialEnemy)enemyManager.SpawnEnemy(EnemyManager.EnemyType.Special);
            enemy.transform.parent = specialRotationPoint;
            enemy.transform.position = position;
            enemy.Init(enemyPosition);

            enemiesToAttack.Add(enemy);
        }

        yield return new WaitForSeconds(2f);

        //Wait till all attacks complete
        while (enemiesToAttack.Count > 0)
        {
            Enemy enemy = enemiesToAttack[Random.Range(0, enemiesToAttack.Count - 1)];
            StartCoroutine(enemy.Attacking());
            enemiesToAttack.Remove(enemy);

            yield return new WaitForSeconds(1.5f);
        }

        specialAttacking = false;
        specialAttackCooldown = CalculateAttackCooldown(minSpecialAttackCooldown, maxSpecialAttackCooldown);

        yield return null;
    }
}
