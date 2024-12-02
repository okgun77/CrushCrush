using UnityEngine;
using RayFire;
using System.Collections;

public class BreakObject : MonoBehaviour
{
    private RayfireRigid rayfireRigid;
    private RayfireBomb rayfireBomb;
    private RayfireSound rayfireSound;
    private ScoreManager scoreManager;
    private TouchManager touchManager;
    private WarningManager warningManager;
    private AudioManager audioManager;
    private ObjectProperties objectProperties;
    private Transform targetPoint;
    private UIManager uiManager;
    private EffectManager effectManager;


    private bool isWarningActive = false;
    private EScoreType scoreType;

    // 1. 캐시 추가
    private static readonly int BaseColorProperty = Shader.PropertyToID("_BaseColor");
    private static readonly int SurfaceProperty = Shader.PropertyToID("_Surface");
    private static readonly int BlendProperty = Shader.PropertyToID("_Blend");
    private static readonly int SrcBlendProperty = Shader.PropertyToID("_SrcBlend");
    private static readonly int DstBlendProperty = Shader.PropertyToID("_DstBlend");
    private static readonly int ZWriteProperty = Shader.PropertyToID("_ZWrite");
    private static readonly int AlphaClipProperty = Shader.PropertyToID("_AlphaClip");
    private static readonly int MetallicProperty = Shader.PropertyToID("_Metallic");
    private static readonly int SmoothnessProperty = Shader.PropertyToID("_Smoothness");

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
        alpha = 0.8f,
        renderQueue = 3000,
        useGravity = false,
        collisionMode = CollisionDetectionMode.ContinuousSpeculative,
        interpolation = RigidbodyInterpolation.Interpolate
    };

    private static GameObject breakVFXPrefab; // 캐시된 VFX 프리팹

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

        // AudioManager 컴포넌트 가져오기
        audioManager = FindAnyObjectByType<AudioManager>();
        if (audioManager == null)
        {
            Debug.LogError("AudioManager를 찾을 수 없습다!");
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

        effectManager = FindAnyObjectByType<EffectManager>();
        if (effectManager == null)
        {
            Debug.LogError("EffectManager를 찾을 수 없습니다!");
            return;
        }


        // VFX 프리팹 로드
        // if (breakVFXPrefab == null)
        // {
        //     breakVFXPrefab = Resources.Load<GameObject>("VFX/BreakVFX");
        //     if (breakVFXPrefab == null)
        //     {
        //         Debug.LogError("BreakVFX prefab not found in Resources folder!");
        //     }
        // }
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
        Debug.Log("BreakObject.OnTouch called");
        if (rayfireRigid != null)
        {
            Debug.Log("Starting object destruction process");
            HandleObjectDestruction();
        }
        else
        {
            Debug.LogError("RayfireRigid component is missing!");
        }
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
            // 파괴 이펙트 생성 (파괴 로직 전에 실행)
            // SpawnBreakVFX();
            effectManager.PlayEffect(EEffectType.BREAK, transform.position);


            GameObject originalObject = null;
            MeshFilter targetMeshFilter = null;

            // 자신과 자식 오브젝트들의 모든 MeshFilter 찾기
            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

            if (meshFilters.Length == 0)
            {
                Debug.LogWarning("No MeshFilter found in object or its children: " + gameObject.name);
                return;
            }

            // LOD Group 체크 및 처리
            LODGroup lodGroup = GetComponent<LODGroup>();
            if (lodGroup != null)
            {
                originalObject = gameObject;
                LOD[] lods = lodGroup.GetLODs();
                if (lods.Length > 0 && lods[0].renderers.Length > 0)
                {
                    // LOD0의 첫 번째 렌더러의 MeshFilter 사용
                    targetMeshFilter = lods[0].renderers[0].GetComponent<MeshFilter>();
                }
            }
            else
            {
                // LOD가 없는 경우 첫 번째 MeshFilter 사용
                targetMeshFilter = meshFilters[0];
            }

            if (targetMeshFilter != null)
            {
                GameObject targetObject = targetMeshFilter.gameObject;

                // RayfireRigid 설정
                RayfireRigid fragmentRigid;
                if (targetObject != gameObject)
                {
                    fragmentRigid = targetObject.AddComponent<RayfireRigid>();
                    CopyRayfireSettings(rayfireRigid, fragmentRigid);
                }
                else
                {
                    fragmentRigid = rayfireRigid;
                }

                // 현재 속도 저장
                Vector3 currentVelocity = Vector3.zero;
                Vector3 currentAngularVelocity = Vector3.zero;
                // if (moveScript != null)
                // {
                //     currentVelocity = moveScript.GetCurrentVelocity();
                //     Destroy(moveScript);
                // }

                // 파괴 실행
                fragmentRigid.Demolish();
                audioManager.PlaySFX("BreakingBones02-Mono");

                // 파편 처리
                foreach (RayfireRigid fragment in fragmentRigid.fragments)
                {
                    if (fragment != null && fragment.gameObject.activeInHierarchy)
                    {
                        InitFragment(fragment);
                        SetupMaterial(fragment);

                        var fragmentMovement = fragment.gameObject.AddComponent<FragmentMovement>();
                        float distanceToCamera = Vector3.Distance(fragment.transform.position, Camera.main.transform.position);
                        fragmentMovement.initialSpreadForce = Mathf.Lerp(2f, 5f, distanceToCamera / 50f);
                        fragmentMovement.cameraMoveMultiplier = Mathf.Lerp(0.3f, 0.5f, distanceToCamera / 50f);
                        fragmentMovement.SetUITarget(uiManager.GetFragmentTargetIcon());
                    }
                }

                // 원본 오브젝트 풀링 처리
                if (originalObject != null)
                {
                    ObjectPoolManager.Instance.ReturnObject(originalObject);
                }

                // 점수 추가 및 연속 파괴 처리
                AddScore();

                var stageManager = FindAnyObjectByType<StageManager>();
                if (stageManager != null)
                {
                    stageManager.AddDestroyCount();
                }

                if (objectProperties.GetFragmentLevel() == 0)
                {
                    scoreManager.IncreaseConsecutiveDestroys();
                }
            }
        }
    }

    private void CopyRayfireSettings(RayfireRigid source, RayfireRigid target)
    {
        target.demolitionType = source.demolitionType;
        target.meshDemolition = source.meshDemolition;
        target.physics = source.physics;
        target.limitations = source.limitations;
        target.materials = source.materials;
    }

    // 4. 머티리얼 설정 로직 분리
    private void SetupTransparentMaterial(Material material)
    {
        // 셰이더 키워드 설정
        material.SetOverrideTag("RenderType", "Transparent");
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.SetFloat("_Surface", 1);
        material.SetFloat(AlphaClipProperty, 0);

        // 렌더 큐 설정
        material.renderQueue = DefaultFragmentSettings.renderQueue;

        // 키워드 설정
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
    }

    private void SetupMaterial(RayfireRigid _fragment)
    {
        var renderer = _fragment.GetComponent<Renderer>();
        if (renderer == null) return;

        // 원본 렌더러에서 재질 속성 가져오기
        var originalRenderer = GetComponent<Renderer>();
        if (originalRenderer == null) return;

        var originalMaterials = originalRenderer.sharedMaterials;
        var newMaterials = new Material[originalMaterials.Length];

        for (int i = 0; i < originalMaterials.Length; i++)
        {
            if (originalMaterials[i] == null) continue;

            // 원본 재질을 정확하게 복제
            var newMat = new Material(originalMaterials[i].shader);
            newMat.CopyPropertiesFromMaterial(originalMaterials[i]);

            // 원본 색상 및 재질 속성 보존
            if (newMat.HasProperty(BaseColorProperty))
            {
                Color originalColor = originalMaterials[i].GetColor(BaseColorProperty);
                originalColor.a = DefaultFragmentSettings.alpha;
                newMat.SetColor(BaseColorProperty, originalColor);
            }

            // 메탈릭/스무스니스 속성 보존
            if (newMat.HasProperty(MetallicProperty))
            {
                float metallic = originalMaterials[i].GetFloat(MetallicProperty);
                newMat.SetFloat(MetallicProperty, metallic);
            }

            if (newMat.HasProperty(SmoothnessProperty))
            {
                float smoothness = originalMaterials[i].GetFloat(SmoothnessProperty);
                newMat.SetFloat(SmoothnessProperty, smoothness);
            }

            // 투명도 설정
            SetupTransparentMaterial(newMat);
            newMaterials[i] = newMat;
        }

        renderer.materials = newMaterials;
    }

    private void AddScore()
    {
        if (scoreManager == null || objectProperties == null) return;

        // 카메라와의 거리 계산
        float distanceToCamera = Vector3.Distance(transform.position, Camera.main.transform.position);
        
        // 점수 계산 및 추가
        int calculatedScore = scoreManager.CalculateScore(
            objectProperties.GetScoreType(),
            objectProperties.GetFragmentLevel(),
            distanceToCamera
        );
        
        scoreManager.AddScore(calculatedScore);
        
        // 연속 파괴 카운트 증가 (프래그먼트가 아닌 경우에만)
        if (objectProperties.GetFragmentLevel() == 0)
        {
            scoreManager.IncreaseConsecutiveDestroys();
        }
    }

    // 5. 파편 초기화 최적화
    private void InitFragment(RayfireRigid _fragment)
    {
        var fragmentGO = _fragment.gameObject;

        // Rayfire 컴포넌트들 제거
        Destroy(_fragment);  // RayfireRigid 제거
        var bomb = fragmentGO.GetComponent<RayfireBomb>();
        if (bomb != null) Destroy(bomb);  // RayfireBomb 제거

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

    // 가: 초기화 메서드 (파편을 동적으로 초기화할 때 사용)
    public void Initialize(EScoreType _scoreType, int _fragmentLevel, float _speedMultiplier)
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

    private void SpawnBreakVFX()
    {
        if (breakVFXPrefab != null)
        {
            // VFX 생성
            GameObject vfx = Instantiate(breakVFXPrefab, transform.position, Quaternion.identity);

            // 카메라와의 거리에 따라 크기 조절
            float distanceToCamera = Vector3.Distance(transform.position, Camera.main.transform.position);
            float scaleFactor = Mathf.Lerp(0.5f, 1.5f, distanceToCamera / 50f); // 거리 50유닛을 기준으로 0.8~2배 크기 조절
            vfx.transform.localScale *= scaleFactor;

            // ParticleSystem 있는 경우 자동 제거
            var particleSystem = vfx.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                // 파티클 시스템의 크기도 조절 (필요한 경우)
                var main = particleSystem.main;
                main.startSizeMultiplier *= scaleFactor;

                float duration = main.duration;
                Destroy(vfx, duration);
            }
            else
            {
                Destroy(vfx, 2f);
            }
        }
    }

    public void SetScoreType(EScoreType type) => scoreType = type;
}