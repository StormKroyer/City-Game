using UnityEngine;

public class Fog : MonoBehaviour
{
    [Tooltip("Transform used to check Y position. If null the script will use Camera.main.transform or the GameObject tagged 'Player'.")]
    [SerializeField] private Transform target;

    [Tooltip("Enable fog when target.y is strictly below this value.")]
    [SerializeField] private float thresholdY = 20f;

    // keep original fog state so we can restore it if this component is disabled
    private bool originalFogEnabled;
    private bool lastAppliedFogState;

    void Start()
    {
        originalFogEnabled = RenderSettings.fog;

        if (target == null)
        {
            // prefer an object explicitly tagged "Player", otherwise fall back to the main camera
            var player = GameObject.FindWithTag("Player");
            if (player != null) target = player.transform;
            else if (Camera.main != null) target = Camera.main.transform;
        }

        // initialize state so Update will apply only if needed
        lastAppliedFogState = !RenderSettings.fog;
    }

    void Update()
    {
        if (target == null) return;

        bool shouldEnableFog = target.position.y < thresholdY;

        // Only change RenderSettings if state actually changes
        if (shouldEnableFog != lastAppliedFogState)
        {
            RenderSettings.fog = shouldEnableFog;
            lastAppliedFogState = shouldEnableFog;
        }
    }

    void OnDisable()
    {
        // Restore the fog state that existed before this component ran
        RenderSettings.fog = originalFogEnabled;
    }
}