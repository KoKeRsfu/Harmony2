using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.IO;

public class VideoPlayerController : MonoBehaviour
{
	
	public TextMeshProUGUI descriptionText;
	
	public string description;
	public string videoUrl;
	
	public GameObject errorMessage;
	
    public VideoPlayer videoPlayer; 
    public Slider videoSlider; 
    public TMP_Text textTime;
    public GameObject playPauseBtn;
    public Sprite playBtn;
    public Sprite pauseBtn;

    private bool playVideo;
    public string filmUrl;
    private bool isDragging = false;

    void Start()
    {
        //videoSlider.onValueChanged.AddListener(HandleSliderChange);
        //videoPlayer.prepareCompleted += (source) =>
        //{
        //    videoSlider.maxValue = (float)videoPlayer.length;
        //};
        //videoPlayer.Prepare();
    }
    void Update()
    {
        if (!isDragging && videoPlayer.isPlaying)
        {
            videoSlider.value = (float)videoPlayer.time;
        }
        TimeCalc((float)videoPlayer.time);
    }
    void HandleSliderChange(float value)
    {
        if (isDragging)
        {
            videoPlayer.time = value;
        }
    }
    public void OnBeginDrag()
    {
        isDragging = true;
        videoPlayer.Pause();
    }
    public void OnEndDrag()
    {
        isDragging = false;
        videoPlayer.Play();
    }
	public IEnumerator GetVideoUrl()//http://62.109.23.170/uploads/test.mp4  http://95.188.79.124/uploads/movie/hozyain_tajgi.avi
	{
		/*
		Debug.Log("пробую получить реквест");
	    UnityWebRequest request = UnityWebRequest.Get("http://95.188.79.124/uploads/movie/hozyain_tajgi.avi"); // http://62.109.23.170/uploads/media/cb5cb3ed-e3c3-4946-b8f3-8f10c5182b92.avi
        
        yield return request.SendWebRequest();

		Debug.Log("реквест получен, обрабатываю");

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
		*/
		videoPlayer.url = videoUrl;
            videoPlayer.prepareCompleted += (source) =>
            {
                videoSlider.maxValue = (float)videoPlayer.length;
            };
          
		descriptionText.text = description;
          
		errorMessage.SetActive(false);
           
		videoPlayer.Prepare();
		videoPlayer.Play();
		playVideo = true;
		yield return new WaitForSeconds(3);
		if (!videoPlayer.isPlaying)
		{
			errorMessage.SetActive(true);
		};
			
		//}
    }
    void TimeCalc(float time)
    {
        int fulltime = (int)time;
        int h = fulltime/360;
        int m = (fulltime/60)%60;
        int s = fulltime%60;
        string text = "";
        if (h < 10)
        {
            text += "0";
            text += h.ToString();
        }
        else text += h.ToString();
        text += ":";
        if (m < 10)
        {
            text += "0";
            text += m.ToString();
        }
        else text += m.ToString();
        text += ":";
        if (s < 10)
        {
            text += "0";
            text += s.ToString();
        }
        else text += s.ToString();

        textTime.text = text;
    }
    public void StopPlayer()
    {
        if (playVideo)
        {
            videoPlayer.Pause();
            playVideo = false;
            playPauseBtn.GetComponent<Image>().sprite = playBtn;
        }
        else
        {
            videoPlayer.Play();
            playVideo = true;
            playPauseBtn.GetComponent<Image>().sprite = pauseBtn;
        }
    }
}
