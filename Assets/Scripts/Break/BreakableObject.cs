using UnityEngine;
using RayFire;

public class BreakableObject : MonoBehaviour
{
    [SerializeField] private float additionalSpeedMultiplier = 2.0f;
    [SerializeField] private ScoreType scoreType;
    [SerializeField] private int fragmentLevel = 0; // 파편 레벨 (0은 원래 오브젝트)

    private RayfireRigid rayfireRigid;
    private RayfireBomb rayfireBomb;
    private RayfireSound rayfireSound;
    private MoveToTargetPoint moveScript;
    private ScoreManager scoreManager;
    private TouchManager touchManager;
    private WarningManager warningManager;
    private HPManager hpManager;

    private bool isWarningActive = false; // 경고 상태를 추적하기 위한 플래그

    private void Start()
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
        rayfireSound = GetComponent<RayfireSound>();
        if (rayfireSound == null)
        {
            Debug.LogWarning("RayfireSound 컴포넌트가 없습니다!");
        }

        // MoveToTargetPoint 컴포넌트 가져오기
        moveScript = GetComponent<MoveToTargetPoint>();

        // ScoreManager 컴포넌트 가져오기
        scoreManager = Object.FindFirstObjectByType<ScoreManager>();
        if (scoreManager == null)
        {
            Debug.LogError("ScoreManager를 찾을 수 없습니다!");
            return;
        }

        // TouchManager 컴포넌트 가져오기 및 등록
        touchManager = FindFirstObjectByType<TouchManager>();
        if (touchManager != null)
        {
            touchManager.RegisterBreakableObject(this);
        }

        // WarningManager 컴포넌트 가져오기
        warningManager = FindFirstObjectByType<WarningManager>();
        if (warningManager == null)
        {
            Debug.LogError("WarningManager를 찾을 수 없습니다!");
            return;
        }

        // HPManager 컴포넌트 가져오기
        hpManager = FindFirstObjectByType<HPManager>();
        if (hpManager == null)
        {
            Debug.LogError("HPManager를 찾을 수 없습니다!");
            return;
        }
    }

    private void OnDestroy()
    {
        // TouchManager에서 등록 해제
        if (touchManager != null)
        {
            touchManager.UnregisterBreakableObject(this);
        }

        // 오브젝트가 파괴될 때 경고 효과 해제
        if (isWarningActive && warningManager != null)
        {
            warningManager.RemoveWarningEffect(this);
        }
    }

    public void OnTouch()
    {
        BreakObject();
    }

    private void BreakObject()
    {
        if (rayfireRigid != null)
        {
            // 현재 속도 가져오기
            Vector3 currentVelocity = Vector3.zero;
            if (moveScript != null)
            {
                currentVelocity = moveScript.GetCurrentVelocity();
                Destroy(moveScript); // 이동 스크립트 제거
            }

            // 오브젝트 파괴 및 파편 처리
            rayfireRigid.Demolish();
            foreach (RayfireRigid fragment in rayfireRigid.fragments)
            {
                SetFragmentProperties(fragment, currentVelocity);
                InitFragment(fragment);
                // SelfDestruct 기능 추가
                var destroyFade = fragment.gameObject.AddComponent<DestroyFade>();
                destroyFade.StartDestruction();
            }
        }

        if (rayfireBomb != null)
        {
            rayfireBomb.Explode(0f); // 지연 없이 즉시 폭발
        }

        // 점수 추가
        AddScore();

        // 파편 레벨이 0일 때만 연속 파괴로 계산
        if (fragmentLevel == 0)
        {
            hpManager.IncreaseConsecutiveDestroys();
        }
    }

    private void AddScore()
    {
        Camera mainCamera = Camera.main;
        float distanceToCamera = Vector3.Distance(transform.position, mainCamera.transform.position);
        int calculatedScore = scoreManager.CalculateScore(scoreType, fragmentLevel, distanceToCamera);
        scoreManager.AddScore(calculatedScore);
    }

    private void InitFragment(RayfireRigid _fragment)
    {
        if (fragmentLevel < scoreManager.GetMaxFragmentLevel())
        {
            if (_fragment.gameObject.GetComponent<BreakableObject>() == null)
            {
                var fragmentScript = _fragment.gameObject.AddComponent<BreakableObject>();
                fragmentScript.scoreType = this.scoreType;
                fragmentScript.fragmentLevel = this.fragmentLevel + 1; // 파편 레벨 증가
            }
        }

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

        RayfireRigid fragmentRigid = _fragment.GetComponent<RayfireRigid>();
        if (fragmentRigid == null)
        {
            fragmentRigid = _fragment.gameObject.AddComponent<RayfireRigid>();
            fragmentRigid.demolitionType = DemolitionType.Runtime;
        }

        RayfireBomb fragmentBomb = _fragment.GetComponent<RayfireBomb>();
        if (fragmentBomb == null && rayfireBomb != null)
        {
            fragmentBomb = _fragment.gameObject.AddComponent<RayfireBomb>();
            fragmentBomb.range = rayfireBomb.range;
            fragmentBomb.strength = rayfireBomb.strength;
            fragmentBomb.variation = rayfireBomb.variation;
            fragmentBomb.chaos = rayfireBomb.chaos;
        }

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
        // 여기에 해당 조건을 추가하세요.
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

    private void SetFragmentProperties(RayfireRigid _fragment, Vector3 _initialVelocity)
    {
        Rigidbody rb = _fragment.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = _initialVelocity * additionalSpeedMultiplier;
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

    // WarningManager에 의해 경고 상태가 업데이트됨
    public void SetWarningState(bool _state)
    {
        isWarningActive = _state;
    }
}
