using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
    [SerializeField] private float sphereCastRadius = 0.5f;
    private List<BreakableObject> breakableObjects = new List<BreakableObject>();
    private GameManager gameManager;

    public void Init(GameManager _gameManager)
    {
        gameManager = _gameManager;
    }

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

    public void RegisterBreakableObject(BreakableObject _breakableObject)
    {
        if (!breakableObjects.Contains(_breakableObject))
        {
            breakableObjects.Add(_breakableObject);
        }
    }

    public void UnregisterBreakableObject(BreakableObject _breakableObject)
    {
        if (breakableObjects.Contains(_breakableObject))
        {
            breakableObjects.Remove(_breakableObject);
        }
    }
}
