using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource audio;

    [SerializeField] private AudioClip bonkSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip moonJumpSound;

    [SerializeField] private Animator animator;


    public static bool audioIsMoonJumping = false;
    private static bool isBonked = false;

    // Use this for extra check to make sure we don't get stuck in slow-mo
    private static float secondsInSlowMotion = 0f;




    private void Update()
    {
        //  Extra check to make sure we're not stuck in slow-mo...
        if (Time.timeScale != 1f)
        {
            secondsInSlowMotion += Time.deltaTime;
        }

        if (secondsInSlowMotion > 2f && !PauseScreenScript.pauseScreen.activeSelf)
        {
            Time.timeScale = 1f;
            secondsInSlowMotion = 0f;
        }
    }





    // This trigger is the box collider at the top of the player's head
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // We don't want to bonk if we're going "through" a collider, which happens if we're going the "one way" through a one-way collider...
        if (GlobalSettings.onMoon || GlobalSettings.onSun)
        {
            if (collision.gameObject.tag == "OneWayDownPlatform")
                return;
        }
        else if (collision.gameObject.tag == "OneWayUpPlatform")
            return;


        // This trigger is for bonk animation/behavior.  Not applicable for enemies.
        // The enemies/hazards will be handled in collisions on their scripts.
        if (collision.gameObject.tag == "Enemy")
            return;


        if (audioIsMoonJumping && 
            //!isBonked && 
            collision.gameObject.tag != "PortalCollider")    // Prevent redundant bonk and don't trigger between earth/moon/sun
        {
            CharacterController2D.m_Rigidbody2D.velocity = new Vector2(0f, 0f);
            var particleSpawner = GetComponentInChildren<ParticleSystem>();
            particleSpawner.Play();

            animator.SetTrigger("bonk");
            isBonked = true;

            AudioOnBonk();


            var coroutine = WaitForBonk();
            StartCoroutine(coroutine);

        }
    }


    private IEnumerator WaitForBonk()
    {

        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(.4f);

        if (!PauseScreenScript.pauseScreen.activeSelf)
            Time.timeScale = .7f;

        yield return new WaitForSecondsRealtime(1.5f);

        if (!PauseScreenScript.pauseScreen.activeSelf)
            Time.timeScale = 1f;

        isBonked = false;
    }


    public void AudioOnJump()
    {
        if (CharacterController2D.jumpCount <= 1 && CharacterController2D.jumping)
            audio.PlayOneShot(jumpSound);
    }

    public void AudioOnDeath()
    {
        audio.PlayOneShot(deathSound);
    }

    public void AudioOnMoonJump()
    {
        audio.PlayOneShot(moonJumpSound);
    }

    public void AudioOnBonk()
    {
        audio.PlayOneShot(bonkSound);
    }

}
