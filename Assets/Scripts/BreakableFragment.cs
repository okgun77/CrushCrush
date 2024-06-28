using UnityEngine;
using RayFire;
public class BreakableFragment : MonoBehaviour
{
    private RayfireRigid rayfireRigid;
    private RayfireBomb rayfireBomb;

    void Start()
    {
        rayfireRigid = GetComponent<RayfireRigid>();
        if (rayfireRigid == null)
        {
            Debug.LogError("RayfireRigid 컴포넌트가 없습니다!");
        }

        rayfireBomb = GetComponent<RayfireBomb>();
        if (rayfireBomb == null)
        {
            Debug.LogWarning("RayfireBomb 컴포넌트가 없습니다!");
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
            AddComponentsToFragments(rayfireRigid.fragments.ToArray());
        }

        if (rayfireBomb != null)
        {
            rayfireBomb.Explode(0f);
        }
    }

    void AddComponentsToFragments(RayfireRigid[] fragments)
    {
        foreach (RayfireRigid fragment in fragments)
        {
            if (fragment.gameObject.GetComponent<BreakableFragment>() == null)
            {
                fragment.gameObject.AddComponent<BreakableFragment>();
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