using System.Collections;
using UnityEngine;
using Cinemachine;

public class TitleController : MonoBehaviour
{
    public static bool HasStartedGameplay { get; private set; } = false;

    [Header("UI")]
    [SerializeField] private GameObject titleCanvas;

    [Header("Cameras")]
    [SerializeField] private CinemachineVirtualCamera camTitle;
    [SerializeField] private CinemachineVirtualCamera camGameplay;

    [Header("Disable During Title")]
    [SerializeField] private MonoBehaviour[] disableOnTitle;

    [Header("Timing")]
    [SerializeField] private float blendWaitSeconds = 2.0f;

    private bool started;

    private void Start()
    {
        if (SessionFlags.SkipTitleOnce)
        {
            SessionFlags.SkipTitleOnce = false;
            EnterGameplayImmediately();
            return;
        }

        EnterTitleMode();
    }

    private void EnterTitleMode()
    {
        started = false;

        if (titleCanvas) titleCanvas.SetActive(true);

        if (camTitle) camTitle.Priority = 100;
        if (camGameplay) camGameplay.Priority = 0;

        SetEnabled(disableOnTitle, false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void EnterGameplayImmediately()
    {
        started = true;
        HasStartedGameplay = true;

        if (titleCanvas) titleCanvas.SetActive(false);

        if (camTitle) camTitle.Priority = 0;
        if (camGameplay) camGameplay.Priority = 100;

        SetEnabled(disableOnTitle, true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnPlayPressed()
    {
        if (started) return;
        started = true;
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        if (titleCanvas) titleCanvas.SetActive(false);

        if (camTitle) camTitle.Priority = 0;
        if (camGameplay) camGameplay.Priority = 100;

        yield return new WaitForSeconds(blendWaitSeconds);

        SetEnabled(disableOnTitle, true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private static void SetEnabled(MonoBehaviour[] list, bool enabled)
    {
        if (list == null) return;
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i] != null) list[i].enabled = enabled;
        }
    }
}
