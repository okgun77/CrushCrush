using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera; // 주 카메라
    [SerializeField] private float detectionRadius = 0.2f; // 충돌 검사 반경 (월드 좌표 기준)
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

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            BreakableObject breakableObject = hit.collider.GetComponent<BreakableObject>();
            if (breakableObject != null)
            {
                Debug.Log($"Object {breakableObject.name} touched.");
                breakableObject.OnTouch();
            }
            else
            {
                // 구체 반경 내의 다른 객체 감지
                Collider[] colliders = Physics.OverlapSphere(hit.point, detectionRadius);
                foreach (Collider collider in colliders)
                {
                    BreakableObject nearbyObject = collider.GetComponent<BreakableObject>();
                    if (nearbyObject != null)
                    {
                        Debug.Log($"Nearby object {nearbyObject.name} touched within radius.");
                        nearbyObject.OnTouch();
                        break;
                    }
                }
            }
        }
    }

    public void RegisterBreakableObject(BreakableObject _breakableObject)
    {
        if (!breakableObjects.Contains(_breakableObject))
        {
            breakableObjects.Add(_breakableObject);
        }
    }

    public void UnregisterBreakableObject(BreakableObject _breakableObject)
    {
        if (breakableObjects.Contains(_breakableObject))
        {
            breakableObjects.Remove(_breakableObject);
        }
    }
}
