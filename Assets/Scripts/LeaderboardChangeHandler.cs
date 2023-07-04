using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardChangeHandler : MonoBehaviour
{
	public Button OkayButton;
	[SerializeField] GameObject[] Checks;
	[SerializeField] GameObject[] Unchecks;
	public enum LeaderboardType
	{
		Highscore,BlocksBusted,AllTimeScore,Levels
	}
	public void OnClickOkay()
	{
		foreach (var item in Checks)
		{
			if(item.gameObject.activeInHierarchy)
			{
				loginWithPlayFab.instance.GetLeaderboard(item.gameObject.name);
				PlayerPrefs.SetString("LastLeaderboard",item.gameObject.name);
			}
		}
	}
	public void UncheckAll()
	{
		OkayButton.interactable = true;
		foreach (var item in Checks)
		{
			item.gameObject.SetActive(false);
		}
	}
}
