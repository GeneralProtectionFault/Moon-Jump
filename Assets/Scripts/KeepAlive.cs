using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepAlive : MonoBehaviour
{
    public static GameObject manager;


    // Start is called before the first frame update
    private void Awake()
    {
        if (manager == null)
        {
            manager = gameObject;
            DontDestroyOnLoad(manager);
        }
        else
            Destroy(gameObject);
    }
}
