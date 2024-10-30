using UnityEngine;
using System.Collections.Generic;
using SpaceGraphicsToolkit;
using System.Linq;

public class JunkSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform planetCenter;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject[] junkPrefabs;
    [SerializeField] private Transform junkContainer;
    
    [Header("Spawn Settings")]
    [SerializeField] private float minSpawnHeight = 2f;
    [SerializeField] private float maxSpawnHeight = 5f;
    [SerializeField] private float visibleRadius = 30f;
    [SerializeField] private float spawnRadius = 45f;
    [SerializeField] private float despawnRadius = 60f;
    [SerializeField] private float spawnInterval = 0.5f;
    [SerializeField] private int maxJunkCount = 30;
    [SerializeField] private float minJunkDistance = 5f;
    
    private Vector3[] vertices;
    private float planetRadius;
    private List<GameObject> activeJunk = new List<GameObject>();
    private float spawnTimer;
    private SgtPlanet planetComponent;

    private void Start()
    {
        if (!ValidateReferences()) return;
        
        InitializePlanetData();
        
        // 디버그 로그
        Debug.Log($"Planet Center: {planetCenter.position}");
        Debug.Log($"Player Position: {player.position}");
        Debug.Log($"Planet Radius: {planetRadius}");
        
        // 초기 생성 제거
        // for (int i = 0; i < maxJunkCount / 3; i++) {
        //     TrySpawnJunk();
        // }
    }

    private void InitializePlanetData()
    {
        // SgtPlanet 컴포넌트 찾기
        planetComponent = planetCenter.GetComponent<SgtPlanet>();
        
        if (planetComponent != null)
        {
            // SgtPlanet의 Radius와 로컬 스케일을 고려하여 실제 반지름 계산
            planetRadius = planetComponent.Radius * planetCenter.lossyScale.x;
            Debug.Log($"Planet radius from SgtPlanet: {planetRadius}");
        }
        else
        {
            Debug.LogError("SgtPlanet component not found on planet object!");
            planetRadius = 1000f; // 기본값
        }
    }

    private bool ValidateReferences()
    {
        if (player == null)
        {
            Debug.LogError("Player reference is missing!");
            return false;
        }
        
        if (planetCenter == null)
        {
            Debug.LogError("Planet Center reference is missing!");
            return false;
        }
        
        if (junkContainer == null)
        {
            junkContainer = new GameObject("JunkContainer").transform;
        }
        
        if (junkPrefabs == null || junkPrefabs.Length == 0)
        {
            Debug.LogError("Junk prefabs are missing!");
            return false;
        }
        
        return true;
    }

    private void Update()
    {
        // null 항목 제거를 먼저 수행
        activeJunk.RemoveAll(junk => junk == null);
        
        spawnTimer += Time.deltaTime;
        
        if (spawnTimer >= spawnInterval)
        {
            // 최소 개수 유지를 위한 로직 추가
            int minJunkCount = maxJunkCount / 3;  // 최소 유지할 쓰레기 개수
            int spawnCount = minJunkCount - activeJunk.Count;  // 부족한 만큼 생성
            
            if (spawnCount > 0)
            {
                for (int i = 0; i < spawnCount; i++)
                {
                    if (activeJunk.Count < maxJunkCount)
                    {
                        TrySpawnJunk();
                    }
                }
            }
            else if (activeJunk.Count < maxJunkCount)  // 최소 개수는 충족했지만 최대 개수보다 적을 때
            {
                TrySpawnJunk();  // 하나씩 추가
            }
            
            spawnTimer = 0f;
            CleanupJunk();
        }
    }

    private void TrySpawnJunk()
    {
        bool spawned = false;
        int attempts = 0;
        const int MAX_ATTEMPTS = 10;
        
        while (!spawned && attempts < MAX_ATTEMPTS)
        {
            attempts++;
            Vector3 spawnPosition = GetSpawnPositionOutsidePlayerView();
            float distanceToPlayer = Vector3.Distance(spawnPosition, player.position);
            
            // 거리 조건 확인
            if (distanceToPlayer >= visibleRadius && distanceToPlayer <= spawnRadius)
            {
                if (!IsTooCloseToOtherJunk(spawnPosition))
                {
                    SpawnJunkAt(spawnPosition);
                    spawned = true;
                    Debug.Log($"Successfully spawned junk at attempt {attempts}");
                }
                else
                {
                    Debug.Log("Position too close to other junk");
                }
            }
            else
            {
                Debug.Log($"Invalid distance: {distanceToPlayer} (should be between {visibleRadius} and {spawnRadius})");
            }
        }
        
        if (!spawned)
        {
            Debug.LogWarning($"Failed to spawn junk after {MAX_ATTEMPTS} attempts");
        }
    }

    private Vector3 GetSpawnPositionOutsidePlayerView()
    {
        // 플레이어 기준으로 랜덤한 방향 생성
        float randomAngle = Random.Range(-180f, 180f);
        Vector3 randomDirection = Quaternion.Euler(0, randomAngle, 0) * player.forward;
        
        // visibleRadius와 spawnRadius 사용
        float spawnDistance = Random.Range(visibleRadius, spawnRadius);
        Vector3 basePosition = player.position + (randomDirection * spawnDistance);
        
        // 행성 중심 방향으로의 벡터
        Vector3 toPlanetCenter = (planetCenter.position - basePosition).normalized;
        
        // 행성 표면 위치 계산
        Vector3 surfacePosition = planetCenter.position + (toPlanetCenter * -planetRadius);
        
        // 표면으로부터 약간 위로 올린 최종 스폰 위치
        float heightAboveSurface = Random.Range(minSpawnHeight, maxSpawnHeight);
        Vector3 spawnPosition = surfacePosition + (toPlanetCenter * -heightAboveSurface);
        
        Debug.Log($"Player Position: {player.position}");
        Debug.Log($"Random Direction: {randomDirection}");
        Debug.Log($"Base Position: {basePosition}");
        Debug.Log($"Surface Position: {surfacePosition}");
        Debug.Log($"Final Spawn Position: {spawnPosition}");
        Debug.Log($"Distance from player: {Vector3.Distance(spawnPosition, player.position)}");
        
        return spawnPosition;
    }

    private bool IsValidSpawnPosition(Vector3 position)
    {
        float distanceFromPlayer = Vector3.Distance(position, player.position);
        
        // visibleRadius와 spawnRadius 사용
        if (distanceFromPlayer < visibleRadius || distanceFromPlayer > spawnRadius)
        {
            Debug.Log($"Distance check failed: {distanceFromPlayer} (should be between {visibleRadius} and {spawnRadius})");
            return false;
        }
        
        // 카메라 시야 체크
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(position);
        bool isVisible = screenPoint.z > 0 && 
                        screenPoint.x >= 0 && screenPoint.x <= 1 && 
                        screenPoint.y >= 0 && screenPoint.y <= 1;
                        
        if (isVisible)
        {
            Debug.Log("Position is visible in camera view");
            return false;
        }
        
        return true;
    }

    private void SpawnJunkAt(Vector3 position)
    {
        GameObject prefab = junkPrefabs[Random.Range(0, junkPrefabs.Length)];
        GameObject junk = Instantiate(prefab, position, Quaternion.identity, junkContainer);
        
        Vector3 upDirection = (junk.transform.position - planetCenter.position).normalized;
        junk.transform.rotation = Quaternion.LookRotation(Random.onUnitSphere, upDirection);
        
        activeJunk.Add(junk);
    }

    private bool IsTooCloseToOtherJunk(Vector3 position)
    {
        foreach (GameObject junk in activeJunk.ToList())
        {
            if (junk == null) continue;
            
            if (Vector3.Distance(position, junk.transform.position) < minJunkDistance)
            {
                return true;
            }
        }
        return false;
    }

    private void CleanupJunk()
    {
        for (int i = activeJunk.Count - 1; i >= 0; i--)
        {
            if (activeJunk[i] == null) continue;
            
            float distanceToPlayer = Vector3.Distance(
                activeJunk[i].transform.position,
                player.position
            );
            
            if (distanceToPlayer > despawnRadius)
            {
                Destroy(activeJunk[i]);
                activeJunk.RemoveAt(i);
            }
        }
    }
} 