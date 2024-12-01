using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TouchManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;                                         // 메인 카메라 참조
    [SerializeField] private float detectionRadius = 0.2f;                              // 터치 감지 반경 (픽셀 단위)
    [SerializeField] private TextMeshProUGUI logText;                                   // 로그 표시
    [SerializeField] private GameObject touchPointPrefab;                               // 터치 지점을 표시할 Quad 프리팹
    [SerializeField] private Vector3 touchPointScale = new Vector3(0.1f, 0.1f, 0.1f);   // 터치 포인트의 크기 설정
    [SerializeField] private float rayDistance = 100f;  // 레이캐스트가 나아갈 거리
    

    private AudioManager audioManager;
    private List<BreakObject> breakableObjects = new List<BreakObject>();   // List of BreakObject instead of BreakableObject
    // private GameManager gameManager;

    private bool isPaused = false;      // 일시정지 상태
    private bool isTouchDetected = false;
    private Vector3 lastInputPosition;
    private int currentTouchCheckFrame = 0;

    private CombatManager combatManager;

    //public void Init(GameManager _gameManager)
    //{
    //    gameManager = _gameManager;
    //}

    private void Awake()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
        if (audioManager == null)
        {
            Debug.LogError("AudioManager를 찾을 수 없습니다!");
            return;
        }

        combatManager = FindAnyObjectByType<CombatManager>();
        if (combatManager == null)
        {
            Debug.LogWarning("CombatManager를 찾을 수 없습니다!");
        }
    }

    private void Update()
    {
        if (!isPaused)
        {
            ScreenTouch();
        }
    }

    // 터치 혹은 클릭한 위치에 바로 ray를 쏘아서 가장 먼저 닿은 오브젝트를 확인,
    // 해당 오브젝트가 ObjectProperties 컴포넌트를 가지고 있다면 IsBreakable 여부를 확인.
    // 파괴 가능하면 BreakObject 컴포넌트를 추가하여 OnTouch 실행
    public void SetPaused(bool _isPaused)
    {
        isPaused = _isPaused;
    }

    private void DoRay(Vector3 _position)
    {
        audioManager.PlaySFX("Wpn_Laser_01");
        LogMessage($"pointer detected at position: {_position}");
        
        Ray ray = Camera.main.ScreenPointToRay(_position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            Debug.Log("부딪힌 물체 : " + hit.collider.gameObject.name);

            // 충돌한 오브젝트에서 IDamageable과 ObjectProperties 모두 확인
            IDamageable target = hit.collider.gameObject.GetComponent<IDamageable>();
            ObjectProperties objectProperties = hit.collider.gameObject.GetComponent<ObjectProperties>();

            if (target != null && objectProperties != null && objectProperties.IsBreakable())
            {
                // CombatManager를 통해 공격 처리
                if (combatManager != null)
                {
                    combatManager.ProcessTouchAttack(target);
                }
                else
                {
                    Debug.LogWarning("CombatManager is not assigned!");
                    // CombatManager가 없는 경우 기존 로직으로 처리
                    var breakObject = hit.collider.gameObject.GetComponent<BreakObject>();
                    if (breakObject == null)
                    {
                        breakObject = hit.collider.gameObject.AddComponent<BreakObject>();
                        breakObject.Initialize(objectProperties.GetScoreType(), 
                                            objectProperties.GetFragmentLevel(), 
                                            2.0f);
                    }
                    breakObject.OnTouch();
                }
            }
            else
            {
                Debug.Log("Object is not breakable or doesn't have required components.");
            }

            Debug.DrawLine(ray.origin, hit.point, Color.red, 1f);
        }
        else
        {
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * rayDistance, Color.red, 1f);
        }
    }



    private void ScreenTouch()
    {
        // 터치 혹은 클릭한 위치에서 DoRay 함수 실행
        //if문을 작성하면 매 터치마다 if문에서 확인을 하기 때문에 #if를 사용하는 것이 좋음
        //#if는 빌드할 때 #if에 참이 되는 내용만 컴파일하게됨
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
    }

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

    // Register and unregister BreakObject instead of BreakableObject
    public void RegisterBreakObject(BreakObject _breakeObject)
    {
        if (!breakableObjects.Contains(_breakeObject))
        {
            breakableObjects.Add(_breakeObject);
        }
    }

    public void UnregisterBreakObject(BreakObject _breakeObject)
    {
        if (breakableObjects.Contains(_breakeObject))
        {
            breakableObjects.Remove(_breakeObject);
        }
    }
}
