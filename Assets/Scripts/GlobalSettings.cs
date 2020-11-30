using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    public static bool onMoon = false;
    public static bool onSun = false;
    public static bool levelStarting = true;
    [SerializeField] public static float moonGravity = -.3f;
    [SerializeField] public static float earthGravity = 1f;

    public static float xCoordinate = 0f;

    // Reason for this variable...  If we're updating every frame (timer script), then call a method to kill the player, it will call it every frame...
    // Use this to flag death.  Only call the death method(s) once, then flip this back
    public static bool isDead = false;
    public static bool isOnGoo = false;

    // For certain levels that oscillate between sun & moon
    public static bool sunIsOut = false;
}
