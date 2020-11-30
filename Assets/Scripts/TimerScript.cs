using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimerScript : MonoBehaviour
{
    private GameObject timerLabel;
    [SerializeField] private float timeRemaining;
    private TextMesh timerText;

    [SerializeField] private GameObject timerHolder;
    [SerializeField] private Animator animator;

    [SerializeField] private float holdBreath1Seconds;
    [SerializeField] private float holdBreath2Seconds;
    [SerializeField] private float holdBreath3Seconds;

    public static event EventHandler<GameObject> TimedDeathEvent;



    private void Start()
    {
        timerLabel = GameObject.Find("FloatingTimer");
        timerText = timerLabel.GetComponent<TextMesh>();

        if (GlobalSettings.onMoon)
            timerLabel.GetComponent<MeshRenderer>().enabled = true;
        else
            timerLabel.GetComponent<MeshRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        timerLabel.transform.position = timerHolder.transform.position;

        if (GlobalSettings.onMoon && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining < 0)
                timeRemaining = 0f;
            
            timerText.text = timeRemaining.ToString();

            if (timeRemaining < holdBreath3Seconds)
                animator.SetInteger("breathState", 3);
            else if (timeRemaining < holdBreath2Seconds)
                animator.SetInteger("breathState", 2);
            else if (timeRemaining < holdBreath1Seconds)
                animator.SetInteger("breathState", 1);
            else
                animator.SetInteger("breathState", 0);


        }
        else if (timeRemaining <= 0)
        {
            OutOfAir();
        }
    }


    /// <summary>
    /// Method to do stuff after running out of air
    /// Player dies, reset scene, etc...
    /// </summary>
    void OutOfAir()
    {
        // *** Use this if we want to send the player to the default spot on the moon...
        // Normally, though, we'll reload from earth

        //if (!GlobalSettings.isDead)
        //{
        //    // (isDead will be set to true in the method triggered from this event)
        //    TimedDeathEvent?.Invoke(this, null);
        //}


        // Load the earth scene for the current level
        if (!GlobalSettings.isDead) // (isDead will be set to true in the method triggered from this event)
        {
            GlobalSettings.levelStarting = true; // This variable to indicate to use the Spawn Start instead of Spawn Default

            TimedDeathEvent?.Invoke(this, null);
        }
    }

    public void OnTimedDeathComplete()
    {
        GlobalSettings.xCoordinate = 0f;

        var currentScene = SceneManager.GetActiveScene().name;
        var currentLevel = currentScene.Substring(5, 3);

        if (GlobalSettings.onMoon)
            SceneManager.LoadScene("Level" + currentLevel + "_earth");
    }
}
