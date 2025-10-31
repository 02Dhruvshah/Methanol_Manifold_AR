
using UnityEngine;

public class FlowController : MonoBehaviour
{
    [Header("Effects")]
    [SerializeField] private ParticleSystem[] particleSystems;
    [SerializeField] private AudioSource[] audioSources;
    [SerializeField] private GameObject cutawayPanel; 

    [Header("Boot Behavior")]
    [SerializeField] private bool startFlowOnBoot = false; 

    [Header("Debug")][SerializeField] private bool enableDebugLogs = false;

    public bool IsFlowing { get; private set; }

    void Awake()
    {
        
        if (particleSystems != null)
        {
            foreach (var ps in particleSystems)
            {
                if (ps == null) continue;
                var m = ps.main; m.playOnAwake = false; 
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