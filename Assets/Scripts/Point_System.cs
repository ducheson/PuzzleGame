using UnityEngine;

public class Point_System : MonoBehaviour
{
    public static Point_System Instance;
    public int currentPoint = 0;

    void Awake()
    {
        Instance = this;
    }

    public void UpdatePoint(int index)
    {
        currentPoint += 1 << index;
    }

    public int GetCurrentPoint()
    {
        return currentPoint;
    }
}