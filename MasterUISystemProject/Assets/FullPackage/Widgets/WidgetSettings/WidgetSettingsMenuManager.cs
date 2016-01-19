﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WidgetSettingsMenuManager : MonoBehaviour {

    public Image buttonImage;
    public GameObject chooseWidgetPanel;
    public WidgetSettingsManager settingsManager;

    private bool shouldOpen = false;

    public void ToggleWidgetPanel()
    {
        shouldOpen = !shouldOpen;
        if (shouldOpen)
        {
            chooseWidgetPanel.SetActive(true);
            buttonImage.color = Color.red;
            settingsManager.GenerateSettingsFileButtons();
        }
        else
        {
            chooseWidgetPanel.SetActive(false);
            buttonImage.color = Color.white;
        }
    }
}