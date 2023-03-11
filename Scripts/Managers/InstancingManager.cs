/*InstancingManager generates instances of a tree prefab based on settings, 
including a grouping chance and neighbor distance.
It also includes the ability to clear all instances. 
The script includes a singleton instance and uses ray casting 
to find a suitable position for each instance.*/

using UnityEngine;

public class InstancingManager : MonoBehaviour
{
    // Singleton instance
    public static InstancingManager instance { get; private set; }

    // Prefab and parent for the instances
    public GameObject pineTreePrefab;
    public Transform instancingParent;

    // Ocean object and seed settings
    public GameObject ocean;
    public int seedToUse;
    public bool shouldUseSpecificSeed;

    private void Awake()
    {
        // Set up the singleton instance
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    // Generates instances based on the current settings
    public void generateInstances()
    {
        // Use a specific seed if required
        if (shouldUseSpecificSeed)
        {
            shouldUseSpecificSeed = false;
            Random.InitState(seedToUse);
            PlanetSettings.instance.lastInstancingSeed = seedToUse;
        }
        // Otherwise use the current time as the seed
        else
        {
            PlanetSettings.instance.lastInstancingSeed = System.Environment.TickCount;
            Random.InitState(PlanetSettings.instance.lastInstancingSeed);
        }

        // Generate instances
        Vector3 dir = Vector3.right;
        for (int i = 0; i < PlanetSettings.instance.treeCount; i++)
        {
            // Calculate the direction of the instance
            if (Random.Range(0, 101) <= PlanetSettings.instance.groupingChance)
            {
                dir += new Vector3(Random.Range(-PlanetSettings.instance.neighborDistance, PlanetSettings.instance.neighborDistance),
                                   Random.Range(-PlanetSettings.instance.neighborDistance, PlanetSettings.instance.neighborDistance),
                                   Random.Range(-PlanetSettings.instance.neighborDistance, PlanetSettings.instance.neighborDistance));
            }
            else
                dir = Vector3.Normalize(new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)));


            // Raycast to find a position for the instance
            Ray ray = new Ray(Vector3.zero, dir);
            var origin = ray.GetPoint(50);
            RaycastHit hit;
            if (Physics.Raycast(origin, -dir, out hit))
            {
                // Check if the instance is underwater and retry if necessary
                if (hit.point.magnitude <= ocean.transform.localScale.y / 2)
                {
                    i--;
                    continue;
                }

                // Create the instance
                var obj = Instantiate(pineTreePrefab, hit.point, Quaternion.identity, instancingParent);
                obj.transform.localScale = Vector3.one * PlanetSettings.instance.treeScale * 0.02f;
                obj.transform.LookAt(obj.transform.position + dir);
                obj.transform.eulerAngles += new Vector3(90, 0, 0);
            }
        }
    }

    // Clears all instances from the parent object
    public void clearInstances()
    {
        for (int i = 0; i < instancingParent.childCount; i++)
        {
            Destroy(instancingParent.GetChild(i).gameObject);
        }
    }
}
