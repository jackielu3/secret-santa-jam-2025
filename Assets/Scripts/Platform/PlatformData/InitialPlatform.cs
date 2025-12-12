using UnityEngine;

public class InitialPlatform : MonoBehaviour, IPlatformLandingAction
{
    public void OnLanded(RhythmPlatform platform, Transform player, float offsetBeats)
    {
        Debug.Log("REEEEEEE");
        PlatformSpawner.Instance.SpawnFromPlatform(platform, player);
    }
}
