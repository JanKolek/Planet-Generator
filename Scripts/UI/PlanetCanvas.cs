/*This class manages all the UI inputs and updates it*/
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlanetCanvas : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider strengthSlider;
    [SerializeField] private Slider baseRoughnessSlider;
    [SerializeField] private Slider roughnessSlider;
    [SerializeField] private Slider mountainStrengthSlider;
    [SerializeField] private Slider mountainRoughnessSlider;
    [SerializeField] private Slider minLevelSlider;
    [SerializeField] private Slider iterationsSlider;
    [SerializeField] private Slider treeScalesSlider;
    [SerializeField] private Slider beachSizeSlider;
    [SerializeField] private Slider grassHeightSlider;
    [SerializeField] private Slider rocksHeightSlider;
    [SerializeField] private Slider transitionsSlider;

    [Header("Inputs")]
    [SerializeField] private TMP_InputField seedInput;
    [SerializeField] private TMP_InputField treeCountInput;
    [SerializeField] private TMP_InputField groupingChanceInput;
    [SerializeField] private TMP_InputField neighborDistanceInput;

    [Header("Toggles")]
    [SerializeField] private Toggle seedToggle;

    [Header("Texts")]
    [SerializeField] private TMP_Text rotateButtonText;

    [Header("Buttons")]
    [SerializeField] private GameObject terrainButton;
    [SerializeField] private GameObject forestButton;
    [SerializeField] private GameObject brushButton;

    [Header("Panels")]
    [SerializeField] private GameObject terrainSettings;
    [SerializeField] private GameObject forestSettings;
    [SerializeField] private GameObject brushSettings;

    [Header("Other References")]
    [SerializeField] private CameraController cameraController;

    private bool isUpdating;

    private int openTabIndex = 0;
    

    //Copies all the settings from UU to settings
    public void updateButton_CLick()
    {
        PlanetSettings.instance.strength = strengthSlider.value;
        PlanetSettings.instance.iterations = (int)iterationsSlider.value;
        PlanetSettings.instance.baseShapeRoughness = baseRoughnessSlider.value;
        PlanetSettings.instance.terrainRoughness = roughnessSlider.value;

        var seed = Convert.ToInt32(seedInput.text);
        if (seedToggle.isOn)
        {
            seed = Random.Range(0, 100000);
            seedInput.text = seed.ToString();
        }
            
        PlanetSettings.instance.noiseOffset =  seed;
        
        
        PlanetSettings.instance.seaLevel = minLevelSlider.value;

        PlanetSettings.instance.mountainStrength = mountainStrengthSlider.value;
        PlanetSettings.instance.mountainFrequency = mountainRoughnessSlider.value;


        PlanetSettings.instance.treeCount = Convert.ToInt32(treeCountInput.text);
        PlanetSettings.instance.groupingChance = float.Parse(groupingChanceInput.text);
        PlanetSettings.instance.neighborDistance = float.Parse(neighborDistanceInput.text);
        PlanetSettings.instance.treeScale = treeScalesSlider.value;
        
        Planet.instance.GeneratePlanetObject();
    }

    //Copies all the settings to UI
    public void updateUI()
    {
        isUpdating = true;
        strengthSlider.value =  PlanetSettings.instance.strength ;
        iterationsSlider.value = PlanetSettings.instance.iterations;
        baseRoughnessSlider.value = PlanetSettings.instance.baseShapeRoughness;
        roughnessSlider.value = PlanetSettings.instance.terrainRoughness;

        seedInput.text = PlanetSettings.instance.noiseOffset.ToString();
         
        minLevelSlider.value = PlanetSettings.instance.seaLevel;
         

        mountainStrengthSlider.value =  PlanetSettings.instance.mountainStrength;
        mountainRoughnessSlider.value =  PlanetSettings.instance.mountainFrequency;


        treeCountInput.text =  PlanetSettings.instance.treeCount.ToString() ;
        groupingChanceInput.text =  PlanetSettings.instance.groupingChance.ToString();
        neighborDistanceInput.text = PlanetSettings.instance.neighborDistance.ToString();
        treeScalesSlider.value = PlanetSettings.instance.treeScale;
        
        beachSizeSlider.value =  PlanetSettings.instance.beachSize; 
        grassHeightSlider.value = PlanetSettings.instance.grassHeight;
        rocksHeightSlider.value =  PlanetSettings.instance.rocksHeight;
        transitionsSlider.value =  PlanetSettings.instance.transitionsSmoothness;
        isUpdating = false;
    }

    //Executes when brush settings is changed automatically
    public void onBrushSettingsChanged()
    {
        if(isUpdating)return;
        PlanetSettings.instance.beachSize = beachSizeSlider.value;
        PlanetSettings.instance.grassHeight = grassHeightSlider.value;
        PlanetSettings.instance.rocksHeight = rocksHeightSlider.value;
        PlanetSettings.instance.transitionsSmoothness = transitionsSlider.value;
        
        Planet.instance.PaintPlanet();
    }

    //Changes rotation mode
    public void rotateButtonClick()
    {
        var on = cameraController.switchRotateMode();
        rotateButtonText.text = on ? "rotate planet: on" : "rotate planet: off";
    }

    //Opens or closes terrain settings panel
    public void openTerrainSettings()
    {
        //open
        if (openTabIndex != 1)
        {
            forestSettings.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -70);
            brushSettings.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -70);
            terrainSettings.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 30);
            
           
            terrainButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-668, 130);
            forestButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 130);
            brushButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(668, 130);

            terrainButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.25f);
            forestButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.08f);
            brushButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.08f);

            openTabIndex = 1;
        }
        else//close
        {
            forestSettings.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -70);
            brushSettings.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -70);
            terrainSettings.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -70);
            
           
            terrainButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-668, 30);
            forestButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 30);
            brushButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(668, 30);
            
            terrainButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.08f);

            openTabIndex = 0;
        }
        
    }
    //Opens or closes forest settings panel
    public void openForestSettings()
    {
        //open
        if (openTabIndex != 2)
        {
            forestSettings.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 30);
            brushSettings.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -70);
            terrainSettings.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -70);
            
           
            terrainButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-668, 130);
            forestButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 130);
            brushButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(668, 130);
            
            terrainButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.08f);
            forestButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.25f);
            brushButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.08f);
            
            openTabIndex = 2;
        }
        else//close
        {
            forestSettings.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -70);
            brushSettings.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -70);
            terrainSettings.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -70);
            
           
            terrainButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-668, 30);
            forestButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 30);
            brushButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(668, 30);
            
            forestButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.08f);

            openTabIndex = 0;
        }
        
    }
    
    //Opens or closes brush settings panel
    public void openBrushSettings()
    {
        //open
        if (openTabIndex != 3)
        {
            forestSettings.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -70);
            brushSettings.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 30);
            terrainSettings.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -70);
            
           
            terrainButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-668, 130);
            forestButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 130);
            brushButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(668, 130);
            
            terrainButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.08f);
            forestButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.08f);
            brushButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.25f);
            
            openTabIndex = 3;
        }
        else//close
        {
            forestSettings.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -70);
            brushSettings.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -70);
            terrainSettings.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -70);
            
           
            terrainButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-668, 30);
            forestButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 30);
            brushButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(668, 30);

            brushButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.08f);
            
            openTabIndex = 0;
        }
        
    }

    //Exit app
    public void onExitButtonClick()
    {
        Application.Quit();
    }
}
