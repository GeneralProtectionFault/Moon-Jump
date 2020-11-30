using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawn : MonoBehaviour
{
    public static void SpawnPlayer(GameObject Player, bool IsRespawnFromDeath, Vector3 spawnLocation = new Vector3())
    {
        GameObject player;

        var scene = SceneManager.GetActiveScene();

        // If no spawn location is specified, use the location of the SpawnDefault empty game object that should :x...be in the scene...
        if (spawnLocation == Vector3.zero)
        {
            // UnityEngine.Debug.Log("Spawning at default location");
            spawnLocation = GameObject.Find("SpawnDefault").transform.position;
        }

        // Only take the x position into account if switching between earth & moon
        if (IsRespawnFromDeath || GlobalSettings.levelStarting)
        {
            player = Instantiate(Player, new Vector3(spawnLocation.x, spawnLocation.y), Quaternion.identity);
        }
        else
        {
            if (GlobalSettings.xCoordinate == 0f)
                player = Instantiate(Player, new Vector3(spawnLocation.x, spawnLocation.y), Quaternion.identity);
            else
                player = Instantiate(Player, new Vector3(GlobalSettings.xCoordinate, spawnLocation.y), Quaternion.identity);
        }

        // Behavior if on the moon
        if (scene.name.Contains("moon"))
        {
            GlobalSettings.onMoon = true;
            player.GetComponent<Rigidbody2D>().gravityScale = GlobalSettings.moonGravity;
        }
        // Behavior if on the earth
        else
        {
            GlobalSettings.onMoon = false;
            player.GetComponent<Rigidbody2D>().gravityScale = GlobalSettings.earthGravity;
        }



        GlobalSettings.levelStarting = false;
        GlobalSettings.isDead = false;
    }
}
