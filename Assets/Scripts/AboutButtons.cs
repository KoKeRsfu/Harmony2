using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;
using TMPro;

public class AboutButtons : MonoBehaviour
{
	public Sprite blackBtn;
	public Sprite blueBtn;
	public bool status; //false = off, true = on
	private bool oldstatus; //false = off, true = on
	public GameObject secondBtn;
	public GameObject textBox;

	void Start()
	{
		oldstatus = false;
		textBox.SetActive(false);
	}
	void Update()
	{
		if (status != oldstatus)
		{
			ChangeView();
			oldstatus = status;
		}
	}
    
	public void BtnAboutClick()
	{
		status = !status;
		secondBtn.GetComponent<AboutButtons>().status = !secondBtn.GetComponent<AboutButtons>().status;
	}
    
	public void ChangeView()
	{
		if (status)
		{
			//включенное состояние
			this.GetComponent<Image>().sprite = blueBtn;
			this.GetComponent<Image>().pixelsPerUnitMultiplier = 10f; 
			this.GetComponentInChildren<TMP_Text>().color = Color.white;
			textBox.SetActive(true);
		}
		else
		{
			//выключенное состояние
			this.GetComponent<Image>().sprite = blackBtn;
			this.GetComponent<Image>().pixelsPerUnitMultiplier = 4.09f;
			this.GetComponentInChildren<TMP_Text>().color = Color.black;
			textBox.SetActive(false);
		}
	}
}
