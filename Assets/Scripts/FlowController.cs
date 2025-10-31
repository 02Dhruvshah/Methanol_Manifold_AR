// ================================
// File: FlowController.cs
// Restored, minimal, and stable. Drives particles, audio, and UI cutaway panel.
// ================================
using UnityEngine;

public class FlowController : MonoBehaviour
{
    [Header("Effects")]
    [SerializeField] private ParticleSystem[] particleSystems;
    [SerializeField] private AudioSource[] audioSources;
    [SerializeField] private GameObject cutawayPanel; // optional UI / mesh to show while flowing

    [Header("Boot Behavior")]
    [SerializeField] private bool startFlowOnBoot = false; // set true if you want auto start

    [Header("Debug")][SerializeField] private bool enableDebugLogs = false;

    public bool IsFlowing { get; private set; }

    void Awake()
    {
        // Ensure effects are not auto-played by their own settings
        if (particleSystems != null)
        {
            foreach (var ps in particleSystems)
            {
                if (ps == null) continue;
                var m = ps.main; m.playOnAwake = false; // prevent auto play
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
        if (audioSources != null)
        {
            foreach (var au in audioSources)
            {
                if (au == null) continue;
                au.playOnAwake = false;
                au.Stop();
            }
        }
        if (cutawayPanel) cutawayPanel.SetActive(false);
        IsFlowing = false;
    }

    void Start()
    {
        if (startFlowOnBoot) StartFlow();
    }

    public void StartFlow()
    {
        if (IsFlowing) return;
        IsFlowing = true;
        if (particleSystems != null) foreach (var ps in particleSystems) if (ps) ps.Play();
        if (audioSources != null) foreach (var au in audioSources) if (au) au.Play();
        if (cutawayPanel) cutawayPanel.SetActive(true);
        Log("Flow START");
    }

    public void StopFlow()
    {
        if (!IsFlowing) return;
        IsFlowing = false;
        if (particleSystems != null) foreach (var ps in particleSystems) if (ps) ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        if (audioSources != null) foreach (var au in audioSources) if (au) au.Stop();
        if (cutawayPanel) cutawayPanel.SetActive(false);
        Log("Flow STOP");
    }

    private void Log(string s)
    {
        if (enableDebugLogs) Debug.Log("[FlowController] " + s, this);
    }
}