using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EffectManager : MonoBehaviour
{
    [System.Serializable]
    public class EffectData
    {
        public EEffectType type;
        public GameObject prefab;
        public int poolSize = 10;
    }

    [SerializeField] private List<EffectData> effectList;
    private Dictionary<(EEffectType, GameObject), Queue<GameObject>> effectPools;

    private void Awake()
    {
        InitEffectPools();
    }

    private void InitEffectPools()
    {
        effectPools = new Dictionary<(EEffectType, GameObject), Queue<GameObject>>();

        foreach (var effectData in effectList)
        {
            var pool = new Queue<GameObject>();
            for (int i = 0; i < effectData.poolSize; ++i)
            {
                var effect = CreateNewEffect(effectData.prefab);
                pool.Enqueue(effect);
            }
            effectPools[(effectData.type, effectData.prefab)] = pool;
        }
    }

    private GameObject CreateNewEffect(GameObject _prefab)
    {
        var effect = Instantiate(_prefab);
        effect.SetActive(false);
        effect.transform.SetParent(transform);

        var spriteRenderer = effect.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingLayerName = "Effects";
            spriteRenderer.sortingOrder = 9000;
        }
        
        var particleSystem = effect.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            var particleRenderer = particleSystem.GetComponent<ParticleSystemRenderer>();
            if (particleRenderer != null)
            {
                particleRenderer.sortingLayerName = "Effects";
                particleRenderer.sortingOrder = 9000;
            }
        }

        Vector3 pos = effect.transform.position;
        pos.z = -1f;
        effect.transform.position = pos;

        return effect;
    }

    public void PlayEffect(EEffectType _type, Vector3 _position, float _duration = 2f)
    {
        var effect = effectList.FirstOrDefault(x => x.type == _type);
        if (effect == null) return;

        var poolKey = (_type, effect.prefab);
        if (!effectPools.ContainsKey(poolKey)) return;

        var pool = effectPools[poolKey];
        if (pool.Count == 0) return;

        var effectObj = pool.Dequeue();
        if (effectObj != null)
        {
            effectObj.transform.position = _position;
            effectObj.SetActive(true);
            StartCoroutine(ReturnToPool(effectObj, poolKey, _duration));
        }
    }

    private IEnumerator ReturnToPool(GameObject _effect, (EEffectType type, GameObject prefab) _poolKey, float _duration)
    {
        yield return new WaitForSeconds(_duration);
        
        if (_effect != null && effectPools.ContainsKey(_poolKey))
        {
            _effect.SetActive(false);
            effectPools[_poolKey].Enqueue(_effect);
        }
    }

    public void PlayRandomEffect(EEffectType _type, Vector3 _position, float _duration = 0.5f, Transform _target = null)
    {
        var effects = effectList.Where(x => x.type == _type).ToList();
        if (effects.Count == 0) return;

        int randomIndex = Random.Range(0, effects.Count);
        var selectedEffect = effects[randomIndex];

        var poolKey = (_type, selectedEffect.prefab);
        if (!effectPools.ContainsKey(poolKey)) return;

        var pool = effectPools[poolKey];
        if (pool.Count == 0) return;

        var effect = pool.Dequeue();
        if (effect != null)
        {
            effect.transform.position = _position;
            effect.SetActive(true);

            if (_type == EEffectType.BREAK)
            {
                var particleSystem = effect.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    StartCoroutine(ReturnToPoolAfterParticleComplete(effect, poolKey));
                    return;
                }
            }

            StartCoroutine(ReturnToPool(effect, poolKey, _duration, _target));
        }
    }

    private IEnumerator ReturnToPool(GameObject _effect, (EEffectType type, GameObject prefab) _poolKey, float _duration, Transform _target)
    {
        float elapsedTime = 0f;
        Vector3 hitPosition = _effect.transform.position; // 초기 타격 위치 저장
        
        while (elapsedTime < _duration)
        {
            if (_effect != null && _target != null)
            {
                // 타겟의 현재 위치로 x, y 업데이트하되
                Vector3 newPosition = _target.position;
                // 원래 타격 위치와의 상대적인 위치 유지
                newPosition.x = _target.position.x + (hitPosition.x - _target.position.x);
                newPosition.y = _target.position.y + (hitPosition.y - _target.position.y);
                // z값은 타겟보다 살짝 앞에 보이도록
                newPosition.z = _target.position.z - 0.1f;  // 타겟보다 0.1f 앞에 위치
                
                _effect.transform.position = newPosition;
            }
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        if (_effect != null && effectPools.ContainsKey(_poolKey))
        {
            _effect.SetActive(false);
            effectPools[_poolKey].Enqueue(_effect);
        }
    }

    private IEnumerator ReturnToPoolAfterParticleComplete(GameObject _effect, (EEffectType type, GameObject prefab) _poolKey)
    {
        var particleSystem = _effect.GetComponent<ParticleSystem>();
        
        while (_effect != null && _effect.activeInHierarchy && particleSystem != null && particleSystem.IsAlive(true))
        {
            yield return null;
        }
        
        if (_effect != null && effectPools.ContainsKey(_poolKey))
        {
            _effect.SetActive(false);
            effectPools[_poolKey].Enqueue(_effect);
        }
    }

    // 특정 타입의 이펙트 개수를 반환하는 메서드 추가
    public int GetEffectCount(EEffectType _type)
    {
        return effectList.Count(x => x.type == _type);
    }
}

