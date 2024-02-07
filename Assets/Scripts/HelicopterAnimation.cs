using UnityEngine;
using System.Collections;

public class HelicopterAnimation : MonoBehaviour
{
    public GameObject propellerB;
    public GameObject propellerS;
    public GameObject helicopterBody;

    public float rotationAmount = 360f; // Jumlah rotasi dalam derajat
    public float rotationDuration = 1.0f; // Durasi rotasi dalam detik

    public void StartPropellerAnimation(GameObject spawnObject, Vector3 initialPosition)
    {
        Debug.Log("BALING2");
        // // Memulai animasi rotasi menggunakan LeanTween dengan setLoopClamp
        LeanTween
            .rotateAround(propellerB, Vector3.up, rotationAmount, rotationDuration)
            .setLoopClamp()
            .setEase(LeanTweenType.linear);

        LeanTween
            .rotateAroundLocal(propellerS, Vector3.forward, rotationAmount, rotationDuration)
            .setLoopClamp()
            .setEase(LeanTweenType.linear);

        // Start propeller animation here
        Debug.Log("START ANIMATION FLY");
        // Animasikan helikopter dari posisi sekarang
        Vector3 takeoffPosition = new Vector3(spawnObject.transform.position.x, spawnObject.transform.position.y + 2f, spawnObject.transform.position.z);

        // Animasikan helikopter dari posisi sekarang ke posisi lepas landas
        LeanTween.move(spawnObject, takeoffPosition, 2f)
            .setEase(LeanTweenType.linear)
            .setOnComplete(() =>
            {
                Debug.Log("START ANIMATION MENDARAT");
                // Setelah animasi naik, terbang di sekitar selama beberapa detik
                LeanTween.rotateAround(spawnObject, Vector3.up, 360f, 2f)
                    .setEase(LeanTweenType.linear)
                    .setOnComplete(() =>
                    {
                        // Setelah terbang di sekitar, kembali mendarat di tempat awal
                        LeanTween.move(spawnObject, initialPosition, 2f)
                            .setEase(LeanTweenType.linear)
                            .setOnComplete(() =>
                            {
                                Debug.Log("SROOPPPP");
                                // Ketika helikopter sudah mendarat, hentikan animasi propeller
                                StopPropellerAnimation();
                            });
                    });
            }); 
    }

    public void StopPropellerAnimation()
    {
        // Hentikan animasi putaran
        LeanTween.cancel(propellerB);
        LeanTween.cancel(propellerS);
    }
}
