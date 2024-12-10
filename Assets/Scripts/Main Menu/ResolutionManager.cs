using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;

    private float currentHZ;
    private int currentResIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();

        resolutionDropdown.ClearOptions();
        //gets current monitor refresh Rate
        currentHZ = Screen.currentResolution.refreshRateRatio.numerator;

        for(int i = 0 ; i < resolutions.Length; i++){
            if(resolutions[i].refreshRateRatio.numerator == currentHZ){
                filteredResolutions.Add(resolutions[i]);
            }
        }

        List<String> options = new List<String>();
        for (int i = 0; i < filteredResolutions.Count ; i++){
            string resolutionOption =   filteredResolutions[i].width + "x" + filteredResolutions[i].height +
            " " + filteredResolutions[i].refreshRateRatio + " HZ";

            options.Add(resolutionOption);

            if(filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height){
                currentResIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResIndex;
        resolutionDropdown.RefreshShownValue();
    }
    public void SetResolution(int resolutionIndex){
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height,true);

    }
}
