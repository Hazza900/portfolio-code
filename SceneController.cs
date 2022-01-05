using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController: MonoBehaviour
{
    public static SceneController instance;
    AsyncOperation operation;

    [SerializeField] Image fade;
    [SerializeField] float fadeInDuration = 2;
    [SerializeField] float fadeOutDuration = 2;

    Coroutine fadeCoroutine;
    [SerializeField] bool loading;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public void LoadSceneFromIndex(int index)
    {
        if (!loading)
        {
            loading = true;
            StartCoroutine(LoadSceneAsync(index));
        }
    }

    public void ReloadCurrentScene()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        LoadSceneFromIndex(index);
    }

    public void LoadSceneInSeconds(int index, float time)
    {
        StartCoroutine(LoadSceneInSecondsCoroutine(index, time));
    }

    private IEnumerator LoadSceneInSecondsCoroutine(int index, float time)
    {
        yield return new WaitForSeconds(time);
        LoadSceneFromIndex(index);
    }

    private IEnumerator LoadSceneAsync(int index)
    {
        operation = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
        operation.allowSceneActivation = false;

        //Stop fade in coroutine if running
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        //wait for fade out coroutine
        yield return FadeTransition(1f, fadeOutDuration);

        //wait for async load to complete
        while (operation.progress < 0.9f)
        {
            yield return null;
        }

        //Allow scene to swap once fade out completed and level loaded
        SceneManager.activeSceneChanged += SceneLoaded;
        operation.allowSceneActivation = true;
    }

    private void SceneLoaded(Scene current, Scene next)
    {
        fadeCoroutine = StartCoroutine(FadeTransition(0f, fadeInDuration));
        loading = false;
    }

    private IEnumerator FadeTransition(float alpha, float time)
    {
        float startingAlpha = fade.color.a;
        float timeElapsed = 0;

        while (timeElapsed < time)
        {
            fade.color = new Color(0, 0, 0, Mathf.Lerp(startingAlpha, alpha, timeElapsed / time));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    #region Mask Transition

    public bool loadedFromRitual = false;
    public int maskQuality = 0;
    public int berry;
    public int feather;
    public int horn;
    
    public float CombatMaskInitilization()
    {
        if (loadedFromRitual)
        {
            loadedFromRitual = false;

            SimpleInventory inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<SimpleInventory>();
            inventory.AddItem(0, berry);
            inventory.AddItem(1, feather);
            inventory.AddItem(2, horn);

            return maskQuality / 9.0f;
        }
        else
        {
            return 1.0f;
        }
    }

    #endregion
}
