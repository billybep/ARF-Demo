using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1.0f;

    void Start()
    {
        LeanTween.alpha(fadeImage.rectTransform, 0.0f, fadeDuration)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() => fadeImage.gameObject.SetActive(false));
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeOut(sceneName));
    }

    IEnumerator FadeOut(string sceneName)
    {
        fadeImage.gameObject.SetActive(true);
        LeanTween.alpha(fadeImage.rectTransform, 1.0f, fadeDuration)
            .setEase(LeanTweenType.easeInQuad)
            .setOnComplete(() => LoadNextScene(sceneName));

        yield return new WaitForSeconds(fadeDuration);
    }

    void LoadNextScene(string sceneName)
    {
        // Jika berada di scene ARScene, atur transparansi gambar
        if (sceneName == "ARScene")
        {
            SetImageTransparency(fadeImage, 0.5f); // Ganti nilai transparansi sesuai kebutuhan
        }

        SceneManager.LoadScene(sceneName);
    }

    // Metode untuk mengatur transparansi gambar
    void SetImageTransparency(Image image, float alpha)
    {
        if (image != null)
        {
            Color imageColor = image.color;
            imageColor.a = alpha;
            image.color = imageColor;
        }
    }
}
