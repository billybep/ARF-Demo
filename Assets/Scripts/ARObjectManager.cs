using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(ARRaycastManager))]

public class ARObjectManager : MonoBehaviour
{
    private HelicopterAnimation helicopterAnimation;
    // [SerializeField] private float spaceB = 2f;
    public GameObject helicopter;
    public GameObject canvasTrivia;

    public AudioClip resetSound; // Suara reset
    public AudioClip btnSound; // Suara reset
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

    private ARSession arSession;

    private void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
        arSession = FindObjectOfType<ARSession>(); // Dapatkan referensi ke ARSession
    }

    private void OnDisable()
    {
        // Pastikan ARSession tidak null dan ARSession aktif sebelum mematikan AR scanning
        if (arSession != null && arSession.enabled)
        {
            arSession.enabled = false;
        }
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

                // Menyesuaikan rotasi agar objek menghadap kamera
                Vector3 lookPos = Camera.main.transform.position - spawnObject.transform.position;
                lookPos.x = 120; // Mengabaikan perubahan rotasi pada sumbu y
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                spawnObject.transform.rotation = rotation;

                helicopterAnimation = spawnObject.GetComponent<HelicopterAnimation>();
                if (helicopterAnimation)
                    Debug.Log("FOUND");
                
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
        }
    }

    public Vector3 targetScaleTrivia = new Vector3(0.008f, 0.008f, 0.008f); // Set the target scale
    public float animationDurationTrivia = 1.5f; // Duration of the animation


    // Scale up animation coroutine
    IEnumerator ScaleUpAnimation(GameObject target, Vector3 targetScale, float duration)
    {
        float timer = 0f;
        // Vector3 initialScale = target.transform.localScale;
        targetScale = Vector3.Min(targetScale, new Vector3(0.01f, 0.01f, 0.01f));

        while (timer < duration)
        {
            float t = timer / duration;
            target.transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            timer += Time.deltaTime;
            yield return null;
        }

        target.transform.localScale = targetScale; // Ensure target scale is reached
    }

    // Scale down animation coroutine
    IEnumerator ScaleDownAnimation(GameObject target, Vector3 targetScale, float duration)
    {
        float timer = 0f;
        Vector3 initialScale = target.transform.localScale;

        while (timer < duration)
        {
            float t = timer / duration;
            target.transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            timer += Time.deltaTime;
            yield return null;
        }

        target.transform.localScale = targetScale; // Ensure target scale is reached
    }

    // Scale down animation coroutine, then toggle canvas active state
    IEnumerator ScaleDownThenToggle(GameObject target, Vector3 targetScale, float duration)
    {
        yield return StartCoroutine(ScaleDownAnimation(target, targetScale, duration));
        // After scaling down animation, toggle the canvas active state
        canvasTrivia.SetActive(!canvasTrivia.activeSelf);
    }

    public void showTrivia()
    {
        Debug.Log("Show TRIVIA");
        // Play sound
        if (audioSource != null && btnSound != null)
        {
            audioSource.PlayOneShot(btnSound);
        }

        bool canvasActive = canvasTrivia.activeSelf;

        if (canvasActive)
        {
            // StartCoroutine(ScaleDownAnimation(canvasTrivia, Vector3.zero, animationDurationTrivia));
            StartCoroutine(ScaleDownThenToggle(canvasTrivia, Vector3.zero, animationDurationTrivia));
        }
        else
        {
            canvasTrivia.SetActive(true);
            StartCoroutine(ScaleUpAnimation(canvasTrivia, Vector3.one, animationDurationTrivia));
        }

        // if (canvasActive)
        // {
        //     Debug.Log("Show TRIVIA Active");
        //     canvasTrivia.SetActive(!canvasTrivia.activeSelf);
        //     // Scale up animation
        //     LeanTween.scale(canvasTrivia, targetScaleTrivia, animationDurationTrivia)
        //         .setEase(LeanTweenType.easeOutBack); 
        // } else {
        //     Debug.Log("Show TRIVIA Inactive");
        //     // Scale down animation
        //         LeanTween
        //         .scale(canvasTrivia, new Vector3(0.01f, 0.01f, 0.01f), 2f)
        //         .setEase(LeanTweenType.easeOutElastic)
        //         .setOnComplete(() =>
        //         {
        //             // Set the canvasTrivia to inactive after the animation completes
        //             // canvasTrivia.SetActive(false);
        //             canvasTrivia.SetActive(!canvasTrivia.activeSelf);
        //         });
        // }

        // canvasTrivia.SetActive(!canvasTrivia.activeSelf);
    }

    public float touchThreshold = 0.1f; // Adjust threshold value as needed
    void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Debug.Log("Touchss");
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (_arRaycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    foreach (var hit in hits)
                    {
                        // Check if the hit point is near the position of the spawned helicopter
                        if (spawnObject != null && Vector3.Distance(hit.pose.position, spawnObject.transform.position) < touchThreshold)
                        {
                            // Perform actions when the helicopter is touched
                            Debug.Log("Helicopter is touched!");
                            // You can add further actions here, such as showing trivia, etc.
                            showTrivia();
                        }
                    }
                }
            }

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
        Debug.Log("RotateHelix");
        float rotationX = Input.GetTouch(0).deltaPosition.x * rotationSpeed * Time.deltaTime;
        spawnObject.transform.Rotate(Vector3.up, -rotationX);
    }

    void ScaleHelicopter()
    {
        Debug.Log("ScaleHelix");
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

    public void HeliAnimation1()
    {
        helicopterAnimation.StartPropellerAnimation(spawnObject, initialPosition);
    }

    // void Start()
    // {
    //      // Menginstansiasi prefab dan menyimpan instance baru ke dalam variabel instatiatePrefab
    //         spawnObject = Instantiate(helicopter, transform.position, transform.rotation);

    //             helicopterAnimation = spawnObject.GetComponent<HelicopterAnimation>();
    //             if (helicopterAnimation)
    //                 Debug.Log("FOUND");

    //                 // Menyesuaikan rotasi agar objek menghadap kamera
    //             Vector3 lookPos = Camera.main.transform.position - spawnObject.transform.position;
    //             lookPos.x = 90; // Mengabaikan perubahan rotasi pada sumbu y
    //             Quaternion rotation = Quaternion.LookRotation(lookPos);
    //             spawnObject.transform.rotation = rotation;
                
    //             LeanTween
    //                 .scale(spawnObject, targetScale, animationDuration)
    //                 .setEase(LeanTweenType.easeOutElastic);
                
    //             // Store the initial position, rotation, and scale of the helicopter
    //             initialPosition = spawnObject.transform.position;
    //             initialRotation = spawnObject.transform.rotation;
    //             initialScale = spawnObject.transform.localScale;

    //             Debug.Log("INIT START POS" + initialPosition);
        
    // }
}