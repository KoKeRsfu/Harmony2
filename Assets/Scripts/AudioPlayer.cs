using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioPlayer : MonoBehaviour
{
	public Sprite btn1x;
	public Sprite btn2x;
	public Sprite play;
	public Sprite pause;
	public Slider slider;
	public GameObject speedBtn;

	public bool status; // false = 1x, true = 2x
	public TMP_Text timerText;
	private bool timerActive;

	public AudioSource audioSource;
    
	//private void Start()
	//{
	//    ResetPlayer();
	//}

	void Update()
	{
		slider.value = (float)audioSource.time;
		TimeCalc((float)audioSource.time);
	}
	void TimeCalc(float time)
	{
		int fulltime = (int)time;
		int m = (fulltime / 60) % 60;
		int s = fulltime % 60;
		string text = "";
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

		timerText.text = text;
	}
	void AdjustTime(float time)
	{
		if (time >= 0 && time <= audioSource.clip.length)
		{
			audioSource.time = time;
		}
		else
		{
			Debug.LogError("Attempted to seek to an invalid position: " + time);
		}
	}
	public void ClickPlay()
	{
		timerActive = !timerActive;
		if (timerActive)
		{
			audioSource.Play();
			this.GetComponent<Image>().sprite = pause;
		}
		else
		{
			audioSource.Pause();
			this.GetComponent<Image>().sprite = play;
		}
	}

	public void ClickSpeed()
	{
		status = !status;
		if (status)
		{
			speedBtn.GetComponent<Image>().sprite = btn2x;
			audioSource.pitch = 1.5f;
		}
		else
		{
			speedBtn.GetComponent<Image>().sprite = btn1x;
			audioSource.pitch = 1f;
		}
	}

	public void ResetPlayer()
	{
		slider.maxValue = audioSource.clip.length;
		slider.onValueChanged.AddListener(AdjustTime);
		status = false;
		timerActive = false;
		timerText.text = "00:00";
        
		//audioSource.Play();
	}
}
