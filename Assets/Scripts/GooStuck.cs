using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class GooStuck : MonoBehaviour
{


    [SerializeField] private AudioSource audio;
    [SerializeField] private AudioClip gooSound;



    // In MudPlatformScript, we set the GlobalSettigs.isOnMudPlatform to false on the EXIT of the collider
    // Since forcing a small jump "attempt" exits the collider, we need to NOT do that while this animation is going on
    // This variable will store that
    public static bool jumpingInGoo;
    private static int gooInstances = 0;




    private void Awake()
    {
        jumpingInGoo = true;
        gooInstances += 1;


        if (GlobalSettings.onMoon)
        {
            Vector3 theScale = transform.localScale;
            theScale.y *= -1;
            transform.localScale = theScale;

            transform.position = new Vector3(transform.position.x, transform.position.y + .85f, transform.position.z);
        }
    }





    // Destroy the object as soon as the animation is completed!
    public void KillMeNow()
    {
        jumpingInGoo = false;
        gooInstances -= 1;
        Destroy(this.gameObject);
    }

    


    /// <summary>
    /// Jump is already disabled, we will, however, add a specific/controlled attempt to jump...up, then sprung back down...hopefully...
    /// </summary>
    public void StuckJump()
    {
        // UnityEngine.Debug.Log("Stuck jumping");
        CharacterController2D.OverrideMovement(0f, 0f);

        if (!GlobalSettings.onMoon && !GlobalSettings.onSun)
            CharacterController2D.m_Rigidbody2D.velocity = new Vector2(0f, 2.5f);
        else
            CharacterController2D.m_Rigidbody2D.velocity = new Vector2(0f, -2.5f);
    }



    public void AudioOnGoo()
    {
        audio.PlayOneShot(gooSound);
    }
}
