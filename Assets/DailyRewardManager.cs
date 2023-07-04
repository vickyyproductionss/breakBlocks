using PlayFab.ClientModels;
using PlayFab;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DailyRewardManager : MonoBehaviour
{
    [SerializeField] GameObject RewardGold;
    [SerializeField] GameObject RewardGem;
    [SerializeField] TMP_Text GoldAmount;
    [SerializeField] TMP_Text GemAmount;
    [SerializeField] TMP_Text StreakInfo;
    void Start()
    {
		//NewUserReward();
		if (PlayerPrefs.HasKey("DailyReward"))
		{
			DailyReward();
		}
		else
		{
			NewUserReward();
		}
	}
	public void OnClickCollectButton()
	{
		int _goldAmount = 0;
		int _gemAmount = 0;
		if(int.TryParse(GoldAmount.text,out _goldAmount))
		{
			ExecuteCloudScript("AddVirtualCurrency", _goldAmount, "GO");
		}
		if(int.TryParse(GemAmount.text,out _gemAmount))
		{
			ExecuteCloudScript("AddVirtualCurrency", _gemAmount, "GE");
		}
		gameObject.SetActive(false);
	}
	public void ExecuteCloudScript(string functionName, int _amount, string _code)
	{
		var request = new ExecuteCloudScriptRequest
		{
			FunctionName = functionName,
			GeneratePlayStreamEvent = true,
			FunctionParameter = new
			{
				StatName = _code,
				Amount = _amount
			}
		};

		PlayFabClientAPI.ExecuteCloudScript(request, OnCloudScriptExecuted, OnCloudScriptError);
	}

	private void OnCloudScriptExecuted(ExecuteCloudScriptResult result)
	{
		// Handle the cloud script execution result here
		Debug.Log("Cloud script executed successfully!");
		Debug.Log(result.FunctionResult.ToString() + " Got this");
		if (result.FunctionResult.ToString() == "200")
		{

		}
		else
		{
			GameManager.instance.ShowMessage("Error occured \n Check your connection!!!", true);
		}
	}

	private void OnCloudScriptError(PlayFabError error)
	{
		GameManager.instance.ShowMessage("Error occured \n Check your connection!!!", true);
		// Handle any errors that occur during cloud script execution
		Debug.LogError("Cloud script execution error: " + error.GenerateErrorReport());
	}
	void NewUserReward()
    {
        RewardGold.SetActive(true);
        RewardGem.SetActive(false);
        GoldAmount.text = "250";
        StreakInfo.text = "Welcome bonus";
        PlayerPrefs.SetInt("DailyReward", 1);
        PlayerPrefs.SetString("LastRewardDate", System.DateTime.Now.ToString());
    }
    void DailyReward()
    {
        DateTime LastCollectTime = DateTime.Parse(PlayerPrefs.GetString("LastRewardDate"));
        DateTime CurrentTime = DateTime.Now;
        double HoursDifference = (LastCollectTime - CurrentTime).TotalHours;

		if (HoursDifference > 24)
        {
			PlayerPrefs.SetString("LastRewardDate", CurrentTime.ToString());
			if (HoursDifference > 48)
            {
				//Not eligible for streak
				int streak = PlayerPrefs.GetInt("DailyReward");
				streak = 1;
				PlayerPrefs.SetInt("DailyReward", streak);
				StreakInfo.text = $"STREAK x {streak} DAYS";
				if (streak <= 3)
				{
					RewardGold.SetActive(true);
					RewardGem.SetActive(false);
					GoldAmount.text = (250 * streak).ToString();
				}
				else if (streak <= 7)
				{
					RewardGold.SetActive(true);
					RewardGem.SetActive(true);
					GoldAmount.text = (300 * streak).ToString();
					GemAmount.text = (10 * streak).ToString();
				}
				else if (streak <= 15)
				{
					RewardGold.SetActive(true);
					RewardGem.SetActive(true);
					GoldAmount.text = (400 * streak).ToString();
					GemAmount.text = (20 * streak).ToString();
				}
				else if (streak <= 30)
				{
					RewardGold.SetActive(true);
					RewardGem.SetActive(true);
					GoldAmount.text = (500 * streak).ToString();
					GemAmount.text = (30 * streak).ToString();
				}
				else if (streak > 30)
				{
					RewardGold.SetActive(true);
					RewardGem.SetActive(true);
					GoldAmount.text = (750 * streak).ToString();
					GemAmount.text = (50 * streak).ToString();
				}
			}
            else
            {
                //Eligible for streak
                int streak = PlayerPrefs.GetInt("DailyReward");
                streak++;
                PlayerPrefs.SetInt("DailyReward", streak);
                StreakInfo.text = $"STREAK x {streak} DAYS";
                if(streak <= 3)
                {
					RewardGold.SetActive(true);
					RewardGem.SetActive(false);
					GoldAmount.text = (250*streak).ToString();
				}
                else if(streak <= 7)
                {
					RewardGold.SetActive(true);
					RewardGem.SetActive(true);
					GoldAmount.text = (300 * streak).ToString();
					GemAmount.text = (10 * streak).ToString();
				}
				else if (streak <= 15)
				{
					RewardGold.SetActive(true);
					RewardGem.SetActive(true);
					GoldAmount.text = (400 * streak).ToString();
					GemAmount.text = (20 * streak).ToString();
				}
				else if (streak <= 30)
				{
					RewardGold.SetActive(true);
					RewardGem.SetActive(true);
					GoldAmount.text = (500 * streak).ToString();
					GemAmount.text = (30 * streak).ToString();
				}
				else if (streak > 30)
				{
					RewardGold.SetActive(true);
					RewardGem.SetActive(true);
					GoldAmount.text = (750 * streak).ToString();
					GemAmount.text = (50 * streak).ToString();
				}
			}
        }
		else
		{
			this.gameObject.SetActive(false);
		}
    }
}
