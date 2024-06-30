using System.Collections.Generic;
using UnityEngine;
using RayFire;

public class BreakableObject : MonoBehaviour
{
    private RayfireRigid rayfireRigid;
    private RayfireBomb rayfireBomb;
    private RayfireSound rayfireSound;

    void Start()
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
    }

    void Update()
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

    void BreakObject()
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
        // RayfireSound는 자동으로 재생됩니다.
    }

    void AddComponentsToFragments(RayfireRigid[] fragments)
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

                // 명시적으로 RayfireSound 컴포넌트 활성화
                
                fragmentSound.enabled = true;
                

                // fragmentSound.enabled = rayfireSound.enabled;
                // fragmentSound.initialization = rayfireSound.initialization;
                fragmentSound.demolition = rayfireSound.demolition;
                fragmentSound.collision = rayfireSound.collision;
                // 필요한 경우 추가 속성도 복사할 수 있습니다.
            }
        }
    }
}