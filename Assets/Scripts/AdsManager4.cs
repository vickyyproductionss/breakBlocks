using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsManager4 : MonoBehaviour
{
    public static AdsManager4 instance;
    //string Ad_ID = "Rewarded_video";
    //string Ad_ID_Reward = "Rewarded_Android";

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        
    }
    
    void showBanner()
    {
        
    }
    public void HideBanner()
    {
    }
    public void showInterstitial(string AD_ID)
    {
        
    }
    public void showRewardedVideoAd(string AD_ID)
    {
        
    }
    IEnumerator RepeatBannerAd()
    {
        yield return new WaitForSeconds(1);
        showBanner();
    }
    
    // Update is called once per frame
    
    public void OnUnityAdsDidError(string message)
    {
        Debug.Log("Error" + message);
    }

    

    public void OnUnityAdsDidStart(string placementId)
    {
        Debug.Log("Ad started");
    }

    public void OnUnityAdsReady(string placementId)
    {
        Debug.Log("Ad is ready");
    }
}
