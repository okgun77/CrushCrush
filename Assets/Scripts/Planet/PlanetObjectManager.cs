using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;  // PrefabUtility를 위한 추가
#endif

public class PlanetObjectManager : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableObject
    {
        public GameObject prefab;
        [Range(0f, 1f)] public float spawnProbability = 1f;
        public bool maintainOriginalScale = true;
    }

    [Header("Required References")]
    [SerializeField] private MeshFilter planetMeshFilter;

    [Header("Spawn Settings")]
    [SerializeField] private SpawnableObject[] spawnableObjects;
    [SerializeField] private Transform objectContainer;
    
    [Header("Distribution Settings")]
    [SerializeField] private int objectCount = 50;
    [SerializeField] private float distanceFromSurface = 2f;
    [SerializeField] private float minObjectSpacing = 3f;
    
    [Header("Object Orientation")]
    [SerializeField] private bool alignToSurface = true;
    [SerializeField] private Vector3 randomRotation = new Vector3(15f, 15f, 15f);

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private Mesh planetMesh;
    private Vector3[] vertices;
    private Vector3[] normals;

    private void Awake()
    {
        ValidateReferences();
    }

    private void ValidateReferences()
    {
        // MeshFilter 찾기
        if (planetMeshFilter == null)
        {
            planetMeshFilter = GetComponent<MeshFilter>();
            if (planetMeshFilter == null)
            {
                Debug.LogError("MeshFilter not found!");
                return;
            }
        }

        // Mesh 데이터 가져오기
        if (planetMeshFilter != null)
        {
            planetMesh = planetMeshFilter.sharedMesh;
            if (planetMesh == null)
            {
                Debug.LogError("No mesh assigned to MeshFilter!");
                return;
            }
            vertices = planetMesh.vertices;
            normals = planetMesh.normals;
        }

        // Object Container 설정
        if (objectContainer == null)
        {
            GameObject container = new GameObject("SpawnedObjects");
            objectContainer = container.transform;
        }
    }

    [ContextMenu("Generate Objects")]
    public void GenerateObjects()
    {
        if (planetMesh == null)
        {
            Debug.LogError("Planet mesh not found!");
            return;
        }

        ClearExistingObjects();
        List<Vector3> spawnPoints = GenerateSpawnPoints();
        SpawnObjectsAtPositions(spawnPoints);
    }

    private List<Vector3> GenerateSpawnPoints()
    {
        List<Vector3> points = new List<Vector3>();
        int attempts = 0;
        int maxAttempts = objectCount * 10;

        while (points.Count < objectCount && attempts < maxAttempts)
        {
            // 랜덤한 버텍스 선택
            int randomVertexIndex = Random.Range(0, vertices.Length);
            Vector3 vertexPoint = planetMeshFilter.transform.TransformPoint(vertices[randomVertexIndex]);
            Vector3 normal = planetMeshFilter.transform.TransformDirection(normals[randomVertexIndex]);
            
            // 표면으로부터 일정 거리 떨어진 위치 계산
            Vector3 spawnPoint = vertexPoint + (normal.normalized * distanceFromSurface);

            if (IsValidSpawnPoint(spawnPoint, points))
            {
                points.Add(spawnPoint);
            }

            attempts++;
        }

        return points;
    }

    private bool IsValidSpawnPoint(Vector3 newPoint, List<Vector3> existingPoints)
    {
        foreach (Vector3 point in existingPoints)
        {
            if (Vector3.Distance(newPoint, point) < minObjectSpacing)
            {
                return false;
            }
        }
        return true;
    }

    private void SpawnObjectsAtPositions(List<Vector3> positions)
    {
        foreach (Vector3 position in positions)
        {
            SpawnableObject objectToSpawn = SelectRandomObject();
            if (objectToSpawn == null) continue;

            // 오브젝트 생성
            GameObject obj = Instantiate(objectToSpawn.prefab);
            
            // 위치 설정
            obj.transform.position = position;
            
            if (alignToSurface)
            {
                Vector3 normal = GetNearestVertexNormal(position);
                AlignObjectToNormal(obj, normal);
            }

            // 관리를 위해 컨테이너에 추가
            obj.transform.parent = objectContainer;

            spawnedObjects.Add(obj);
        }
    }

    private Vector3 GetNearestVertexNormal(Vector3 position)
    {
        float nearestDistance = float.MaxValue;
        Vector3 nearestNormal = Vector3.up;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertexPoint = planetMeshFilter.transform.TransformPoint(vertices[i]);
            float distance = Vector3.Distance(position, vertexPoint);
            
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestNormal = planetMeshFilter.transform.TransformDirection(normals[i]);
            }
        }

        return nearestNormal.normalized;
    }

    private void AlignObjectToNormal(GameObject obj, Vector3 normal)
    {
        Quaternion alignment = Quaternion.FromToRotation(Vector3.up, normal);
        Vector3 randomRot = new Vector3(
            Random.Range(-randomRotation.x, randomRotation.x),
            Random.Range(-randomRotation.y, randomRotation.y),
            Random.Range(-randomRotation.z, randomRotation.z)
        );
        
        obj.transform.rotation = alignment * Quaternion.Euler(randomRot);
    }

    private SpawnableObject SelectRandomObject()
    {
        float totalProbability = 0f;
        foreach (var obj in spawnableObjects)
        {
            totalProbability += obj.spawnProbability;
        }

        float random = Random.Range(0f, totalProbability);
        float currentSum = 0f;

        foreach (var obj in spawnableObjects)
        {
            currentSum += obj.spawnProbability;
            if (random <= currentSum)
            {
                return obj;
            }
        }

        return spawnableObjects[0];
    }

    private void ClearExistingObjects()
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj != null)
            {
                DestroyImmediate(obj);
            }
        }
        spawnedObjects.Clear();
    }
} 