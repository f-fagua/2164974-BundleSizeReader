using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Addressables.InitializeAsync().Completed += (op) =>
        {
            Caching.ClearCache();
            Caching.compressionEnabled = false;
            Addressables.LoadSceneAsync("Assets/Scenes/SampleScene.unity", LoadSceneMode.Additive).Completed +=
                handle =>
                {
                    Debug.LogError("Cache Path" + Caching.currentCacheForWriting.path);
                };
        };
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
