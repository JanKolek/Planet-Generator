/*This class represents the planet object.
It contains various properties and methods for generating the planet's mesh, 
controlling the level of detail, painting the material, and generating instances such as trees. 
It also contains references to other components such as the PlanetCanvas and ExportManager.*/

using UnityEngine;

public class Planet : MonoBehaviour {
    
    // A reference to the single instance of the Planet class
    public static Planet instance { get; private set; }
    
    // The resolution of the planet mesh, which determines its level of detail
    [Range(2,256)]
    public int resolution = 256;
    
    // The GameObject representing the ocean
    public GameObject oceanObject;

    // An array of MeshFilter objects representing the planet's meshes
    private MeshFilter[] planetMeshFilters;
    
    // An array of PlanetMesh objects representing the planet's regions
    private PlanetMesh[] planetMeshes;
    
    // An array of face directions used for creating the planet's regions
    private static readonly Vector3[] faceDirections = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

    //Material applied on the planet
    public Material planetMaterial;

    // An array of LOD transforms used for controlling the level of detail of the planet mesh
    public Transform[] lods;

    private bool startPlanetGenerated;

    // A reference to the PlanetCanvas component used for updating the UI
    public PlanetCanvas canvas;
    
    // Shader properties used for rendering the planet material
    private static readonly int GrassDirtDistance = Shader.PropertyToID("_GrassDirtDistance");
    private static readonly int RockDistance = Shader.PropertyToID("_RockDistance");
    private static readonly int BeachSize = Shader.PropertyToID("_Beach_size");
    private static readonly int TransitionSmoothness = Shader.PropertyToID("_TransitionSmoothness");
    private static readonly int OceanLevel = Shader.PropertyToID("_OceanLevel");

    private void Awake()
    {
        // Ensure that only one instance of Planet exists
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        // Create a new instance of PlanetSettings if it doesn't exist

        PlanetSettings.instance ??= new PlanetSettings();
    }

    private void Start()
    {
        GeneratePlanetObject();
    }

    // Initialize the planet mesh
    private void Initialize()
    {
        
        if (planetMeshFilters == null || planetMeshFilters.Length == 0)
            planetMeshFilters = new MeshFilter[lods.Length*6];
        
        planetMeshes = new PlanetMesh[lods.Length*6];

       

        // Create the planet meshes for each LOD
        for (int i = 0; i < lods.Length*6; i++)
        {
            if (planetMeshFilters[i] == null)
            {
                // Create a new game object to hold the mesh
                GameObject meshObj = new GameObject("region") {transform = {parent = lods[i/6]}};

                // Add a mesh renderer component to the game object and assign the planet material
                meshObj.AddComponent<MeshRenderer>().sharedMaterial = planetMaterial;
                
                // Add a mesh filter component to the game object and assign a new mesh
                planetMeshFilters[i] = meshObj.AddComponent<MeshFilter>();
                planetMeshFilters[i].sharedMesh = new Mesh();
            }

            planetMeshes[i] = new PlanetMesh(planetMeshFilters[i].sharedMesh, resolution / (i/6+1), faceDirections[i%6]);
        }

        //Try to load the default planet form file
        if (!startPlanetGenerated)
        {
            startPlanetGenerated = true;
            if (!ExportManager.instance.loadFile("default.dat", false))
                canvas.updateButton_CLick();
            
        }
        
        //Generate the rest
        GeneratePlanet();
        GenerateInstances();
        PaintPlanet();
        canvas.updateUI();
       

    }

    //Function used for regenerating the whole planet
    public void GeneratePlanetObject()
    {
        Initialize();
    }

    //Function used for updating the planet material variabled
    public void PaintPlanet()
    {
        planetMaterial.SetFloat(GrassDirtDistance, PlanetSettings.instance.grassHeight);
        planetMaterial.SetFloat(RockDistance, PlanetSettings.instance.rocksHeight);
        planetMaterial.SetFloat(BeachSize, PlanetSettings.instance.beachSize);
        planetMaterial.SetFloat(TransitionSmoothness, PlanetSettings.instance.transitionsSmoothness);
        planetMaterial.SetFloat(OceanLevel,  oceanObject.transform.localScale.x *0.5f);
        
    }

    //Function used for generating the meshes
    private void GeneratePlanet()
    {
        //Generate mesh for each region
        foreach (PlanetMesh region in planetMeshes)
            region.CreateMesh();
        
        //Add a mesh collider to each region
        foreach (MeshFilter filter in planetMeshFilters)
        {
            var meshCollider = filter.gameObject.GetComponent<MeshCollider>();
            
            if (meshCollider == null)
                meshCollider = filter.gameObject.AddComponent<MeshCollider>();

            meshCollider.sharedMesh = filter.sharedMesh;
        }
        
        //Set ocean scale
        oceanObject.transform.localScale = Vector3.one * 2.035f  + 0.0000001f*Vector3.one*(PlanetSettings.instance.seaLevel-1);
    }

    //Function used for generating instances such as trees
    private static void GenerateInstances()
    {
        InstancingManager.instance.clearInstances();
        InstancingManager.instance.generateInstances();
    }
    
    
}