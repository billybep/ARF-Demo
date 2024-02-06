using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]

public class ARObjectManager : MonoBehaviour
{
    public GameObject helicopter;

    public AudioClip resetSound; // Suara reset
    public AudioSource audioSource; // AudioSource untuk memutar suara


    public float animationDuration = 0.8f;
    public Vector3 targetScale = new Vector3(0.02f, 0.02f, 0.02f);

    public float rotationSpeed = 50f;
    public float pinchScaleFactor = 0.001f; // Adjust pinch sensitivity
    public float maxScale = 1.2f; // Upper scale limit
    public float minScale = 0.01f; // Lower scale limit

    private GameObject spawnObject;
    private ARRaycastManager _arRaycastManager;
    private Vector2 touchPosition;

    private Vector3 initialPosition; // Store the initial position of the helicopter
    private Quaternion initialRotation; // Store the initial rotation of the helicopter
    private Vector3 initialScale; // Store the initial scale of the helicopter

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }

    void Update()
    {
        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        if (_arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;

            if (spawnObject == null)
            {
                spawnObject = Instantiate(helicopter, hitPose.position, hitPose.rotation);
                
                LeanTween
                    .scale(spawnObject, targetScale, animationDuration)
                    .setEase(LeanTweenType.easeOutElastic);
                
                // Store the initial position, rotation, and scale of the helicopter
                initialPosition = spawnObject.transform.position;
                initialRotation = spawnObject.transform.rotation;
                initialScale = spawnObject.transform.localScale;
            }
            else
            {
                HandleTouchInput();
            }

            spawnObject.transform.position = hitPose.position;
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            // 1 finger drag: Rotate the helicopter
            RotateHelicopter();
        }
        else if (Input.touchCount == 2)
        {
            // 2 fingers pinch: Scale the helicopter
            ScaleHelicopter();
        }
    }

    void RotateHelicopter()
    {
        float rotationX = Input.GetTouch(0).deltaPosition.x * rotationSpeed * Time.deltaTime;
        spawnObject.transform.Rotate(Vector3.up, -rotationX);
    }

    void ScaleHelicopter()
    {
        Touch touch0 = Input.GetTouch(0);
        Touch touch1 = Input.GetTouch(1);

        Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
        Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

        float prevTouchDeltaMag = (touch0PrevPos - touch1PrevPos).magnitude;
        float touchDeltaMag = (touch0.position - touch1.position).magnitude;

        float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        // Adjust scale based on pinch gesture with sensitivity
        Vector3 newScale = spawnObject.transform.localScale - Vector3.one * deltaMagnitudeDiff * pinchScaleFactor;
        newScale = Vector3.ClampMagnitude(newScale, maxScale); // Limit upper scale
        newScale = Vector3.Max(newScale, Vector3.one * minScale); // Ensure minimum scale

        // Use LeanTween to smoothly interpolate the scale
        LeanTween
            .scale(spawnObject, newScale, 0.3f)
            .setEase(LeanTweenType.linear);
    }

    // Metode untuk mereset objek ke posisi, rotasi, dan skala default
    public void ResetObject()
    {
        Debug.Log("ResetButton");
        
        if (spawnObject != null)
        {
            // Play reset sound fx
            if (audioSource != null && resetSound != null)
            {
                audioSource.PlayOneShot(resetSound);
                Debug.Log("Play FX");
            }

            // Return the helicopter to its initial position, rotation, and scale
            LeanTween
                .move(spawnObject, initialPosition, animationDuration)
                .setEase(LeanTweenType.easeOutElastic);
            LeanTween
                .rotate(spawnObject, initialRotation.eulerAngles, animationDuration)
                .setEase(LeanTweenType.easeOutElastic);
            LeanTween
                .scale(spawnObject, initialScale, animationDuration)
                .setEase(LeanTweenType.easeOutElastic);
        }
    }
}