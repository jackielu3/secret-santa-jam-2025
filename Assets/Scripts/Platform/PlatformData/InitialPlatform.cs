using UnityEngine;

public class InitialPlatform : MonoBehaviour, IPlatformLandingAction
{
    public void OnLanded(RhythmPlatform platform, Transform player, float offsetBeats, bool lastInChain)
    {
        PlatformSpawner.Instance.SpawnFromPlatform(platform, player);
    }
}
