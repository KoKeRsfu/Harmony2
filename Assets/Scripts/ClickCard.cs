using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;

public class ClickCard : MonoBehaviour
{
	public Archive.Content thisCard;
	public GameObject content;
	public GameObject controller;
	public GameObject archive;
	public GameObject contentGallery;
	public GameObject button;

	public bool update;
	public bool aboutBtn;
	public bool factsBtn;
	public bool bottBtn; 
	public int aboutBtnint;
	public int factsBtnint;
	public int bottBtnint;

	public Sprite plug;
	bool coverOn = false;
	float x = 0f;
	float y = 0f;
	float hight = 366f;

	public List<Archive.Media> mediaList;
	private void Start()
	{
		mediaList = new List<Archive.Media>();
	}
	public void GalleryBtn()
	{
		//controller.GetComponent<MainMenu>().level = 4;
		contentGallery.GetComponent<Galery>().mediaList = mediaList;
		controller.GetComponent<MainMenu>().CLickGalery();
		//contentGallery.GetComponent<Galery>().ClickGalery(mediaList);
		contentGallery.GetComponent<Galery>().LoadMedia();
	}

	public void ClickOnCard()
	{
		Debug.Log("ClickOnCard");
		
		StartCoroutine("ClickOnCardCo");
	}

	private IEnumerator ClickOnCardCo()
	{
		archive.GetComponent<Archive>().requestManager.activeTasks++;
		archive.GetComponent<Archive>().requestManager.Show();
		
		yield return archive.GetComponent<Archive>().MediaRequest(archive.GetComponent<Archive>().objectContent.contents[transform.GetSiblingIndex()].id);
			
		thisCard = transform.parent.parent.parent.parent.GetChild(0).GetChild(transform.GetSiblingIndex()).GetComponent<ClickCard>().thisCard;
			
		yield return new WaitForSeconds(0.1f);
			
		transform.parent.gameObject.SetActive(true);
		if (thisCard.cover is null) coverOn = false;
		else { SetCoverImage(); }
		CreateObject();
		Invoke("CreateObject", 0.1f);
		controller.GetComponent<MainMenu>().level = 3;
		
		archive.GetComponent<Archive>().requestManager.TaskCompleted();
		
		Canvas.ForceUpdateCanvases();
	}

	private void Update()
	{
		if(content.activeInHierarchy && update)
		{
			ContentSizeFitter view = content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<ContentSizeFitter>();
			view.enabled = false;
			view.enabled = true;
			view.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
			view.SetLayoutVertical();
			LayoutRebuilder.ForceRebuildLayoutImmediate(view.gameObject.GetComponent<RectTransform>()); update = false;
			Canvas.ForceUpdateCanvases();
		}

		////bool verticalNeeded = content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.gameObject.transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<ScrollRect>().content.rect.height 
		////    > content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.gameObject.transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<ScrollRect>().viewport.rect.height;
		////if (verticalNeeded && !content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.gameObject.transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.activeInHierarchy)
		////    content.transform.parent.GetChild(2).gameObject.SetActive(false);
		////else if(verticalNeeded && content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.gameObject.transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.activeInHierarchy)
		////    content.transform.parent.GetChild(2).gameObject.SetActive(true);
		////else if(!verticalNeeded)
		////    content.transform.parent.GetChild(2).gameObject.SetActive(false);
	}//10501

	void CreateObject() 
	{
		content.transform.parent.GetChild(3).gameObject.GetComponent<Scrollbar>().value = 1;
		content.transform.parent.GetChild(2).gameObject.GetComponent<Scrollbar>().value = 1;

		bottBtn = false;
		//TAG
		content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = thisCard.tag.title;

		//TAG - RectTransform
		content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta
			= new Vector2(content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).transform.GetChild(0).gameObject.gameObject.GetComponent<RectTransform>().sizeDelta.x, 35f);

		//TITLE
		content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = thisCard.title;

		//DESCRIPTION = SUBTITLE
		if(thisCard.description != null && thisCard.description != "")
		{
			content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text = thisCard.description;
			content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(2).gameObject.SetActive(true);
		}
		else content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(2).gameObject.SetActive(false);


		//TEXT = Описание 10501 - скролвью
		content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.gameObject.transform.GetChild(5)
			.gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = thisCard.text;


		//ABOUT = ABOUTINFO 10500
		bool about = thisCard.information.GetAllValidInfo(content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform);
        

		if (about && !string.IsNullOrEmpty(thisCard.text))
		{
			//thisCard.information.GetAllValidInfo(content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform);
			content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(false); //about
			content.transform.parent.GetChild(3).gameObject.SetActive(false);
			content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.SetActive(true); //description
			//кнопки
			content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(4).gameObject.SetActive(true);
			aboutBtn = true;
		}
		else if(about && string.IsNullOrEmpty(thisCard.text))
		{
			//thisCard.information.GetAllValidInfo(content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform);
			content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(true);
			content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.SetActive(false);
			content.transform.parent.GetChild(2).gameObject.SetActive(false);
			//кнопки
			content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(4).gameObject.SetActive(false);
			aboutBtn = false;
		}
		else if (!about && !string.IsNullOrEmpty(thisCard.text))
		{
			content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(false);
			content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.SetActive(true);
			content.transform.parent.GetChild(3).gameObject.SetActive(false);
			//кнопки
			content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(4).gameObject.SetActive(false);
			aboutBtn = false;
		}
		else
		{
			content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(false);
			content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.SetActive(false);
			content.transform.parent.GetChild(2).gameObject.SetActive(false);
			content.transform.parent.GetChild(3).gameObject.SetActive(false);
			//кнопки
			content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(4).gameObject.SetActive(false);
			aboutBtn = false;
		}

		//VerticalLayoutGroup verticalLayoutGroup = content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<VerticalLayoutGroup>();
		//verticalLayoutGroup.SetLayoutVertical();
		//LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)verticalLayoutGroup.transform);


		//GALLERY
		mediaList = thisCard.media.Where(m => m.type == "PHOTO_VIDEO" || m.type == "COVER").ToList();
		//int c = contentGallery.GetComponent<Galery>().LoadMediaCount(mediaList);
		//Debug.LogWarning(c + "   - rjk");
		if (mediaList.Count == 0) { content.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.SetActive(false); }
		else
		{
			content.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.SetActive(true);
			bottBtn = true;
			content.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(this.GetComponent<ClickCard>().GalleryBtn);
		}

		//MOVIE
		List<Archive.Media> movieCheck = thisCard.media.Where(m => m.type == "MOVIE").ToList();
		if (movieCheck.Count > 0) 
		{
			content.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.SetActive(true);
			
			controller.GetComponent<VideoPlayerController>().videoUrl = 	movieCheck[movieCheck.Count - 1].url;
			controller.GetComponent<VideoPlayerController>().description = movieCheck[movieCheck.Count - 1].description;
		}
		else 
		{
			content.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.SetActive(false);
		}
		//if (movieCheck.Count == 0) { content.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.SetActive(false); }
		//else
		//{
		bottBtn = true;
		//    Debug.LogWarning(movieCheck.Count);
		//    if(thisCard.title == "Где-то гремит война")		
		//    {
		//        content.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.SetActive(true);
		//        content.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(controller.GetComponent<MainMenu>().CLickVideo);
		//        controller.GetComponent<VideoPlayerController>().filmUrl = movieCheck[0].url;
		//    }
		//    else
		//    { content.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.SetActive(false); }
		//}

		//SOUND
		List<Archive.Media> soundCHeck = thisCard.media.Where(m => m.type == "SOUND").ToList();
		if (soundCHeck.Count == 0) { content.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject.SetActive(false);}
		else
		{
			bottBtn = true;
			content.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject.SetActive(true);
			AudioSource audio = content.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject.GetComponent<AudioSource>();
			//StartCoroutine(archive.GetComponent<Archive>().MusicRequest(soundCHeck[0].url, audio));
			StartCoroutine(archive.GetComponent<Archive>().AssignAudioClip(soundCHeck[0].id, GetSubstringAfterLastDot(soundCHeck[0].url),audio,() =>
			{
				content.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<AudioPlayer>().ResetPlayer();
			}));
            
			//content.transform.GetChild(1).gameObject.transform.GetChild(7).gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = soundCHeck[0].description;
		}

		//FACTS
		List<Archive.Media> factCheck = thisCard.media.Where(m => m.type == "FACT").ToList();
		Debug.Log("количество фактов 1: " + factCheck.Count);
		if (factCheck.Count > 0) 
		{
			content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(3).gameObject.SetActive(true);
			factsBtn = true;
			controller.GetComponent<Facts>().facts = factCheck;
			//controller.GetComponent<Facts>().StartCoroutine("CreateFacts");
		}
		else 
		{
			content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(3).gameObject.SetActive(false); 
			factsBtn = false;
		}

		//COVER ?
		//Включена ли обложка
		if (coverOn)
		{
			content.transform.GetChild(0).gameObject.SetActive(true);
			content.GetComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.UpperCenter;
			x = 595f;
		}
		else
		{
			content.GetComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.UpperLeft;
			x = 1528f;
			content.transform.GetChild(0).gameObject.SetActive(false);
		}

		//Изменение размеров объектов

		//viewport - 105010 и текст - 1050100
		content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta =
			new Vector2(x, content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta.y);
		content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta =
			new Vector2(x, content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta.y);
		// title - 101, about - 10500, subtitle - 103
		content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<RectTransform>().sizeDelta =
			new Vector2(x, content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<RectTransform>().sizeDelta.y);
		content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta =
			new Vector2(x, content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta.y);
		content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(2).gameObject.GetComponent<RectTransform>().sizeDelta =
			new Vector2(x, content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(2).gameObject.GetComponent<RectTransform>().sizeDelta.y);
		button.GetComponent<AboutButtons>().ChangeView();
		button.GetComponent<AboutButtons>().ChangeView();

		RectTransform viewRect = content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>();
		y = 0f;
		y = hight;
		//Debug.LogWarning(y);
		aboutBtnint = 52;
		factsBtnint = 39;
		bottBtnint = 110;
		if(!aboutBtn)
		{
			y += aboutBtnint;
		}
		if(!factsBtn)
		{
			y += factsBtnint;
		}
		if(!bottBtn)
		{
			y += bottBtnint;
		}
		//Debug.LogWarning(y);
		update = true;
		//scrollview - 10501
		viewRect.sizeDelta = new Vector2(x, y);

		ContentSizeFitter view = content.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<ContentSizeFitter>();
		view.enabled = false;
		view.enabled = true;
		view.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
		view.SetLayoutVertical();
		LayoutRebuilder.ForceRebuildLayoutImmediate(view.gameObject.GetComponent<RectTransform>());

		view.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
		view.SetLayoutVertical();
		LayoutRebuilder.ForceRebuildLayoutImmediate(view.gameObject.GetComponent<RectTransform>());
	}
	public string GetSubstringAfterLastDot(string input)
	{
		// Проверяем, содержит ли строка хотя бы одну точку
		int lastDotIndex = input.LastIndexOf('.');
		if (lastDotIndex == -1)
		{
			return "";  // Возвращаем пустую строку, если точек нет
		}

		// Возвращаем подстроку, начиная с позиции следующей за последней точкой
		return input.Substring(lastDotIndex + 1);
	}
	public void SetCoverImage()
	{
		//Debug.LogWarning("Начинаю SetCoverImage");
		Archive.Media validMedia = thisCard.media
			.Where(m => m.type == "COVER" && !string.IsNullOrEmpty(m.url))
			.FirstOrDefault();

		if (validMedia is not null)
		{
			// Загрузка изображения из локального файла
			string filePath = Path.Combine(archive.GetComponent<Archive>().folderPath, validMedia.id + ".png");
            
			if (File.Exists(filePath))
			{  
				if(IsFileLargerThan1_8MB(filePath))
				{
					filePath = Path.Combine(archive.GetComponent<Archive>().folderResizePath, validMedia.id + ".png");
				}
				Texture2D texture = archive.GetComponent<Archive>().LoadAndResizeTexture(filePath, 1024, 1024);
				Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
				Image targetImage = content.transform.GetChild(0).gameObject.GetComponent<Image>();
				if (targetImage != null)
				{
					targetImage.sprite = sprite;
					coverOn = true;
				}
				else
				{
					Debug.LogError("Компонент Image не найден в объекте content.");
					coverOn = false;
				}
			}
			else
			{
				thisCard.cover = null;
				coverOn = false;
			}

		}
		else
		{
			thisCard.cover = null;
			coverOn = false;
		}
	}

	public static bool IsFileLargerThan1_8MB(string filePath)
	{
		// Проверяем, существует ли файл
		if (!File.Exists(filePath))
		{
			Console.WriteLine("Файл не найден: " + filePath);
			return false;
		}

		// Создаём объект FileInfo
		FileInfo fileInfo = new FileInfo(filePath);

		// Получаем размер файла в байтах
		long sizeInBytes = fileInfo.Length;

		// Переводим 1.8 МБ в байты (1 МБ = 1024 * 1024 байт)
		long sizeThreshold = (long)(1.8 * 1024 * 1024);

		// Сравниваем размер файла с пороговым значением
		bool isLarger = sizeInBytes > sizeThreshold;

		// Возвращаем результат проверки
		return isLarger;
	}
	//public void SetCoverImage()
	//{
	//    //Debug.LogWarning("Начинаю SetCoverImage");
	//    Archive.Media validMedia = thisCard.media
	//        .Where(m => m.type == "COVER" && !string.IsNullOrEmpty(m.url))
	//        .FirstOrDefault();

	//    if(validMedia is not null)
	//    {
	//        Texture2D texture;
	//        if (archive.GetComponent<Archive>().imageCache.ContainsKey(validMedia.id))
	//        {
	//            Debug.LogWarning(validMedia.url);

	//            texture = archive.GetComponent<Archive>().imageCache[validMedia.id];
	//        }
	//        else
	//        {
	//            texture = archive.GetComponent<Archive>().coverCache[validMedia.id];

	//        }
	//        //Debug.LogWarning(texture.width + "  " + texture.height);
	//        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
	//        Image targetImage = content.transform.GetChild(0).gameObject.GetComponent<Image>();
	//        targetImage.sprite = sprite;
	//        coverOn = true;
	//    }
	//    else
	//    {
	//        thisCard.cover = null;
	//        coverOn = false;
	//    }
	//}

	IEnumerator LoadCover(GameObject card, string urlImage)
	{
		Debug.LogWarning("Load Cover");
		UnityWebRequest request = UnityWebRequestTexture.GetTexture(urlImage);
		request.timeout = 15;
		yield return request.SendWebRequest();
		Debug.LogWarning(request.result);
		if (request.result == UnityWebRequest.Result.Success) // Проверка успешности запроса
		{
			Debug.LogWarning("Успешный запрос");
			Texture2D texture = DownloadHandlerTexture.GetContent(request);
			if (texture)
			{
				Debug.LogWarning("Есть текстура");
				Sprite image = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
				//Debug.LogWarning("texture" + texture.imageContentsHash);
				Image targetImage = card.GetComponent<Image>(); // Путь к Image должен быть указан правильно
				//Debug.LogWarning("targetImage.sprite"+targetImage.sprite.texture.imageContentsHash);
				if (targetImage)
				{
					Debug.LogWarning("Изображение");
					targetImage.sprite = image;
					//Debug.LogWarning("targetImage.sprite"+targetImage.sprite.texture.imageContentsHash);
					//Debug.LogWarning("Изображение установлено!");
				}
				else
				{
					Debug.LogError("Компонент Image не найден!");
				}
			}
			else
			{
				Debug.LogError("Текстура не загружена из запроса.");
			}
		}
	}
}

