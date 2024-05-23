using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Audio : MonoBehaviour
{
	public AudioMixer audioMixer;
	public int volAudio;
	public List<GameObject> picVolume;
	void Start()
	{
		volAudio = 3;
		audioMixer.SetFloat("MasterVolume", 0);
	}
	public void BtnVolume()
	{
		volAudio += 1;
		if (volAudio == 4) volAudio = 1;
		switch (volAudio)
		{
		case 1:
			audioMixer.SetFloat("MasterVolume", -20.0f);
			break;
		case 2:
			audioMixer.SetFloat("MasterVolume", -10.0f);
			break;
		case 3:
			audioMixer.SetFloat("MasterVolume", 0f);
			break;
		}

		foreach (GameObject pic in picVolume)
		{
			pic.SetActive(false);
		}
		picVolume[volAudio - 1].SetActive(true);
	}
}
