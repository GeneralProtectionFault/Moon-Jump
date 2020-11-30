using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenScript : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Level001_earth");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
