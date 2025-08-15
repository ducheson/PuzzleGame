using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class Ads_Manager : MonoBehaviour
{
    public static Ads_Manager Instance;

    private RewardedAd rewardedAd;
    private string adUnitId = "ca-app-pub-3940256099942544/5224354917"; // TEST ID

    // Events
    public Action OnRewardEarned;   // Triggered when player earns reward
    public Action OnAdClosed;       // Triggered when ad is closed

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        MobileAds.Initialize(initStatus =>
        {
            LoadRewardedAd();
        });
    }

    private void LoadRewardedAd()
    {
        AdRequest request = new AdRequest();

        RewardedAd.Load(adUnitId, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError($"Rewarded ad failed to load: {error}");
                return;
            }

            rewardedAd = ad;
            Debug.Log("Rewarded ad loaded successfully.");

            // Register event handlers
            rewardedAd.OnAdFullScreenContentClosed += HandleAdClosed;
            rewardedAd.OnAdFullScreenContentFailed += HandleAdFailedToShow;
        });
    }

    public void ShowReviveAd()
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"Player earned reward: {reward.Amount} {reward.Type}");
                
                // Trigger the external callback for reward
                OnRewardEarned?.Invoke();
            });
        }
        else
        {
            Debug.Log("Rewarded ad not ready—cannot trigger reward now.");
            LoadRewardedAd();
        }
    }

    private void HandleAdClosed()
    {
        Debug.Log("Rewarded ad closed.");

        // Trigger external callback for ad closed
        OnAdClosed?.Invoke();

        LoadRewardedAd(); // Preload next
    }

    private void HandleAdFailedToShow(AdError adError)
    {
        Debug.LogError($"Ad failed to show: {adError}");
        LoadRewardedAd();
    }
}
