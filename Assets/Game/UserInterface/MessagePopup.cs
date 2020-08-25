using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
[DisallowMultipleComponent]
public class MessagePopup : MonoBehaviour
{
	private static MessagePopup instance;
	public static MessagePopup Instance
	{
		get { return instance; }
	}
	private CanvasGroup canvasGroup;

	public Button inputPrefab;
	public RectTransform inputOptionParent;
	public Text messageText;

	private void Awake()
	{
		canvasGroup = GetComponent<CanvasGroup>();

		if (instance == null)
			instance = this;
		else if (this != instance)
			Destroy(this.gameObject);
	}
	public void Show(string message)
	{
		messageText.text = message;
		StartCoroutine(CO_ShowTimedMessage());
	}

	public void Show(string message, params Tuple<string, UnityAction>[] inputOptions)
	{
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
		messageText.text = message;
		foreach (var option in inputOptions)
		{
			var optionInstance = Instantiate(inputPrefab, inputOptionParent);
			optionInstance.GetComponentInChildren<Text>().text = option.Item1;
			optionInstance.onClick.AddListener(option.Item2);
		}
		StartCoroutine(FadeIn());
	}

	private IEnumerator CO_ShowTimedMessage()
	{
		yield return StartCoroutine(FadeIn());
		yield return new WaitForSecondsRealtime(1f);
		yield return StartCoroutine(FadeOut());

	}

	private IEnumerator FadeIn()
	{
		float t = 0;

		while (t < 1)
		{
			t = Mathf.Clamp01(t + Time.deltaTime);
			canvasGroup.alpha = Mathf.Lerp(0, 1, t);
			yield return null;
		}
	}

	private IEnumerator FadeOut()
	{
		float t = 1;
		while (t > 0)
		{
			t = Mathf.Clamp01(t - Time.deltaTime);
			canvasGroup.alpha = Mathf.Lerp(0, 1, t);
			yield return null;
		}
	}

}
