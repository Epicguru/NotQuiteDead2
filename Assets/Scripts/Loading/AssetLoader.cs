using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AssetLoader : MonoBehaviour
{
    public static AssetLoader Instance;

    public UI_LoadingMenu UI;
    public string GameScene;

    [ReadOnly]
    public bool LoadedStatic = false;

    private delegate void Run();

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void Start()
    {
        StartCoroutine(LoadStaticAssets());
    }

    private IEnumerator LoadStaticAssets()
    {
        if (!LoadedStatic)
        {
            LoadedStatic = false;
            List<KeyValuePair<string, Run>> actions = new List<KeyValuePair<string, Run>>();



            actions.Add(new KeyValuePair<string, Run>("Loading: Items...", () => { Item.LoadAll(); }));
            actions.Add(new KeyValuePair<string, Run>("Loading: Commands...", () => { Commands.LoadCommands(); }));
            actions.Add(new KeyValuePair<string, Run>("Loading: Projectiles...", () => { Projectile.LoadAll(); }));



            int total = actions.Count;
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            // Loop and execute...
            for (int i = 0; i < total; i++)
            {
                var pair = actions[i];
                // Execute...
                UI.Title = pair.Key;
                UI.Percentage = i / total;
                yield return null;
                watch.Restart();
                try
                {
                    pair.Value.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception when loading on step #{0} - '{1}':\n{2}".Form(i, pair.Key, e));
                }
                watch.Stop();
                Debug.Log("'{0}' - Took {1} milliseconds.".Form(pair.Key, watch.ElapsedMilliseconds));
            }

            LoadedStatic = true;
            StartCoroutine(LoadScene());
        }
    }

    private void UnloadStaticAssets()
    {
        if (!LoadedStatic)
        {
            // Unload all here.
            Item.UnloadAll();
            Projectile.UnloadAll();

            // Resources cleanup...
            Resources.UnloadUnusedAssets();

            // Do some GC.
            System.GC.Collect();

            LoadedStatic = false;
        }
    }

    private IEnumerator LoadScene()
    {
        UI.Title = "Loading Map...";
        UI.Percentage = 0f;
        var op = SceneManager.LoadSceneAsync(GameScene);
        while (!op.isDone)
        {
            UI.Percentage = op.progress;
            yield return null;
        }
        OnGameSceneSetUp();        
    }

    private static void OnGameSceneSetUp()
    {
        // Code here is run after the scene has finished loading...
    }
}