using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] private bool restartSkipsTitleIfAlreadyStarted = true;

    [SerializeField] private AudioClip defaultMusic;
    [SerializeField] private float defaultBpm = 70;
    [SerializeField] private float defaultOffsetSeconds = 0f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SessionFlags.SkipTitleOnce = true;

            if (BeatConductor.Instance != null)
            {
                BeatConductor.Instance.EnsureSong(defaultMusic, defaultBpm, defaultOffsetSeconds);
            }

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
