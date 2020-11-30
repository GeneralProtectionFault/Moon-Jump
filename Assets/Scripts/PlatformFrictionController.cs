using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlatformFrictionController : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    [SerializeField] private bool ice;
    [SerializeField] private bool mud;

    [Range(1.0f,1.5f)]
    [SerializeField] private float iceSlipFactor;
    [Range(1.0f, 1.5f)]
    [SerializeField] private float mudStickFactor;

    private SurfaceEffector2D surfaceEffector;
    private Vector2 currentPlayerVelocity;

    private void Start()
    {
        surfaceEffector = GetComponent<SurfaceEffector2D>();
        //playerRb = Player.gameObject.GetComponent<Rigidbody2D>();
    }


    private void FixedUpdate()
    {
        currentPlayerVelocity = CharacterController2D.m_Rigidbody2D.velocity;
        //UnityEngine.Debug.Log(currentPlayerVelocity.x);
        //return;

        // If the player is moving on the platform
        if (currentPlayerVelocity != Vector2.zero)
        {
            if (ice)
            {
                // Get the x direction of the movement
                var xVelocity = currentPlayerVelocity.x;

                // Multiply...
                surfaceEffector.speed = xVelocity * iceSlipFactor;
            }
            else if (mud)
            {
                var xVelocity = currentPlayerVelocity.x;

                // Multiply...
                surfaceEffector.speed = xVelocity * -1 * mudStickFactor;
            }

            if (Mathf.Abs(surfaceEffector.speed) > 6)
            {
                if (surfaceEffector.speed < 0)
                    surfaceEffector.speed = -6f;
                else
                    surfaceEffector.speed = 6f;
            }
                
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {

        }
    }
}
