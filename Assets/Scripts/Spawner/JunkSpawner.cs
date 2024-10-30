using UnityEngine;
using System.Collections.Generic;
using SpaceGraphicsToolkit;
using System.Linq;

public class JunkSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform planetCenter;      // 행성의 중심점
    [SerializeField] private Transform player;           // 플레이어 Transform
    [SerializeField] private GameObject[] junkPrefabs;   // 생성할 쓰레기 프리팹 배열
    [SerializeField] private Transform junkContainer;    // 생성된 쓰레기들을 담을 부모 객체
    
    [Header("Spawn Settings")]
    [SerializeField] private float minSpawnHeight = 2f;  // 행성 표면으로부터 최소 스폰 높이
    [SerializeField] private float maxSpawnHeight = 5f;  // 행성 표면으로부터 최대 스폰 높이
    [SerializeField] private float visibleRadius = 30f;  // 플레이어 주변 최소 스폰 거리
    [SerializeField] private float spawnRadius = 45f;    // 플레이어 주변 최대 스폰 거리
    [SerializeField] private float despawnRadius = 60f;  // 플레이어로부터 제거될 거리
    [SerializeField] private float spawnInterval = 0.5f; // 스폰 시도 간격
    [SerializeField] private int maxJunkCount = 30;      // 최대 쓰레기 개수
    [SerializeField] private float minJunkDistance = 5f; // 쓰레기 간 최소 거리
    [SerializeField] private float screenEdgeMargin = 0.1f;  // 화면 가장자리 여유 공간 (0.1 = 10%)
    
    [Header("Effects")]
    [SerializeField] private JunkFadeEffect fadeEffectSettings;  // EffectManager의 JunkFadeEffect 참조
    
    private float planetRadius;                          // 행성의 실제 반지름
    private List<GameObject> activeJunk = new List<GameObject>();  // 현재 활성화된 쓰레기 목록
    private float spawnTimer;                           // 스폰 타이머
    private SgtPlanet planetComponent;                  // 행성 컴포넌트
    private Camera mainCamera;  // 카메라 캐시용 변수 추가

    private void Start()
    {
        if (!ValidateReferences()) return;
        
        InitializePlanetData();
        mainCamera = Camera.main;  // 카메라 참조 캐시
        
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
            
            if (distanceToPlayer >= visibleRadius && 
                distanceToPlayer <= spawnRadius && 
                !IsPositionVisible(spawnPosition) &&  // 시야 체크 가
                !IsTooCloseToOtherJunk(spawnPosition))
            {
                SpawnJunkAt(spawnPosition);
                spawned = true;
                if (Debug.isDebugBuild)
                    Debug.Log($"Successfully spawned junk at attempt {attempts}");
            }
        }
        
        if (!spawned && Debug.isDebugBuild)
        {
            Debug.LogWarning($"Failed to spawn junk after {MAX_ATTEMPTS} attempts");
        }
    }

    private Vector3 GetSpawnPositionOutsidePlayerView()
    {
        float randomAngle = Random.Range(-150f, 150f);
        Vector3 randomDirection = Quaternion.Euler(0, randomAngle, 0) * player.forward;
        
        float minDistance = visibleRadius + 5f;
        float maxDistance = spawnRadius - 5f;
        float spawnDistance = Random.Range(minDistance, maxDistance);
        Vector3 basePosition = player.position + (randomDirection * spawnDistance);
        
        Vector3 toPlanetCenter = (planetCenter.position - basePosition).normalized;
        Vector3 surfacePosition = planetCenter.position + (toPlanetCenter * -planetRadius);
        float heightAboveSurface = Random.Range(minSpawnHeight, maxSpawnHeight);
        
        return surfacePosition + (toPlanetCenter * -heightAboveSurface);
    }

    private bool IsPositionVisible(Vector3 _position)
    {
        if (mainCamera == null) return false;
        
        Vector3 directionToPosition = (_position - mainCamera.transform.position).normalized;
        float angleToPosition = Vector3.Angle(mainCamera.transform.forward, directionToPosition);
        float halfFOV = mainCamera.fieldOfView * 0.5f;
        
        if (angleToPosition > halfFOV)
        {
            return false;  // 시야각 밖에 있음
        }
        
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(_position);
        bool isInView = screenPoint.z > 0 && 
                       screenPoint.x >= (0f + screenEdgeMargin) && 
                       screenPoint.x <= (1f - screenEdgeMargin) && 
                       screenPoint.y >= (0f + screenEdgeMargin) && 
                       screenPoint.y <= (1f - screenEdgeMargin);
        
        return isInView;
    }

    private void SpawnJunkAt(Vector3 _position)
    {
        GameObject prefab = junkPrefabs[Random.Range(0, junkPrefabs.Length)];
        GameObject junk = Instantiate(prefab, _position, Quaternion.identity, junkContainer);
        
        Vector3 upDirection = (junk.transform.position - planetCenter.position).normalized;
        junk.transform.rotation = Quaternion.LookRotation(Random.onUnitSphere, upDirection);
        
        // 페이드 효과 설정값과 함께 추가
        JunkFadeEffect.CreateEffect(junk, fadeEffectSettings);
        
        activeJunk.Add(junk);
    }

    private bool IsTooCloseToOtherJunk(Vector3 _position)
    {
        foreach (GameObject junk in activeJunk.ToList())
        {
            if (junk == null) continue;
            
            if (Vector3.Distance(_position, junk.transform.position) < minJunkDistance)
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
            
            Vector3 junkPosition = activeJunk[i].transform.position;
            float distanceToPlayer = Vector3.Distance(junkPosition, player.position);
            
            // 거리가 너무 멀거나
            bool isTooFar = distanceToPlayer > despawnRadius;
            
            // 플레이어의 뒤쪽에 있고 일정 거리 이상인 경우 제거
            Vector3 directionToJunk = (junkPosition - player.position).normalized;
            float dotProduct = Vector3.Dot(player.forward, directionToJunk);
            bool isBehindPlayer = dotProduct < -0.5f && distanceToPlayer > visibleRadius;
            
            if (isTooFar || isBehindPlayer)
            {
                if (Debug.isDebugBuild)
                {
                    string reason = isTooFar ? "too far" : "behind player";
                    Debug.Log($"Destroying junk: {reason}");
                }
                
                Destroy(activeJunk[i]);
                activeJunk.RemoveAt(i);
            }
        }
    }
} 