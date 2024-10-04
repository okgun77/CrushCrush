using UnityEngine;

public enum EnemyType
{
    Normal,
    Explosive,
    // 필요한 경우 다른 적 유형 추가
}

public class EnemyHealth : HealthBase
{
    
    [Header("Enemy Settings")]
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private int scoreValue = 100;     // 처치시 획득하는 점수

    private ScoreManager scoreManager;
    private AudioManager audioManager;
    // private ExplosionEffect explosionEffect; // 폭발 효과를 위한 컴포넌트 (예시)

    // 프로퍼티
    public EnemyType EnemyType
    {
        get => enemyType;
        set => enemyType = value;
    }

    public int ScoreValue
    {
        get => scoreValue;
        set => scoreValue = value;
    }

    protected override void Awake()
    {
        base.Awake();

        scoreManager = FindAnyObjectByType<ScoreManager>();
        audioManager = FindAnyObjectByType<AudioManager>();

        // 이벤트 구독
        OnDeath += OnEnemyDeath;
    }

    protected override void Die()
    {
        base.Die();

        // 적 사망 시 필요한 로직 구현
        HandleDeathEffect();
        AddScore();
        Destroy(gameObject);
    }

    private void HandleDeathEffect()
    {
        switch (enemyType)
        {
            case EnemyType.Normal:
                // 일반 적일 경우
                break;
            case EnemyType.Explosive:
                // 폭발하는 적일 경우 폭발 효과 실행
                // explosionEffect?.Explode();
                // 주변 오브젝트에 스플래시 데미지 적용 로직 구현
                ApplySplashDamage();
                break;
                // 다른 유형의 적에 대한 처리 추가 가능
        }

        // 사운드 재생
        audioManager?.PlaySFX("EnemyDeathSound");
    }

    private void ApplySplashDamage()
    {
        // 주변 오브젝트에 스플래시 데미지를 적용하는 로직 구현
    }

    private void AddScore()
    {
        scoreManager?.AddScore(scoreValue);
    }

    private void OnEnemyDeath()
    {
        // 적 사망 시 추가 로직 (애니메이션, 사운드 등)
    }
}