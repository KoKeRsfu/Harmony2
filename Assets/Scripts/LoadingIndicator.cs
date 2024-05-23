using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LoadingIndicator : MonoBehaviour
{
	public List<UnityWebRequest> activeRequests = new List<UnityWebRequest>();
	public int activeTasks = 0;
	
	public int listCount;
	void Awake()
	{
		Hide();  // Изначально скрыть лоадер
	}

	protected void Update()
	{
		listCount = activeRequests.Count;
	}
	
	public void Show()
	{
		gameObject.GetComponent<Image>().enabled = true;
		//this.gameObject.SetActive(true);
	}
	public void Hide()
	{
		gameObject.GetComponent<Image>().enabled = false;
		//this.gameObject.SetActive(false);
	}
	public void AddRequest(UnityWebRequest request) // Добавление запроса в список активных
	{
		activeTasks++;
		Show();
		activeRequests.Add(request);
		//StartCoroutine(HandleRequest(request));
	}
	public IEnumerator HandleRequest(UnityWebRequest request) // Обработка запроса
	{
		yield return request.SendWebRequest();
		activeRequests.Remove(request);
		TaskCompleted();
	}
	public void TaskCompleted() // Проверка, все ли запросы завершены
	{
		activeTasks--;
		if (activeTasks == 0)
		{
			Debug.Log("Все запросы завершены");
			Hide();
		}
	}
	public void AddTask(IEnumerator taskRoutine)
	{
		activeTasks++;  // Увеличиваем счётчик активных задач
		Show();
		StartCoroutine(HandleCustomTask(taskRoutine));
	}

	// Обработка пользовательских задач
	private IEnumerator HandleCustomTask(IEnumerator taskRoutine)
	{
		yield return StartCoroutine(taskRoutine);
		TaskCompleted();
	}
}
