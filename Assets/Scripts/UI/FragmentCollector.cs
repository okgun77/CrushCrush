using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class FragmentCollector : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fragmentCountText;
    [SerializeField] private IconPunchEffect iconPunchEffect;
    
    private int collectedCount = 0;

    public UnityEvent<int> onFragmentCollected;  // 파편 수집 시 이벤트

    private void Awake()
    {
        if (iconPunchEffect == null)
        {
            iconPunchEffect = GetComponent<IconPunchEffect>();
        }
        UpdateCountText();
    }

    public void OnFragmentArrived()
    {
        collectedCount++;
        UpdateCountText();
        iconPunchEffect?.PlayEffect();
        onFragmentCollected?.Invoke(collectedCount);
    }

    private void UpdateCountText()
    {
        if (fragmentCountText != null)
        {
            fragmentCountText.text = collectedCount.ToString();
        }
    }

    // 필요한 경우 카운트 초기화
    public void ResetCount()
    {
        collectedCount = 0;
        UpdateCountText();
    }

    // 현재 수집된 파편 수 반환
    public int GetCollectedCount() => collectedCount;
} 