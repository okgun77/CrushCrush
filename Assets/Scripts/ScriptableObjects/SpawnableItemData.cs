using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "SpawnableItemData", menuName = "Game/Spawnable Item Data")]
public class SpawnableItemData : ScriptableObject
{
    public List<SpawnableItem> items = new List<SpawnableItem>();

    [Header("Drag & Drop Zone")]
    [SerializeField] private Object[] draggedPrefabs;

    private void OnValidate()
    {
        // 드래그된 프리팹들 처리
        if (draggedPrefabs != null && draggedPrefabs.Length > 0)
        {
            foreach (var obj in draggedPrefabs)
            {
                if (obj is GameObject prefab)
                {
                    // 이미 등록된 프리팹인지 확인
                    bool alreadyExists = items.Exists(item => item.prefab == prefab);
                    if (!alreadyExists)
                    {
                        var objectProperties = prefab.GetComponent<ObjectProperties>();
                        if (objectProperties != null)
                        {
                            items.Add(new SpawnableItem
                            {
                                prefab = prefab,
                                spawnWeight = 1f,
                                objectType = objectProperties.ObjectType
                            });
                        }
                        else
                        {
                            Debug.LogWarning($"ObjectProperties component not found on prefab: {prefab.name}");
                        }
                    }
                }
            }
            // 드래그된 프리팹 배열 초기화
            draggedPrefabs = new Object[0];
        }

        // 기존 아이템들의 ObjectType 업데이트
        foreach (var item in items)
        {
            if (item.prefab != null)
            {
                var objectProperties = item.prefab.GetComponent<ObjectProperties>();
                if (objectProperties != null)
                {
                    item.objectType = objectProperties.ObjectType;
                }
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SpawnableItemData))]
    public class SpawnableItemDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            SpawnableItemData data = (SpawnableItemData)target;

            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("여러 프리팹을 이곳에 드래그 & 드롭하세요.", MessageType.Info);
            EditorGUILayout.Space(5);

            // 기본 인스펙터 그리기
            DrawDefaultInspector();

            // 삭제 버튼
            EditorGUILayout.Space(10);
            if (GUILayout.Button("모든 프리팹 제거"))
            {
                if (EditorUtility.DisplayDialog("확인", 
                    "모든 프리팹을 제거하시겠습니까?", 
                    "예", 
                    "아니오"))
                {
                    data.items.Clear();
                    EditorUtility.SetDirty(data);
                }
            }
        }
    }
#endif
}

[System.Serializable]
public class SpawnableItem
{
    public GameObject prefab;
    [Range(0f, 100f)]
    public float spawnWeight = 1f;
    [ReadOnly]
    public EObjectType objectType;
}

// ReadOnly 속성 구현
public class ReadOnlyAttribute : PropertyAttribute { }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
#endif 