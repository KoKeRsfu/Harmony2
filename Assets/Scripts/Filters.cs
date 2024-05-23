using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using Image = UnityEngine.UI.Image;

public class Filters : MonoBehaviour
{
	public Sprite blackBtn;
	public Sprite blueBtn;
	public bool status; //false = black, true = blue
	public GameObject filterController;
	public GameObject cardController;
	public GameObject controller;
	public void Start()
	{
		status = false;
	}

	public void BtnClickFilter()
	{
		status = !status;
		if (status)
		{
			this.GetComponent<Image>().sprite = blueBtn;
			this.GetComponent<Image>().pixelsPerUnitMultiplier = 10f; 
			this.GetComponentInChildren<TMP_Text>().color = Color.white;
			string text = gameObject.GetComponentInChildren<TMP_Text>().text;
			cardController.GetComponent<Cards>().filtersList.Add(text);
			cardController.GetComponent<Cards>().Change();
			//filterController.GetComponent<ButtonFilters>().filtersTitles.Add(text);
			//Debug.Log("add filter " + text);
		}
		else
		{
			this.GetComponent<Image>().sprite = blackBtn;
			this.GetComponent<Image>().pixelsPerUnitMultiplier = 4.09f;
			this.GetComponentInChildren<TMP_Text>().color = Color.black;
			string text = gameObject.GetComponentInChildren<TMP_Text>().text;
			int id = -1;
			for (int i = 0; i < cardController.GetComponent<Cards>().filtersList.Count; i++)
			{
				if (cardController.GetComponent<Cards>().filtersList[i] == text) 
				{
					id = i;
					break;
				}
			}
			cardController.GetComponent<Cards>().filtersList.RemoveAt(id);
			cardController.GetComponent<Cards>().change = true;
			//Debug.Log("del filter " + text+ " id ");
		}
		//cardController.GetComponent<Cards>().StartCreate(filterController.GetComponent<ButtonFilters>().filtersTitles);
		//cardController.GetComponent<Cards>().StartCreate(cardController.GetComponent<Cards>().filtersList);
	}
}
