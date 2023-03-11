/*This class generates a mesh for a planet surface.
It takes a mesh object, resolution, and a local Y-axis vector as inputs,
and generates a mesh by calculating vertices and triangles. 
It uses a noise function to generate terrain and mountain points for the vertices.*/

using UnityEngine;

public class PlanetMesh {
    
    private readonly Mesh mesh;
    private readonly int resolution; //Number of vertices on one axis of plane

    private readonly Vector3 localAxisY;
    private readonly Vector3 localAxisX;
    private readonly Vector3 localAxisZ;
    private readonly Noise noise; 
    
    public PlanetMesh(Mesh mesh, int resolution, Vector3 localAxisY)
    {
        this.mesh = mesh;
        this.resolution = resolution;
        this.localAxisY = localAxisY;
        noise = new Noise();

        //Construct local X axis from Y axis
        localAxisX = new Vector3(localAxisY.y, localAxisY.z, localAxisY.x);
        //Perpendicular Z axis on Y and X axis
        localAxisZ = Vector3.Cross(localAxisY, localAxisX);
    }
    

    //Constructs mesh from local variables
    public void CreateMesh()
    {
        // Initialize variables
        int numVertices = resolution * resolution;
        Vector3[] vertices = new Vector3[numVertices];
        int numTriangles = (resolution - 1) * (resolution - 1) * 6;
        int[] triangles = new int[numTriangles];
        int triangleIndex = 0;

        // Generate vertices and triangles
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                // Calculate index and progress
                int i = x + y * resolution;
                Vector2 progress = new Vector2(x, y) / (resolution - 1);

                // Generate cube and sphere points
                Vector3 cubePoint = localAxisY + (2 * progress.x - 1) * localAxisX + (2 * progress.y - 1) * localAxisZ;
                Vector3 spherePoint = cubePoint.normalized;
                vertices[i] = GetGeneratedPoint(spherePoint);

                // Generate triangles
                if (x != resolution - 1 && y != resolution - 1)
                {
                    // "Left" triangle
                    triangles[triangleIndex] = i;
                    triangles[triangleIndex + 2] = i + resolution;
                    triangles[triangleIndex + 1] = i + resolution + 1;

                    // "Right" triangle
                    triangles[triangleIndex + 3] = i;
                    triangles[triangleIndex + 4] = i + 1;
                    triangles[triangleIndex + 5] = i + resolution + 1;

                    // Increment triangle index
                    triangleIndex += 6;
                }
            }
        }

        // Clear and set mesh data
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    private Vector3 GetGeneratedPoint(Vector3 pointOnSphere)
    {
        // Calculate first layer value
        float elevation = GetTerrainPoint(pointOnSphere);
        float firstLayerValue = elevation;

        // Calculate elevation with mountain point
        float mask = firstLayerValue;
        float mountainPoint = GetMountainPoint(pointOnSphere);
        elevation += mountainPoint * mask;

        // Calculate final point and return it
        float radius = PlanetSettings.instance.radius;
        Vector3 finalPoint = pointOnSphere * radius * (1 + elevation);
        return finalPoint;
    }

    private float GetTerrainPoint(Vector3 point)
    {
        float noiseValue = 0;
        float frequency = PlanetSettings.instance.baseShapeRoughness;
        float amplitude = 1;

        // Iterate through noise functions
        for (int i = 0; i < PlanetSettings.instance.iterations; i++)
        {
            // Calculate noise value
            Vector3 noiseOffset = Vector3.one * PlanetSettings.instance.noiseOffset;
            float v = noise.Evaluate(point * frequency + noiseOffset);
        
            // Update noise value and iteration parameters
            noiseValue += (v + 1) * 0.5f * amplitude;
            frequency *= PlanetSettings.instance.terrainRoughness;
            amplitude *= 0.5f;
        }

        // Apply sea level and strength settings
        noiseValue = Mathf.Max(0, noiseValue - PlanetSettings.instance.seaLevel);
        noiseValue *= PlanetSettings.instance.strength;

        // Return final noise value
        return noiseValue;
    }

    private float GetMountainPoint(Vector3 point)
    {
        float noiseValue = 0;
        float frequency = PlanetSettings.instance.mountainFrequency;
        float amplitude = 1;
        float weight = 1;

        // Iterate through noise functions
        for (int i = 0; i < PlanetSettings.instance.iterations; i++)
        {
            // Calculate noise value
            var noiseInput = point * frequency + Vector3.one * PlanetSettings.instance.noiseOffset;
            float noiseOutput = 1 - Mathf.Abs(noise.Evaluate(noiseInput));
            noiseOutput *= noiseOutput * weight;
            
            // Update noise value and iteration parameters
            noiseValue += noiseOutput * amplitude;
            weight = noiseOutput;
            frequency *= PlanetSettings.instance.mountainFrequency;
            amplitude *= 0.5f;
        }

        // Return final noise value
        return noiseValue * PlanetSettings.instance.mountainStrength;
    }
    
    
}