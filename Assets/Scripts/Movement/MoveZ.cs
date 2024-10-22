using UnityEngine;

public class MoveZ : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Vector3 moveDirection = new Vector3(0, 0, -1);

    private void Update()
    {
        transform.Translate(moveDirection.normalized * moveSpeed * Time.deltaTime);
    }
}
