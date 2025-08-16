using UnityEngine;

public class Time_System : MonoBehaviour
{
    public static Time_System Instance;
    public float currentTime = 0f;
    public bool isCounting = true;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (isCounting)
        {
            currentTime += Time.deltaTime;
        }
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }

    public void StartTimer()
    {
        isCounting = true;
    }

    public void StopTimer()
    {
        isCounting = false;
    }

    public void ResetTime()
    {
        currentTime = 0;
    }
}
