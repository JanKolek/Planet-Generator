/*This class represents a set of settings for generating a planet.
 It is marked as Serializable, which means it can be stored and loaded in Unity projects.
 It has various public fields for setting different parameters of the planet generation */
using System;
using UnityEngine;

[Serializable]
class PlanetSettings 
{
    // This static field represents the instance of the PlanetSettings class that is currently being used.
    // It allows other scripts to easily access and modify the settings.
    public static PlanetSettings instance;

    // These fields control the general properties of the planet.
    [Header("General")]
    public float radius = 1;

    // These fields control the base terrain generation.
    [Header("Base terrain settings")]
    public float strength = 1;
    [Range(1,8)]
    public int iterations = 8;
    public float baseShapeRoughness = 1;
    public float terrainRoughness = 2;
    public float noiseOffset = 42;
    public float seaLevel = 1;
    
    // These fields control the mountain generation.
    [Header("Mountains setting")]
    public float mountainStrength = 1;
    public float mountainFrequency = 1;

    // These fields control the instancing of trees.
    [Header("Instancing settings")] 
    public int treeCount = 500;
    public float groupingChance = 95;
    public float neighborDistance = 0.015f;
    public float treeScale = 1;
    public int lastInstancingSeed = 0;
    
    // These fields control the brush settings.
    [Header("Brush settings")] 
    public float beachSize = 0.02f;
    public float grassHeight = 1.162985f;
    public float rocksHeight = 1.138341f;
    public float transitionsSmoothness = 1;


   


}