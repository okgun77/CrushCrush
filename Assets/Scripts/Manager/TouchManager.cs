using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

public class TouchManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;                                         // 메인 카메라 참조
    [SerializeField] private float detectionRadius = 0.2f;                              // 터치 감지 반경 (픽셀 단위)
    [SerializeField] private int maxTouchCheckFrames = 3;                               // 터치 체크 최대 프레임
    [SerializeField] private TextMeshProUGUI logText;                                   // 로그 표시
    [SerializeField] private GameObject touchPointPrefab;                               // 터치 지점을 표시할 Quad 프리팹
    [SerializeField] private Vector3 touchPointScale = new Vector3(0.1f, 0.1f, 0.1f);   // 터치 포인트의 크기 설정

    [SerializeField] private float rayDistance = 100f;  // 레이캐스트가 나아갈 거리
    private List<BreakableObject> breakableObjects = new List<BreakableObject>();
    // private GameManager gameManager;

    private AudioManager audioManager;

    private bool isTouchDetected = false;
    private Vector3 lastInputPosition;
    private int currentTouchCheckFrame = 0;

    //public void Init(GameManager _gameManager)
    //{
    //    gameManager = _gameManager;
    //}

    private void Start()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
        if (audioManager == null)
        {
            Debug.LogError("AudioManager를 찾을 수 없습니다!");
            return;
        }

    }


    private void Update()
    {

        ScreenTouch();
    }
    //터치 혹은 클릭한 위치에 바로 ray를 쏘아서 가장 먼저 닿은 오브젝트를 확인,
    //해당 오브젝트가 breakableObject 컴포넌트를 가지고있다면 OnTouch 실행
    //보통 Raycast를 사용하면 즉시 부수는 경우 이 방법을 자주 사용합니다.
    //터치한 지점에 무언가(미사일 같은) 투사체가 발사시켜야하는 경우는 사용x
    //
    private void DoRay(Vector3 _position)
    {

        audioManager.PlaySFX("Wpn_Laser_01");

        LogMessage($"pointer detected at position: {_position}");
        // 카메라의 위치에서 레이 발사
        Ray ray = Camera.main.ScreenPointToRay(_position);
        RaycastHit hit;



        // 레이가 지정한 거리에 있는 오브젝트와 충돌했을 때
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            Debug.Log("부딪힌 물체 : " + hit.collider.gameObject.name);

            // 충돌한 오브젝트에 BreakableObject가 붙어있는지 확인
            BreakableObject breakableObject = hit.collider.gameObject.GetComponent<BreakableObject>();

            Debug.Log("breakableObject 가 있는지 체크" + (breakableObject != null));
            
            if (breakableObject != null)
            {
                // BreakableObject의 OnTouch() 함수 호출
                breakableObject.OnTouch();
            }

            // 레이가 부딪힌 위치까지의 선을 그린다
            Debug.DrawLine(ray.origin, hit.point, Color.red, 1f); // 1초 동안 빨간색 선 표시
        }
        else
        {
            // 레이가 부딪힌 것이 없을 경우 최대 거리까지 선을 그린다
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * rayDistance, Color.red, 1f); // 1초 동안 빨간색 선 표시
        }
        ShowTouchPoint(_position);
    }
    private void ScreenTouch()
    {
        //bool inputDetected = false; //중복으로 사용 할 필요없는 bool 변수
        //Vector3 inputPosition = Vector3.zero; //중복으로 사용할 필요없는 Vec3 변수

        //if문을 작성하면 매 터치마다 if문에서 확인을 하기때문에 #if를 사용하는 것이 좋음
        //#if는 빌드할때 #if에 참이 되는 내용만 컴파일하게됨
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            DoRay(Input.mousePosition);
        }
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                DoRay(touch.position);
            }
        }
#endif

        //기존 내용
        //if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        //{
        //    if (Input.touchCount > 0)
        //    {
        //        Touch touch = Input.GetTouch(0);
        //        if (touch.phase == TouchPhase.Began)
        //        {
        //            inputDetected = true;
        //            inputPosition = touch.position;
        //            LogMessage($"Touch detected at position: {inputPosition}");
        //            ShowTouchPoint(inputPosition);
        //        }
        //    }
        //}
        //else
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        inputDetected = true;
        //        inputPosition = Input.mousePosition;
        //        LogMessage($"Mouse click detected at position: {inputPosition}");
        //        ShowTouchPoint(inputPosition);
        //    }
        //}

        //최적화를 위해DoEvent 함수로 옮김
        //if (inputDetected)
        //{
        //    isTouchDetected = true;
        //    lastInputPosition = inputPosition;
        //    currentTouchCheckFrame = 0;
        //}
    }



    //원래 사용하시던 DetectObject() 함수의 경우 매 터치마다 foreach 문을 통해 breakableObjects List를 매번 검색합니다.
    //[문제점]
    //1. breakableObjects 의 갯수가 많아지거나 터치 횟수가 많아지면 퍼포먼스 저하의 우려가 있음
    //2. 게임개발이 고도화 되면 최적화 잡기가 힘들어짐
    //해결방안 : RayCast 방식으로 변경 -> DoRay() 함수 확인

    //private void DetectObject(Vector3 _inputPosition)
    //{
    //    Vector2 screenPoint = new Vector2(_inputPosition.x, _inputPosition.y);

    //    foreach (BreakableObject obj in breakableObjects)
    //    {
    //        // 오브젝트의 콜라이더를 가져옴
    //        Collider collider = obj.GetComponent<Collider>();
    //        if (collider == null)
    //        {
    //            continue; // 콜라이더가 없는 경우 건너뜀.
    //        }

    //        // 콜라이더의 경계 상자를 스크린 좌표로 변환하여 검사
    //        Bounds bounds = collider.bounds;
    //        Vector2 minScreenPoint = mainCamera.WorldToScreenPoint(bounds.min);
    //        Vector2 maxScreenPoint = mainCamera.WorldToScreenPoint(bounds.max);

    //        // 스크린 좌표에서 터치 위치가 콜라이더 경계 내에 있는지 검사
    //        if (screenPoint.x >= minScreenPoint.x && screenPoint.x <= maxScreenPoint.x &&
    //            screenPoint.y >= minScreenPoint.y && screenPoint.y <= maxScreenPoint.y)
    //        {
    //            LogMessage($"Object {obj.name} touched within collider bounds.");
    //            obj.OnTouch();  // 오브젝트의 OnTouch 메서드를 호출
    //            ResetTouchDetection();
    //            break;
    //        }
    //    }
    //}

    private void ShowTouchPoint(Vector3 _screenPosition)
    {
        if (touchPointPrefab != null)
        {
            // 적절한 Z값 설정, 예를 들어 10f (카메라로부터 10 단위 떨어진 거리)
            float zDistanceFromCamera = 10f;
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(_screenPosition.x, _screenPosition.y, zDistanceFromCamera));

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

    private void LogMessage(string _message)
    {
        Debug.Log(_message);
        if (logText != null)
        {
            logText.text = _message;
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
