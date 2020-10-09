using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextSceneButton : MonoBehaviour
{
    [Tooltip("The index of the scene you want loaded, by default it will load the next scene in the build index")]
    [SerializeField] int m_nextSceneId = -1;

    Scene currentScene;
    public Button loadSceneButton;

    void Start()
    {
        currentScene = SceneManager.GetActiveScene();
        loadSceneButton.onClick.AddListener(ChangeScene);
    }

    void ChangeScene()
    {
        ValidateSceneId();

        SceneManager.LoadScene(m_nextSceneId, LoadSceneMode.Single);
    }

    private void ValidateSceneId()
    {
        //if its not set or otherwise negative go to the next scene
        if (m_nextSceneId < 0)
        {
            m_nextSceneId = currentScene.buildIndex + 1;
        }

        //if its not a valid scene go to the start scene
        if (m_nextSceneId >= SceneManager.sceneCountInBuildSettings)
        {
            m_nextSceneId = 0;
        }
    }
}
