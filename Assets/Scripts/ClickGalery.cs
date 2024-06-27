using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClickGalery : MonoBehaviour
{
	public GameObject MediaPlayer;
	public GameObject controller;
	public GameObject galeryController;
	public Button leftBtn;
	public Button rightBtn;
	public int myIndex;
	public int index;
	bool btn = true;
	public string description;
	public void ClickPhoto()
	{
		index = myIndex;
		Debug.LogWarning("Count "+galeryController.GetComponent<Galery>().listSprite.Count);
		Debug.LogWarning(myIndex);
		controller.GetComponent<MainMenu>().CLickOnePhoto();
		MediaPlayer.transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<Image>().sprite = galeryController.GetComponent<Galery>().listSprite[myIndex];
		MediaPlayer.transform.GetChild(2).gameObject.transform.GetChild(3).GetComponent<TMP_Text>().text = description;
		if(btn)
		{
			leftBtn.onClick.AddListener(ClickLeft);
			rightBtn.onClick.AddListener(ClickRight);
			btn = false;
		}
		if (index == galeryController.GetComponent<Galery>().listSprite.Count - 1) rightBtn.interactable = false;
		else rightBtn.interactable = true;
		if (index == 0) leftBtn.interactable = false;
		else leftBtn.interactable = true;
	}
	public void ClickRight()
	{
		if(index < galeryController.GetComponent<Galery>().listSprite.Count -1)
		{
			index++;
			
			UpdateButtons();
		}
	}
	public void ClickLeft()
	{
		if (index > 0)
		{
			index--;

			UpdateButtons();
		}
	}
	
	public void UpdateButtons() 
	{
		MediaPlayer.transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<Image>().sprite = galeryController.GetComponent<Galery>().listSprite[index];
		MediaPlayer.transform.GetChild(2).gameObject.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = galeryController.GetComponent<Galery>().listDescriptions[index];
		if (index == 0) 
		{
			leftBtn.interactable = false;
		}
		else leftBtn.interactable = true;
		if (index < galeryController.GetComponent<Galery>().listSprite.Count - 1) 
		{
			rightBtn.interactable = true;
		}
		else 
		{
			rightBtn.interactable = false;
		}
	}
}
