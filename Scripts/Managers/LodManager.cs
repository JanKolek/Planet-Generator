/*This script manages the level of detail (LOD) of a 3D model based on the position of the camera.
 It uses an array of Transform objects to represent the different LODs and toggles between them. 
 The script also includes a button text to show LODs on/off.*/
using TMPro;
using UnityEngine;

public class LodManager : MonoBehaviour
{
    // an array of Transform objects representing the different LODs of the 3D model
    public Transform [] lods ;
    
    // a Transform object representing the camera
    public Transform camera;
    
    // keeps track of the current LOD state
    private int lodState = -1;
    
    // a boolean that determines whether to use LODs or not
    public bool useLods;
    
    // a TMP_Text object representing the text of a button to toggle LODs on/off
    public TMP_Text lodBtnText;
    
    // Update is called once per frame
    void Update()
    {
        if (lods.Length < 2)
            return;

        if (!useLods)
        {
            // if useLods is false, show the lowest LOD and hide the rest
            if (lodState == 0) return;
            showLod(lods[0], true);
            showLod(lods[1], false);
            showLod(lods[2], false);
            lodState = 0;
        }
        else
        {
            // determine the current LOD state based on the position of the camera and show the corresponding LOD
            switch (camera.localPosition.z)
            {
                case > -2.0f when lodState!=0:
                    showLod(lods[0], true);
                    showLod(lods[1], false);
                    showLod(lods[2], false);
                    lodState = 0;
                    break;
                
                case > -3.25f and <= -2f when lodState != 1:
                    showLod(lods[0], false);
                    showLod(lods[1], true);
                    showLod(lods[2], false);
                    lodState = 1;
                    break;
                
                case < -3.25f when lodState != 2:
                    showLod(lods[0], false);
                    showLod(lods[1], false);
                    showLod(lods[2], true);
                    lodState = 2;
                    break;
            }
        }
    
    }

    // shows or hides the child MeshRenderer components of a Transform object
    void showLod(Transform lod, bool show)
    {
        for (int i = 0; i < lod.childCount; i++)
        {
            lod.GetChild(i).GetComponent<MeshRenderer>().enabled = show;
        }
    }
    // called when the LOD button is clicked; toggles useLods and updates the button text
    public void onLodBtnClick()
    {
        if (useLods)
        {
            useLods = false;
            lodBtnText.text = "use level of detail: off";
        }
        else
        {
            useLods = true;
            lodBtnText.text = "use level of detail: on";
        }
    }
}
