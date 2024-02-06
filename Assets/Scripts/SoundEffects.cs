using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundEffects : MonoBehaviour
{
    public AudioSource src;
    public AudioClip sfxButtonClick;

    public void SfxButtonClick()
    {
        StartCoroutine(PlaySfxAndLoadScene());
    }

    public void StartSFX()
    {
        StartCoroutine(fxSoundGeneral());
    }


    IEnumerator PlaySfxAndLoadScene()
    {
        // Mulai memainkan suara
        src.clip = sfxButtonClick;
        src.Play();

        // Tunggu hingga selesai memainkan suara
        yield return new WaitForSeconds(src.clip.length);

        // Setelah selesai memainkan suara, pindah ke scene berikutnya
        SceneManager.LoadScene("ARScene");
    }

    IEnumerator fxSoundGeneral()
    {
        // Mulai memainkan suara
        src.clip = sfxButtonClick;
        src.Play();

        // Tunggu hingga selesai memainkan suara
        yield return new WaitForSeconds(src.clip.length);
    }
}
