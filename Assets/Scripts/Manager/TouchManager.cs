using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera; // 주 카메라
    [SerializeField] private float detectionRadius = 0.2f; // 충돌 검사 반경 (스크린 좌표 기준)
    private List<BreakableObject> breakableObjects = new List<BreakableObject>();
    private GameManager gameManager;

    public void Init(GameManager _gameManager)
    {
        gameManager = _gameManager;
    }

    private void Update()
    {
        ScreenTouch();
    }

    private void ScreenTouch()
    {
        bool inputDetected = false;
        Vector3 inputPosition = Vector3.zero;

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    inputDetected = true;
                    inputPosition = touch.position;
                    Debug.Log($"Touch detected at position: {inputPosition}");
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                inputDetected = true;
                inputPosition = Input.mousePosition;
                Debug.Log($"Mouse click detected at position: {inputPosition}");
            }
        }

        if (inputDetected)
        {
            DetectObject(inputPosition);
        }
    }

    private void DetectObject(Vector3 inputPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(inputPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            BreakableObject breakableObject = hit.collider.GetComponent<BreakableObject>();
            if (breakableObject != null)
            {
                Debug.Log($"Object {breakableObject.name} touched.");
                breakableObject.OnTouch();
            }
            else
            {
                // 2D 화면에서 거리 기반 검사를 위해 주위에 있는 오브젝트 검사
                Vector2 screenPoint = new Vector2(inputPosition.x, inputPosition.y);
                foreach (BreakableObject obj in breakableObjects)
                {
                    Vector2 objScreenPoint = mainCamera.WorldToScreenPoint(obj.transform.position);
                    float distance = Vector2.Distance(screenPoint, objScreenPoint);

                    if (distance <= detectionRadius)
                    {
                        Debug.Log($"Object {obj.name} touched within screen radius.");
                        obj.OnTouch();
                        break;
                    }
                }
            }
        }
    }

    public void RegisterBreakableObject(BreakableObject breakableObject)
    {
        if (!breakableObjects.Contains(breakableObject))
        {
            breakableObjects.Add(breakableObject);
        }
    }

    public void UnregisterBreakableObject(BreakableObject breakableObject)
    {
        if (breakableObjects.Contains(breakableObject))
        {
            breakableObjects.Remove(breakableObject);
        }
    }
}
