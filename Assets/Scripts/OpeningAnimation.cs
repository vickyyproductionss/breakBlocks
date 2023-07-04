using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OpeningAnimation : MonoBehaviour
{
	public AudioSource Impact;
	public AudioSource Woosh;
	public float targetHeight;
	public RectTransform ParentCanvas;
	public float durationHeight;
	public float durationWidth;
	public RectTransform rectTransform;
	[SerializeField]private bool AutoClose;
	[SerializeField]private float ClosingTime;

	private void OnEnable()
	{
		Invoke("ChangeWidthOpen", 0.1f);
		if(AutoClose)
		{
			StartCoroutine(ClosePopup(ClosingTime + durationHeight + durationWidth));
		}
	}
	void ChangeHeightOpen()
	{
		SoundManager.instance.Audiosource_Play(SoundManager.instance.whoosh);
		LeanTween.size(rectTransform, new Vector2(rectTransform.sizeDelta.x, targetHeight), durationHeight).setEase(LeanTweenType.easeSpring);
	}
	void ChangeWidthOpen()
	{
		SoundManager.instance.Audiosource_Play(SoundManager.instance.impact);
		LeanTween.size(rectTransform, new Vector2(ParentCanvas.sizeDelta.x, rectTransform.sizeDelta.y), durationWidth).setEase(LeanTweenType.easeSpring).setOnComplete(ChangeHeightOpen);
	}

	IEnumerator ClosePopup(float time)
	{
		yield return new WaitForSeconds(time);
		ChangeHeightClose();
	}

	public void ChangeHeightClose()
	{
		SoundManager.instance.Audiosource_Play(SoundManager.instance.whoosh);
		LeanTween.size(rectTransform, new Vector2(rectTransform.sizeDelta.x, 2), durationHeight).setEase(LeanTweenType.easeSpring).setOnComplete(ChangeWidthClose);
	}
	void ChangeWidthClose()
	{
		SoundManager.instance.Audiosource_Play(SoundManager.instance.impact);
		LeanTween.size(rectTransform, new Vector2(0, rectTransform.sizeDelta.y), durationWidth).setEase(LeanTweenType.easeSpring).setOnComplete(DisableGO);
	}
	void DisableGO()
	{
		gameObject.SetActive(false);
	}
}
