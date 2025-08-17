using UnityEngine;
using UnityEngine.SceneManagement;


public class Button_Manager : MonoBehaviour
{
    private bool isPause = false;
    private PauseMenu pauseMenu;
    private ResultMenu resultMenu;
    private Data_Manager dataManager;
    private HistoryUI historyUI;
    private HomeMenu homeMenu;
    private CameraManager cameraManager;
    private Revive_System revive_System;

    private void Start()
    {
        dataManager = Data_Manager.Instance;

        pauseMenu = PauseMenu.FindAnyObjectByType<PauseMenu>();
        resultMenu = ResultMenu.FindAnyObjectByType<ResultMenu>();
        historyUI = HistoryUI.FindAnyObjectByType<HistoryUI>();
        homeMenu = HomeMenu.FindAnyObjectByType<HomeMenu>();
        cameraManager = CameraManager.FindAnyObjectByType<CameraManager>();
        revive_System = Revive_System.FindAnyObjectByType<Revive_System>();
    }

    public void Pause()
    {
        if (pauseMenu.IsAnimating()) return;

        if (!isPause)
        {
            isPause = true;
            Time_System.Instance.StopTimer();
            pauseMenu.ShowPauseMenu();
        }
    }

    public void UnPause()
    {
        if (pauseMenu.IsAnimating()) return;

        if (isPause)
        {
            isPause = false;
            Time_System.Instance.StartTimer();
            pauseMenu.HidePauseMenu();
        }
    }

    public void UnPauseImediate()
    {
        if (pauseMenu.IsAnimating()) return;

        if (isPause)
        {
            isPause = false;
            pauseMenu.HideImmediateMenu();
        }
    }

    public void Restart()
    {
        LoadingEffect.Instance.LoadInEffect(() =>
        {
            Time.timeScale = 1f;
            dataManager.SaveCurrentScore();
            dataManager.ResetCurrentData();
            revive_System.ResetRevive();

            UnPauseImediate();
            resultMenu.HideImmediateMenu();
            
            LoadingEffect.Instance.LoadOutEffect();
        });
    }

    public void Play()
    {
        LoadingEffect.Instance.LoadInEffect(() =>
        {
            Time.timeScale = 1f;
            dataManager.ResetCurrentData();
            revive_System.ResetRevive();
            
            homeMenu.HideHomeMenu();
            cameraManager.ShowMainView();

            LoadingEffect.Instance.LoadOutEffect();
        });
    }

    public void Menu()
    {
        LoadingEffect.Instance.LoadInEffect(() =>
        {
            Time.timeScale = 1f;
            dataManager.SaveCurrentScore();
            dataManager.ResetCurrentData();
            revive_System.ResetRevive();

            UnPauseImediate();
            resultMenu.HideImmediateMenu();

            homeMenu.ShowHomeMenu();
            cameraManager.ShowFilteredView();

            LoadingEffect.Instance.LoadOutEffect();
        });
    }

    public void OpenHistory()
    {
        historyUI.OpenHistory();
    }

    public void CloseHistory()
    {
        historyUI.CloseHistory();
    }

    public void ClearHistory()
    {
        dataManager.ClearHistory();
        historyUI.OpenHistory();
    }

    public void Revive()
    {
        if (!revive_System.hasWatchedAd)
            Ads_Manager.Instance.ShowReviveAd();
        else
            revive_System.Revive();
    }
}
