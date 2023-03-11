/*This is  handles saving and loading game data.
 It includes functions for opening and canceling 
 save and load dialogs, saving and loading data to and from files, 
 and displaying lists of saved files in the load dialog. 
 The class uses binary serialization to store and retrieve data, 
 and has public game objects and input fields for UI elements.
  It also includes a singleton instance to ensure there is only one instance of the class.*/
using System.IO;
using System.Linq;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;

public class ExportManager : MonoBehaviour
{

    // Singleton instance
    public static ExportManager instance;
    
    // Public game objects
    public GameObject saveDialog; // UI element for save dialog
    public GameObject loadDialog; // UI element for load dialog
    public GameObject fileRowPrefab; // Prefab for each file in load dialog
    public GameObject filesArea; // Parent object for fileRowPrefabs
    
    // Public input field
    public TMP_InputField saveFileNameInput; // Input field for save file name


    private void Awake()
    {
        // Set up the singleton instance
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    // Function for opening save dialog
    public void openSaveDialog()
    {
        saveDialog.SetActive(true);
        loadDialog.SetActive(false);
    }

    // Function for cancelling save and load dialog
    public void cancelSave()
    {
        saveDialog.SetActive(false);
        loadDialog.SetActive(false);
    }

    // Function for saving planet settings
    public void saveFile()
    {

        // Check if SavedPlanets directory exists, if not create it
        if (!Directory.Exists(System.IO.Directory.GetCurrentDirectory()+"/SavedPlanets"))
        {
            Directory.CreateDirectory(System.IO.Directory.GetCurrentDirectory()+"/SavedPlanets");
        }
       
        // Create a new binary formatter and file
        var bf = new BinaryFormatter ();
        var file = File.Create (System.IO.Directory.GetCurrentDirectory()+"/SavedPlanets" + "/"+saveFileNameInput.text+".dat");
        
        // Serialize PlanetSettings.instance to the file
        bf.Serialize (file, PlanetSettings.instance);
        file.Close();
        
        // Set the save dialog to inactive
        saveDialog.SetActive(false);
    }

    // Function for opening load dialog
    public void openLoadFileDialog()
    {
        // Set save dialog to inactive
        saveDialog.SetActive(false);
        
        // Destroy all child objects in filesArea
        for (int i = 0; i < filesArea.transform.childCount; i++)
        {
            Destroy(filesArea.transform.GetChild(i).gameObject);
        }
        
        // Set load dialog to active
        loadDialog.SetActive(true);
        
        // Get a list of files in SavedPlanets directory, sort by last write time, and instantiate a fileRowPrefab for each file in filesArea
        var info = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory()+"/SavedPlanets");
        var fileInfo = info.GetFiles().OrderBy(f => f.LastWriteTime).Reverse().ToList();
        
        foreach (var file in fileInfo)
        {
           var obj =  Instantiate(fileRowPrefab, filesArea.transform);
           obj.GetComponent<FileButton>().fileText.text = file.Name;
        }
        

    }
    
    // Function for loading planet settings from file
    public bool loadFile(string fileName, bool generatePlanet)
    {
        // Check if file exists
        if(File.Exists(System.IO.Directory.GetCurrentDirectory()+"/SavedPlanets" + "/"+fileName))
        {
            
            //Load content o the saved file into planet settings
            BinaryFormatter bf = new BinaryFormatter ();
            FileStream file = File.Open (System.IO.Directory.GetCurrentDirectory()+"/SavedPlanets" + "/"+fileName, FileMode.Open);
            PlanetSettings data = (PlanetSettings)bf.Deserialize(file);
            file.Close ();
            PlanetSettings.instance = data;
            InstancingManager.instance.shouldUseSpecificSeed = true;
            InstancingManager.instance.seedToUse = PlanetSettings.instance.lastInstancingSeed;

            //Regenerate planet
            if(generatePlanet)
                 Planet.instance.GeneratePlanetObject();

            //Close load dialog
            loadDialog.SetActive(false);
            return true;


        }
       
        //Close dialog
        loadDialog.SetActive(false);
        return false;
        
        
       
    }
}
