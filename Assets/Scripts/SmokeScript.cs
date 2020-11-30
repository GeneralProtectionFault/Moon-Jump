using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeScript : MonoBehaviour
{
    private void Awake()
    {
        if (GlobalSettings.onMoon)
        {
            Vector3 theScale = transform.localScale;
            theScale.y *= -1;
            transform.localScale = theScale;

            transform.position = new Vector3(transform.position.x, transform.position.y + .8f, transform.position.z);
        }
    }


    // Destroy the object as soon as the animation is completed!
    public void KillMeNow()
    {
        Destroy(this.gameObject);
    }
}
