using UnityEngine;
using UnityEngine.SceneManagement;


public class Button_Manager : MonoBehaviour
{
    private bool isPause = false;
    private PauseMenu pauseMenu;
    private Data_Manager dataManager;

    private LoadingEffect loadingEffect;
    private HistoryUI historyUI;

    private void Start()
    {
        dataManager = Data_Manager.Instance;

        loadingEffect = LoadingEffect.Instance;

        pauseMenu = PauseMenu.FindAnyObjectByType<PauseMenu>();
        historyUI = HistoryUI.FindAnyObjectByType<HistoryUI>();
    }

    public void TogglePause()
    {
        if (pauseMenu.IsAnimating()) return;

        if (isPause)
        {
            isPause = false;
            Time_System.Instance.StartTimer();
            pauseMenu.HidePauseMenu();
        }
        else
        {
            isPause = true;
            Time_System.Instance.StopTimer();
            pauseMenu.ShowPauseMenu();
        }
    }

    public void Restart()
    {
        dataManager.SaveCurrentScore();
        loadingEffect.LoadSceneWithTransition("GameScene");
    }

    public void Menu()
    {
        dataManager.SaveCurrentScore();
        loadingEffect.LoadSceneWithTransition("HomeScene");
    }

    public void Play()
    {
        loadingEffect.LoadSceneWithTransition("GameScene");
    }

    public void ToggleHistory()
    {
        historyUI.OpenHistory();
    }

    public void ClearHistory()
    {
        dataManager.ClearHistory();
        historyUI.OpenHistory();
    }
}
