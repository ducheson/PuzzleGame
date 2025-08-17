using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera mainCamera;
    public Camera filteredCamera;

    private void Start()
    {
        mainCamera.enabled = false;
        filteredCamera.enabled = true;
    }

    public void ShowMainView()
    {
        mainCamera.enabled = true;
        filteredCamera.enabled = false;
    }
    public void ShowFilteredView()
    {
        mainCamera.enabled = false;
        filteredCamera.enabled = true;
    }

}
