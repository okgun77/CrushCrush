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

    private void SetupMaterial(RayfireRigid _fragment)
    {
        Renderer renderer = _fragment.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material[] newMaterials = new Material[renderer.materials.Length];
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                Material newMat = new Material(renderer.materials[i]);
                if (newMat.HasProperty("_BaseColor"))
                {
                    Color color = newMat.GetColor("_BaseColor");
                    color.a = 0.3f;
                    newMat.SetColor("_BaseColor", color);
                    
                    newMat.SetOverrideTag("RenderType", "Transparent");
                    newMat.SetFloat("_Surface", 1);
                    newMat.SetFloat("_Blend", 1);
                    newMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    newMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    newMat.SetInt("_ZWrite", 0);
                    newMat.DisableKeyword("_ALPHATEST_ON");
                    newMat.EnableKeyword("_ALPHABLEND_ON");
                    newMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    newMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                    newMat.renderQueue = 3000;
                }
                newMaterials[i] = newMat;
            }
            renderer.materials = newMaterials;
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
        // 모든 종류의 Collider 제거
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
        
        // Rigidbody 설정
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

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

    // 추가: 초기화 메서드 (파편을 동적으로 초기화할 때 사용)
    public void Initialize(ScoreType _scoreType, int _fragmentLevel, float _speedMultiplier)
    {
        // 필요한 데이터를 초기화합니다.
        objectProperties.SetScoreType(_scoreType);
        objectProperties.SetFragmentLevel(_fragmentLevel);
        // 속도 배율 등 추가 속성을 초기화할 수 있습니다.
    }

}
