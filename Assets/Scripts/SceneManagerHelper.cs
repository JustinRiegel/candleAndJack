using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManagerHelper : MonoBehaviour
{
    public static SceneManagerHelper instance;

    void Awake()
    {
        //singleton! (only one can survive!!)
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void ChangeScene(int scene)
    {
        scene = ValidateSceneId(scene);

        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    private int ValidateSceneId(int scene)
    {
        if (scene < 0) //if its not set or otherwise negative go to the next scene
        {
            scene = SceneManager.GetActiveScene().buildIndex + 1;
        }
        else if (scene > SceneManager.sceneCountInBuildSettings) //if its not a valid scene go to the start scene
        {
            scene = 0;
        }
        return scene;
    }
}
