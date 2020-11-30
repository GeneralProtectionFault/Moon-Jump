using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorController : MonoBehaviour
{
    [SerializeField] private float secondsBetweenMeteors;
    [SerializeField] private float meteorSpeed;
    [SerializeField] private int meteorAngle;

    [SerializeField] private GameObject meteorObject;


    private bool spawning = false;
    private IEnumerator spawnCoroutine;
    

    void Start()
    {
        spawnCoroutine = WaitToSpawn(secondsBetweenMeteors);
        StartCoroutine(spawnCoroutine);
    }



    private IEnumerator WaitToSpawn(float waitTime)
    {
        while (true)
        {
            var meteor = Instantiate(meteorObject, transform.position, Quaternion.identity);
            AddForceAtAngle(meteor.GetComponent<Rigidbody2D>(), meteorSpeed, meteorAngle);


            yield return new WaitForSeconds(waitTime);
        }
    }




    public void AddForceAtAngle(Rigidbody2D rb, float force, float angle)
    {
        float xcomponent = Mathf.Cos(angle * Mathf.PI / 180) * force;
        float ycomponent = Mathf.Sin(angle * Mathf.PI / 180) * force;

        rb.AddForce(new Vector2(xcomponent,ycomponent));
    }


}
