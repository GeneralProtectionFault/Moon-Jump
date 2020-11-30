using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardScript : MonoBehaviour
{
    [SerializeField] private GameObject spawnEmptyObject;

    public static EventHandler<GameObject> DeathEvent;


    private void Start()
    {
        if (spawnEmptyObject == null)
        {
            spawnEmptyObject = GameObject.Find("SpawnStart");
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        var otherObject = collision.gameObject;

        // UnityEngine.Debug.Log(spawnEmptyObject);

        if (otherObject.tag == "Player")
        {
            GlobalSettings.levelStarting = true; // This variable to indicate to use the Spawn Start instead of Spawn Default
            DeathEvent?.Invoke(this, spawnEmptyObject);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        var otherObject = collision.gameObject;

        // UnityEngine.Debug.Log(spawnEmptyObject);

        if (otherObject.tag == "Player")
        {
            GlobalSettings.levelStarting = true; // This variable to indicate to use the Spawn Start instead of Spawn Default
            DeathEvent?.Invoke(this, spawnEmptyObject);
        }
    }
}
