using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySpawnedObject : MonoBehaviour
{
    private float secondsToDestroy = 5;



    // Update is called once per frame
    void Update()
    {
        secondsToDestroy -= Time.deltaTime;

        if (secondsToDestroy <= 0)
            Destroy(gameObject);
    }
}
