using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BackButtonHandler : MonoBehaviour
{
    private static BackButtonHandler _instance;

    public AudioSource src;
    public AudioClip sfxFadeOut;
    public float fadeOutDuration = 1.0f;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Update()
    {
        #if UNITY_ANDROID
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StartCoroutine(FadeOutAndExit());
            }
        #endif

        #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StartCoroutine(FadeOutAndExit());
            }
        #endif
    }

    IEnumerator FadeOutAndExit()
    {
        // Play fading out sound
        src.clip = sfxFadeOut;
        src.Play();

        // Gradually reduce the volume over fadeOutDuration
        float timer = 0.0f;
        while (timer < fadeOutDuration)
        {
            src.volume = Mathf.Lerp(1.0f, 0.0f, timer / fadeOutDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        // Wait for a short duration for the fading out sound to complete
        yield return new WaitForSeconds(src.clip.length);

        // Exit the application (works on Android)
        Application.Quit();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
