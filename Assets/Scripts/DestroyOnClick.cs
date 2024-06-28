using UnityEngine;
using RayFire;

public class DestroyOnClick : MonoBehaviour
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
                DestroyObject();
            }
        }
    }

    void DestroyObject()
    {
        if (rayfireRigid != null)
        {
            rayfireRigid.Demolish();
            ApplyExplosionForce();
        }
    }

    void ApplyExplosionForce()
    {
        foreach (RayfireRigid fragment in rayfireRigid.fragments)
        {
            Rigidbody rb = fragment.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(500f, transform.position, 5f);
            }
        }
    }
}
