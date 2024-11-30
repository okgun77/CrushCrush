using UnityEngine;
using System.Collections.Generic;

public class ParticleSpeedController : MonoBehaviour
{
    [SerializeField]
    private List<ParticleSystem> particleSystems = new List<ParticleSystem>();

    private void Start()
    {
        // 시작할 때 모든 파티클 시스템의 velocityOverLifetime 활성화
        foreach (var ps in particleSystems)
        {
            if (ps != null)
            {
                var velocityModule = ps.velocityOverLifetime;
                velocityModule.enabled = true;
            }
        }
    }

    public void SetParticleSpeed(int _index, float _speed)
    {
        if (IsValidIndex(_index))
        {
            var velocityModule = particleSystems[_index].velocityOverLifetime;
            velocityModule.speedModifier = new ParticleSystem.MinMaxCurve(_speed);
        }
    }

    public void SetAllParticleSpeeds(float _speed)
    {
        foreach (var ps in particleSystems)
        {
            if (ps != null)
            {
                var velocityModule = ps.velocityOverLifetime;
                velocityModule.speedModifier = new ParticleSystem.MinMaxCurve(_speed);
            }
        }
    }

    public float GetParticleSpeed(int _index)
    {
        if (IsValidIndex(_index))
        {
            return particleSystems[_index].velocityOverLifetime.speedModifier.constant;
        }
        return 0f;
    }

    public void MultiplyParticleSpeed(int _index, float _multiplier)
    {
        if (IsValidIndex(_index))
        {
            var velocityModule = particleSystems[_index].velocityOverLifetime;
            float currentSpeed = velocityModule.speedModifier.constant;
            velocityModule.speedModifier = new ParticleSystem.MinMaxCurve(currentSpeed * _multiplier);
        }
    }

    private bool IsValidIndex(int _index)
    {
        return _index >= 0 && _index < particleSystems.Count;
    }
}