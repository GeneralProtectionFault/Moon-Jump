using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource audio;
    
    [SerializeField] AudioClip musicLevel1;
    


    // Start is called before the first frame update
    void Start()
    {
        audio.clip = musicLevel1;
        audio.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
