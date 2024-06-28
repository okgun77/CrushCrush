using RayFire;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToBreakObject : MonoBehaviour
{

    private RayfireRigid rayfireRigid;
    private RayfireBomb rayfireBomb;

    void Start()
    {
        rayfireRigid = GetComponent<RayfireRigid>();
        if (rayfireRigid == null)
        {
            Debug.LogError("RayfireRigid 컴포넌트가 없습니다!");
            return;
        }

        rayfireBomb = GetComponent<RayfireBomb>();
        if (rayfireBomb == null)
        {
            Debug.LogWarning("RayfireBomb 컴포넌트가 없습니다! 파편에는 폭발 효과가 없습니다.");
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
            Invoke("TriggerExplosion", 0.1f);  // 지연을 통해 폭발이 확실히 발생하도록 함
        }
    }

    void TriggerExplosion()
    {
        if (rayfireBomb != null)
        {
            rayfireBomb.Explode(0f);  // 지연 없이 즉시 폭발하도록 0f 설정
        }
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
                    // 컨벡스 메시 생성 실패 시 다른 콜라이더 추가
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
        }
    }
}
