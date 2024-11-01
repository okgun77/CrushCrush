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

    // 1. 캐시 추가
    private static readonly int BaseColorProperty = Shader.PropertyToID("_BaseColor");
    private static readonly int SurfaceProperty = Shader.PropertyToID("_Surface");
    private static readonly int BlendProperty = Shader.PropertyToID("_Blend");
    private static readonly int SrcBlendProperty = Shader.PropertyToID("_SrcBlend");
    private static readonly int DstBlendProperty = Shader.PropertyToID("_DstBlend");
    private static readonly int ZWriteProperty = Shader.PropertyToID("_ZWrite");

    // 2. 파편 설정 구조체 추가
    private struct FragmentSettings
    {
        public float alpha;
        public int renderQueue;
        public bool useGravity;
        public CollisionDetectionMode collisionMode;
        public RigidbodyInterpolation interpolation;
    }

    private static readonly FragmentSettings DefaultFragmentSettings = new FragmentSettings
    {
        alpha = 0.3f,
        renderQueue = 3000,
        useGravity = false,
        collisionMode = CollisionDetectionMode.ContinuousSpeculative,
        interpolation = RigidbodyInterpolation.Interpolate
    };

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
            Debug.LogError("AudioManager를 찾을 수 없습��다!");
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
            Vector3 currentVelocity = Vector3.zero;
            Vector3 currentAngularVelocity = Vector3.zero;

            if (moveScript != null)
            {
                currentVelocity = moveScript.GetCurrentVelocity();
                Destroy(moveScript);
            }

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                currentVelocity = rb.linearVelocity;
                currentAngularVelocity = rb.angularVelocity;
            }

            rayfireRigid.Demolish();
            audioManager.PlaySFX("BreakingBones02-Mono");

            foreach (RayfireRigid fragment in rayfireRigid.fragments)
            {
                if (fragment != null && fragment.gameObject.activeInHierarchy)
                {
                    // 먼저 파편 초기화 (콜라이더 등 기본 설정)
                    InitFragment(fragment);

                    // 투명도 설정
                    SetupMaterial(fragment);

                    // 마지막으로 movement 컴포넌트 추가
                    var fragmentMovement = fragment.gameObject.AddComponent<FragmentMovement>();
                    float distanceToCamera = Vector3.Distance(fragment.transform.position, Camera.main.transform.position);
                    fragmentMovement.initialSpreadForce = Mathf.Lerp(2f, 5f, distanceToCamera / 50f);
                    fragmentMovement.cameraMoveMultiplier = Mathf.Lerp(0.3f, 0.5f, distanceToCamera / 50f);
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

    // 4. 머티리얼 설정 로직 분리
    private void SetupTransparentMaterial(Material material)
    {
        material.SetOverrideTag("RenderType", "Transparent");
        material.SetFloat(SurfaceProperty, 1);
        material.SetFloat(BlendProperty, 1);
        material.SetInt(SrcBlendProperty, (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt(DstBlendProperty, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt(ZWriteProperty, 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        material.renderQueue = DefaultFragmentSettings.renderQueue;
    }

    private void SetupMaterial(RayfireRigid _fragment)
    {
        var renderer = _fragment.GetComponent<Renderer>();
        if (renderer == null) return;

        // 3. 배열 재할당 최소화
        var materials = renderer.materials;
        var newMaterials = new Material[materials.Length];

        for (int i = 0; i < materials.Length; i++)
        {
            var newMat = new Material(materials[i]);
            if (newMat.HasProperty(BaseColorProperty))
            {
                var color = newMat.GetColor(BaseColorProperty);
                color.a = DefaultFragmentSettings.alpha;
                newMat.SetColor(BaseColorProperty, color);
                
                SetupTransparentMaterial(newMat);
            }
            newMaterials[i] = newMat;
        }
        renderer.materials = newMaterials;
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

    // 5. 파편 초기화 최적화
    private void InitFragment(RayfireRigid _fragment)
    {
        var fragmentGO = _fragment.gameObject;
        
        // 콜라이더 일괄 제거
        foreach (var collider in fragmentGO.GetComponents<Collider>())
        {
            Destroy(collider);
        }

        // ObjectProperties 설정
        var fragmentProperties = fragmentGO.GetComponent<ObjectProperties>() 
            ?? fragmentGO.AddComponent<ObjectProperties>();
        
        SetupFragmentProperties(fragmentProperties);

        // Rigidbody 설정
        var rb = fragmentGO.GetComponent<Rigidbody>() ?? fragmentGO.AddComponent<Rigidbody>();
        SetupRigidbody(rb);

        // RayfireRigid 설정
        SetupRayfireComponents(_fragment);
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

    // ��가: 초기화 메서드 (파편을 동적으로 초기화할 때 사용)
    public void Initialize(ScoreType _scoreType, int _fragmentLevel, float _speedMultiplier)
    {
        // 필요한 데이터를 초기화합니다.
        objectProperties.SetScoreType(_scoreType);
        objectProperties.SetFragmentLevel(_fragmentLevel);
        // 속도 배율 등 추가 속성을 초기화할 수 있습니다.
    }

    private void SetupFragmentProperties(ObjectProperties fragmentProperties)
    {
        if (fragmentProperties == null || objectProperties == null) return;
        
        fragmentProperties.SetFragmentLevel(objectProperties.GetFragmentLevel() + 1);
        fragmentProperties.SetScoreType(objectProperties.GetScoreType());
        fragmentProperties.SetBreakable(true);
    }

    private void SetupRigidbody(Rigidbody rb)
    {
        if (rb == null) return;
        
        rb.useGravity = DefaultFragmentSettings.useGravity;
        rb.collisionDetectionMode = DefaultFragmentSettings.collisionMode;
        rb.interpolation = DefaultFragmentSettings.interpolation;
    }

    private void SetupRayfireComponents(RayfireRigid fragment)
    {
        if (fragment == null) return;

        // RayfireRigid 설정
        fragment.demolitionType = DemolitionType.Runtime;

        // RayfireBomb 설정
        if (rayfireBomb != null)
        {
            var fragmentBomb = fragment.gameObject.GetComponent<RayfireBomb>() 
                ?? fragment.gameObject.AddComponent<RayfireBomb>();
            fragmentBomb.range = rayfireBomb.range;
            fragmentBomb.strength = rayfireBomb.strength;
            fragmentBomb.variation = rayfireBomb.variation;
            fragmentBomb.chaos = rayfireBomb.chaos;
        }

        // RayfireSound 설정
        if (rayfireSound != null)
        {
            var fragmentSound = fragment.gameObject.GetComponent<RayfireSound>() 
                ?? fragment.gameObject.AddComponent<RayfireSound>();
            fragmentSound.enabled = true;
            fragmentSound.demolition = rayfireSound.demolition;
            fragmentSound.collision = rayfireSound.collision;
        }
    }

}
