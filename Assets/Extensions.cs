using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public static class Extensions
{

	public static void ClearChildren(this Transform transform)
	{
		for (var i = transform.childCount - 1; i > -1; i--)
			UnityEngine.Object.Destroy(transform.GetChild(i).gameObject);
	}
	
	public static void ClearChildrenImmediate(this Transform transform)
	{
		for (var i = transform.childCount - 1; i > -1; i--)
			UnityEngine.Object.DestroyImmediate(transform.GetChild(i).gameObject);
	}

	public static void ClearChildren(this Transform transform, int start, int end)
	{
		for (var i = end; i > start - 1; i--)
			if (transform.childCount > i)
				UnityEngine.Object.Destroy(transform.GetChild(i).gameObject);
	}

	public static void SetChildrenActive(this Transform transform, bool state)
	{
		for (var i = transform.childCount - 1; i > -1; i--)
			transform.GetChild(i).gameObject.SetActive(state);
	}

	public static void SetChildrenActive(this Transform transform, bool state, int startIndex, int endIndex)
	{
		endIndex = Mathf.Clamp(endIndex, 0, transform.childCount);
		startIndex = Mathf.Clamp(startIndex, 0, endIndex - 1);

		for (var i = endIndex - 1; i > startIndex - 1; i--)
			transform.GetChild(i).gameObject.SetActive(state);
	}

	public static void DepthFirst(this Transform transform, string key, out Transform result)
	{
		result = null;
		//check this node to see if its the one I want
		if (transform.name == key)
		{
			result = transform;
			return;
		}

		foreach (Transform t in transform)
		{
			t.DepthFirst(key, out result);
			//if found then end all further searches down the tree
			if (result)
				break;
		}
	}

	public static Transform DepthFirst(this Transform transform, string key)
	{
		//check this node to see if its the one I want
		if (transform.name == key)
		{
			return transform;
		}

		foreach (Transform t in transform)
		{
			var val = t.DepthFirst(key);
			//if found then end all further searches down the tree
			if (val != null)
				return val;
		}

		return null;
	}

	public static RectTransform DepthFirst(this RectTransform transform, string key)
	{
		//check this node to see if its the one I want
		if (transform.name == key)
		{
			return transform;
		}

		foreach (RectTransform t in transform)
		{
			var val = t.DepthFirst(key);
			//if found then end all further searches down the tree
			if (val != null)
				return val;
		}

		return null;
	}

	public static void DepthFirst(this RectTransform transform, string key, out RectTransform result)
	{
		result = null;
		//check this node to see if its the one I want
		if (transform.name == key)
		{
			result = transform;
			return;
		}

		foreach (RectTransform t in transform)
		{
			t.DepthFirst(key, out result);
			//if found then end all further searches down the tree
			if (result)
				break;
		}

	}
	public static void SetRectWidth(this RectTransform trans, float width)
	{
		trans.sizeDelta = new Vector2(width, trans.sizeDelta.y);
	}

	public static void SpreadAnchor(this RectTransform trans)
	{
		trans.anchorMax = Vector2.one;
		trans.anchorMin = Vector2.zero;

		trans.offsetMax = Vector2.zero;
		trans.offsetMin = Vector2.zero;
	}

	public static T GetRandomElement<T>(this List<T> list) 
	{
		return list[UnityEngine.Random.Range(0, list.Count - 1)];
	}

	public static T GetRandomElement<T>(this IEnumerable<T> list)
	{
		return list.ElementAt(UnityEngine.Random.Range(0, list.Count() - 1));
	}

	/// <summary>
	/// Set pivot without changing the position of the element
	/// </summary>
	public static void SetPivot(this RectTransform rectTransform, Vector2 pivot)
	{
		Vector3 deltaPosition = rectTransform.pivot - pivot;    // get change in pivot
		deltaPosition.Scale(rectTransform.rect.size);           // apply sizing
		deltaPosition.Scale(rectTransform.localScale);          // apply scaling
		deltaPosition = rectTransform.rotation * deltaPosition; // apply rotation

		rectTransform.pivot = pivot;                            // change the pivot
		rectTransform.localPosition -= deltaPosition;           // reverse the position change
	}

	public static void AnchorBottomLeft(this RectTransform trans)
	{
		trans.anchorMax = Vector2.zero;
		trans.anchorMin = Vector2.zero;

		trans.pivot = new Vector2(0, 0);
		trans.anchoredPosition = Vector2.zero;

		trans.localScale = Vector3.one;
	}

	public static void AnchorLeft(this RectTransform trans)
	{
		trans.anchorMax = new Vector2(0, 0.5f);
		trans.anchorMin = new Vector2(0, 0.5f);

		trans.pivot = new Vector2(0, 0.5f);
		trans.anchoredPosition = Vector2.zero;

		trans.localScale = Vector3.one;
	}

	public static void AnchorRight(this RectTransform trans)
	{
		trans.anchorMax = new Vector2(1, 0.5f);
		trans.anchorMin = new Vector2(1, 0.5f);

		trans.pivot = new Vector2(1, 0.5f);
		trans.anchoredPosition = Vector2.zero;

		trans.localScale = Vector3.one;
	}

	public static void AnchorTop(this RectTransform trans, float yOffset = 0)
	{
		trans.anchorMax = new Vector2(0.5f, 1);
		trans.anchorMin = new Vector2(0.5f, 1);

		trans.pivot = new Vector2(0.5f, 0.5f);
		trans.anchoredPosition = Vector2.zero + Vector2.up * yOffset;

		trans.localScale = Vector3.one;
	}

	public static void AnchorBottom(this RectTransform trans, float yOffset)
	{
		trans.anchorMax = new Vector2(0.5f, 0);
		trans.anchorMin = new Vector2(0.5f, 0);

		trans.pivot = new Vector2(0.5f, 0.5f);
		trans.anchoredPosition = Vector2.zero + Vector2.up * yOffset;

		trans.localScale = Vector3.one;
	}

	public static void AnchorBottomRight(this RectTransform trans)
	{
		trans.anchorMax = new Vector2(1, 0);
		trans.anchorMin = new Vector2(1, 0);

		trans.pivot = new Vector2(1, 0);
		trans.anchoredPosition = Vector2.zero;

		trans.localScale = Vector3.one * 0.8f;
	}

	public static void AnchorCenter(this RectTransform trans)
	{
		trans.anchorMax = new Vector2(0.5f, 0.5f);
		trans.anchorMin = new Vector2(0.5f, 0.5f);

		trans.pivot = new Vector2(0.5f, 0.5f);
		trans.anchoredPosition = Vector2.zero;

		trans.localScale = Vector3.one;
	}

	public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
	{
		return listToClone.Select(item => (T)item.Clone()).ToList();
	}

	public static float GetNormalizedPosition(this DateTime sampleTime, DateTime start, DateTime end)
	{
		float t = (sampleTime.Ticks - start.Ticks) /
					(end.Ticks - start.Ticks);
		return t;
	}

	public static void SetLeft(this RectTransform rt, float left)
	{
		rt.offsetMin = new Vector2(left, rt.offsetMin.y);
	}

	public static void SetRight(this RectTransform rt, float right)
	{
		rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
	}

	public static void SetTop(this RectTransform rt, float top)
	{
		rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
	}

	public static void SetBottom(this RectTransform rt, float bottom)
	{
		rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
	}

	public static float GetRectWorldWidth(this RectTransform rt)
	{
		// Convert the rectangle to world corners and grab the top left
		Vector3[] corners = new Vector3[4];
		rt.GetWorldCorners(corners);

		return (corners[2] - corners[1]).x;
	}

	/*
	 *	0
	 *	0	0
	 *		0	0
	 *	0	0
	 */
	public static void SetLayerRecursively(this Transform trans, int layer)
	{

		GameObject go = trans.gameObject;
		go.layer = layer;
		for (int i = 0; i < go.transform.childCount; i++)
			go.transform.GetChild(i).SetLayerRecursively(layer);
	}

	public static void SetListActiveState<T>(this List<T> behaviors, bool state) where T : MonoBehaviour
	{
		foreach (var g in behaviors)
			g.gameObject.SetActive(state);
	}

	public static void TransferChildren(this Transform originalParent, Transform newParent)
	{
		for (int i = originalParent.childCount - 1; i > 0; i--)
		{
			originalParent.GetChild(i).SetParent(newParent);
		}
	}

	public static GameObject Clone(this RectTransform rectTransform)
	{
		var DummyGO = new GameObject("Dummy Rect", typeof(RectTransform));
		var newRect = DummyGO.GetComponent<RectTransform>();
		newRect.anchoredPosition = rectTransform.anchoredPosition;
		newRect.anchorMax = rectTransform.anchorMax;
		newRect.anchorMin = rectTransform.anchorMin;
		newRect.offsetMax = rectTransform.offsetMax;
		newRect.offsetMin = rectTransform.offsetMin;
		return DummyGO;
	}

	public static void On(this CanvasGroup canvasGroup)
	{
		canvasGroup.alpha = 1;
		canvasGroup.blocksRaycasts = true;
		canvasGroup.interactable = true;
	}

	public static void Off(this CanvasGroup canvasGroup)
	{
		canvasGroup.alpha = 0;
		canvasGroup.blocksRaycasts = false;
		canvasGroup.interactable = false;
	}


	public static bool Compare(this Vector3 point0, Vector3 point1)
	{
		return Mathf.RoundToInt(point0.x) == Mathf.RoundToInt(point1.x) &&
		Mathf.RoundToInt(point0.y) == Mathf.RoundToInt(point1.y) &&
		Mathf.RoundToInt(point0.z) == Mathf.RoundToInt(point1.z);
	}
}

