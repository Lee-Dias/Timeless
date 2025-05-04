using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI sensTextSlider;
    [SerializeField]
    private Slider senseSlider;
    [SerializeField]
    private TextMeshProUGUI volumeTextSlider;
    [SerializeField]
    private Slider volumeSlider;
    private List<Resolution> filteredResolutions;

    void Update()
    {
        sensTextSlider.text = senseSlider.value.ToString("F0");
        volumeTextSlider.text = volumeSlider.value.ToString("F0");
    }
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, true);

    }
}
