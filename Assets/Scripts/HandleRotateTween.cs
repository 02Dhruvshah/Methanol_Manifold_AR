
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class HandleRotateTween : MonoBehaviour
{
    public enum Axis { X, Y, Z }

    [Header("Rotation")]
    public Axis axis = Axis.Z;         
    public float lockedAngle = 90f;     
    public float rotateDuration = 0.4f;
    public Ease ease = Ease.OutCubic;

    [Header("Visuals (optional)")]
    public Renderer handleRenderer;
    public Color unlockedColor = Color.red;
    public Color lockedColor = Color.green;

    [Header("Events")]
    public UnityEvent onLocked;
    public UnityEvent onUnlocked;

    public bool IsLocked { get; private set; }
    public bool IsRotating { get; private set; }

    Quaternion _initialLocalRot;
    Tween _t;

    void Awake()
    {
        _initialLocalRot = transform.localRotation;
        SetColor(unlockedColor);
    }

    public void RotateToLock()
    {
        if (IsLocked || IsRotating) return;
        RotateTo(true);
    }

    public void RotateToUnlock()
    {
        if (!IsLocked || IsRotating) return;
        RotateTo(false);
    }

    public void LockImmediate()
    {
        KillTween();
        ApplyAngleImmediate(lockedAngle);
        SetState(locked: true, invokeEvent: false);
    }

    public void UnlockImmediate()
    {
        KillTween();
        ApplyAngleImmediate(0f);
        SetState(locked: false, invokeEvent: false);
    }

    void RotateTo(bool toLock)
    {
        KillTween();
        IsRotating = true;

        float targetAngle = toLock ? lockedAngle : 0f;
        Quaternion target = _initialLocalRot * Quaternion.AngleAxis(targetAngle, AxisVector());

        _t = transform.DOLocalRotateQuaternion(target, rotateDuration)
            .SetEase(ease)
            .OnComplete(() =>
            {
                IsRotating = false;
                SetState(locked: toLock, invokeEvent: true);
            });
    }

    void SetState(bool locked, bool invokeEvent)
    {
        IsLocked = locked;
        SetColor(locked ? lockedColor : unlockedColor);
        if (invokeEvent)
        {
            if (locked) onLocked?.Invoke();
            else onUnlocked?.Invoke();
        }
    }

    void ApplyAngleImmediate(float angle)
    {
        transform.localRotation = _initialLocalRot * Quaternion.AngleAxis(angle, AxisVector());
    }

    Vector3 AxisVector() => axis == Axis.X ? Vector3.right : axis == Axis.Y ? Vector3.up : Vector3.forward;

    void SetColor(Color c)
    {
        if (handleRenderer) handleRenderer.material.color = c;
    }

    void KillTween()
    {
        if (_t != null && _t.IsActive())
        {
            _t.Kill(false);
            _t = null;
        }
        IsRotating = false;
    }
}
