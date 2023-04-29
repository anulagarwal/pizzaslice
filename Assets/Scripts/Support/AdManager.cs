using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;


public class AdManager  : MonoBehaviour,  IBannerAdListener
{
    public static AdManager Instance = null;
    [Header("Attributes")]
    [SerializeField] bool isBannerOn;
    [SerializeField] bool isInterstitialOn;
    [SerializeField] bool isRewardedOn;
    [SerializeField] AdType adNetwork;
    [SerializeField] string key;
    [SerializeField] RewardType rewardState;

    bool isRewardedGD;

    
    private void Awake()
    {
        Application.targetFrameRate = 100;
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

       
        Instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        Appodeal.initialize(key, Appodeal.INTERSTITIAL, true);
        Appodeal.initialize(key, Appodeal.BANNER_BOTTOM, this);
        Appodeal.setSmartBanners(true);
        LoadInterstitial();
        LoadBanner();
    }

    #region Load ads


    public void LoadRewarded()
    {
        switch (adNetwork)
        {
            case AdType.Appodeal:
                Appodeal.setAutoCache(Appodeal.REWARDED_VIDEO, false);
                Appodeal.cache(Appodeal.REWARDED_VIDEO);
                break;

            case AdType.GD:
                break;

        }
    }


    public void LoadInterstitial()
    {
        
                Appodeal.setAutoCache(Appodeal.INTERSTITIAL, false);
                Appodeal.cache(Appodeal.INTERSTITIAL);
           
    }

    public void LoadBanner()
    {       
      Appodeal.show(Appodeal.BANNER_BOTTOM);                        
    }
    #endregion

    public void UpdateRewardState(RewardType t)
    {
        rewardState = t;
    }
    public bool IsRewardedAvailable()
    {

        switch (adNetwork)
        {
            case AdType.Appodeal:
                if (Appodeal.isLoaded(Appodeal.REWARDED_VIDEO))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case AdType.GD:
                if (isRewardedGD)
                {
                    return true;
                }
                else
                {
                    return false;
                }
        }
        return false;
    }

    public bool IsInterstitialAvailable()
    {
        switch (adNetwork)
        {
            case AdType.Appodeal:
                if (Appodeal.isLoaded(Appodeal.INTERSTITIAL))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case AdType.GD:
                
                break;
        }
        return false;
    }


    #region Show ads

    public void ShowRewarded()
    {
        switch (adNetwork)
        {
            case AdType.Appodeal:
                if (Appodeal.isLoaded(Appodeal.REWARDED_VIDEO))
                {
                    Appodeal.show(Appodeal.REWARDED_VIDEO);
                }
                break;

            case AdType.GD:


                break;

        }
    }

    public void ShowInterstitial()
    {
       

                if (Appodeal.isLoaded(Appodeal.INTERSTITIAL))
                {
                    Appodeal.show(Appodeal.INTERSTITIAL);
                }
            
        

    }


    public void ShowBanner()
    {

    }
    #endregion



    #region Rewarded Video callback handlers Appodeal

    //Called when rewarded video was loaded (precache flag shows if the loaded ad is precache).
    public void onRewardedVideoLoaded(bool isPrecache)
    {
        print("Video loaded");
    }

    // Called when rewarded video failed to load
    public void onRewardedVideoFailedToLoad()
    {
        print("Video failed");
    }

    // Called when rewarded video was loaded, but cannot be shown (internal network errors, placement settings, or incorrect creative)
    public void onRewardedVideoShowFailed()
    {
        print("Video show failed");
    }

    // Called when rewarded video is shown
    public void onRewardedVideoShown()
    {
        print("Video shown");
    }

    // Called when reward video is clicked
    public void onRewardedVideoClicked()
    {
        print("Video clicked");
    }

    // Called when rewarded video is closed
    public void onRewardedVideoClosed(bool finished)
    {
        print("Video closed");
    }

    // Called when rewarded video is viewed until the end
    public void onRewardedVideoFinished(double amount, string name)
    {
        print("Reward: " + amount + " " + name);
    }

    //Called when rewarded video is expired and can not be shown
    public void onRewardedVideoExpired()
    {
        print("Video expired");
    }

    #endregion


    #region Rewarded Video callback handlers GD
    public void OnResumeGame()
    {
        // RESUME MY GAME
    }

    public void OnPauseGame()
    {
        // PAUSE MY GAME
    }

    public void OnRewardGame()
    {
        // REWARD PLAYER HERE
    }

    public void OnRewardedVideoSuccess()
    {
        // Rewarded video succeeded/completed.;
    }

    public void OnRewardedVideoFailure()
    {
        // Rewarded video failed.;
    }

    public void OnPreloadRewardedVideo(int loaded)
    {
        // Feedback about preloading ad after called GameDistribution.Instance.PreloadRewardedAd
        // 0: SDK couldn't preload ad
        // 1: SDK preloaded ad

        if(loaded == 1)
        {
            isRewardedGD = true;
        }
        else
        {
            isRewardedGD = false;
        }
    }

    public void onBannerLoaded(int height, bool isPrecache)
    {
        LoadBanner();
        throw new System.NotImplementedException();

    }

    public void onBannerFailedToLoad()
    {
        throw new System.NotImplementedException();
    }

    public void onBannerShown()
    {
        throw new System.NotImplementedException();
    }

    public void onBannerShowFailed()
    {
        throw new System.NotImplementedException();
    }

    public void onBannerClicked()
    {
        throw new System.NotImplementedException();
    }

    public void onBannerExpired()
    {
        throw new System.NotImplementedException();
    }
    #endregion


}
