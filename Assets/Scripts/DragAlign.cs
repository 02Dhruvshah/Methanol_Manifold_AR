using UnityEngine;
using System.Collections;

public class DragAlign : MonoBehaviour
{
    [Header("Targets")]
    [Tooltip("Male connector transform you want to snap to")]
    public Transform target;

    [Tooltip("Manager on the MALE (target) side that owns the engage logic")]
    public SnapAndEngage snapManager;

    [Header("Tuning")]
    public float snapDistance = 0.05f;    
    public float moveSpeed = 6f;           
    public float snapEaseSpeed = 6f;       

    private bool isDragging = false;
    private bool isSnapping = false;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        if (!mainCam) Debug.LogError("Main Camera not found! Tag your AR camera as MainCamera.");
        if (!target) Debug.LogError("[DragAlign] Target not assigned.");
        if (!snapManager) Debug.LogWarning("[DragAlign] snapManager not set. Assign the SnapAndEngage on the MALE.");
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0)) TryStartDrag(Input.mousePosition);
        else if (Input.GetMouseButtonUp(0)) EndDrag();
        else if (isDragging && !isSnapping) DragMove(Input.mousePosition);
#else
        if (Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began) TryStartDrag(t.position);
            else if ((t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary) && isDragging && !isSnapping)
                DragMove(t.position);
            else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
                EndDrag();
        }
#endif
    }

    void TryStartDrag(Vector2 screenPos)
    {
        if (isSnapping) return;

        Ray ray = mainCam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                isDragging = true;
                // Debug.Log("Dragging " + name);
            }
        }
    }

    void DragMove(Vector2 screenPos)
    {
        Ray ray = mainCam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 newPos = hit.point;
            transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * moveSpeed);

            
            if (target && Vector3.Distance(transform.position, target.position) <= snapDistance)
                StartCoroutine(SnapToTarget());
        }
    }

    IEnumerator SnapToTarget()
    {
        isSnapping = true;
        isDragging = false;

        Vector3 start = transform.position;
        Vector3 end = target.position;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * snapEaseSpeed;
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        transform.position = end;
       
        if (snapManager) snapManager.OnAligned();
        isSnapping = false;
    }

    void EndDrag()
    {
        isDragging = false;
    }
}
