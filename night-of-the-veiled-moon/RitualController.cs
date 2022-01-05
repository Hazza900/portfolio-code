using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RitualController : MonoBehaviour
{
    public static RitualController instance;
    EnemyCombatController enemyCombatController;
    EnemyManager enemyManager;
    
    [Header("Ritual Settings")]
    public float ritualLength = 60f;
    public float ritualProgress = 0f;
    public float ritualRate = 1f;
    public bool ritualActive;

    [Header("Mask Settings")]
    [SerializeField] float maskQuality; //0-1 value determined by number of correct reagents

    [Header("Player Vitals")]
    public float minPotentialHealth = 30;
    public float maxPotentialHealth = 100;
    float maxHealth;
    public float currentHealth;
    public Image damageNotification;

    [Header("Ritual VFX")]
    public Renderer vfx;
    Material vfxMat;

    [Header("Debug UI")]
    public Image ritualFill;
    public Image healthFill;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        enemyCombatController = GetComponent<EnemyCombatController>();
        enemyManager = GetComponent<EnemyManager>();

        maskQuality = SceneController.instance.CombatMaskInitilization();
        maxHealth = Mathf.Lerp(minPotentialHealth, maxPotentialHealth, Mathf.Clamp01(maskQuality));
        currentHealth = maxHealth;

        vfxMat = vfx.material;

        Invoke("StartRitual", 3f);
    }

    void Update()
    {
        //Update ritual progression vfx
        vfxMat.SetFloat("Vector1_55ce67a8926c4e22b1f28c251b0f1822", Mathf.Lerp(-0.41f, 1f, (ritualProgress / ritualLength)));

        ritualFill.fillAmount = ritualProgress / ritualLength;
        healthFill.fillAmount = currentHealth / maxHealth;

        if (ritualActive)
        {
            if (currentHealth <= 0)
            {
                RitualFailed();
                return;
            }

            if (ritualProgress >= ritualLength)
            {
                RitualComplete();
                return;
            }

            ritualProgress += Time.deltaTime * ritualRate;
        }
    }

    public void TakeDamage(float damage)
    {
        if (ritualActive)
        {
            currentHealth -= damage;

            //Play damage notifier
            if (damageNotificationCoroutine != null)
                StopCoroutine(damageNotificationCoroutine);

            damageNotificationCoroutine = StartCoroutine(DamageNotificationAnimation(currentHealth <= 0));
        }
    }

    Coroutine damageNotificationCoroutine;
    [SerializeField] float opacityIncreaseTime;
    [SerializeField] float opacityDecreaseTime;
    [SerializeField] float opacityDelay;

    private IEnumerator DamageNotificationAnimation(bool lethal)
    {
        float alpha = damageNotification.color.a;
        //Get percentage complete how full the alpha currently is
        float t = Mathf.InverseLerp(0f, 0.7f, alpha);
        float timeElapsed = Mathf.Lerp(0, opacityIncreaseTime, t);
        
        while (timeElapsed < opacityIncreaseTime)
        {
            damageNotification.color = new Color(damageNotification.color.r, damageNotification.color.g, damageNotification.color.b, alpha);
            alpha = Mathf.Lerp(0, 0.7f, timeElapsed / opacityIncreaseTime);
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        if (lethal)
        {
            yield break;
        }

        yield return new WaitForSeconds(opacityDelay);
        timeElapsed = 0f;

        while (timeElapsed < opacityDecreaseTime)
        {
            alpha = Mathf.Lerp(0.7f, 0f, timeElapsed / opacityDecreaseTime);
            timeElapsed += Time.deltaTime;
            damageNotification.color = new Color(damageNotification.color.r, damageNotification.color.g, damageNotification.color.b, alpha);

            yield return null;
        }

        damageNotification.color = new Color(damageNotification.color.r, damageNotification.color.g, damageNotification.color.b, 0f);
    }

    void RitualComplete()
    {
        Debug.Log("Ritual Complete!");
        ritualActive = false;

        enemyManager.DestroyAllEnemies();

        SceneController.instance.LoadSceneInSeconds(0, 3f);
    }

    void RitualFailed()
    {
        ritualActive = false;

        SceneController.instance.LoadSceneInSeconds(2, 2f);

        Debug.Log("Ritual Failed!");
    }

    public void StartRitual()
    {
        ritualActive = true;
        
        StartCoroutine(enemyCombatController.AmbientAttacks());
        //StartCoroutine(enemyCombatController.BossSpecialAttacks());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 2);
    }
}