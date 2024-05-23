using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ButtonFilters : MonoBehaviour
{
	public GameObject btnPrefab;
	public GameObject btnLayout2;
	public GameObject scrollView;
	public GameObject controller;
	public GameObject cardCreator;
	//public GameObject prefabOpen;
	//public GameObject prefabClose;
	//private GameObject btnOpen;
	public bool status; //true = 1, false = 2
	public List<string> filtersTitles; //
	private List<string> theseFilters = new List<string>();

	public void StartCreate(List<string> btnT)
	{
		if (status)
		{
			DeleteAllChildren(this.gameObject);
			DeleteAllChildren(btnLayout2);
			//theseFilters = btnT;
			scrollView.GetComponent<RectTransform>().anchoredPosition =
				new Vector2(0, -138);
		}
		else
		{
			scrollView.GetComponent<RectTransform>().anchoredPosition =
				new Vector2(0, -202);
		}
		//Debug.Log(btnT);
		//CreateFilterBtn(theseFilters);
		CreateFilterBtn(btnT);
	}
	public static void DeleteAllChildren(GameObject parent)
	{
		int childCount = parent.transform.childCount;
		for (int i = childCount - 1; i >= 0; i--)
		{
			Transform child = parent.transform.GetChild(i);
			Destroy(child.gameObject);
		}
	}
	void CreateFilterBtn(List<String> btnTexts)
	{
		int len = 0;
		int nmbStop = -1;
		//Debug.Log("кол-во тэгов: "+btnTexts.Count);
		for (int i = 0; i < btnTexts.Count; i++)
		{
			len += (btnTexts[i].Length) * 17;
			len += 45;
			//Debug.Log(len);
			if (len < 1650)
			{
				GameObject newButton = Instantiate(btnPrefab, this.transform);
				TMP_Text buttonTextComponent = newButton.GetComponentInChildren<TMP_Text>();
				buttonTextComponent.text = btnTexts[i];
				//Debug.Log(btnTexts[i]);
				newButton.GetComponent<RectTransform>().sizeDelta =
					new Vector2(buttonTextComponent.GetPreferredValues().x, 50f);
				newButton.GetComponent<Filters>().filterController = this.gameObject;
				newButton.GetComponent<Filters>().cardController = cardCreator;
				newButton.GetComponent<Filters>().controller = controller;
				//theseFilters.Remove(btnTexts[i]);
			}
			else
			{
				nmbStop = i;
				//btnLayout2.GetComponent<ButtonFilters>().StartCreate(theseFilters);
				//status = false;
				break;
			}
		}
		if (nmbStop != -1 && status)
		{
			//Debug.Log(nmbStop);
			btnTexts.RemoveRange(0, nmbStop);
			btnLayout2.GetComponent<ButtonFilters>().StartCreate(btnTexts);
		}

		//if (!status)
		//{
		//    GameObject newButton = Instantiate(prefabClose, this.transform);
		//    btnOpen = newButton;
		//    scrollView.GetComponent<RectTransform>().anchoredPosition =
		//        new Vector2(0, -138);
		//    //this.transform.gameObject.SetActive(false);
		//}
		//if (nmbStop != -1 && status)
		//{
		//    //Debug.Log(nmbStop);
		//    GameObject newButton = Instantiate(prefabOpen, this.transform);
		//    newButton.GetComponent<Button>().onClick.AddListener(this.GetComponent<ButtonFilters>().OpenFilters);
		//    btnTexts.RemoveRange(0, nmbStop);
		//    btnLayout2.GetComponent<ButtonFilters>().StartCreate(btnTexts);
		//    btnLayout2.GetComponent<ButtonFilters>().btnOpen = newButton;
		//}
		//else if(!status)
		//{
		//    GameObject newButton = Instantiate(prefabClose, this.transform);
		//    newButton.GetComponent<Button>().onClick.AddListener(this.GetComponent<ButtonFilters>().CloseFilters);
		//    scrollView.GetComponent<RectTransform>().anchoredPosition =
		//        new Vector2(0, -138);
		//    this.transform.gameObject.SetActive(false);
		//}
	}

	//public void OpenFilters()
	//{
	//    this.transform.gameObject.SetActive(false);
	//    btnLayout2.SetActive(true);
	//    scrollView.GetComponent<RectTransform>().anchoredPosition =
	//            new Vector2(0, -202);
	//}

	//public void CloseFilters()
	//{
	//    btnOpen.SetActive(true);
	//    btnLayout2.SetActive(false);
	//    scrollView.GetComponent<RectTransform>().anchoredPosition =
	//            new Vector2(0, -138);
	//}
}
