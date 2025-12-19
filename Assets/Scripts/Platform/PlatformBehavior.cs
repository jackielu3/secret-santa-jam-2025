using System;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Platform Behavior", fileName = "NewPlatformBehavior")]
public class PlatformBehavior : ScriptableObject
{
    [Header("Look")]
    public Color platformColor = Color.white;

    [Header("Timing")]
    public float hitWindowBeats = 0.25f;
    public float despawnAfterBeats = 2f;
    public float perfectWindowBeats = 0.2f;
    public float okayWindowBeats = 0.5f;

    [Header("Spawning")]
    [Tooltip("How many platforms to spawn when player lands on this one.")]
    public int platformsToSpawn = 1;
    [Tooltip("ONLY IF platformsToSpawn = 1")]
    public int splitsToSpawn = 0;

    [Tooltip("Beats between each spawned platform (if > 1).")]
    public float beatsBetweenSpawns = 1f;

    public Vector3[] relativePositions;

    [Header("Default Distance")]
    public float defaultForwardDistance = 5f;
    public float defaultVerticalOffset = 0f;
    public float defaultSideOffset = 0f;

    [Header("Random Spawn Distance")]
    public bool useRandom = false;
    public float spawnRadius = 0f;
    [Range(0f, 360f)] public float spawnArcDegrees = 0f;
    public float maxTopOffset = 0f;
    public float maxBotOffset = 0f;

    [Header("Rotation")]
    public Vector3 rotationMult = Vector3.one;
    
    public PlatformBehavior nextBehavior;

    [Header("Lose Plane")]
    public float loseOffset = 3f;

    [Header("Line")]
    public Vector3 dottedLineOffset = Vector3.zero;

    [Header("Special")]
    public bool first = false;
    public bool last = false;
}