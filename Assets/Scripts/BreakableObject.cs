using System.Collections.Generic;
using UnityEngine;
using RayFire;

public class BreakableObject : MonoBehaviour
{
    private RayfireRigid rayfireRigid;
    private RayfireBomb rayfireBomb;
    private RayfireSound rayfireSound;
    private MoveToTargetPoint moveScript;

    // 추가 속도 설정
    [SerializeField] private float additionalSpeedMultiplier = 2.0f;

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
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == gameObject)
            {
                BreakObject();
            }
        }
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

            // 파괴 및 파편에 속도 적용
            rayfireRigid.Demolish();
            AddComponentsToFragments(rayfireRigid.fragments.ToArray(), currentVelocity);
        }
        if (rayfireBomb != null)
        {
            rayfireBomb.Explode(0f);  // 지연 없이 즉시 폭발하도록 0f 설정
        }
    }

    private void AddComponentsToFragments(RayfireRigid[] fragments, Vector3 initialVelocity)
    {
        foreach (RayfireRigid fragment in fragments)
        {
            if (fragment.gameObject.GetComponent<BreakableObject>() == null)
            {
                fragment.gameObject.AddComponent<BreakableObject>();
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
                    col = fragment.gameObject.AddComponent<MeshCollider>();
                    (col as MeshCollider).convex = true;
                }
                catch
                {
                    Destroy(col);
                    col = fragment.gameObject.AddComponent<SphereCollider>();
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

            // 파편에 초기 속도 및 추가 속도 적용
            if (rb != null)
            {
                rb.velocity = initialVelocity * additionalSpeedMultiplier;
            }
        }
    }

    private T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        System.Reflection.FieldInfo[] fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy as T;
    }
}
