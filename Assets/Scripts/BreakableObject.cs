using System.Collections.Generic;
using UnityEngine;
using RayFire;

public class BreakableObject : MonoBehaviour
{
    private RayfireRigid rayfireRigid;
    private RayfireBomb rayfireBomb;
    private RayfireSound rayfireSound;
    [SerializeField] private SlowMotionTrigger slowMotionTrigger; // SlowMotionTrigger 참조

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

        // SlowMotionTrigger 컴포넌트 가져오기
        if (slowMotionTrigger == null)
        {
            slowMotionTrigger = GetComponent<SlowMotionTrigger>();
        }
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
            rayfireRigid.Demolish();
            AddComponentsToFragments(rayfireRigid.fragments.ToArray());
        }
        if (rayfireBomb != null)
        {
            rayfireBomb.Explode(0f);  // 지연 없이 즉시 폭발하도록 0f 설정
        }

        // 슬로우모션 트리거
        slowMotionTrigger?.TriggerSlowMotion();
    }

    private void AddComponentsToFragments(RayfireRigid[] fragments)
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
                // RayfireSound 설정을 복사합니다.

                fragmentSound.enabled = true;
                fragmentSound.demolition = rayfireSound.demolition;
                fragmentSound.collision = rayfireSound.collision;
                // 필요한 경우 추가 속성도 복사할 수 있습니다.
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
