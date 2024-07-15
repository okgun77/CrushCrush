using UnityEngine;
using RayFire;

public class BreakableObject : MonoBehaviour
{
    private RayfireRigid rayfireRigid;
    private RayfireBomb rayfireBomb;
    private RayfireSound rayfireSound;
    private MoveToTargetPoint moveScript;
    private ScoreManager scoreManager;
    private TouchManager touchManager;

    [SerializeField] private float additionalSpeedMultiplier = 2.0f;
    [SerializeField] private ScoreType scoreType;
    [SerializeField] private int fragmentLevel = 0; // 파편 레벨 (0은 원래 오브젝트)

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
        scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager == null)
        {
            Debug.LogError("ScoreManager를 찾을 수 없습니다!");
            return;
        }

        // TouchManager 컴포넌트 가져오기 및 등록
        touchManager = FindObjectOfType<TouchManager>();
        if (touchManager != null)
        {
            touchManager.RegisterBreakableObject(this);
        }
    }

    private void OnDestroy()
    {
        // TouchManager에서 등록 해제
        if (touchManager != null)
        {
            touchManager.UnregisterBreakableObject(this);
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
            }
        }

        if (rayfireBomb != null)
        {
            rayfireBomb.Explode(0f); // 지연 없이 즉시 폭발
        }

        // 점수 추가
        AddScore();
    }

    private void AddScore()
    {
        int calculatedScore = scoreManager.CalculateScore(scoreType, fragmentLevel);
        scoreManager.AddScore(calculatedScore);
    }

    private void InitFragment(RayfireRigid fragment)
    {
        if (fragmentLevel < scoreManager.GetMaxFragmentLevel())
        {
            if (fragment.gameObject.GetComponent<BreakableObject>() == null)
            {
                var fragmentScript = fragment.gameObject.AddComponent<BreakableObject>();
                fragmentScript.scoreType = this.scoreType;
                fragmentScript.fragmentLevel = this.fragmentLevel + 1; // 파편 레벨 증가
            }
        }

        Rigidbody rb = fragment.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = fragment.gameObject.AddComponent<Rigidbody>();
        }

        Collider col = fragment.GetComponent<Collider>();
        if (col == null)
        {
            try
            {
                MeshCollider meshCollider = fragment.gameObject.AddComponent<MeshCollider>();
                meshCollider.convex = true;

                // Convex Hull 생성 가능한지 검증
                if (meshCollider.sharedMesh.vertexCount < 4 || AreVerticesCoplanar(meshCollider.sharedMesh))
                {
                    Debug.LogWarning("Mesh does not have enough vertices or vertices are coplanar for Convex Hull creation.");
                    Destroy(meshCollider);
                    SphereCollider sphereCollider = fragment.gameObject.AddComponent<SphereCollider>();
                }
            }
            catch
            {
                Destroy(col);
                SphereCollider sphereCollider = fragment.gameObject.AddComponent<SphereCollider>();
            }
        }

        RayfireRigid fragmentRigid = fragment.GetComponent<RayfireRigid>();
        if (fragmentRigid == null)
        {
            fragmentRigid = fragment.gameObject.AddComponent<RayfireRigid>();
            fragmentRigid.demolitionType = DemolitionType.Runtime;
        }

        RayfireBomb fragmentBomb = fragment.GetComponent<RayfireBomb>();
        if (fragmentBomb == null && rayfireBomb != null)
        {
            fragmentBomb = fragment.gameObject.AddComponent<RayfireBomb>();
            fragmentBomb.range = rayfireBomb.range;
            fragmentBomb.strength = rayfireBomb.strength;
            fragmentBomb.variation = rayfireBomb.variation;
            fragmentBomb.chaos = rayfireBomb.chaos;
        }

        RayfireSound fragmentSound = fragment.GetComponent<RayfireSound>();
        if (fragmentSound == null && rayfireSound != null)
        {
            fragmentSound = fragment.gameObject.AddComponent<RayfireSound>();
            fragmentSound.enabled = true;
            fragmentSound.demolition = rayfireSound.demolition;
            fragmentSound.collision = rayfireSound.collision;
        }
    }

    private bool AreVerticesCoplanar(Mesh mesh)
    {
        if (mesh.vertexCount < 4)
            return true;

        Vector3[] vertices = mesh.vertices;
        Vector3 normal = Vector3.Cross(vertices[1] - vertices[0], vertices[2] - vertices[0]);

        for (int i = 3; i < vertices.Length; i++)
        {
            if (Vector3.Dot(normal, vertices[i] - vertices[0]) != 0)
                return false;
        }
        return true;
    }

    private void SetFragmentProperties(RayfireRigid fragment, Vector3 initialVelocity)
    {
        Rigidbody rb = fragment.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = initialVelocity * additionalSpeedMultiplier;
        }

        // 파편의 알파 값 적용
        Renderer renderer = fragment.GetComponent<Renderer>();
        if (renderer != null)
        {
            foreach (var mat in renderer.materials)
            {
                if (mat.HasProperty("_Color"))
                {
                    mat.SetFloat("_Surface", 1); // Transparent
                    mat.SetFloat("_Blend", 1); // Alpha Blend
                    Color color = mat.color;
                    color.a = 0.1f; // 알파 값 조정
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
                    mat.SetFloat("_Alpha", 0.1f); // 알파 값 조정
                }
            }
        }
    }
}
