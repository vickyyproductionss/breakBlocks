using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverPopupController : MonoBehaviour
{
	public AudioSource Impact;
	public AudioSource Woosh;
	public float targetHeight;
	public RectTransform ParentCanvas;
	public float duration;
	public RectTransform rectTransform;
	public GameObject WinEffect;
	public TMP_Text Message;
	public TMP_Text ContinueButtonText;
	public TMP_Text Golds;
	public TMP_Text Gems;

	public bool IsWin;

	private void Start()
	{
		GameManager.instance.PowerupButtons.SetActive(false);
		GameManager.instance.ParentBalls.SetActive(false);
		GameManager.instance.ParentBlocks.SetActive(false);
		if (IsWin)
		{
			StartCoroutine(ShowWinEffect());
			Message.text = "LEVEL COMPLETE";
			ContinueButtonText.text = "NEXT";
			IsWin = false;
		}
		else
		{
			Message.text = "GAME OVER";
			ContinueButtonText.text = "RETRY";
		}
		PlayerDataManager.Instance.ExecuteCloudScript("completedLevel", PlayerPrefs.GetInt("mission"), PlayerGameData.Instance.GoldsThisMatch/25, PlayerGameData.Instance.GemsThisMatch);
		loginWithPlayFab.instance.SendLeaderboard(PlayerPrefs.GetInt("highScore"));
		loginWithPlayFab.instance.SendLeaderboard(PlayerGameData.Instance.ScoreThisMatch, "LifetimeScore");
		loginWithPlayFab.instance.SendLeaderboard(PlayerGameData.Instance.BlocksBusted, "LifetimeBlocks");
	}
	IEnumerator ShowWinEffect()
	{
		WinEffect.SetActive(true);
		yield return new WaitForSeconds(3);
		WinEffect.SetActive(false);
	}

	private void OnEnable()
	{
		PlayerGameData.Instance.ScoreThisMatch = GameManager.instance.score;
		PlayerGameData.Instance.IsGameOver = true;
		ChangeWidth();
		Golds.text = "+" + (PlayerGameData.Instance.GoldsThisMatch/25).ToString();
		Gems.text = "+" + PlayerGameData.Instance.GemsThisMatch.ToString();
	}


	void ChangeHeight()
	{
		Woosh.Play();
		LeanTween.size(rectTransform, new Vector2(rectTransform.sizeDelta.x, targetHeight), duration)
			.setEase(LeanTweenType.easeSpring);
	}
	void ChangeWidth()
	{
		Impact.Play();
		LeanTween.size(rectTransform, new Vector2(ParentCanvas.sizeDelta.x, rectTransform.sizeDelta.y), 1)
			.setEase(LeanTweenType.easeSpring).setOnComplete(ChangeHeight);
	}
}
