using UnityEngine;
using RayFire;

public class BreakableObject : MonoBehaviour
{
    private RayfireRigid rayfireRigid;

    void Start()
    {
        rayfireRigid = GetComponent<RayfireRigid>();
        if (rayfireRigid == null)
        {
            Debug.LogError("RayfireRigid 컴포넌트가 없습니다!");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == gameObject)
            {
                Break();
            }
        }
    }

    void Break()
    {
        if (rayfireRigid != null)
        {
            rayfireRigid.Demolish();
            AddComponentsToFragments();
        }
    }

    void AddComponentsToFragments()
    {
        foreach (RayfireRigid fragment in rayfireRigid.fragments)
        {
            Rigidbody rb = fragment.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = fragment.gameObject.AddComponent<Rigidbody>();
            }

            Collider col = fragment.GetComponent<Collider>();
            if (col == null)
            {
                col = fragment.gameObject.AddComponent<MeshCollider>();
                (col as MeshCollider).convex = true;
            }

            RayfireRigid fragmentRigid = fragment.GetComponent<RayfireRigid>();
            if (fragmentRigid == null)
            {
                fragmentRigid = fragment.gameObject.AddComponent<RayfireRigid>();
                fragmentRigid.demolitionType = DemolitionType.Runtime;
            }

            fragment.gameObject.AddComponent<BreakableFragment>();
        }
    }
}
