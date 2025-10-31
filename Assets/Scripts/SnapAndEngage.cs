
using UnityEngine;
using UnityEngine.Events;

public class SnapAndEngage : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FlowController flowController;    
    [SerializeField] private HandleRotateTween handleTween;   

    [Header("Behavior Flags")]
    [Tooltip("Turn flow OFF when handle locks.")]
    [SerializeField] private bool stopFlowOnLock = true;

    [Tooltip("Turn flow OFF when alignment event fires.")]
    [SerializeField] private bool stopFlowOnAlign = false;

    [Tooltip("If true, unlocking also clears 'aligned'.")]
    [SerializeField] private bool unlockClearsAlignment = true;

    [Tooltip("Auto-rotate handle to lock when aligned becomes true.")]
    [SerializeField] private bool autoLockOnAlign = false;

    [Tooltip("If true, start flow when BOTH aligned & locked are true (ignored if stopFlowOnLock = true).")]
    [SerializeField] private bool startFlowWhenAlignedAndLocked = true;

    [Header("State (read-only)")]
    [SerializeField] private bool aligned = false;

    [Header("Events (optional passthrough)")]
    public UnityEvent onAligned;
    public UnityEvent onLocked;
    public UnityEvent onUnlocked;

    [Header("Debug")][SerializeField] private bool enableDebugLogs = false;

    void Awake()
    {
       
        if (!flowController) flowController = FindObjectOfType<FlowController>();
        if (!handleTween) handleTween = GetComponentInChildren<HandleRotateTween>();

        
        if (handleTween)
        {
            handleTween.onLocked.AddListener(HandleLocked);
            handleTween.onUnlocked.AddListener(HandleUnlocked);
        }
    }

  
    public void OnAligned()
    {
        aligned = true;
        onAligned?.Invoke();
        Log("Aligned");

        if (stopFlowOnAlign && flowController && flowController.IsFlowing)
        {
            flowController.StopFlow();
            Log("Stopped flow on align (flag)");
        }

        if (autoLockOnAlign && handleTween && !handleTween.IsLocked)
        {
            handleTween.RotateToLock();  
            return;
        }

        TryStartFlowIfEligible();
    }

    public void Lock() { if (handleTween) handleTween.RotateToLock(); }
    public void Unlock() { if (handleTween) handleTween.RotateToUnlock(); }
    public void ToggleLock()
    {
        if (!handleTween) return;
        if (handleTween.IsLocked) handleTween.RotateToUnlock();
        else handleTween.RotateToLock();
    }

    public void ResetEngagement()
    {
        if (flowController && flowController.IsFlowing) flowController.StopFlow();
        aligned = false;
        if (handleTween) handleTween.UnlockImmediate();
        Log("Reset: aligned=false, unlocked immediate");
    }

   
    void HandleLocked()
    {
        onLocked?.Invoke();
        Log("Locked (from tween)");

        if (stopFlowOnLock && flowController && flowController.IsFlowing)
        {
            flowController.StopFlow();
            Log("Stopped flow on lock (flag)");
            return;
        }

        TryStartFlowIfEligible();
    }

    void HandleUnlocked()
    {
        onUnlocked?.Invoke();
        Log("Unlocked (from tween)");

        if (flowController && flowController.IsFlowing)
        {
            flowController.StopFlow();
            Log("Stopped flow on unlock");
        }
        if (unlockClearsAlignment)
        {
            aligned = false;
            Log("Cleared alignment on unlock (flag)");
        }
    }

  
    void TryStartFlowIfEligible()
    {
        if (!flowController || flowController.IsFlowing) return;
        if (stopFlowOnLock) return; 

        if (startFlowWhenAlignedAndLocked && aligned && handleTween && handleTween.IsLocked)
        {
            flowController.StartFlow();
            Log("Flow START (aligned && locked && flag)");
        }
    }

    void Log(string s) { if (enableDebugLogs) Debug.Log($"[SnapAndEngage] {s}", this); }
}
