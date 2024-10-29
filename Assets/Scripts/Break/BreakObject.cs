using UnityEngine;
using RayFire;
using System.Collections;

public class BreakObject : MonoBehaviour
{
    private RayfireRigid rayfireRigid;
    private RayfireBomb rayfireBomb;
    private RayfireSound rayfireSound;
    private MoveToTargetPoint moveScript;
    private ScoreManager scoreManager;
    private TouchManager touchManager;
    private WarningManager warningManager;
    private HPManager hpManager;
    private AudioManager audioManager;
    private ObjectProperties objectProperties;  // ObjectProperties 참조
    private Transform targetPoint;
    private UIManager uiManager;

    private bool isWarningActive = false; // 경고 상태를 추적하기 위한 플래그

    private void Awake()
    {
        // RayfireRigid 컴포넌트 가져오기
        rayfireRigid = GetComponent<RayfireRigid>();
        if (rayfireRigid == null)
        {
            Debug.LogError("RayfireRigid 컴포넌트가 없습니다!");
            return;
        }

        // RayfireBomb 컴포넌트 가져오기
        rayfireBomb = GetComponent<RayfireBomb>();
        if (rayfireBomb == null)
        {
            Debug.LogWarning("RayfireBomb 컴포넌트가 없습니다!");
        }

        // RayfireSound 컴포넌트 가져오기
        //rayfireSound = GetComponent<RayfireSound>();
        //if (rayfireSound == null)
        //{
        //    Debug.LogWarning("RayfireSound 컴포넌트가 없습니다!");
        //}

        // MoveToTargetPoint 컴포넌트 가져오기
        moveScript = GetComponent<MoveToTargetPoint>();

        // ScoreManager 컴포넌트 가져오기
        scoreManager = FindAnyObjectByType<ScoreManager>();
        if (scoreManager == null)
        {
            Debug.LogError("ScoreManager를 찾을 수 없습니다!");
            return;
        }

        // TouchManager 컴포넌트 가져오기 및 등록
        touchManager = FindAnyObjectByType<TouchManager>();
        if (touchManager != null)
        {
            touchManager.RegisterBreakObject(this);
        }

        // WarningManager 컴포넌트 가져오기
        warningManager = FindAnyObjectByType<WarningManager>();
        if (warningManager == null)
        {
            Debug.LogError("WarningManager를 찾을 수 없습니다!");
            return;
        }

        // HPManager 컴포넌트 가져오기
        hpManager = FindAnyObjectByType<HPManager>();
        if (hpManager == null)
        {
            Debug.LogError("HPManager를 찾을 수 없습니다!");
            return;
        }

        // AudioManager 컴포넌트 가져오기
        audioManager = FindAnyObjectByType<AudioManager>();
        if (audioManager == null)
        {
            Debug.LogError("AudioManager를 찾을 수 없습니다!");
            return;
        }

        // ObjectProperties 가져오기
        objectProperties = GetComponent<ObjectProperties>();
        if (objectProperties == null)
        {
            Debug.LogError("ObjectProperties 컴포넌트가 없습니다!");
            return;
        }

        // 플레이어(또는 TargetPoint) 위치를 가져오기
        targetPoint = GameObject.Find("TargetPoint").transform; // TargetPoint로 설정된 빈 오브젝트를 찾아서 참조
        if (targetPoint == null)
        {
            Debug.LogError("TargetPoint가 없습니다.");
            return;
        }

        // UIManager 참조 가져오기
        uiManager = FindAnyObjectByType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("UIManager를 찾을 수 없습니다!");
            return;
        }
    }

    private void OnDestroy()
    {
        // TouchManager에서 등록 해제
        if (touchManager != null)
        {
            touchManager.UnregisterBreakObject(this);
        }

        // 오브젝트가 파괴될 때 경고 효과 해
        if (isWarningActive && warningManager != null)
        {
            warningManager.RemoveWarningEffect(this);
        }
    }

    public void OnTouch()
    {
        Break();
    }

    // 파괴 처리를 수행하는 함수
    public void HandleObjectDestruction()
    {
        if (objectProperties.GetHealth() <= 0)
        {
            Break();
        }
    }

    private void Break()
    {
        if (rayfireRigid != null)
        {
            // 현재 속도와 회전 속도 가져오기
            Vector3 currentVelocity = Vector3.zero;
            Vector3 currentAngularVelocity = Vector3.zero;

            if (moveScript != null)
            {
                currentVelocity = moveScript.GetCurrentVelocity();
                Destroy(moveScript); // 이동 스크립트 제거
            }

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                currentVelocity = rb.linearVelocity;  // 현재 속도 가져오기
                currentAngularVelocity = rb.angularVelocity;  // 현재 회전 속도 가져오기
            }

            // 오브젝트 파괴 및 파편 처리
            rayfireRigid.Demolish();

            // 파괴 오디오 플레이
            audioManager.PlaySFX("BreakingBones02-Mono");

            // Demolish() 호출 후 파편이 생성되므로 이 시점에서 파편을 초기화합니다.
            foreach (RayfireRigid fragment in rayfireRigid.fragments)
            {
                if (fragment != null && fragment.gameObject.activeInHierarchy) // 비활성화되지 않은 파편만 처리
                {
                    // 파편에 필요한 컴포넌트 추가 및 초기화
                    InitFragment(fragment);

                    // UI 타겟으로 이동하도록 설정
                    var fragmentMovement = fragment.gameObject.AddComponent<FragmentMovement>();
                    fragmentMovement.SetUITarget(uiManager.GetFragmentTargetIcon());
                }
            }
        }

        if (rayfireBomb != null)
        {
            rayfireBomb.Explode(0f); // 지연 없이 즉시 폭발
        }

        // 점수 추가
        AddScore();

        // 파편 레벨이 0일 때만 연속 파괴로 계산
        if (objectProperties.GetFragmentLevel() == 0)
        {
            hpManager.IncreaseConsecutiveDestroys();
        }
    }





    private void AddScore()
    {
        if (scoreManager == null)
        {
            Debug.LogError("ScoreManager를 찾을 수 없습니다! AddScore 작업을 중단합니다.");
            // return; // scoreManager가 null일 경우 작업을 중단
        }

        Camera mainCamera = Camera.main;
        float distanceToCamera = Vector3.Distance(transform.position, mainCamera.transform.position);
        int calculatedScore = scoreManager.CalculateScore(objectProperties.GetScoreType(), objectProperties.GetFragmentLevel(), distanceToCamera); // fragmentLevel과 scoreType을 ObjectProperties에서 가져옴
        scoreManager.AddScore(calculatedScore);
    }

    private void InitFragment(RayfireRigid _fragment)
    {
        // 모든 Collider 컴포넌트 제거
        Collider[] colliders = _fragment.gameObject.GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            Destroy(collider);
        }

        // 파편에 ObjectProperties가 없으면 추가
        ObjectProperties fragmentProperties = _fragment.gameObject.GetComponent<ObjectProperties>();
        if (fragmentProperties == null)
        {
            fragmentProperties = _fragment.gameObject.AddComponent<ObjectProperties>();
        }

        // 파편 오브젝트의 레벨과 속성을 상속
        fragmentProperties.SetFragmentLevel(objectProperties.GetFragmentLevel() + 1);
        fragmentProperties.SetScoreType(objectProperties.GetScoreType());
        fragmentProperties.SetBreakable(true);

        // 파편에 BreakObject가 없으면 추가
        if (_fragment.gameObject.GetComponent<BreakObject>() == null)
        {
            var fragmentScript = _fragment.gameObject.AddComponent<BreakObject>();
            fragmentScript.Initialize(
                objectProperties.GetScoreType(),
                objectProperties.GetFragmentLevel() + 1,
                2.0f
            );
        }

        // 파편의 물리적 속도와 알파 값을 설정
        SetFragmentProperties(_fragment, Vector3.zero, Vector3.zero);

        // Rigidbody 설정
        Rigidbody rb = _fragment.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = _fragment.gameObject.AddComponent<Rigidbody>();
        }

        // RayfireRigid 설정
        RayfireRigid fragmentRigid = _fragment.GetComponent<RayfireRigid>();
        if (fragmentRigid == null)
        {
            fragmentRigid = _fragment.gameObject.AddComponent<RayfireRigid>();
            fragmentRigid.demolitionType = DemolitionType.Runtime;
        }

        // RayfireBomb 설정
        RayfireBomb fragmentBomb = _fragment.GetComponent<RayfireBomb>();
        if (fragmentBomb == null && rayfireBomb != null)
        {
            fragmentBomb = _fragment.gameObject.AddComponent<RayfireBomb>();
            fragmentBomb.range = rayfireBomb.range;
            fragmentBomb.strength = rayfireBomb.strength;
            fragmentBomb.variation = rayfireBomb.variation;
            fragmentBomb.chaos = rayfireBomb.chaos;
        }

        // RayfireSound 설정
        RayfireSound fragmentSound = _fragment.GetComponent<RayfireSound>();
        if (fragmentSound == null && rayfireSound != null)
        {
            fragmentSound = _fragment.gameObject.AddComponent<RayfireSound>();
            fragmentSound.enabled = true;
            fragmentSound.demolition = rayfireSound.demolition;
            fragmentSound.collision = rayfireSound.collision;
        }
    }




    private bool ShouldUseBoxCollider(RayfireRigid _fragment)
    {
        // BoxCollider를 사용할 조건을 정의합니다.
        // 예: 특정 메쉬 이름 또는 태그를 기준으로 결정
        return true; // 기본적으로 BoxCollider를 사용하도록 설정
    }

    private void AdjustColliderSize(Collider _collider, RayfireRigid _fragment)
    {
        if (_collider is BoxCollider boxCollider)
        {
            boxCollider.size = _fragment.meshRenderer.bounds.size;
            boxCollider.center = _fragment.meshRenderer.bounds.center - _fragment.transform.position;
        }
        else if (_collider is SphereCollider sphereCollider)
        {
            // SphereCollider의 반지름을 파편의 경계 박스를 기준으로 설정 
            Vector3 boundsSize = _fragment.meshRenderer.bounds.size;
            sphereCollider.radius = Mathf.Max(boundsSize.x, boundsSize.y, boundsSize.z) / 2.0f;
            sphereCollider.center = _fragment.meshRenderer.bounds.center - _fragment.transform.position;
        }
    }

    private void SetFragmentProperties(RayfireRigid _fragment, Vector3 _initialVelocity, Vector3 _initialAngularVelocity)
    {
        Renderer renderer = _fragment.GetComponent<Renderer>();
        if (renderer != null)
        {
            foreach (var mat in renderer.materials)
            {
                if (mat.HasProperty("_Color"))
                {
                    // 투명도 설정만 초기화
                    mat.SetFloat("_Surface", 1);
                    mat.SetFloat("_Blend", 1);
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.EnableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                }
            }
        }
    }


    // WarningManager에 의해 경고 상태가 업데이트됨
    public void SetWarningState(bool _state)
    {
        isWarningActive = _state;
    }

    // 추가: 초기화 메서드 (파편을 동적으로 초기화할 때 사용)
    public void Initialize(ScoreType _scoreType, int _fragmentLevel, float _speedMultiplier)
    {
        // 필요한 데이터를 초기화합니다.
        objectProperties.SetScoreType(_scoreType);
        objectProperties.SetFragmentLevel(_fragmentLevel);
        // 속도 배율 등 추가 속성을 초기화할 수 있습니다.
    }

}
