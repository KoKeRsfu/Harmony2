using System.IO;
using UnityEngine;

public class Logger : MonoBehaviour
{
	private string logFilePath;

	void Awake()
	{
		// Указываем путь к файлу лога
		logFilePath = Path.Combine(Application.persistentDataPath, "appLog.txt");

		// Подписываемся на событие логирования
		Application.logMessageReceived += HandleLog;
	}

	private void HandleLog(string logString, string stackTrace, LogType type)
	{
		// Форматируем лог
		string logEntry = $"{System.DateTime.Now.ToLongTimeString()} {type}: {logString}\n";
		if (type == LogType.Exception || type == LogType.Error)
		{
			logEntry += $"StackTrace: {stackTrace}\n";
		}

		// Записываем лог в файл
		File.AppendAllText(logFilePath, logEntry);
	}

	void OnDestroy()
	{
		// Отписываемся от события при уничтожении объекта
		Application.logMessageReceived -= HandleLog;
	}
}
