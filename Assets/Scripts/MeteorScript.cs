using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorScript : MonoBehaviour
{
    private float secondsToDestroy = 9;
    [SerializeField] private float rotationMultiplier;


    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, 0f, rotationMultiplier, Space.Self);


        secondsToDestroy -= Time.deltaTime;

        if (secondsToDestroy <= 0)
            Destroy(gameObject);
    }
}
