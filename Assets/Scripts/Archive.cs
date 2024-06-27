using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine.Video;

public class Archive : MonoBehaviour
{
	//public const string adminApi = "http://192.168.0.184:8888/api/harmony/";

	//public const string adminApi = "http://62.109.23.170:8888/api/harmony/"; //тест
	
	//public const string adminApi = "http://95.188.79.124:8888/api/harmony/"; //прод
	public const string adminApi = "http://192.168.0.240:8888/api/harmony/"; //локалка
	
	public List<string> categoriesType = new List<string>() { "MUSIC", "THEATRE", "CINEMA", "LITERATURE", "JOURNEY" };
	public List<string> mediaType = new List<string>() { "FACT", "PHOTO_VIDEO", "MOVIE", "SOUND", "COVER" }; //только для запроса conntent/{id content}/media
	public List<CategoriesList> cats;
	public RootObject objectContent;
	public Dictionary<string, string> cache =  new Dictionary<string, string>();
	//public Dictionary<string, Texture2D> coverCache = new Dictionary<string, Texture2D>(); //только обложки в мини размере
	//public Dictionary<string, Texture2D> imageCache = new Dictionary<string, Texture2D>(); // все остальное
	//private List<Texture2D> textures = new();
	public CacheInspector cacheInspector = new CacheInspector();
	private int countCache = 0;
	private int countCover = 0;
	//private int countImage = 0;
	public GameObject infoPrefab;
	public LoadingIndicator requestManager;
	public string folderPath;
	public string folderResizePath;
	public string audioFolder;
	public string videoFolder;

	public int checkFetch;
	public int checkCategory;

	[Serializable]
	public class SerializableDictionary<TKey, TValue>
	{
		[SerializeField]
		private List<TKey> keys = new List<TKey>();
		[SerializeField]
		private List<TValue> values = new List<TValue>();
		public Dictionary<TKey, TValue> ToDictionary()
		{
			Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
			for (int i = 0; i < keys.Count; i++)
			{
				if (i < values.Count)
				{
					dictionary[keys[i]] = values[i];
				}
			}
			return dictionary;
		}
		public void FromDictionary(Dictionary<TKey, TValue> dictionary)
		{
			keys.Clear();
			values.Clear();
			foreach (var kvp in dictionary)
			{
				keys.Add(kvp.Key);
				values.Add(kvp.Value);
			}
		}
	}
	[Serializable]
	public class CacheInspector
	{
		public SerializableDictionary<string, string> cache = new SerializableDictionary<string, string>();
		public SerializableDictionary<string, Texture2D> coverCache = new SerializableDictionary<string, Texture2D>();
		//public SerializableDictionary<string, Texture2D> imageCache = new SerializableDictionary<string, Texture2D>();
	}

    
	void Start()
	{
		//cache = cacheInspector.cache.ToDictionary();
		//coverCache = cacheInspector.coverCache.ToDictionary();
		objectContent = new RootObject();
		checkFetch = 0;
		checkCategory = 0;
		folderPath = Path.Combine(Application.persistentDataPath, "image");
		if (!Directory.Exists(folderPath))
		{
			Directory.CreateDirectory(folderPath);
		}
		folderResizePath = Path.Combine(Application.persistentDataPath, "resizeimage");
		if (!Directory.Exists(folderResizePath))
		{
			Directory.CreateDirectory(folderResizePath);
		}
		audioFolder = Path.Combine(Application.persistentDataPath, "audio");
		if (!Directory.Exists(audioFolder))
		{
			Directory.CreateDirectory(audioFolder);
		}
		videoFolder = Path.Combine(Application.persistentDataPath, "video");
		if (!Directory.Exists(videoFolder))
		{
			Directory.CreateDirectory(videoFolder);
		}
	}
	private void Update()
	{
		if (countCache != cache.Count) { cacheInspector.cache.FromDictionary(cache); countCache = cache.Count; }
		//if (countCover != coverCache.Count) { cacheInspector.coverCache.FromDictionary(coverCache); countCover = coverCache.Count; }
		//if (countImage != imageCache.Count) { cacheInspector.imageCache.FromDictionary(imageCache); countImage = imageCache.Count; }
	}

    #region Создание классов
	[Serializable]
	public class Tag
	{
		public string id;
		public string title;
	}
	[Serializable]
	public class TagList
	{
		public List<Tag> tags;
	}
	[Serializable]
	public class Category
	{
		public string id;
		public string title;
		public string description;
	}
	[Serializable]
	public class CategoriesList
	{
		public string categoryType;
		public TagList tagLists;
		public Category category;
	}
	[Serializable]
	public class Media
	{
		public string id;
		public string url;
		//public string resizedUrl;
		public string description;
		public string type;
	}
	[Serializable]
	public class Content
	{
		public string id;               //для запроса {type category}/list
		public string title;            //для запроса {type category}/list
		public string description;      //для запроса {type category}/list   
		public string aboutInfo;        //для запроса conntent/{id content}
		public InformationAbout information;        
		public string text;             //для запроса conntent/{id content}
		public string categoryType;     //для запроса conntent/{id content}
		public Tag tag;                 //для запроса {type category}/list
		public Media cover;             //для запроса {type category}/list
		public List<Media> media;       //для запроса conntent/{id content}/media

		/*не нашла опять
		public List<Media> facts;
		public Media movie;
		public Media sound; */
	}
	[Serializable]
	public class PaginationInfo
	{
		public int totalItems;
		public int perPage;
	}
	[Serializable]
	public class RootObject
	{
		public List<Content> contents;
		public PaginationInfo paginationInfo;
		public RootObject()
		{
			contents = new List<Content>();
			paginationInfo = new PaginationInfo();
		}
		public RootObject(RootObject root)
		{
			contents = root.contents;
			paginationInfo = root.paginationInfo;
		}
	}
	[Serializable]
	public class InformationAbout
	{
		public string Artist { get; set; } //Художник
		public string ArtisticDirector { get; set; } //Художественный руководитель
		public string Choreographer { get; set; } //Хореограф
		public string Composer { get; set; } //Композитор
		public string Conductor { get; set; } //Дирижёр
		public string Director { get; set; } //Режиссер
		public string Operator { get; set; } //оператор
		public string ProductionYear { get; set; } //Год постановки
		public string Screenwriter { get; set; } //Сценарист
		public string Studio { get; set; } //Студия
		public string Theatre { get; set; } //Театр
		public string LogError;//Театр
		public GameObject aboutInfoPrefab;
		public string Roles { get; set; } //В ролях

		private static readonly string[] propertyOrder = new string[]
		{
			"Artist", "ArtisticDirector", "Choreographer", "Composer", "Conductor",
			"Director", "Operator", "ProductionYear", "Screenwriter",
			"Studio", "Theatre", "Roles"
		};

		private static readonly string[] namesProperty = new string[]
		{
			"Художник", "Художественный руководитель", "Хореограф", "Композитор", "Дирижёр",
			"Режиссер", "Оператор", "Год постановки",  "Сценарист",
			"Студия", "Театр", "В ролях"
		};

		public void CreateInfoLine(int index, Transform parentLine)
		{
			if (index < 0 || index >= propertyOrder.Length)
			{
				Debug.LogError("Index out of range");
			}
			PropertyInfo propertyInfo = typeof(InformationAbout).GetProperty(propertyOrder[index]);
			if (propertyInfo != null)
			{
                
				GameObject lineInstance = Instantiate(aboutInfoPrefab, parentLine); //линия в about
				string propertyValue = (string)propertyInfo.GetValue(this, null);

				TMP_Text nameText = lineInstance.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<TMP_Text>();
				TMP_Text valueText = lineInstance.transform.GetChild(0).gameObject.transform.GetChild(1).GetComponent<TMP_Text>();

				nameText.text = namesProperty[index];
				valueText.text = propertyValue;

				// Перестраиваем макет
				ContentSizeFitter nameFitter = nameText.GetComponent<ContentSizeFitter>();
				ContentSizeFitter valueFitter = valueText.GetComponent<ContentSizeFitter>();

				nameFitter.enabled = true;
				valueFitter.enabled = true;

				nameFitter.SetLayoutVertical();
				valueFitter.SetLayoutVertical();



				//Обновляем размер родительского RectTransform
				LayoutRebuilder.ForceRebuildLayoutImmediate(nameText.rectTransform);
				LayoutRebuilder.ForceRebuildLayoutImmediate(valueText.rectTransform);

				//Пересчитываем размер контейнера
				LayoutRebuilder.ForceRebuildLayoutImmediate(lineInstance.GetComponent<RectTransform>());

				//Обновляем размеры родительского контейнера
				RectTransform parentRectTransform = parentLine.GetComponent<RectTransform>();
				LayoutRebuilder.ForceRebuildLayoutImmediate(parentRectTransform);

				RectTransform textParentRect = nameText.transform.parent.GetComponent<RectTransform>();
				LayoutRebuilder.ForceRebuildLayoutImmediate(textParentRect);

				//Если есть еще один уровень родительских объектов(например, вложенные панели)
				RectTransform parentOfTextParentRect = textParentRect.parent.GetComponent<RectTransform>();
				if (parentOfTextParentRect != null)
				{
					LayoutRebuilder.ForceRebuildLayoutImmediate(parentOfTextParentRect);
				}

				//Наконец, обновляем размер родительского объекта всех линий(parentLine)
				RectTransform parentLineRect = parentLine.GetComponent<RectTransform>();
				LayoutRebuilder.ForceRebuildLayoutImmediate(parentLineRect);

				// В зависимости от компоновки может потребоваться дополнительно обновить ContentSizeFitter на parentLine
				ContentSizeFitter parentLineFitter = parentLine.GetComponent<ContentSizeFitter>();
				if (parentLineFitter != null)
				{
					parentLineFitter.SetLayoutVertical();
					parentLineFitter.SetLayoutHorizontal();
				}





				//ContentSizeFitter contentSizeFitter = lineInstance.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<ContentSizeFitter>();
				//contentSizeFitter.SetLayoutHorizontal();
				//LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)contentSizeFitter.transform);

				//float maxH = Math.Max(lineInstance.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().GetPreferredValues().y,
				//    lineInstance.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().GetPreferredValues().y);
				//Debug.LogWarning("max " + maxH);
				//Debug.LogWarning("m1 " + lineInstance.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().GetPreferredValues().y);
				//Debug.LogWarning("m2 " + lineInstance.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().GetPreferredValues().y);
				//lineInstance.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(
				//    lineInstance.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta.x, maxH);
				//Debug.LogWarning("text " + lineInstance.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta.y);
				//lineInstance.GetComponent<RectTransform>().sizeDelta = new Vector2(
				//    lineInstance.GetComponent<RectTransform>().sizeDelta.x, (maxH + 9f));
				//Debug.LogWarning("line " + lineInstance.GetComponent<RectTransform>().sizeDelta.y);
				//parentLine.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(
				//    parentLine.gameObject.GetComponent<RectTransform>().sizeDelta.x, (parentLine.gameObject.GetComponent<RectTransform>().sizeDelta.y + maxH + 17f));
				//Debug.LogWarning("about " + parentLine.gameObject.GetComponent<RectTransform>().sizeDelta.y);
			}
		}
		public bool GetAllValidInfo(Transform parentLine) // true - есть что-то false - нет ничего
		{
			ButtonFilters.DeleteAllChildren(parentLine.gameObject);
			if (!string.IsNullOrEmpty(LogError))
			{
				Debug.LogError(LogError);
				return false;
			}
			else
			{
				int x = 0;
				for (int i = 0; i < propertyOrder.Length; i++)
				{
					PropertyInfo propertyInfo = typeof(InformationAbout).GetProperty(propertyOrder[i]);
					string propertyValue = (string)propertyInfo.GetValue(this, null);
					if (!string.IsNullOrEmpty(propertyValue))
					{
						CreateInfoLine(i, parentLine);
						x++;
					}
				}
				if (x > 0)
				{
					parentLine.GetChild(parentLine.childCount - 1).gameObject.transform.GetChild(1).gameObject.SetActive(false);
					return true;
				}
				else return false;
			}
		}
	}
    #endregion

    #region Корутины
	public IEnumerator CategoryRequest()//Запросы category/tags и category/{type}/info
	{
		int h = 0;
		foreach (string type in categoriesType)
		{
			Debug.Log(type);
			TagList tagLists = new TagList();
			Category category = new Category();
			string url = adminApi + "category/tags?type=" + type;
			Debug.Log(url);
			UnityWebRequest request = UnityWebRequest.Get(url);
			//yield return request.SendWebRequest();
			requestManager.AddRequest(request);
			yield return StartCoroutine(requestManager.HandleRequest(request));
			if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
			{
				Debug.LogError("Error: " + request.error);
			}
			else
			{
				try
				{
					tagLists = JsonUtility.FromJson<TagList>("{\"tags\":" + request.downloadHandler.text + "}");
				}
					catch (Exception e)
					{
						Debug.LogError("Could not parse tag data: " + e.Message);
					}
			}

			url = adminApi + "category/" + type + "/info";
			request = UnityWebRequest.Get(url);
			//Debug.Log(url);
			//yield return request.SendWebRequest();
			requestManager.AddRequest(request);
			yield return StartCoroutine(requestManager.HandleRequest(request));
			if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
			{
				Debug.LogError("Error: " + request.error);
			}
			else
			{
				try
				{
					category = JsonUtility.FromJson<Category>(request.downloadHandler.text);
				}
					catch (Exception e)
					{
						Debug.LogError("Could not parse tag data: " + e.Message);
					}
			}

			cats.Add(new CategoriesList() { categoryType = type, category = category, tagLists = tagLists });
		}
	}

	public IEnumerator FetchContent() // Запрос category/{type}/list
	{
		requestManager.activeTasks++;
		foreach (string type in categoriesType)
		{
			string url = adminApi + "category/" + type + "/list";
			if (!cache.ContainsKey(url))
			{
				using (UnityWebRequest request = UnityWebRequest.Get(url))
				{
					requestManager.AddRequest(request);
					yield return StartCoroutine(requestManager.HandleRequest(request));

					if (request.result == UnityWebRequest.Result.Success)
					{
						string json = request.downloadHandler.text;
						cache[url] = json;
						Debug.Log("Data fetched from API and cached.");

						RootObject root = JsonUtility.FromJson<RootObject>(json);
						if (root.contents.Count > 0)
						{
							foreach (Content content in root.contents)
																	{
								if (string.IsNullOrEmpty(content.cover.url))
								{
									continue;
								}
								UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(content.cover.url);
								//requestManager.AddRequest(imageRequest);
								//yield return StartCoroutine(requestManager.HandleRequest(imageRequest));
								yield return imageRequest.SendWebRequest();

								if (imageRequest.result == UnityWebRequest.Result.Success)
								{
									Texture2D texture = DownloadHandlerTexture.GetContent(imageRequest);
									byte[] imageBytes = texture.EncodeToPNG();

									string savePath = Path.Combine(folderResizePath, content.cover.id + ".png");
									if (File.Exists(savePath))
									{
										Debug.Log("File already exists: " + savePath);
										//yield break; // Прекратить выполнение если файл уже существует
									}
									File.WriteAllBytes(savePath, imageBytes);
									Debug.Log("Image saved to " + savePath);

									//textures.Add(texture);
									//coverCache[content.cover.id] = texture;
								}
								else if (imageRequest.responseCode == 404)
								{
									Debug.LogError("Error 404: Not found for " + content.cover.url);
								}
								else
								{
									Debug.LogError("Error downloading image: " + imageRequest.error + " from " + content.cover.url);
								}
							}
						}
					}
					else
					{
						Debug.LogError("Error fetching data: " + request.error);
					}
				}
			}
		}
		requestManager.TaskCompleted();
		//textures.Clear();
	}
   
	public IEnumerator CardsRequest(string categoryType, Action<List<string>> callback) //Запрос category/{type}/list
	{
		string url = adminApi + "category/" + categoryType + "/list";
		if (cache.ContainsKey(url))
		{
			RootObject newObject = new RootObject(JsonUtility.FromJson<RootObject>(cache[url]));
			objectContent = newObject;
			for (int i = 0; i < objectContent.contents.Count; i++)
			{
				if (objectContent.contents[i].description is not null) objectContent.contents[i].description = ConvertHTMLToTMP(objectContent.contents[i].description);
				yield return StartCoroutine(ContentRequest(objectContent.contents[i].id));

				if (objectContent.contents[i].cover != null)
				{
				    if (objectContent.contents[i].cover.id is not null)
				    {
					    //if (coverCache.ContainsKey(objectContent.contents[i].cover.id))
				        {

				            yield return StartCoroutine(ContentRequest(objectContent.contents[i].id));
				        }
				    }
				}
				else
				{

				}
			}
			// Все обложки обработаны, вызываем callback
			//textures.Clear();
			callback(null);
            
		}
		else
		{
			// Если данных в кеше нет, вызываем callback с пустым списком или null
			//textures.Clear();
			callback(null);
		}
	}
	public IEnumerator ContentRequest(string contentId) //Запрос content/{id} и content/{id}/media
	{
		//Debug.LogWarning("Запустился ContentRequest");
		// Запрос для получения основной информации о контенте
		string contentUrl = adminApi + "content/" + contentId;
		yield return FetchData(contentUrl, (data) =>
		{
			//Debug.Log("Data fetched from API and cached: " + data);
			UpdateContent(contentId, data);
		});
	}
	
	public IEnumerator MediaRequest(string contentId)
	{
		// Запрос для получения медиа, связанного с контентом
		string mediaUrl = adminApi + "content/" + contentId + "/media";
		string data2 = "";
		yield return FetchData(mediaUrl, (data) =>
		{
			data2 = data;
			
			//Debug.Log("Data fetched from API and cached: " + data);
		});
		
		yield return UpdateMedia(contentId, data2);
	}
	
	private IEnumerator FetchData(string url, Action<string> onSuccess)
	{
		/*if (cache.TryGetValue(url, out string cachedData))
		{
			Debug.Log("Data retrieved from cache: " + cachedData);
			onSuccess?.Invoke(cachedData);
			yield break;
		}*/

		UnityWebRequest request = UnityWebRequest.Get(url);
		requestManager.AddRequest(request);
		yield return StartCoroutine(requestManager.HandleRequest(request));
		//yield return request.SendWebRequest();

		if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
		{
			Debug.LogError("Error: " + request.error);
		}
		else
		{
			cache[url] = request.downloadHandler.text;
			onSuccess?.Invoke(request.downloadHandler.text);
		}
	}
	private void UpdateContent(string contentId, string jsonData)
	{
		try
		{
			Content content = JsonUtility.FromJson<Content>(jsonData);
			Content targetContent = objectContent.contents.Find(c => c.id == contentId);
			if (targetContent != null)
			{
				targetContent.aboutInfo = content.aboutInfo;
				//Debug.LogWarning("targetContent.aboutInfo = " + targetContent.aboutInfo);
				if (targetContent.information == null)
					targetContent.information = new InformationAbout();
				try
				{
					InformationAbout info = JsonConvert.DeserializeObject<InformationAbout>(targetContent.aboutInfo);
					if (info != null)
					{
						targetContent.information = info;
					}
					else
					{
						// Не удалось десериализовать информацию, возможно, данные некорректны
						targetContent.information.LogError = "Не удалось десериализовать информацию";
					}
				}
					catch (JsonSerializationException jex)
					{
						// Логирование ошибки сериализации
						//Debug.LogError("Serialization Error: " + jex.Message);
						targetContent.information.LogError = "<b><i>Ошибки сериализации</i></b>";//+ jex.Message;
					}
					catch (Exception ex)
					{
						// Логирование других ошибок
						//Debug.LogError("General Error: " + ex.Message);
						targetContent.information.LogError = "<b><i>Неправильный формат данных</i></b>";//+ ex.Message;
					}
				if (content.text is not null) targetContent.text = ConvertHTMLToTMP(content.text);
				targetContent.categoryType = content.categoryType;
				//targetContent.factCover = content.factCover;
				//targetContent.photoVideo = content.photoVideo;
				targetContent.information.aboutInfoPrefab = infoPrefab;
			}
		}
			catch (Exception e)
			{
				Content targetContent = objectContent.contents.Find(c => c.id == contentId);
				targetContent.information.aboutInfoPrefab = infoPrefab;
				Debug.LogError("Could not parse content data: " + e.Message + " " + jsonData);
			}
	}
	private IEnumerator UpdateMedia(string contentId, string jsonData)
	{
/*		try
		{
			//Debug.LogWarning("try");
			Content content = JsonUtility.FromJson<Content>(jsonData);
			Content targetContent = objectContent.contents.Find(c => c.id == contentId);
			if (targetContent != null)
			{
				//Debug.LogWarning("!=NULL");
				targetContent.media = content.media;
				//Debug.LogWarning(targetContent.media.ToString());
				// Optionally process each media item here, e.g., fetch textures
				yield return StartCoroutine(CacheMedia(targetContent.media, targetContent));
			}
		}
			catch (Exception e)
			{
				Debug.LogError("Could not parse media data: " + e.Message + " " + jsonData);
		}
		*/

		Content content = JsonUtility.FromJson<Content>(jsonData);
		Content targetContent = objectContent.contents.Find(c => c.id == contentId);
		if (targetContent != null)
		{
			//Debug.LogWarning("!=NULL");
			targetContent.media = content.media;
			//Debug.LogWarning(targetContent.media.ToString());
			// Optionally process each media item here, e.g., fetch textures
			yield return CacheMedia(targetContent.media, targetContent);
		}
		
	}
	public string ReplaceMediaWithResize(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return input;  // Возвращает исходную строку, если она пуста или null
		}

		// Замена всех вхождений "media" на "resize"
		string output = input.Replace("media", "resize");
		return output;
	}
	public IEnumerator CacheMedia(List<Media> listMedia, Content content)
	{
		foreach (Media media in listMedia)
		{
			if (media.id != null && media.url != null && (media.type == "COVER" || media.type == "PHOTO_VIDEO" || media.type == "FACT"))
			{
				// Определяем размер файла через заголовок Content-Length (этот запрос можно оптимизировать, если сервер поддерживает HEAD запросы)
				//string newUrl = ReplaceMediaWithResize(media.url);
				string newUrl = media.url;
				UnityWebRequest sizeRequest = UnityWebRequest.Head(newUrl);
				yield return sizeRequest.SendWebRequest();
				if (sizeRequest.result != UnityWebRequest.Result.Success)
				{
					Debug.LogError("Failed to retrieve file size: " + sizeRequest.error + " url: " + newUrl);
					continue;
				}


				long contentLength = long.Parse(sizeRequest.GetResponseHeader("Content-Length"));
				Debug.LogWarning("Размер: " + contentLength + " url: " + newUrl);
				long remainingLength = contentLength;
				long start = 0;
				const int chunkSize = 2048 * 2048; // Размер загружаемого фрагмента, например 1 МБ

				List<byte> imageData = new List<byte>();

				while (remainingLength > 0)
				{
					long end = start + chunkSize - 1;
					if (end >= contentLength)
					{
						end = contentLength - 1;
					}

					UnityWebRequest request = UnityWebRequest.Get(newUrl);
					request.SetRequestHeader("Range", $"bytes={start}-{end}");
					yield return request.SendWebRequest();

					if (request.result == UnityWebRequest.Result.Success)
					{
						byte[] chunk = request.downloadHandler.data;
						imageData.AddRange(chunk);
						remainingLength -= chunk.Length;
						start += chunkSize;
					}
					else
					{
						Debug.LogError("Error downloading chunk: " + request.error);
						break;
					}
				}

				// Сохраняем полное изображение после полной загрузки
				if (remainingLength == 0)
				{
					string savePath = Path.Combine(folderPath, media.id + ".png");
					//if(IsFileLargerThan1_8MBsavePath))
					if (File.Exists(savePath))
					{
						Debug.Log("File already exists: " + savePath);
						yield break; // Прекратить выполнение если файл уже существует
					}
					File.WriteAllBytes(savePath, imageData.ToArray());
					Debug.Log("Complete image saved to " + savePath);

					// Если изображение нужно использовать в Unity, его можно загрузить из файла или прямо из массива байтов
					Texture2D texture = new Texture2D(2, 2);
					texture.LoadImage(imageData.ToArray());
					//textures.Add(texture);
					//imageCache[media.id] = texture;
				}
			}
			if(media.id != null && media.url != null && media.type == "SOUND")
			{
				string localPath = Path.Combine(audioFolder, media.id + "."+ GetSubstringAfterLastDot(media.url));
				Debug.LogWarning(content.id+"^ "+media.url + "  =  " + GetSubstringAfterLastDot(media.url));
				
				if (File.Exists(localPath))
					{
					Debug.Log("File already exists: " + localPath);
						yield break; // Прекратить выполнение если файл уже существует
					}
					UnityWebRequest request = UnityWebRequest.Get(media.url);
					requestManager.activeTasks++;	
					yield return request.SendWebRequest();
				
					if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
					{
						Debug.LogError("Ошибка загрузки аудио: " + request.error);
						requestManager.TaskCompleted();
					}
					else
					{
						requestManager.TaskCompleted();
						File.WriteAllBytes(localPath, request.downloadHandler.data);
						Debug.Log("Аудио сохранено: " + localPath);
					}
			}
			/*if (media.id != null && media.url != null && media.type == "MOVIE")
			{
			    string localPath = Path.Combine(videoFolder, media.id + "." + GetSubstringAfterLastDot(media.url));
			    Debug.LogWarning(content.id + "^ " + media.url + "  =  " + GetSubstringAfterLastDot(media.url));
			    UnityWebRequest request = UnityWebRequest.Get(media.url);
			    yield return request.SendWebRequest();

			    if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
			    {
			        Debug.LogError("Ошибка загрузки аудио: " + request.error);
			    }
			    else
			    {
			        File.WriteAllBytes(localPath, request.downloadHandler.data);
			        Debug.Log("Видео сохранено: " + localPath);
			    }
			}*/
			}
		//textures.Clear();
	}

	public static string ConvertHTMLToTMP(string htmlText)
	{
		// Замена перевода строки <br> и <br/>
		htmlText = Regex.Replace(htmlText, "<br>", "\n");
		htmlText = Regex.Replace(htmlText, "<br/>", "\n");

		// Замена <strong> аналогично <b>
		htmlText = Regex.Replace(htmlText, "<strong>", "<b>");
		htmlText = Regex.Replace(htmlText, "</strong>", "</b>");

		// Замена <em> аналогично <i>
		htmlText = Regex.Replace(htmlText, "<em>", "<i>");
		htmlText = Regex.Replace(htmlText, "</em>", "</i>");

		// Условная замена для <p>
		htmlText = Regex.Replace(htmlText, "<p></p>", "");
		htmlText = Regex.Replace(htmlText, @"^\s*<p>\s*", "", RegexOptions.Multiline);
		htmlText = Regex.Replace(htmlText, @"\s*<p>\s*", "\n");

		// Условная замена для </p>
		htmlText = Regex.Replace(htmlText, @"\s*</p>\s*$", "", RegexOptions.Multiline);
		htmlText = Regex.Replace(htmlText, @"\s*</p>\s*", "\n");

		// Замена &nbsp; на пространство
		htmlText = Regex.Replace(htmlText, "&nbsp;", " ");

		return htmlText;
	}

	//______________________________________________________________________________________________________________
	public IEnumerator CardsRequest(string categoryType) //старый запрос
	{
		Debug.Log("Start request");
		string url = adminApi + "category/" + categoryType + "/list";
		Debug.Log(url);
		if (cache.ContainsKey(url))
		{
			Debug.Log("cache " + cache[url]);
			RootObject newObject = new RootObject(JsonUtility.FromJson<RootObject>(cache[url]));
			objectContent = newObject;
			Debug.Log(cache[url]);
			for (int i = 0; i < objectContent.contents.Count; i++)
			{
				if (objectContent.contents[i].description is not null) objectContent.contents[i].description = ConvertHTMLToTMP(objectContent.contents[i].description);
				yield return StartCoroutine(ContentRequest(objectContent.contents[i].id));
				Debug.LogWarning("COVER CHECK");
				if (objectContent.contents[i].cover is not null)
				{
					if (objectContent.contents[i].cover.url is not null && objectContent.contents[i].cover.url == "http://62.109.23.170null"
						|| objectContent.contents[i].cover.url == "http://192.168.0.184null"
						|| objectContent.contents[i].cover.url == "http://95.188.79.124null")
					{
						objectContent.contents[i].cover = null;
					}
				else if (objectContent.contents[i].cover.url is not null)
				{

					if (!string.IsNullOrEmpty(objectContent.contents[i].cover.url))
					{

						UnityWebRequest request = UnityWebRequestTexture.GetTexture(objectContent.contents[i].cover.url);
						requestManager.AddRequest(request);
						yield return StartCoroutine(requestManager.HandleRequest(request));
						//yield return request.SendWebRequest();
						if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
						{
							if (request.responseCode == 404)
							{
								Debug.LogError("Error 404: Not found.");
							}
							else
							{
								Debug.LogError("Error: " + request.error + "  " + objectContent.contents[i].cover.url);
							}
						}
						else
						{
							Texture2D texture = DownloadHandlerTexture.GetContent(request);
							//Debug.Log("hash " + texture.imageContentsHash);
							//textures.Add(texture);
							//coverCache[objectContent.contents[i].cover.id] = textures[textures.Count - 1];
							//Debug.Log(m.url);
							//coverCache[objectContent.contents[i].cover.id] = DownloadHandlerTexture.GetContent(request);
							Debug.Log(objectContent.contents[i].cover.url);
						}
					}
				}
				else
				{
					objectContent.contents[i].cover = null;
				}

			}
		}
		//Debug.Log("objectContent" + objectContent.contents.Count);
		yield break; // Прерываем выполнение, если данные уже есть в кэше
	}
	//Debug.LogWarning("Закончен CardsRequest");
	//textures.Clear();
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

    public IEnumerator AssignAudioClip(string fileId, string format, AudioSource audioSource, Action onComplete)
    {
        requestManager.activeTasks++;
        string filePath = "file://" + Path.Combine(Application.persistentDataPath, "audio", fileId + "." + format);
        WWW www = new WWW(filePath);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("Ошибка загрузки аудио: " + www.error + " " + filePath);
        }
        else
        {
            audioSource.clip = www.GetAudioClip();
            audioSource.Play();
            Debug.Log("Аудио загружено и назначено: " + filePath);
            onComplete?.Invoke();
        }
        requestManager.TaskCompleted();
    }

 /*   public IEnumerator AssignVideoClip(string fileId, string format, VideoPlayer videoPlayer, Action onComplete)
    {
        string filePath = Path.Combine(Application.persistentDataPath, "video", fileId + "." + format);
        string url = "file://" + filePath;

        // Проверяем, существует ли файл
        if (!System.IO.File.Exists(filePath))
        {
            Debug.LogError("Файл не найден: " + filePath);
            yield break;
        }

        // Настройка VideoPlayer
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = url;

        // Подписка на событие окончания загрузки видео
        videoPlayer.prepareCompleted += OnPrepareCompleted;
        videoPlayer.errorReceived += OnErrorReceived;

        videoPlayer.Prepare(); // Подготовка видео к воспроизведению

        void OnPrepareCompleted(VideoPlayer source)
        {
            videoPlayer.prepareCompleted -= OnPrepareCompleted; // Отписка от события
            videoPlayer.errorReceived -= OnErrorReceived;

            videoPlayer.Play(); // Воспроизведение видео
            Debug.Log("Видео загружено и воспроизводится: " + filePath);
            onComplete?.Invoke();
        }

        void OnErrorReceived(VideoPlayer source, string message)
        {
            videoPlayer.prepareCompleted -= OnPrepareCompleted;
            videoPlayer.errorReceived -= OnErrorReceived;
            Debug.LogError("Ошибка загрузки видео: " + message);
        }
    }
*/
    //public IEnumerator AssignAudioClip(string fileId, string format, AudioSource audioSource)
    //{
    //    string filePath = Path.Combine(audioFolder, fileId + "." + format);
    //    if (!File.Exists(filePath))
    //    {
    //        Debug.LogError("Файл не найден: " + filePath);
    //        yield break;
    //    }
    //    AudioType audioType;
    //    switch (format)
    //    {
    //        case "wav":
    //            audioType = AudioType.WAV;
    //            break;
    //        case "mp3":
    //            audioType = AudioType.MPEG;
    //            break;
    //        case "ogg":
    //            audioType = AudioType.OGGVORBIS;
    //            break;
    //        default:
    //            Debug.LogError("Неподдерживаемый формат аудио: " + format);
    //            yield break;
    //    }

    //    using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, audioType))
    //    {
    //        yield return request.SendWebRequest();

    //        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
    //        {
    //            Debug.LogError(filePath+ " Ошибка загрузки аудио: " + request.error);
    //        }
    //        else
    //        {
    //            AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
    //            audioSource.clip = clip;
    //            audioSource.Play();
    //            Debug.Log("Аудио загружено и назначено: " + filePath);
    //        }
    //    }
    //}

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
    public IEnumerator MusicRequest(string url, AudioSource audioSource)
    {
        Debug.LogWarning(url);
        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.UNKNOWN))
        {
            requestManager.AddRequest(request);
            yield return StartCoroutine(requestManager.HandleRequest(request));
            //yield return www.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                // Обработка ошибок HTTP, включая 404
                if (request.responseCode == 404)
                {
                    Debug.LogError("Error 404: Not found.");
                }
                else
                {
                    Debug.LogError("Ошибка загрузки аудио: " + request.error);
                }
            }
            else
            {
                // Получаем загруженный аудиоклип
                AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
                // Устанавливаем аудиоклип в источник аудио
                audioSource.clip = clip;
                audioSource.gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<AudioPlayer>().ResetPlayer();
            }
        }
    }

    string cloudName = "dnm7a9eo7";
    string uploadPreset = "p5fekcku";  // Преднастроенный конфиг на Cloudinary для загрузки
    string apiUrl = "https://api.cloudinary.com/v1_1/{0}/image/upload";

    // Функция для загрузки изображения
    public IEnumerator UploadImage(string filePath, int maxWidth, int maxHeight)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found: " + filePath);
            yield break;
        }
        Debug.LogWarning("Работает новый метод");
        byte[] fileData = File.ReadAllBytes(filePath);
        string base64Data = System.Convert.ToBase64String(fileData);
        string url = $"https://api.cloudinary.com/v1_1/{cloudName}/image/upload";

        WWWForm form = new WWWForm();
        form.AddField("file", "data:image/jpeg;base64," + base64Data);
        form.AddField("upload_preset", uploadPreset);
        form.AddField("transformation", $"w_{maxWidth},h_{maxHeight},c_limit"); // c_limit maintains the aspect ratio

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Image uploaded and resized successfully.");
                Debug.Log(www.downloadHandler.text); // Handle response here
            }
            else
            {
                Debug.LogError("Failed to upload image: " + www.error + "  "+ url+ "  "+ form.data);
            }
        }
    }

    public Texture2D LoadAndResizeTexture(string filePath, int maxWidth, int maxHeight)
    {
        // Загрузка изображения из файла
        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found: " + filePath);
            return null;
        }
        //StartCoroutine(UploadImage(filePath, maxWidth, maxHeight));
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D sourceTexture = new Texture2D(2, 2);
        sourceTexture.LoadImage(fileData);  // Создает текстуру из загруженных данных

        // Проверка необходимости изменения размера
        if (sourceTexture.width <= maxWidth && sourceTexture.height <= maxHeight)
        {
            return sourceTexture;  // Возврат, если изменение размера не требуется
        }

        // Создание RenderTexture с новыми размерами
        float scale = Mathf.Min(maxWidth / (float)sourceTexture.width, maxHeight / (float)sourceTexture.height);
        int targetWidth = Mathf.RoundToInt(sourceTexture.width * scale);
        int targetHeight = Mathf.RoundToInt(sourceTexture.height * scale);

        RenderTexture renderTex = RenderTexture.GetTemporary(targetWidth, targetHeight);
        Graphics.Blit(sourceTexture, renderTex);

        // Создание новой Texture2D и копирование данных из RenderTexture
        RenderTexture.active = renderTex;
        Texture2D resizedTexture = new Texture2D(targetWidth, targetHeight);
        resizedTexture.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
        resizedTexture.Apply();

        // Очистка
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(renderTex);

        return resizedTexture;
    }
    //public Texture2D ResizeTexture(Texture2D originalTexture, int maxWidth, int maxHeight)
    //{
    //    Texture2D newTexture = new Texture2D(maxWidth, maxHeight);
    //    float scale = Mathf.Min(maxWidth / (float)originalTexture.width, maxHeight / (float)originalTexture.height);
    //    int width = Mathf.RoundToInt(originalTexture.width * scale);
    //    int height = Mathf.RoundToInt(originalTexture.height * scale);
    //    Color[] pixels = originalTexture.GetPixels();
    //    Color[] newPixels = newTexture.GetPixels();

    //    for (int y = 0; y < height; y++)
    //    {
    //        for (int x = 0; x < width; x++)
    //        {
    //            float xFrac = x / (float)width;
    //            float yFrac = y / (float)height;
    //            float gx = xFrac * originalTexture.width;
    //            float gy = yFrac * originalTexture.height;
    //            int gxi = (int)gx;
    //            int gyi = (int)gy;
    //            newPixels[x + y * width] = originalTexture.GetPixel(gxi, gyi);
    //        }
    //    }

    //    newTexture.SetPixels(newPixels);
    //    newTexture.Apply();
    //    return newTexture;
    //}
    public (int newWidth, int newHeight) CalculateScaledDimensions(int originalWidth, int originalHeight)
    {
        const int maxSize = 2048;

        // Определение наибольшей стороны
        float maxDimension = Mathf.Max(originalWidth, originalHeight);

        // Вычисление множителя масштабирования
        float scale = maxSize / maxDimension;

        // Уменьшаем масштаб, чтобы размер был немного меньше максимального
        scale *= 0.95f;  // Уменьшаем размер на 5% от максимального

        // Вычисление новых размеров
        int newWidth = Mathf.RoundToInt(originalWidth * scale);
        int newHeight = Mathf.RoundToInt(originalHeight * scale);

        return (newWidth, newHeight);
    }
    //public IEnumerator FetchContent()//Запрос category/{type}/list
    //{
    //    foreach (string type in categoriesType)
    //    {
    //        string url = adminApi + "category/" + type + "/list";
    //        if(!cache.ContainsKey(url))
    //        {
    //            //Debug.Log(url);
    //            using (UnityWebRequest request = UnityWebRequest.Get(url))
    //            {
    //                requestManager.AddRequest(request);
    //                yield return StartCoroutine(requestManager.HandleRequest(request));
    //                //yield return request.SendWebRequest();

    //                if (request.result == UnityWebRequest.Result.Success)
    //                {
    //                    // Сохраняем полученные данные в кэше
    //                    cache[url] = request.downloadHandler.text;
    //                    Debug.Log("Data fetched from API and cached: " + cache[url]);

    //                    RootObject root = JsonUtility.FromJson<RootObject>(cache[url]);
    //                    //Debug.Log(root.contents.Count);
    //                    if (root.contents.Count != 0)
    //                    {
    //                        foreach (Content content in root.contents)
    //                        {
    //                            Texture2D texture;
    //                            if (content.cover.url is null || content.cover.url == "") // ссылки нет
    //                            {

    //                            }
    //                            else if (!coverCache.ContainsKey(content.cover.id))   // ссылка есть
    //                            {
    //                                UnityWebRequest request1 = UnityWebRequestTexture.GetTexture(content.cover.url);
    //                                requestManager.AddRequest(request1);
    //                                yield return StartCoroutine(requestManager.HandleRequest(request1));
    //                                //yield return request1.SendWebRequest();
    //                                if (request1.result == UnityWebRequest.Result.ConnectionError || request1.result == UnityWebRequest.Result.ProtocolError)
    //                                {
    //                                    // Обработка ошибок HTTP, включая 404
    //                                    //Debug.Log(coverCache.Count);
    //                                    if (request1.responseCode == 404)
    //                                    {
    //                                        //Debug.LogError("Error 404: Not found.");
    //                                    }
    //                                    else
    //                                    {
    //                                        //Debug.LogError("Error: " + request1.error +" url "+ content.cover.url);
    //                                    }
    //                                }
    //                                else
    //                                {
    //                                    texture = DownloadHandlerTexture.GetContent(request1);
    //                                    byte[] imageBytes = texture.EncodeToPNG();

    //                                    // Сохранение массива байтов в файл
    //                                    string nameFile = content.cover.id + ".png";
    //                                    string savePath = Path.Combine(folderPath, nameFile);
    //                                    File.WriteAllBytes(savePath, imageBytes);
    //                                    Debug.Log("Image saved to " + savePath);
    //                                    //Debug.Log("hash " + texture.imageContentsHash);
    //                                    textures.Add(texture);
    //                                    coverCache[content.cover.id] = textures[textures.Count - 1];
    //                                    //Debug.Log(content.cover.url);
    //                                }
    //                            }

    //                        }
    //                        //yield break; // Прерываем выполнение, если данные уже есть в кэше
    //                    }
    //                }
    //                else
    //                {
    //                    Debug.LogError("Error fetching data: " + request.error);
    //                }
    //            }
    //        }
    //    }
    //    //Debug.Log("Cache ^" + cache.Count);
    //    textures.Clear();
    //}
    //public IEnumerator CacheMedia(List<Media> listMedia)
    //{
    //    //Debug.LogWarning("CacheMedia");
    //    foreach (Media media in listMedia)
    //    {
    //        if (media.id != null && media.url != null)
    //        {
    //            if (media.type == "COVER" || media.type == "PHOTO_VIDEO" || media.type == "FACT")
    //            {
    //                using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(media.url, nonReadable: false))
    //                {
    //                    //yield return request.SendWebRequest();
    //                    requestManager.AddRequest(request);
    //                    yield return StartCoroutine(requestManager.HandleRequest(request));
    //                    if (request.result == UnityWebRequest.Result.Success)
    //                    {
    //                        if (!imageCache.ContainsKey(media.id))
    //                        {
    //                            Texture2D texture = DownloadHandlerTexture.GetContent(request);
    //                            if (texture.width > 1024 || texture.height > 1024)
    //                            {
    //                                (int newWidth, int newHeight) = CalculateScaledDimensions(texture.width, texture.height);
    //                                Texture2D resizedTexture = ResizeTexture(texture, (int)(newWidth), (int)(newHeight));
    //                                textures.Add(resizedTexture);
    //                            }
    //                            else textures.Add(texture);
    //                            byte[] imageBytes = texture.EncodeToPNG();

    //                            // Сохранение массива байтов в файл
    //                            string nameFile = media.id + ".png";
    //                            string savePath = Path.Combine(folderPath, nameFile);
    //                            File.WriteAllBytes(savePath, imageBytes);
    //                            Debug.Log("Image saved to " + savePath);
    //                            imageCache[media.id] = textures[textures.Count - 1];
    //                        }
    //                        else
    //                        {
    //                            Debug.LogWarning("Такой id уже есть");
    //                            Texture2D texture = DownloadHandlerTexture.GetContent(request);
    //                            Texture2D texture1 = imageCache[media.id];
    //                            if (texture.width > texture1.width)
    //                            {
    //                                Debug.LogWarning("новое изображение больше!");
    //                                Debug.LogWarning(media.id);
    //                                Debug.LogWarning(imageCache[media.id].height);
    //                                Texture2D resizedTexture = ResizeTexture(texture, (int)(texture.width * 0.75), (int)(texture.height * 0.75));
    //                                textures.Add(resizedTexture);
    //                                //textures.Add(texture);
    //                                imageCache[media.id] = textures[textures.Count - 1];
    //                                Debug.LogWarning(imageCache[media.id].height);
    //                                byte[] imageBytes = texture.EncodeToPNG();

    //                                // Сохранение массива байтов в файл
    //                                string nameFile = media.id + ".png";
    //                                string savePath = Path.Combine(folderPath, nameFile);
    //                                File.WriteAllBytes(savePath, imageBytes);
    //                                Debug.Log("Image saved to " + savePath);
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //            if (media.type == "SOUND")
    //            {
    //                //Debug.LogWarning("!SOUND");
    //            }
    //        }

    //    }
    //    textures.Clear();
    //}
    #endregion
}
