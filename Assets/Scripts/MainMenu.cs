using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.IO;
using System.Net.Mime;

public class MainMenu : MonoBehaviour
{
	public GameObject MainContent;
	public GameObject PanelTop;
	public GameObject ScrollView;
	public GameObject Object;
	public GameObject btnLayout1;
	public GameObject btnLayout2;
	public GameObject CardContentObject;
	public GameObject MediaPlayer;
	public GameObject Photos;
	public GameObject Facts;
	public GameObject btnbck;
	public GameObject titleT;
	public GameObject btnAbout;
	public GameObject archive;

	public int level;
	private int levelOld;
	public string title;
	public TMP_Text Title;
	public TMP_Text Description;
	public bool flagGalery; 

    #region Приватные методы
	void Start()
	{
		//Debug.Log(Time.time);
        
		//StartCoroutine(archive.GetComponent<Archive>().CategoryRequest());
		//StartCoroutine(archive.GetComponent<Archive>().FetchContent());
		flagGalery = false;
		level = levelOld = 1;
		ChangeCategory(level);

		StartCoroutine(archive.GetComponent<Archive>().CategoryRequest());
		StartCoroutine(archive.GetComponent<Archive>().FetchContent());
		//archive.GetComponent<Archive>().requestManager.TaskCompleted();
	}


	//протестить
	void CleanUpMemory()
	{
		System.GC.Collect(); // Вызов сборщика мусора
		Resources.UnloadUnusedAssets(); // Опционально можно освободить неиспользуемые ресурсы
	}

	void Update()
	{
		if (levelOld != level)
		{
			CleanUpMemory();
			ChangeCategory(level);
		}
	}
	void ChangeCategory(int lv)
	{
		switch (lv) 
		{
		case 1:
			MainContent.SetActive(true);
			PanelTop.SetActive(false);
			ScrollView.SetActive(false);
			Object.SetActive(false);
			Title.gameObject.SetActive(true);
			Description.gameObject.SetActive(true);
			btnLayout1.SetActive(true);
			btnLayout2.SetActive(true);
			MediaPlayer.SetActive(false);
			Photos.SetActive(false);
			Facts.SetActive(false);
			//archive.GetComponent<Archive>().imageCache.Clear();

			levelOld = 1;
			
			break;
			
		case 2:
			MainContent.SetActive(false);
			PanelTop.SetActive(true);
            
			ScrollView.SetActive(true);
			Object.SetActive(false);
			Title.gameObject.SetActive(true);
			Description.gameObject.SetActive(true);
			btnLayout1.SetActive(true);
			btnLayout2.SetActive(true);
			MediaPlayer.SetActive(false);
			Photos.SetActive(false);
			Facts.SetActive(false);
			GameObject child = ScrollView.transform.GetChild(1).gameObject;

			// Проверяем, активен ли дочерний объект в иерархии
			if (child.activeInHierarchy)
			{
				Scrollbar scrollbar = child.GetComponent<Scrollbar>();
				if (scrollbar != null)
				{
					scrollbar.value = 1;
				}
				else
				{
					Debug.LogError("Slider component not found on the child object.");
				}
			}

			//if(ScrollView.transform.GetChild(1).gameObject.activeInHierarchy) ScrollView.transform.GetChild(1).gameObject.GetComponent<Slider>().value = ScrollView.transform.GetChild(1).gameObject.GetComponent<Slider>().minValue;

			levelOld = 2;
			
			break;
			
		case 3:
			PanelTop.SetActive(true);
			//ScrollView.SetActive(false);
			Object.SetActive(true);
			Title.gameObject.SetActive(false);
			Description.gameObject.SetActive(false);
			btnLayout1.SetActive(false);
			btnLayout2.SetActive(false);
			MediaPlayer.SetActive(false);
			Photos.SetActive(false);
			Facts.SetActive(false);
			btnbck.transform.GetChild(0).gameObject.SetActive(false);
			btnbck.transform.GetChild(1).gameObject.SetActive(true);
			btnbck.transform.GetChild(2).gameObject.SetActive(false);
			btnbck.transform.GetChild(3).gameObject.SetActive(true);
			btnbck.transform.GetChild(4).gameObject.GetComponent<TMP_Text>().color = Color.black;
			titleT.SetActive(true);
			if (btnAbout.GetComponent<AboutButtons>().status) btnAbout.GetComponent<AboutButtons>().BtnAboutClick();

			levelOld = 3;
			
			break;

		case 4:
			PanelTop.SetActive(true);
			ScrollView.SetActive(false);
			Object.SetActive(false);
			Title.gameObject.SetActive(false);
			Description.gameObject.SetActive(false);
			btnLayout1.SetActive(false);
			btnLayout2.SetActive(false);
			//MediaPlayer.SetActive(false);
			//Photos.SetActive(true);
			//Facts.SetActive(false);
			levelOld = 4;
			
			break;
		}
		
	}
    #endregion

    #region Кнопки Категорий
	private IEnumerator CategoryChoiced(int number)
	{
		archive.GetComponent<Archive>().requestManager.activeTasks++;
		ScrollView.SetActive(true);
		Archive arc = archive.GetComponent<Archive>();
		yield return StartCoroutine(arc.FetchContent());
		arc.objectContent = new Archive.RootObject();
		yield return StartCoroutine(arc.CardsRequest(arc.categoriesType[number], CardContentObject.GetComponent<Cards>().StartCreate));
		//StartCoroutine(archive.GetComponent<Archive>().CardsRequest(arc.categoriesType[number], CardContentObject.GetComponent<Cards>().StartCreate));
		//Debug.Log(arc.categoriesType[number]);
		//Debug.Log(arc.cats[number].categoryType);
		Title.text = title = arc.cats[number].category.title;
		Description.text = arc.cats[number].category.description;
		btnLayout1.GetComponent<ButtonFilters>().filtersTitles.Clear();
		foreach (Archive.Tag tag in arc.cats[number].tagLists.tags)
		{
			btnLayout1.GetComponent<ButtonFilters>().filtersTitles.Add(tag.title);
		}
		btnLayout1.GetComponent<ButtonFilters>().StartCreate(btnLayout1.GetComponent<ButtonFilters>().filtersTitles);
		btnLayout1.GetComponent<ButtonFilters>().filtersTitles.Clear();
		//CardContentObject.GetComponent<Cards>().StartCreate();
		level = 2;
		LayoutRebuilder.ForceRebuildLayoutImmediate(ScrollView.GetComponent<RectTransform>());
		Canvas.ForceUpdateCanvases();
		archive.GetComponent<Archive>().requestManager.TaskCompleted();
	}
	public IEnumerator StartSwitchCategory(int number)
	{
		//archive.GetComponent<Archive>().requestManager.Show();
		archive.GetComponent<Archive>().requestManager.activeTasks ++;
		Debug.LogWarning("Запуск");
		yield return StartCoroutine(CategoryChoiced(number));
		archive.GetComponent<Archive>().requestManager.TaskCompleted();
	}
	public void BtnCategory1()
	{
		StartCoroutine(StartSwitchCategory(0));
	}
	public void BtnCategory2()
	{
		StartCoroutine(StartSwitchCategory(1));
	}
	public void BtnCategory3()
	{
		StartCoroutine(StartSwitchCategory(2));
	}
	public void BtnCategory4()
	{
		StartCoroutine(StartSwitchCategory(3));
	}
	public void BtnCategory5()
	{
		StartCoroutine(StartSwitchCategory(4));
	}
    #endregion
    
    #region Другие методы
	public void BtnBackMenu()
	{
		level -= 1;
		if (level == 2) Title.text = title;
		if(flagGalery && level == 4)
		{
			Photos.SetActive(true);
			MediaPlayer.SetActive(false);
			btnbck.transform.GetChild(0).gameObject.SetActive(false);
			btnbck.transform.GetChild(1).gameObject.SetActive(true);
			btnbck.transform.GetChild(2).gameObject.SetActive(false);
			btnbck.transform.GetChild(3).gameObject.SetActive(true);
			btnbck.transform.GetChild(4).gameObject.GetComponent<TMP_Text>().color = Color.black;
		}
	}
	public void CLickContent()
	{
		level = 3;
	}
	public void CLickVideo()
	{
		level = 4;
		flagGalery = false;
		StartCoroutine(this.GetComponent<VideoPlayerController>().GetVideoUrl());
		MediaPlayer.SetActive(true);
		btnbck.transform.GetChild(0).gameObject.SetActive(true);
		btnbck.transform.GetChild(1).gameObject.SetActive(false);
		btnbck.transform.GetChild(2).gameObject.SetActive(true);
		btnbck.transform.GetChild(3).gameObject.SetActive(false);
		btnbck.transform.GetChild(4).gameObject.GetComponent<TMP_Text>().color = Color.white;
		MediaPlayer.transform.GetChild(1).gameObject.SetActive(true);
		MediaPlayer.transform.GetChild(2).gameObject.SetActive(false);
		//titleT.SetActive(false);
	}
	public void CLickFacts()
	{
		//this.GetComponent<Facts>().factsList = new List<string> {"Текст первого факта", "Текст второго факта", 
		//    "Текст третьего факта", "Текст четвертого факта", "Текст пятого факта"};
		this.GetComponent<Facts>().StartCoroutine("CreateFacts");
		level = 4;
		flagGalery = false;
		Facts.SetActive(true);
	}
	public void CLickGalery()
	{
		level = 4;
		flagGalery = true;
		Photos.SetActive(true);
	}
	public void CLickOnePhoto()
	{
		level = 5;
		MediaPlayer.SetActive(true);
		btnbck.transform.GetChild(0).gameObject.SetActive(true);
		btnbck.transform.GetChild(1).gameObject.SetActive(false);
		btnbck.transform.GetChild(2).gameObject.SetActive(true);
		btnbck.transform.GetChild(3).gameObject.SetActive(false);
		btnbck.transform.GetChild(4).gameObject.GetComponent<TMP_Text>().color = Color.white;

		MediaPlayer.transform.GetChild(1).gameObject.SetActive(false);
		MediaPlayer.transform.GetChild(2).gameObject.SetActive(true);
		titleT.SetActive(false);
	}

	public void CLickOneVideo()
	{
		level = 5;
		MediaPlayer.SetActive(true);
		btnbck.transform.GetChild(0).gameObject.SetActive(true);
		btnbck.transform.GetChild(1).gameObject.SetActive(false);
		btnbck.transform.GetChild(2).gameObject.SetActive(true);
		btnbck.transform.GetChild(3).gameObject.SetActive(false);
		btnbck.transform.GetChild(4).gameObject.GetComponent<TMP_Text>().color = Color.white;

		MediaPlayer.transform.GetChild(1).gameObject.SetActive(true);
		MediaPlayer.transform.GetChild(2).gameObject.SetActive(false);
		titleT.SetActive(false);
	}
    #endregion
}
