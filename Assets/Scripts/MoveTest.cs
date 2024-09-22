using UnityEngine;
using DG.Tweening;


public class MoveTest : MonoBehaviour
{
    private Vector3 targetPos = new Vector3(0, 5, 0);

    private void Update()
    {
        transform.DOMove(targetPos, 1.0f).SetEase(Ease.InBounce);
    }
}
