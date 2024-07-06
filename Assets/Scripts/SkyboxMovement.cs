using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxMovement : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1.0f;

    private void Update()
    {
        // 시간에 따라 회전
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotationSpeed);
    }
}
