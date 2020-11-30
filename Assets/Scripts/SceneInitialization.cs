using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SceneInitialization : MonoBehaviour
{
    [SerializeField] GameObject Player;

    private GameObject entryArrow;
    


    // Start is called before the first frame update
    void Start()
    {
        // The canvas and its child objects are prefabs
        // Unity seems to be a bit wonky when dragging prefabs into prefabs (stuff not already in the scene, maybe a reference issue...)
        // This is an attempt to get the specific button into the current menu object so non-mouse navigation will work without clicking somewhere first.
        var pauseCanvas = GameObject.Find("CanvasPauseScreen");
        var resumeButton = pauseCanvas.transform.Find("PanelPause/ButtonResume");

        EventSystem eventSystem = EventSystem.current;

        eventSystem.firstSelectedGameObject = resumeButton.gameObject;


        var currentScene = SceneManager.GetActiveScene().name;
        var currentArea = currentScene.Substring(9);





        if (currentArea != "sun")
            GlobalSettings.onSun = false;
        else
        {
            GlobalSettings.sunIsOut = false;
            GlobalSettings.onSun = true;
        }

        // If the level is just starting, don't start in the air...
        if (GlobalSettings.levelStarting)
        {
            var startObject = GameObject.Find("SpawnStart");
            if (startObject == null)
                startObject = GameObject.Find("SpawnDefault");

            
            if (GameObject.Find("Player") == null)
                PlayerSpawn.SpawnPlayer(Player, false, startObject.transform.position);

            
        }
        else // Otherwise, spawn in the air, because the player will be arriving in the scene from jumping off the other scene...
        {
            // UnityEngine.Debug.Log("Spawning player at scene start");
            PlayerSpawn.SpawnPlayer(Player, false);

            // Spawn the indicator to show the entry point to the scene
            float indicatorYPosition = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).y;

            // If we're on earth, put the arrow at the top instead of the bottom, and flip the dumb thing over
            if (!GlobalSettings.onMoon)
            {
                indicatorYPosition = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, 1f)).y;
                entryArrow = Instantiate((GameObject)Resources.Load("EntryArrow"), new Vector3(GlobalSettings.xCoordinate, indicatorYPosition), Quaternion.identity);
                
                Vector3 objectScale = entryArrow.transform.localScale;
                objectScale *= -1;
                entryArrow.transform.localScale = objectScale;
            }

            entryArrow = Instantiate((GameObject)Resources.Load("EntryArrow"), new Vector3(GlobalSettings.xCoordinate, indicatorYPosition), Quaternion.identity);
        }

    }


    private void Update()
    {
        if (entryArrow != null)
        {
            // Keep the arrow on the edge of the screen even if the camera moves vertically
            float yCoordinate = 0f;

            if (!GlobalSettings.onMoon)
                 yCoordinate = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, 1f)).y;
            else
                yCoordinate = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).y;

            entryArrow.transform.position = new Vector3(GlobalSettings.xCoordinate, yCoordinate, 0f);
        }
    }


}
