using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Idle : MonoBehaviour
{
    public float idleTimeThreshold = 60f; 
    private float lastActivityTime;
    public AudioSource audio;
    public VideoPlayer video;
    void Start()
    {
        lastActivityTime = Time.time;
    }
    
    void Update()
    {
        if (Input.anyKeyDown)
        {
            lastActivityTime = Time.time;
        }
        if (audio.isPlaying)
        {
            lastActivityTime = Time.time;
        }
        if (video.isPlaying)
        {
            lastActivityTime = Time.time;
        }
        if (Time.time - lastActivityTime > idleTimeThreshold)
        {
            //Debug.Log("Пользователь бездействует более " + idleTimeThreshold + " секунд.");
            this.GetComponent<MainMenu>().level = 1;
        }
    }
}
