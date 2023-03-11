/*This class controls the camera's rotation and zoom around a pivot point,
which can be either a planet or a static object. The rotation mode can be switched 
between rotating the planet and the camera, and the sensitivity of the rotation and
zooming can be set. Mouse input is used to control the rotation and zooming, and the
minimum and maximum distance that the camera can zoom can be defined.*/

using UnityEngine;
using UnityEngine.EventSystems; 

public class CameraController : MonoBehaviour
{
    // The point around which the camera will orbit
    public Transform pivot;
    // The planet the camera is focusing on (optional)
    public Transform planet;
    // The original position of the camera
    public Transform camOrigin;
    // Helper object to rotate the planet (optional)
    public Transform rotHelper;

    // Sensitivity of the rotation movement
    public float rotationSensitivity;
    // Sensitivity of the zoom movement
    public float zoomSensitivity;

    // The distance from the camera to the pivot point
    private float targetLocalDistance;
    // The target rotation of the camera
    private Vector3 targetRotation;

    // Whether to rotate the planet or the camera
    public bool rotatePlanet;

    private void Start()
    {
        // Initialize the target local distance and rotation with the current values
        targetLocalDistance = transform.localPosition.z;
        targetRotation = pivot.eulerAngles;
    }

    void Update()
    {
        //ROTATION
        float mouseX = 0;
        float mouseY = 0;
        // Check if the left mouse button is being held down
        if (Input.GetMouseButton(0))
        {
            // Check if the mouse is over a UI element
            if(EventSystem.current.IsPointerOverGameObject()) return;
            
            // Calculate the horizontal and vertical mouse input
             mouseX = Input.GetAxis("Mouse X");
             mouseY = Input.GetAxis("Mouse Y");
    
             // Invert the mouse input if rotating the planet
             if (rotatePlanet)
             {
                 mouseY *= -1;
                 mouseX *= -1;
             }
                
        }
        
        // Apply rotation to the camera
        if(rotatePlanet)
        {
            rotHelper.RotateAround(Vector3.zero, Camera.main.transform.up, mouseX * rotationSensitivity);
            rotHelper.RotateAround(Vector3.zero, Camera.main.transform.right, -mouseY * rotationSensitivity);
        }
        else
            targetRotation += new Vector3(-mouseY * rotationSensitivity, mouseX * rotationSensitivity, 0f);
    
    
    
        //ZOOM
        // Check if the mouse scroll wheel is being used
        if (Input.GetAxis("Mouse ScrollWheel") != 0) {
            // Update the target local distance based on the scroll wheel input
            targetLocalDistance += Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
            // Clamp the distance to a minimum and maximum value
            targetLocalDistance = Mathf.Clamp(targetLocalDistance,-5f, -1.4f);
        }
        
        // Move the camera towards the target local position using a Lerp function
        transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0,0,targetLocalDistance), Time.deltaTime*10);
    }
    
    private void FixedUpdate()
    {
        // Set the pivot to either the planet or the camera origin based on the rotation mode
        pivot = rotatePlanet ? planet : camOrigin;
        // Rotate the pivot towards the target rotation using a Lerp function
        pivot.transform.rotation = Quaternion.Lerp(pivot.transform.rotation, !rotatePlanet ? Quaternion.Euler(targetRotation) : rotHelper.rotation, 0.016f * 10f);
    }
    
    // Switches between rotating the planet and the camera
    public bool switchRotateMode()
    {
        rotatePlanet = !rotatePlanet;
        return rotatePlanet;
    }
}