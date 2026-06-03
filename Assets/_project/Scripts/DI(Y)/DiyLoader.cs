using UnityEngine;
using UnityEngine.SceneManagement;

public static class DiyLoader
{
    static DiyLoader()
    {
        SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
    }

    private static void SceneManagerOnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }
}
