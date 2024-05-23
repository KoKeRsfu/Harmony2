using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Swipe : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	private Vector2 startTouchPosition;
	private Vector2 currentSwipe;

	public float minSwipeLength = 100f; // Минимальная длина свайпа для реакции

	// События для свайпов
	public event System.Action OnSwipeLeft;
	public event System.Action OnSwipeRight;

	public void OnBeginDrag(PointerEventData eventData)
	{
		startTouchPosition = eventData.position; // Запоминаем начальное положение при начале свайпа
	}

	public void OnDrag(PointerEventData eventData)
	{
		// Здесь можно добавить код, если нужно обрабатывать что-то во время движения
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		currentSwipe = eventData.position - startTouchPosition; // Расчёт вектора свайпа
		ProcessSwipe();
	}

	private void ProcessSwipe()
	{
		if (currentSwipe.magnitude < minSwipeLength)
		{
			return; // Если длина свайпа меньше минимальной, игнорируем его
		}

		currentSwipe.Normalize();

		// Проверка направления свайпа (горизонтальный)
		if (Mathf.Abs(currentSwipe.x) > Mathf.Abs(currentSwipe.y))
		{
			if (currentSwipe.x > 0)
			{
				OnSwipeRight?.Invoke(); // Свайп вправо
			}
			else
			{
				OnSwipeLeft?.Invoke(); // Свайп влево
			}
		}
	}
}
