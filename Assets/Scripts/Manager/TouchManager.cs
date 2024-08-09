using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TouchManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float detectionRadius = 0.2f; // 터치 감지 반경 (픽셀 단위)
    [SerializeField] private int maxTouchCheckFrames = 3;
    [SerializeField] private TextMeshProUGUI logText;
    [SerializeField] private GameObject touchPointPrefab; // 터치 지점을 표시할 Quad 프리팹
    [SerializeField] private Vector3 touchPointScale = new Vector3(0.1f, 0.1f, 0.1f); // 터치 포인트의 크기 설정
    private List<BreakableObject> breakableObjects = new List<BreakableObject>();
    private GameManager gameManager;

    private bool isTouchDetected = false;
    private Vector3 lastInputPosition;
    private int currentTouchCheckFrame = 0;

    public void Init(GameManager _gameManager)
    {
        gameManager = _gameManager;
    }

    private void Update()
    {
        if (isTouchDetected)
        {
            currentTouchCheckFrame++;
            DetectObject(lastInputPosition);

            if (currentTouchCheckFrame >= maxTouchCheckFrames)
            {
                ResetTouchDetection();
            }
        }
        else
        {
            ScreenTouch();
        }
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
                    LogMessage($"Touch detected at position: {inputPosition}");
                    ShowTouchPoint(inputPosition);
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                inputDetected = true;
                inputPosition = Input.mousePosition;
                LogMessage($"Mouse click detected at position: {inputPosition}");
                ShowTouchPoint(inputPosition);
            }
        }

        if (inputDetected)
        {
            isTouchDetected = true;
            lastInputPosition = inputPosition;
            currentTouchCheckFrame = 0;
        }
    }

    private void DetectObject(Vector3 inputPosition)
    {
        Vector2 screenPoint = new Vector2(inputPosition.x, inputPosition.y);

        foreach (BreakableObject obj in breakableObjects)
        {
            // 오브젝트의 콜라이더를 가져옵니다.
            Collider collider = obj.GetComponent<Collider>();
            if (collider == null)
            {
                continue; // 콜라이더가 없는 경우 건너뜁니다.
            }

            // 콜라이더의 경계 상자를 스크린 좌표로 변환하여 검사
            Bounds bounds = collider.bounds;
            Vector2 minScreenPoint = mainCamera.WorldToScreenPoint(bounds.min);
            Vector2 maxScreenPoint = mainCamera.WorldToScreenPoint(bounds.max);

            // 스크린 좌표에서 터치 위치가 콜라이더 경계 내에 있는지 검사
            if (screenPoint.x >= minScreenPoint.x && screenPoint.x <= maxScreenPoint.x &&
                screenPoint.y >= minScreenPoint.y && screenPoint.y <= maxScreenPoint.y)
            {
                LogMessage($"Object {obj.name} touched within collider bounds.");
                obj.OnTouch();  // 오브젝트의 OnTouch 메서드를 호출
                ResetTouchDetection();
                break;
            }
        }
    }

    private void ShowTouchPoint(Vector3 screenPosition)
    {
        if (touchPointPrefab != null)
        {
            // 적절한 Z값 설정, 예를 들어 10f (카메라로부터 10 단위 떨어진 거리)
            float zDistanceFromCamera = 10f;
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, zDistanceFromCamera));

            // 터치 포인트 생성 (Quad 프리팹 사용)
            GameObject touchPoint = Instantiate(touchPointPrefab, worldPosition, Quaternion.identity);

            // 터치 포인트의 크기를 설정 (detectionRadius와는 연관 없음)
            touchPoint.transform.localScale = touchPointScale;

            // 터치 포인트를 일정 시간 후에 삭제
            Destroy(touchPoint, 1.0f); // 원하는 시간 후 삭제
        }
    }

    private void ResetTouchDetection()
    {
        isTouchDetected = false;
        lastInputPosition = Vector3.zero;
        currentTouchCheckFrame = 0;
    }

    private void LogMessage(string message)
    {
        Debug.Log(message);
        if (logText != null)
        {
            logText.text = message;
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
