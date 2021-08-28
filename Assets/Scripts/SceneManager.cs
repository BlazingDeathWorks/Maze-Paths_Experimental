using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            NextSample();
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            PreviousSample();
        }
    }

    //If Press [Return]
    private void NextSample()
    {
        int nextScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1;
        if(nextScene < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
        {
            RegisterPosition.Reset();
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
        }
    }

    //If Press [BackSpace]
    private void PreviousSample()
    {
        int previousScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 1;
        if(previousScene >= 0)
        {
            RegisterPosition.Reset();
            UnityEngine.SceneManagement.SceneManager.LoadScene(previousScene);
        }
    }
}
