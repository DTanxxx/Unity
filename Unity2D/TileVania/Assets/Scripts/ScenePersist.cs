using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePersist : MonoBehaviour
{
    int startingSceneIndex;

    private void Awake()
    {
        startingSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int numScenePersist = FindObjectsOfType<ScenePersist>().Length;
        ScenePersist previousScene = FindObjectsOfType<ScenePersist>()[numScenePersist - 1];

        if (numScenePersist > 1
            && previousScene.startingSceneIndex == SceneManager.GetActiveScene().buildIndex)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex != startingSceneIndex)
        {
            Destroy(gameObject);
        }
    }
}
