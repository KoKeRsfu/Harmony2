using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Networking;
using System.IO;

public class Galery : MonoBehaviour
{
    //public ArсhiveContent.Card thisCard;
    public GameObject controller;
    public GameObject archive;
    public Button leftBtn;
    public Button rightBtn;
    
    public Sprite plug;
    public List<Sprite> listSprite = new ();
    public int indexContent;
    public string filmUrl;
    //public RenderTexture videoTexture;

    public List<Archive.Media> mediaList;
    public GameObject imagePrefab; // Префаб для отображения каждого изображения
    public GameObject linePrefab; // префаб линии

    public void LoadMedia()
    {
        ButtonFilters.DeleteAllChildren(gameObject);
        listSprite.Clear();
        int len = 0;
        GameObject line = Instantiate(linePrefab, this.transform);
        foreach (Archive.Media media in mediaList)
        {
            if (media.type == "PHOTO_VIDEO" || media.type == "COVER")
            {
                Texture2D texture;
                Sprite sprite;
                if (!string.IsNullOrEmpty(media.url))
                {
                    // Загрузка изображения из локального файла
                    //string filePath = Path.Combine(Application.persistentDataPath, media.id + ".png");
                    string filePath = Path.Combine(archive.GetComponent<Archive>().folderPath, media.id + ".png");
                    if (File.Exists(filePath))
                    {
                        if (IsFileLargerThan1_8MB(filePath))
                        {
                            filePath = Path.Combine(archive.GetComponent<Archive>().folderResizePath, media.id + ".png");
                        }
                        texture = archive.GetComponent<Archive>().LoadAndResizeTexture(filePath, 1024, 1024);
                    }
                    else
                    {
                        texture = plug.texture; // Используйте заглушку, если файл не найден
                    }

                    sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                }
                else
                {
                    texture = plug.texture;
                    sprite = plug;
                }
                len += CalculateWidth(texture);
                if (len >= linePrefab.GetComponent<RectTransform>().sizeDelta.x)
                {
                    line = Instantiate(linePrefab, this.transform);
                    len = 0;
                }
                CreateImage(sprite, line, media.description, CalculateWidth(texture));
            }
            if (media.type == "MOVIE")
            {
                //CreateVideoButton(media);
            }
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

    //public void ClickGalery()
    //{
    //    LoadMedia();
    //}

    //public void LoadMedia()
    //{
    //    ButtonFilters.DeleteAllChildren(gameObject);
    //    listSprite.Clear();
    //    int len = 0;
    //    GameObject line = Instantiate(linePrefab, this.transform);
    //    foreach (Archive.Media media in mediaList)
    //    {
    //        if(media.type == "PHOTO_VIDEO" || media.type =="COVER")
    //        {
    //            Texture2D texture;
    //            Sprite sprite;
    //            if (!string.IsNullOrEmpty(media.url))
    //            {
    //                if (archive.GetComponent<Archive>().imageCache.ContainsKey(media.id))
    //                {
    //                    texture = archive.GetComponent<Archive>().imageCache[media.id];
    //                    sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));

    //                }
    //                else
    //                {
    //                    break;
    //                    texture = plug.texture;
    //                    sprite = plug;
    //                }
    //            }
    //            else
    //            {
    //                break;
    //                texture = plug.texture;
    //                sprite = plug;
    //            }
    //            len += CalculateWidth(texture);
    //            if(len >= linePrefab.GetComponent<RectTransform>().sizeDelta.x)
    //            {
    //                line = Instantiate(linePrefab, this.transform);
    //                len = 0;
    //            }
    //            CreateImage(sprite, line, media.description, CalculateWidth(texture));

    //        }
    //        if(media.type =="MOVIE")
    //        {
    //            //CreateVideoButton(media);
    //        }
    //        //if(len >= linePrefab.GetComponent<RectTransform>().sizeDelta.x - 276)
    //    }
    //}

    //public int LoadMediaCount(List<Archive.Media> galeryList)
    //{
    //    int count = 0;
    //    ButtonFilters.DeleteAllChildren(gameObject);
    //    listSprite.Clear();
    //    int len = 0;
    //    GameObject line = Instantiate(linePrefab, this.transform);
    //    count = galeryList.Count;
    //    foreach (Archive.Media media in galeryList)
    //    {
    //        if (media.type == "PHOTO_VIDEO" || media.type == "COVER")
    //        {
    //            if (!string.IsNullOrEmpty(media.url))
    //            {
    //                if (archive.GetComponent<Archive>().imageCache.ContainsKey(media.id))
    //                {
    //                    count++;
    //                }
    //                else
    //                {
    //                    break;
    //                }
    //            }
    //            else
    //            {
    //                break;
    //            }
    //        }
    //    }
    //    return count;
    //}

    //void CreateVideoButton(Archive.Media media)
    //{
    //    GameObject videoObject = Instantiate(galeryPrefab, galleryParent);
    //    Button button = videoObject.GetComponent<Button>();
    //    button.onClick.AddListener(() => PlayVideo(media.url));
    //    Text buttonText = videoObject.GetComponentInChildren<Text>();
    //    buttonText.text = "Play Video";
    //}
    void PlayVideo(string url)
    {
        //videoPlayer.url = url;
        //videoPlayer.Play();
    }
    void CreateImage(Sprite sprite, GameObject line, String descriprion, int width = -1)
    {
        listSprite.Add(sprite);
        GameObject imageObject = Instantiate(imagePrefab, line.transform);
        Image imageComponent = imageObject.GetComponent<Image>();
        ClickGalery click = imageObject.AddComponent<ClickGalery>();
        imageComponent.sprite = sprite;
        click.myIndex = listSprite.Count-1;
        click.controller = controller;
        click.galeryController = this.gameObject;
        click.leftBtn = leftBtn;
        click.rightBtn = rightBtn;
        click.description = descriprion;

        click.MediaPlayer = controller.GetComponent<MainMenu>().MediaPlayer;
        if (width > 0)
        {
            RectTransform rectTransform = imageComponent.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(width, 360); // Устанавливаем фиксированную высоту и расчетную ширину
        }
        imageObject.GetComponent<Button>().onClick.AddListener(click.ClickPhoto);
    }

    int CalculateWidth(Texture2D texture)
    {
        // Рассчитываем ширину, чтобы сохранить пропорции изображения при фиксированной высоте 360
        return texture.width * 360 / texture.height;
    }

    public void ClickVideo()
    {
        
    }
}
