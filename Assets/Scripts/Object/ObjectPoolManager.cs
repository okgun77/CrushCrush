using UnityEngine;
using System.Collections.Generic;
using RayFire;

public class ObjectPoolManager : MonoBehaviour
{
    private static ObjectPoolManager instance;
    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();
    
    public static ObjectPoolManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<ObjectPoolManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("ObjectPoolManager");
                    instance = go.AddComponent<ObjectPoolManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    public GameObject GetObject(GameObject prefab)
    {
        string key = prefab.name;
        
        if (!poolDictionary.ContainsKey(key))
            poolDictionary[key] = new Queue<GameObject>();

        GameObject obj;
        Queue<GameObject> pool = poolDictionary[key];

        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
            obj.SetActive(true);
        }
        else
        {
            obj = Instantiate(prefab);
            obj.name = key;
        }

        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        string key = obj.name;
        
        if (!poolDictionary.ContainsKey(key))
            poolDictionary[key] = new Queue<GameObject>();

        ResetObject(obj);
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        poolDictionary[key].Enqueue(obj);
    }

    private void ResetObject(GameObject obj)
    {
        // RayfireRigid 초기화
        var rayfire = obj.GetComponent<RayfireRigid>();
        if (rayfire != null)
        {
            rayfire.demolitionType = DemolitionType.Runtime;
            rayfire.objectType = ObjectType.Mesh;
            rayfire.simulationType = SimType.Dynamic;
        }

        // Rigidbody 초기화
        var rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // ObjectProperties 초기화
        var props = obj.GetComponent<ObjectProperties>();
        if (props != null)
        {
            props.ResetProperties();
        }

        // 위치와 회전 초기화
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;
    }
} 