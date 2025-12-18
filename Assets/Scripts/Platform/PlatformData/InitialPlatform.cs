using UnityEngine;

public class InitialPlatform : MonoBehaviour, IPlatformLandingAction
{
    public void OnLanded(RhythmPlatform platform, Transform player, float offsetBeats, bool lastInChain)
    {
        Debug.Log("YIPPEE");

        PlatformSpawner.Instance.SpawnFromPlatform(platform, player);
    }
}
