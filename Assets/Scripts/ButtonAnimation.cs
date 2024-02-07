using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAnimation : MonoBehaviour
{
    public Button button;
    private Vector3 defaultScale;

    void Start()
    {
        // Menambahkan event listener untuk memanggil metode OnButtonClick saat tombol diklik
        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        Debug.Log("ANIMMMMM");
        defaultScale = new Vector3(1f, 1f, 1f);
        // Memutar animasi ketika tombol diklik
        LeanTween
            .scale(button.gameObject, new Vector3(0.7f, 0.7f, 0.7f), 0.2f)
            .setEase(LeanTweenType.easeOutElastic)
            .setOnComplete(() =>
            {
                // Setelah animasi selesai, kembalikan ke ukuran semula
                LeanTween
                    .scale(button.gameObject, defaultScale, 0.25f)
                    .setEase(LeanTweenType.easeOutElastic);
            });
    }
}
