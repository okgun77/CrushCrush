using UnityEngine;
using System.Collections.Generic;

public class MoveManager : MonoBehaviour
{
    [SerializeField] private List<MonoBehaviour> basicMovementBehaviors; // 기본 이동 관련 스크립트를 저장할 리스트
    [SerializeField] private List<MonoBehaviour> difficultyMovementBehaviors; // 난이도에 따른 이동 관련 스크립트를 저장할 리스트
    [SerializeField] private Transform targetPoint; // 타겟 포인트
    private GameManager gameManager;

    public void Init(GameManager _gameManager)
    {
        gameManager = _gameManager;
        Debug.Log("MoveManager: targetPoint가 설정되었습니다.");
    }

    public void ApplyMovements(GameObject _object)
    {
        // MoveToTargetPoint 항상 추가하여 타겟 포인트로 이동하도록 설정
        var moveToTarget = _object.AddComponent<MoveToTargetPoint>();
        moveToTarget.SetTarget(targetPoint);

        // 기본 이동 스크립트 추가
        foreach (var behavior in basicMovementBehaviors)
        {
            AddAndConfigureComponent(_object, behavior);
        }

        float gameTime = Time.timeSinceLevelLoad;

        // 난이도에 따른 이동 스크립트 추가
        foreach (var behavior in difficultyMovementBehaviors)
        {
            if (behavior is IMovementCondition condition && condition.ShouldAddBehavior(gameTime))
            {
                AddAndConfigureComponent(_object, behavior);
            }
        }
    }

    private void AddAndConfigureComponent(GameObject obj, MonoBehaviour behavior)
    {
        var component = obj.AddComponent(behavior.GetType()) as MonoBehaviour;
        if (component != null)
        {
            CopyComponentSettings(behavior, component);
        }
    }

    private void CopyComponentSettings(MonoBehaviour source, MonoBehaviour destination)
    {
        var type = source.GetType();
        var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        foreach (var field in fields)
        {
            if (field.IsPublic || field.IsDefined(typeof(SerializeField), true))
            {
                field.SetValue(destination, field.GetValue(source));
            }
        }
    }
}
