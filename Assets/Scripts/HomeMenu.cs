using UnityEngine;
using UnityEngine.UI; // Needed for Graphic
using System.Collections.Generic;

public class HomeMenu : MonoBehaviour
{
    public GameObject homeMenu;

    [Header("UI Items to toggle visibility")]
    public List<Graphic> ui_Items = new List<Graphic>();

    private Dictionary<Graphic, float> originalAlphas = new Dictionary<Graphic, float>();

    private void Start()
    {
        homeMenu.SetActive(true);

        // Cache original alpha of each item
        foreach (var item in ui_Items)
        {
            if (item != null)
            {
                originalAlphas[item] = item.color.a;

                // Make them transparent at start
                Color c = item.color;
                c.a = 0f;
                item.color = c;
            }
        }
    }

    public void ShowHomeMenu()
    {
        homeMenu.SetActive(true);

        // Hide UI items (alpha = 0)
        foreach (var item in ui_Items)
        {
            if (item != null)
            {
                Color c = item.color;
                c.a = 0f;
                item.color = c;
            }
        }
    }

    public void HideHomeMenu()
    {
        homeMenu.SetActive(false);

        // Restore UI items alpha
        foreach (var item in ui_Items)
        {
            if (item != null && originalAlphas.ContainsKey(item))
            {
                Color c = item.color;
                c.a = originalAlphas[item];
                item.color = c;
            }
        }
    }
}
