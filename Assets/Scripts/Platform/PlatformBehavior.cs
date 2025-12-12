using UnityEngine;

[CreateAssetMenu(menuName = "Platform Behavior", fileName = "NewPlatformBehavior")]
public class PlatformBehavior: ScriptableObject
{
    [Header("Look")]
    public Color platformColor = Color.white;

    [Header("Timing")]
    public float hitWindowBeats = 0.25f;

    [Header("Spawning")]
    [Tooltip("How many platforms to spawn when player lands on this one.")]
    public int platformsToSpawn = 1;

    [Tooltip("Beats between each spawned platform (if > 1).")]
    public float beatsBetweenSpawns = 1f;

    public Vector3[] relativePositions;

    public bool usePlayerForward = true;

    [Header("Default Distance (if relativePositions is empty)")]
    public float defaultForwardDistance = 5f;
    public float defaultVerticalOffset = 0f;
    public float defaultSideOffset = 0f;

    public PlatformBehavior nextBehavior;
}