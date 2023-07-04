using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;
    public int Golds;
    public int Gems;
    public int Experience = 0;
    public int Highscore;
    public int AllTimeBlocksBusted;
    public int AllTimeScore;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

	#region GetAllLeaderboardsFromServer
	public void GetAllPlayerStatistics()
	{
		loginWithPlayFab.instance.Loading();
		var request = new GetPlayerStatisticsRequest();

		PlayFabClientAPI.GetPlayerStatistics(request, OnStatisticsReceived, OnStatisticsError);
	}

	private void OnStatisticsReceived(GetPlayerStatisticsResult result)
	{
		loginWithPlayFab.instance.LoadingFinished();
		// Process the player statistics data here
		foreach (var stat in result.Statistics)
		{
			var statName = stat.StatisticName;
			var statValue = stat.Value;
            if(statName == "leaderboard_ballQueue")
            {
                Highscore = statValue;
            }
            else if(statName == "LifetimeScore")
            {
                AllTimeScore = statValue;
            }
			else if (statName == "LifetimeBlocks")
			{
				AllTimeBlocksBusted = statValue;
			}
		}
        HomeScreenController.Instance.UpdateUI();
	}

	private void OnStatisticsError(PlayFabError error)
	{
		loginWithPlayFab.instance.LoadingFinished();
		// Handle any errors that occur during statistics retrieval
		Debug.LogError("Statistics retrieval error: " + error.GenerateErrorReport());
	}
	#endregion

	#region UploadMatchData
	public void ExecuteCloudScript(string functionName, int level, int Golds, int Gems)
	{
		var request = new ExecuteCloudScriptRequest
		{
			FunctionName = functionName,
			FunctionParameter = new {
                Level = level,
                Gold = Golds,
                Gem = Gems
            }
		};

		PlayFabClientAPI.ExecuteCloudScript(request, OnCloudScriptExecuted, OnCloudScriptError);
	}

	private void OnCloudScriptExecuted(ExecuteCloudScriptResult result)
	{
		// Handle the cloud script execution result here
		Debug.Log("Cloud script executed successfully!");
        RetrieveVirtualCurrencyBalance();
	}

	private void OnCloudScriptError(PlayFabError error)
	{
		// Handle any errors that occur during cloud script execution
		Debug.LogError("Cloud script execution error: " + error.GenerateErrorReport());
	}
	#endregion

	#region GetCurrencyFromServer
	public void RetrieveVirtualCurrencyBalance()
    {
        loginWithPlayFab.instance.Loading();
        GetPlayerCombinedInfoRequest request = new GetPlayerCombinedInfoRequest
        {
            PlayFabId = PlayFabClientAPI.PlayFabId,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetUserVirtualCurrency = true
            }
        };
        PlayFabClientAPI.GetPlayerCombinedInfo(request, OnGetPlayerCombinedInfoSuccess, OnGetPlayerCombinedInfoFailure);
    }

    private void OnGetPlayerCombinedInfoSuccess(GetPlayerCombinedInfoResult result)
    {
        loginWithPlayFab.instance.LoadingFinished();
        if (result != null && result.InfoResultPayload != null)
        {
            int _gembalance = 0, _goldbalance = 0, _expbalance = 0;
            if (result.InfoResultPayload.UserVirtualCurrency.ContainsKey("GE"))
            {
                _gembalance = result.InfoResultPayload.UserVirtualCurrency["GE"];
            }
            if (result.InfoResultPayload.UserVirtualCurrency.ContainsKey("GO"))
            {
                _goldbalance = result.InfoResultPayload.UserVirtualCurrency["GO"];
            }
            if (result.InfoResultPayload.UserVirtualCurrency.ContainsKey("XP"))
            {
                _expbalance = result.InfoResultPayload.UserVirtualCurrency["XP"];
            }
            PlayerPrefs.SetInt("Gems", _gembalance);
            Gems = _gembalance;
            ResourcesManager.Instance.Gems = _gembalance;
            PlayerPrefs.SetInt("Golds", _goldbalance);
            Golds = _goldbalance;
            ResourcesManager.Instance.Gold = _goldbalance;
            PlayerPrefs.SetInt("Experience", _expbalance);
            Experience = _expbalance;
            HomeScreenController.Instance.UpdateUI();
        }
        else
        {
            Debug.Log("Virtual currency balance not available.");
        }
    }

    private void OnGetPlayerCombinedInfoFailure(PlayFabError error)
	{
		loginWithPlayFab.instance.LoadingFinished();
		Debug.LogError("Failed to retrieve virtual currency balance: " + error.ErrorMessage);
    }
	#endregion
}
