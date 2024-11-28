using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private Dictionary<EEffectType, Queue<GameObject>> effectPools;

    private void Awake()
    {
        InitEffectPools();
    }

    private void InitEffectPools()
    {
        effectPools = new Dictionary<EEffectType, Queue<GameObject>>();

        foreach (var effectData in effectList)
        {
            var pool = new Queue<GameObject>();
            for (int i = 0; i < effectData.poolSize; ++i)
            {
                var effect = CreateNewEffect(effectData.prefab);
                pool.Enqueue(effect);
            }
            effectPools[effectData.type] = pool;
        }
    }

    private GameObject CreateNewEffect(GameObject _prefab)
    {
        var effect = Instantiate(_prefab);
        effect.SetActive(false);
        effect.transform.SetParent(transform);  // EffectManager의 자식으로 생성
        return effect;
    }



    public void PlayEffect(EEffectType _type, Vector3 _position, float _duration = 2f)
    {
        if (!effectPools.ContainsKey(_type)) return;

        var pool = effectPools[_type];
        if (pool.Count == 0) return;

        var effect = pool.Dequeue();
        effect.transform.position = _position;
        effect.SetActive(true);

        StartCoroutine(ReturnToPool(effect, _type, _duration));
    }

    private IEnumerator ReturnToPool(GameObject _effect, EEffectType _type, float _duration)
    {
        yield return new WaitForSeconds(_duration);
        _effect.SetActive(false);
        effectPools[_type].Enqueue(_effect);
    }
}
