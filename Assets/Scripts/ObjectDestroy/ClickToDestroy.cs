using UnityEngine;
using RayFire;

public class ClickToDestroy : MonoBehaviour
{
    private void Update()
    {
        // 마우스 왼쪽 버튼을 클릭했을 때
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 레이캐스트가 어떤 콜라이더와 충돌했다면
            if (Physics.Raycast(ray, out hit))
            {
                // 충돌한 오브젝트에서 RayfireRigid 컴포넌트를 찾는다.
                RayfireRigid rigid = hit.collider.GetComponent<RayfireRigid>();

                if (rigid != null)
                {
                    // 클릭한 지점을 중심으로 파괴를 시작
                    DemolishAtPoint(rigid, hit.point);

                    // 폭발이 필요한 경우
                    RayfireBomb bomb = hit.collider.GetComponent<RayfireBomb>();
                    if (bomb != null)
                    {
                        // 클릭한 지점에서 폭발을 발생시킴
                        TriggerExplosion(bomb, hit.point);
                    }
                }
            }
        }
    }

    void DemolishAtPoint(RayfireRigid _rigid, Vector3 _point)
    {
        // 파괴 시작점 설정
        _rigid.limitations.contactVector3 = _point;

        // 파괴 실행
        _rigid.Demolish();

        // 파편들에도 동일한 기능 부여
        AddComponentsToFragments(_rigid.fragments.ToArray(), _point);
    }

    void TriggerExplosion(RayfireBomb _bomb, Vector3 _point)
    {
        // 폭발 위치 설정
        _bomb.transform.position = _point;

        // 폭발 실행
        _bomb.Explode(0f);  // 지연 없이 즉시 폭발
    }

    void AddComponentsToFragments(RayfireRigid[] fragments, Vector3 explosionPoint)
    {
        foreach (RayfireRigid fragment in fragments)
        {
            if (fragment.gameObject.GetComponent<ClickToDestroy>() == null)
            {
                fragment.gameObject.AddComponent<ClickToDestroy>();
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

            RayfireBomb fragmentBomb = fragment.GetComponent<RayfireBomb>();
            if (fragmentBomb == null)
            {
                fragmentBomb = fragment.gameObject.AddComponent<RayfireBomb>();
            }
        }
    }
}
