using UnityEngine;
using System.Collections.Generic;

public class ObjectMovementManager : MonoBehaviour
{
    private Dictionary<GameObject, IMovementPattern> objectPatterns;
    private Dictionary<GameObject, MovementData> movementDataMap;
    private Transform playerTransform;
    private GameManager gameManager;

    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
        Initialize();
    }

    private void Initialize()
    {
        objectPatterns = new Dictionary<GameObject, IMovementPattern>();
        movementDataMap = new Dictionary<GameObject, MovementData>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        
        if (playerTransform == null)
        {
            Debug.LogError("Player not found! Make sure the player object has the 'Player' tag.");
        }
    }

    public void AssignMovementPattern(GameObject obj, MovementType type, MovementData data)
    {
        if (obj == null) return;

        IMovementPattern pattern = CreatePattern(type);
        pattern.Initialize(data);
        
        // 이미 존재하는 패턴 제거
        if (objectPatterns.ContainsKey(obj))
        {
            RemoveObject(obj);
        }
        
        objectPatterns[obj] = pattern;
        movementDataMap[obj] = data;
    }

    private void Update()
    {
        if (objectPatterns == null) return;

        List<GameObject> completedObjects = new List<GameObject>();

        foreach (var kvp in objectPatterns)
        {
            GameObject obj = kvp.Key;
            IMovementPattern pattern = kvp.Value;

            if (obj == null || !obj.activeInHierarchy)
            {
                completedObjects.Add(obj);
                continue;
            }

            if (pattern.IsComplete)
            {
                completedObjects.Add(obj);
                continue;
            }

            Vector3 movement = pattern.CalculateMovement(obj.transform, playerTransform, Time.deltaTime);
            obj.transform.position += movement;
        }

        foreach (var obj in completedObjects)
        {
            RemoveObject(obj);
        }
    }

    private IMovementPattern CreatePattern(MovementType type)
    {
        return type switch
        {
            MovementType.Straight => new StraightMovement(),
            MovementType.Zigzag => new ZigzagMovement(),
            MovementType.Spiral => new SpiralMovement(),
            _ => new StraightMovement()
        };
    }

    public void RemoveObject(GameObject obj)
    {
        if (obj != null && objectPatterns.ContainsKey(obj))
        {
            objectPatterns.Remove(obj);
            movementDataMap.Remove(obj);
        }
    }

    public void ClearAllPatterns()
    {
        objectPatterns?.Clear();
        movementDataMap?.Clear();
    }

    private void OnDestroy()
    {
        ClearAllPatterns();
    }
} 