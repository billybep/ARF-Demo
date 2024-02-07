using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] public float distanceFromCamera = 1.2f; // Sesuaikan jarak sesuai kebutuhan

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
        }
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            // Mendapatkan posisi kamera
            Vector3 cameraPosition = mainCamera.transform.position;

            // Mengatur posisi objek canvas UI sedikit di depan kamera berdasarkan jarak tertentu
            transform.position = cameraPosition + mainCamera.transform.forward * distanceFromCamera;

            // Menyamakan rotasi objek canvas UI dengan rotasi kamera
            Quaternion cameraRotation = Quaternion.LookRotation(mainCamera.transform.forward, mainCamera.transform.up);
            transform.rotation = cameraRotation;
        }
    }
}
