using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitLevelScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GlobalSettings.levelStarting = true;

            var currentLevel = Convert.ToInt32(SceneManager.GetActiveScene().name.Substring(5,3));
            SceneManager.LoadScene("Level" + (currentLevel + 1).ToString("D3") + "_earth");
        }
            
    }
}
