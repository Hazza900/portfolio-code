using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public int numberOfSlots = 16;
    public float radius;
    public float segmentAngle;

    public EnemySlot[] enemySlots;
    public List<Enemy> enemies;

    [SerializeField] CombatParticleSpawning particleSpawner;

    public enum EnemyType { Basic, Boss, Special }

    [Header("Enemy Prefabs")]
    [SerializeField] GameObject basicPrefab;
    [SerializeField] GameObject bossPrefab;
    [SerializeField] GameObject specialPrefab;

    [Header("Debug Stuff")]
    public GameObject slotPrefab;

    void Awake()
    {
        segmentAngle = 360 / (float)numberOfSlots;
        enemies = new List<Enemy>();

        GenerateEnemySlots();
    }

    #region Slot Generation and Querrying

    void GenerateEnemySlots()
    {
        enemySlots = new EnemySlot[numberOfSlots];

        for (int i = 0; i < enemySlots.Length; i++)
        {
            //Calculate slot spawn position for enemy
            float angle = (segmentAngle * i);
            float angleRadians = angle * Mathf.Deg2Rad;

            float x = Mathf.Sin(angleRadians);
            float y = Mathf.Cos(angleRadians);
            Vector3 position = new Vector3(x, 0, y);
            
            Vector3 enemySpawnPosition = position * (radius * 3f);
            position = position * radius;

            //set position where enemy will spawn

            //Temporary to show slot spawn positions
            //Instantiate(slotPrefab, position, Quaternion.identity, transform);
            //Instantiate(slotPrefab, enemySpawnPosition, Quaternion.identity, transform);

            enemySlots[i] = new EnemySlot(this, i, angle, position, enemySpawnPosition);
        }
    }

    int GetRandomEmptySlotIndex()
    {
        int availibleSlots = enemySlots.Length - enemies.Count;

        //Return dud value if all slots are in use (THIS SHOULD NEVER HAPPEN)
        if (availibleSlots == 0)
            return -1;

        //Get random value to determine which empty slot will be selected (ie, 0 means first empty slot. 1 means second etc...) 
        int randomIndex = Random.Range(0, availibleSlots);
        int tracker = 0;

        //Increment through each slot. For each empty slot, see if it's the random relative index.
        //return the actual index of this slot if it is
        for (int i = 0; i < enemySlots.Length; i++)
        {
            if (enemySlots[i].enemy != null)
                continue;

            if (tracker != randomIndex)
            {
                tracker++;
                continue;
            }
            else
            {
                return i;
            }
        }

        //Code shouldn't use this return statement, just to satisfy debugger
        return -1;
    }

    #endregion

    public Enemy GetRandomEnemy()
    {
        if (enemies.Count <= 0)
            return null;

        return enemies[Random.Range(0, enemies.Count - 1)];
    }

    public void DestroyAllEnemies()
    {
        foreach (Enemy enemy in enemies)
        {
            enemy.AttackBlocked();
        }
    }

    #region Enemy Spawning

    //Generates random index to spawn enemy at

    public void SpawnButton()
    {
        Enemy enemy = SpawnEnemy(EnemyType.Basic, -1);
        enemy.Init(0f);
    }

    public void SpawnButton2()
    {
        Enemy enemy = SpawnEnemy(EnemyType.Boss, 12);
        enemy.Init(0f);
    }

    public void Butt()
    {
        SceneController.instance.LoadSceneFromIndex(0);
    }

    public Enemy SpawnEnemy(EnemyType type, int slotIndex)
    {
        int index = slotIndex;

        if (slotIndex == -1)
        {
            //Get random slot index
            index = GetRandomEmptySlotIndex();
            if (index == -1)
            {
                Debug.Log("No empty slots remaining");
                return null;
            } 
        }

        //Create enemy and assign to slot
        Enemy enemy = SpawnEnemy(type);
        enemySlots[index].AssignEnemyToSlot(enemy);

        return enemy;
    }

    public Enemy SpawnEnemy(EnemyType type)
    {
        GameObject enemyGameObject;

        switch (type)
        {
            case EnemyType.Basic:
                enemyGameObject = basicPrefab;
                break;

            case EnemyType.Boss:
                enemyGameObject = bossPrefab;
                break;

            case EnemyType.Special:
                enemyGameObject = specialPrefab;
                break;

            default:
                enemyGameObject = basicPrefab;
                break;
        }

        //Instantiate enemy prefab and get component to return
        enemyGameObject = Instantiate(enemyGameObject, Vector3.zero, Quaternion.identity, transform);
        Enemy enemy = enemyGameObject.GetComponent<Enemy>();
        enemies.Add(enemy);

        return enemy;
    }

    #endregion

    #region Enemy Slot Manipulation

    //randomly shuffle enemies between occupied slots
    public void ShuffleSlots()
    {
        //List of slots that had enemies in them
        List<EnemySlot> slotsTemp = new List<EnemySlot>();

        foreach (Enemy enemy in enemies)
        {
            //add reference to this slot to temporary list
            slotsTemp.Add(enemy.slot);

            //decouple each enemy from their slot
            enemy.slot.RemoveEnemyFromSlot(false);
        }

        foreach (Enemy enemy in enemies)
        {
            //Get random slot from temporary list
            int index = Random.Range(0, slotsTemp.Count);

            //Assign enemy to the slot
            slotsTemp[index].AssignEnemyToSlot(enemy);

            //Remove slot from temporary list
            slotsTemp.RemoveAt(index);
        }
    }

    #endregion

    #region Angle stuff
    int GetIndexFromAngle(Vector3 position)
    {
        float angle = Vector3.SignedAngle(Vector3.forward, position, Vector3.up);
        angle = angle < 0 ? 360 + angle : angle;
        return Mathf.RoundToInt(angle / 22.5f);
    }
    #endregion
}
