/*This is a class for a file button that has a label text and an "onClick" method
 which calls a function in the ExportManager class to load a file with the file text.*/
using TMPro;
using UnityEngine;

public class FileButton : MonoBehaviour
{
    //Label text
    public TMP_Text fileText;
    
    //Executes when clicked file button
    public void onClick()
    {
        ExportManager.instance.loadFile(fileText.text, true);
    }
}
