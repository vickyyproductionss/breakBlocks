using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HomeScreenController : MonoBehaviour
{
    [SerializeField] private TMP_Text Rank, Experience, LifetimeScore, LifetimeBlocks, Gems, Golds;
    [SerializeField] public GameObject DailyRewardScreen;
    public static HomeScreenController Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    private void start()
    {
        ResourcesManager.Instance.OnInventoryUpdate += UpdateGemsAndGolds;
    }
    public void UpdateUI()
    {
        Rank.text = PlayerPrefs.GetInt("Rank").ToString();
        Experience.text = PlayerDataManager.Instance.Experience.ToString();
        LifetimeBlocks.text = PlayerDataManager.Instance.AllTimeBlocksBusted.ToString();
        LifetimeScore.text = PlayerDataManager.Instance.AllTimeScore.ToString();
    }
    void UpdateGemsAndGolds(int gems, int golds)
    {
        Gems.text = gems.ToString();
        Golds.text = golds.ToString();
    }
}
