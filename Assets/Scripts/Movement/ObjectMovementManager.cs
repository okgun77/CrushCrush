using UnityEngine;
using System.Collections.Generic;

public class ObjectMovementManager : MonoBehaviour
{
    private Dictionary<GameObject, IMovementPattern> objectPatterns;
    private Dictionary<GameObject, MovementData> movementDataMap;
    private Transform playerTransform;
    private GameManager gameManager;

    public void Init(GameManager _gameManager)
    {
        this.gameManager = _gameManager;
        Initialize();
        
        Debug.Log("ObjectMovementManager initialized");
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

    public void AssignMovementPattern(GameObject _obj, MovementType _type, MovementData _data)
    {
        if (_obj == null) return;

        IMovementPattern pattern = CreatePattern(_type);
        pattern.Initialize(_data);
        
        // 이미 존재하는 패턴 제거
        if (objectPatterns.ContainsKey(_obj))
        {
            RemoveObject(_obj);
        }
        
        objectPatterns[_obj] = pattern;
        movementDataMap[_obj] = _data;
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

    private IMovementPattern CreatePattern(MovementType _type)
    {
        return _type switch
        {
            MovementType.Straight => new StraightMovement(),
            MovementType.Zigzag => new ZigzagMovement(),
            MovementType.Spiral => new SpiralMovement(),
            _ => new StraightMovement()
        };
    }

    public void RemoveObject(GameObject _obj)
    {
        if (_obj != null && objectPatterns.ContainsKey(_obj))
        {
            objectPatterns.Remove(_obj);
            movementDataMap.Remove(_obj);
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