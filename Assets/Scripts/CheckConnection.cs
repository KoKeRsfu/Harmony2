using UnityEngine.Networking;
using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class CheckConnection : MonoBehaviour
{
	public TMP_Text resultText; // Свяжите это с Text на вашей сцене через инспектор
	private string url = "http://95.188.79.124:8888/swagger"; // Замените URL на нужный вам адрес

	private void Start()
	{
		StartCoroutine(CheckConnectionCoroutine());
	}

	IEnumerator CheckConnectionCoroutine()
	{
		while (true) // Бесконечный цикл для периодической проверки
		{
			// Отправляем запрос
			using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
			{
				// Ожидаем завершения запроса
				yield return webRequest.SendWebRequest();
				DateTime now = DateTime.Now;
				if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
					webRequest.result == UnityWebRequest.Result.ProtocolError)
				{
					// В случае ошибки выводим сообщение о провале
					resultText.text = $"{now.ToString("HH: mm: ss")}: Ошибка подключения: {webRequest.error}";
				}
				else
				{
					// При успешном подключении выводим сообщение об успехе
					resultText.text = $"{now.ToString("HH: mm: ss")}: Успешное подключение!";
				}
			}

			// Ожидаем 5 секунд перед следующим запросом
			yield return new WaitForSeconds(10f);
		}
	}
}
