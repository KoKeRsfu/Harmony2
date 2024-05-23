using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class Facts : MonoBehaviour
{
	//public List<String> factsList;

	public GameObject firstFact;
	public GameObject secondFact;
	public GameObject thirdFact;
	public GameObject archive;

	public GameObject sliderBox;
	public GameObject prefabSlider;
	public Sprite blueSprite;
	public Sprite graySprite;
	public int status;
	public Swipe swipeDetector;

	public List<Archive.Media> facts = new List<Archive.Media>();
	//public List<Sprite> facts;
	void Start()
	{
		if (swipeDetector != null)
		{
			swipeDetector.OnSwipeLeft += ClickRight;
			swipeDetector.OnSwipeRight += ClickLeft; 
		}
	}

	private void OnDestroy()
	{
		if (swipeDetector != null)
		{
			swipeDetector.OnSwipeLeft -= ClickRight;
			swipeDetector.OnSwipeRight -= ClickLeft;
		}
	}

	//public void CreateFacts(List<Archive.Media> list)
	//{
	//    facts.Clear();
	//    facts = list;
	//    status = 0;
	//    firstFact.SetActive(false);
	//    secondFact.SetActive(false);
	//    thirdFact.SetActive(false);

	//    ButtonFilters.DeleteAllChildren(sliderBox);
	//    for(int i = 0; i < list.Count; i ++)
	//    {
	//        GameObject newSlider = Instantiate(prefabSlider, sliderBox.transform);
	//        if (i == 0)
	//        {
	//            Texture2D texture = archive.GetComponent<Archive>().coverCache[list[i].id];
	//            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
	//            secondFact.GetComponent<Image>().sprite = sprite;
	//            //secondFact.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = (i+1).ToString();
	//            //secondFact.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = factsList[i];
	//            newSlider.GetComponent<Image>().sprite = blueSprite;
	//            secondFact.SetActive(true);
	//        }
	//        if (i == 1)
	//        {
	//            Texture2D texture = archive.GetComponent<Archive>().coverCache[list[i].id];
	//            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
	//            thirdFact.GetComponent<Image>().sprite = sprite;
	//            //thirdFact.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = (i+1).ToString();
	//            //thirdFact.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = factsList[i];
	//            thirdFact.SetActive(true);
	//        }
	//    }
	//}
    
	public void ClickRight()
	{
		SwitchFacts(true);
	}
    
	public void ClickLeft()
	{
		SwitchFacts(false);
	}
    
	public void SwitchFacts(bool right)
	{
		if (right)
		{
			if (status < facts.Count - 1)
			{
				sliderBox.transform.GetChild(status).GetComponent<Image>().sprite = graySprite;
				status += 1;
			}
			else return;
		}
		else
		{
			if (0 < status)
			{
				sliderBox.transform.GetChild(status).GetComponent<Image>().sprite = graySprite;
				status -= 1;
			}
			else return;
		}
		sliderBox.transform.GetChild(status).GetComponent<Image>().sprite = blueSprite;

		//if (status > 0)
		//{
		//    //firstFact.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = (status).ToString();
		//    Texture2D texture = archive.GetComponent<Archive>().coverCache[facts[status-1].id];
		//    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
		//    firstFact.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = sprite;
		//    firstFact.SetActive(true);
		//}
		//else firstFact.SetActive(false);

		////secondFact.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = (status+1).ToString();
		//Texture2D texture1 = archive.GetComponent<Archive>().coverCache[facts[status].id];
		//Sprite sprite1 = Sprite.Create(texture1, new Rect(0, 0, texture1.width, texture1.height), new Vector2(0.5f, 0.5f));
		//secondFact.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = sprite1;

		//if (status + 1 <= facts.Count - 1)
		//{
		//    //thirdFact.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = (status+2).ToString();
		//    Texture2D texture2 = archive.GetComponent<Archive>().coverCache[facts[status + 1].id];
		//    Sprite sprite2 = Sprite.Create(texture2, new Rect(0, 0, texture2.width, texture2.height), new Vector2(0.5f, 0.5f));
		//    thirdFact.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = sprite2;
		//    thirdFact.SetActive(true);
		//}
		//else thirdFact.SetActive(false);
	}
}
