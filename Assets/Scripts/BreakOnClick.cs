using System.Collections.Generic;
using UnityEngine;
using RayFire;
public class BreakOnClick : MonoBehaviour
{
    private RayfireRigid rayfireRigid;
    private RayfireBomb rayfireBomb;

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
            Debug.LogError("RayfireBomb 컴포넌트가 없습니다!");
            return;
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
    }

    void AddComponentsToFragments(RayfireRigid[] fragments)
    {
        foreach (RayfireRigid fragment in fragments)
        {
            if (fragment.gameObject.GetComponent<BreakOnClick>() == null)
            {
                fragment.gameObject.AddComponent<BreakOnClick>();
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
            if (fragmentBomb == null)
            {
                fragmentBomb = fragment.gameObject.AddComponent<RayfireBomb>();
                fragmentBomb.range = rayfireBomb.range;
                fragmentBomb.strength = rayfireBomb.strength;
                fragmentBomb.variation = rayfireBomb.variation;
                fragmentBomb.chaos = rayfireBomb.chaos;
            }
        }
    }
}