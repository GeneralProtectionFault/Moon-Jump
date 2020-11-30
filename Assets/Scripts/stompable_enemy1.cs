using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stompable_enemy1 : MonoBehaviour
{
    [SerializeField] private GameObject spawnEmptyObject;



    private void OnCollisionEnter2D(Collision2D collision)
    {
       //  UnityEngine.Debug.Log("Collision direction: " + (collision.transform.position - transform.position).normalized.y);
        if (collision.gameObject.tag == "Player")
        {
            var collisionDirection = (collision.transform.position - transform.position).normalized;
            if (collisionDirection.y >  .90f)
                KillMe();
            else
                KillPlayer(collision.gameObject);
        }

    }

    private void KillPlayer(GameObject player)
    {
        Destroy(player);

        Vector3 spawnLocation = new Vector3();
        if (spawnEmptyObject != null)
            spawnLocation = new Vector3(spawnEmptyObject.transform.position.x, spawnEmptyObject.transform.position.y);

        // Respawn the player
        if (spawnLocation == Vector3.zero)
            PlayerSpawn.SpawnPlayer((GameObject)Resources.Load("Player"), true);
        else
            PlayerSpawn.SpawnPlayer((GameObject)Resources.Load("Player"), true, spawnLocation);
    }

    private void KillMe()
    {
        Destroy(this.gameObject);
    }
}
