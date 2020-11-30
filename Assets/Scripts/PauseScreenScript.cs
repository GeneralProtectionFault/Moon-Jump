using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseScreenScript : MonoBehaviour
{
    public static GameObject pauseScreen { get; private set; }
    public static GameObject pauseCanvas { get; private set; }

    

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
        PauseScreenInitialize();
    }

    private void OnSceneLoad(Scene theScene, LoadSceneMode mode)
    {
        PauseScreenInitialize();
    }


    private void PauseScreenInitialize()
    {

        if (pauseCanvas == null)
            pauseCanvas = GameObject.Find("CanvasPauseScreen");
        // UnityEngine.Debug.Log(pauseCanvas);
        if (pauseScreen == null)
            pauseScreen = pauseCanvas.transform.GetChild(0).gameObject;
        // UnityEngine.Debug.Log(pauseScreen);


        if (pauseScreen.activeInHierarchy)
        {
            // UnityEngine.Debug.Log("Disappearing the pause!");
            pauseScreen.SetActive(false);
            Time.timeScale = 1f;
        }
        // pauseScreen.SetActive(false);
    }


    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseScreen.SetActive(false);
    }


    public void Restart()
    {
        //CharacterController2D.spawnEmptyObject = GameObject.Find("SpawnStart");
        CharacterController2D.RestartLevel();
    }


    public void Quit()
    {
        Application.Quit();
    }




    public void Pause(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            // UnityEngine.Debug.Log(pauseScreen);
            if (pauseScreen != null)
            {
                if (!pauseScreen.activeInHierarchy)
                {
                    pauseScreen.SetActive(true);
                    Time.timeScale = 0f;
                }
                else
                {
                    pauseScreen.SetActive(false);
                    Time.timeScale = 1f;
                }
            }
        }
    }
}
