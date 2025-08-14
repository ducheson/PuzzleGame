using TMPro;
using UnityEngine;

public class PointDisplay : MonoBehaviour
{
    public TextMeshProUGUI pointText;

    private int displayedValue = 0;

    public bool immediate = false;

    void Start()
    {
        pointText.raycastTarget = false;
    }

    void Update()
    {
        if (immediate)
        {
            int immediateValue = Point_System.Instance.GetCurrentPoint();
            pointText.text = immediateValue.ToString();
            return;
        }

        if (Point_System.Instance == null || pointText == null) return;

        int targetValue = Point_System.Instance.GetCurrentPoint();
        if (displayedValue == targetValue) return;

        int difference = Mathf.Abs(targetValue - displayedValue);
        int step = GetStepSize(difference);

        displayedValue += (displayedValue < targetValue) ? step : -step;
        pointText.text = displayedValue.ToString();
    }

    private int GetStepSize(int diff)
    {
        if (diff < 10) return 1;
        if (diff < 100) return 2;
        if (diff < 1000) return 20;
        return 200;
    }
}
