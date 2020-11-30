using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private GameObject player;

    private Vector3 camVelocity = Vector3.zero;
    private float sceneLeftEdgeXValue;
    private float sceneRightEdgeXValue;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        sceneLeftEdgeXValue = GameObject.Find("SceneEdgeLeft").transform.position.x;
        sceneRightEdgeXValue = GameObject.Find("SceneEdgeRight").transform.position.x;

        UnityEngine.Debug.Log(sceneLeftEdgeXValue);
   
    }




    private void LateUpdate()
    {
        // CHANGE LOGIC - use viewport and edge of scene!
        var leftEdgeLocation = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x;
        var rightEdgeLocation = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, 1f)).y;

        UnityEngine.Debug.Log(leftEdgeLocation);

        if (player != null)
        {
            if (leftEdgeLocation <= sceneLeftEdgeXValue || rightEdgeLocation >= sceneRightEdgeXValue)
                return;

            var desiredPosition = new Vector3(player.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
            Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, desiredPosition, ref camVelocity, .2f);
        }
    }
}
