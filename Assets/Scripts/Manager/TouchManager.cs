using UnityEngine;
using System.Collections.Generic;

public class TouchManager : MonoBehaviour
{
    [SerializeField] private float sphereCastRadius = 0.5f; // SphereCast 반지름
    private List<BreakableObject> breakableObjects = new List<BreakableObject>();

    private void Update()
    {
        ScreenTouch();
    }

    private void ScreenTouch()
    {
        bool inputDetected = false;
        Vector3 inputPosition = Vector3.zero;

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    inputDetected = true;
                    inputPosition = touch.position;
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                inputDetected = true;
                inputPosition = Input.mousePosition;
            }
        }

        if (inputDetected)
        {
            Ray ray = Camera.main.ScreenPointToRay(inputPosition);
            RaycastHit hit;
            if (Physics.SphereCast(ray, sphereCastRadius, out hit))
            {
                BreakableObject breakableObject = hit.collider.GetComponent<BreakableObject>();
                if (breakableObject != null)
                {
                    breakableObject.OnTouch();
                }
            }
        }
    }

    public void RegisterBreakableObject(BreakableObject breakableObject)
    {
        if (!breakableObjects.Contains(breakableObject))
        {
            breakableObjects.Add(breakableObject);
        }
    }

    public void UnregisterBreakableObject(BreakableObject breakableObject)
    {
        if (breakableObjects.Contains(breakableObject))
        {
            breakableObjects.Remove(breakableObject);
        }
    }
}
