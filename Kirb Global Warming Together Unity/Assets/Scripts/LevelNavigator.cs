using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelNavigator : MonoBehaviour
{
    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void ClosePausePanel()
    {
        gameObject.SetActive(false);
    }

    public void SetTimeScale(float timescale)
    {
        Time.timeScale = timescale;
    }
}
