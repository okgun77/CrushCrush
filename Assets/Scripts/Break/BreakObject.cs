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

    }

    private void OnDestroy()
    {
        // TouchManager에서 등록 해제
        if (touchManager != null)
        {
            touchManager.UnregisterBreakObject(this);
        }

        // 오브젝트가 파괴될 때 경고 효과 해제
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

                    // 파편이 플레이어 쪽으로 날아가도록 설정
                    fragment.gameObject.AddComponent<FragmentMovement>().MoveToTarget(targetPoint);
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
        // 파편에 ObjectProperties가 없으면 추가
        ObjectProperties fragmentProperties = _fragment.gameObject.GetComponent<ObjectProperties>();
        if (fragmentProperties == null)
        {
            fragmentProperties = _fragment.gameObject.AddComponent<ObjectProperties>();
        }

        // 파편 오브젝트의 레벨과 속성을 상속
        fragmentProperties.SetFragmentLevel(objectProperties.GetFragmentLevel() + 1);  // 파편 레벨 증가
        fragmentProperties.SetScoreType(objectProperties.GetScoreType());  // 점수 타입 상속
        fragmentProperties.SetBreakable(true);  // 파편도 파괴 가능하도록 설정

        // 파편에 BreakObject가 없으면 추가
        if (_fragment.gameObject.GetComponent<BreakObject>() == null)
        {
            var fragmentScript = _fragment.gameObject.AddComponent<BreakObject>();

            // fragmentScript 초기화 (초기화 함수 호출)
            fragmentScript.Initialize(
                objectProperties.GetScoreType(),
                objectProperties.GetFragmentLevel() + 1, // 파편 레벨 증가
                2.0f // 추가 속도 배율 (예시 값)
            );
        }

        // 파편의 물리적 속도와 알파 값을 설정 (SetFragmentProperties 호출)
        SetFragmentProperties(_fragment, Vector3.zero, Vector3.zero);

        // Rigidbody와 Collider 설정
        Rigidbody rb = _fragment.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = _fragment.gameObject.AddComponent<Rigidbody>();
        }

        Collider col = _fragment.GetComponent<Collider>();
        if (col == null)
        {
            try
            {
                // BoxCollider 또는 SphereCollider 사용
                if (ShouldUseBoxCollider(_fragment))
                {
                    col = _fragment.gameObject.AddComponent<BoxCollider>();
                }
                else
                {
                    col = _fragment.gameObject.AddComponent<SphereCollider>();
                }

                // 콜라이더 크기를 파편 크기에 맞게 조정
                AdjustColliderSize(col, _fragment);
            }
            catch
            {
                Debug.LogWarning("Failed to create BoxCollider or SphereCollider.");
            }
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

        // 파편이 사라지는 DestroyFade 컴포넌트 추가
        var destroyFade = _fragment.gameObject.AddComponent<DestroyFade>();
        destroyFade.StartDestruction(); // 서서히 사라지기 시작
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
        Rigidbody rb = _fragment.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // 파편에 기존의 물리적 속도 전달
            // rb.linearVelocity = _initialVelocity * 2.0f;  // 추가 속도 배율 적용
            // rb.angularVelocity = _initialAngularVelocity; // 회전 속도도 유지

            // 물리 속도가 없을 경우 임의로 추가적인 힘을 가할 수도 있습니다.
            if (rb.linearVelocity.magnitude < 0.1f)
            {
                rb.AddForce(Random.onUnitSphere * 2.0f, ForceMode.Impulse);  // 랜덤한 방향으로 힘을 가함
            }
        }

        // 파편의 알파 값 적용
        Renderer renderer = _fragment.GetComponent<Renderer>();
        if (renderer != null)
        {
            foreach (var mat in renderer.materials)
            {
                if (mat.HasProperty("_Color"))
                {
                    mat.SetFloat("_Surface", 1); // Transparent
                    mat.SetFloat("_Blend", 1); // Alpha Blend
                    Color color = mat.color;
                    color.a = 0.3f; // 알파 값 조정
                    mat.color = color;
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.EnableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                }
                else if (mat.HasProperty("_Alpha"))
                {
                    mat.SetFloat("_Alpha", 0.3f); // 알파 값 조정
                }
            }
        }
    }


    // 파편이 플레이어 쪽으로 움직이는 코루틴
    private IEnumerator MoveFragmentToTarget(GameObject _fragment)
    {
        Rigidbody rb = _fragment.GetComponent<Rigidbody>();

        while (rb != null && Vector3.Distance(_fragment.transform.position, targetPoint.position) > 0.1f)
        {
            Vector3 direction = (targetPoint.position - _fragment.transform.position).normalized;
            rb.linearVelocity = direction * 8.0f;  // 속도 설정
            yield return null;  // 한 프레임 대기
        }

        // 도착 후 파편을 사라지게 처리
        // Destroy(_fragment);

        // 타겟에 도달한 후 서서히 사라지는 효과 적용
        DestroyFade destroyFade = _fragment.GetComponent<DestroyFade>();
        if (destroyFade == null)
        {
            destroyFade = _fragment.AddComponent<DestroyFade>();  // 없으면 추가
        }

        // 서서히 사라지는 효과 시작
        destroyFade.StartDestruction();
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
