using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.Play();
    }
    public void StopBGM()
    {
        audioSource.Stop();
    }
    public void ResumeBGM()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
    public void SetVolume(float volume)
    {
        audioSource.volume= volume;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
