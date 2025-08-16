using UnityEngine;
using GoogleMobileAds.Api;
using System;
using UnityEngine.SceneManagement;

public class Ads_Manager : MonoBehaviour
{
    public static Ads_Manager Instance;

    // Rewarded Ad
    private RewardedAd rewardedAd;
    private string rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917"; // TEST ID

    // Banner Ad
    private BannerView bannerView;
    private string bannerAdUnitId = "ca-app-pub-3940256099942544/6300978111"; // TEST Banner ID

    // Events
    public Action OnRewardEarned;
    public Action OnAdClosed;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Subscribe to sceneLoaded event
            SceneManager.sceneLoaded += OnSceneLoaded;
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
            LoadBannerAd(); // Load banner immediately for first scene
        });
    }

    #region Rewarded Ad
    private void LoadRewardedAd()
    {
        AdRequest request = new AdRequest();

        RewardedAd.Load(rewardedAdUnitId, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError($"Rewarded ad failed to load: {error}");
                return;
            }

            rewardedAd = ad;
            Debug.Log("Rewarded ad loaded successfully.");
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
        OnAdClosed?.Invoke();
        LoadRewardedAd();
    }

    private void HandleAdFailedToShow(AdError adError)
    {
        Debug.LogError($"Ad failed to show: {adError}");
        LoadRewardedAd();
    }
    #endregion

    #region Banner Ad
    public void LoadBannerAd()
    {
        // Destroy any existing banner first
        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
        }

        // Get adaptive banner size based on current screen width
        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(Screen.width);

        // Create banner with adaptive size
        bannerView = new BannerView(bannerAdUnitId, adaptiveSize, AdPosition.Bottom);

        // Load ad
        AdRequest request = new AdRequest();
        bannerView.LoadAd(request);

        // Show immediately
        bannerView.Show();
    }

    public void ShowBanner()
    {
        // If banner exists, show it; if not, reload
        if (bannerView != null)
        {
            bannerView.Show();
        }
        else
        {
            LoadBannerAd();
        }
    }

    public void HideBanner()
    {
        if (bannerView != null)
        {
            bannerView.Hide();
        }
    }
    #endregion

    #region Scene Handling
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Automatically show banner whenever a new scene loads
        LoadBannerAd();
        ShowBanner();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    #endregion
}
