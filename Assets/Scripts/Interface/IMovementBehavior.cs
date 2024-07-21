using UnityEngine;

public interface IMovementBehavior
{
    void Init(MoveSettings _settings, Transform _targetPoint);
}
