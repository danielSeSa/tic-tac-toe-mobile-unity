using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CustomButtonSound : MonoBehaviour
{
    [SerializeField] private AudioClip customClickSFX;

    private void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        if (AudioManager.Instance != null)
        {
            if (customClickSFX != null)
            {
                AudioManager.Instance.PlaySFX(customClickSFX);
            }
            else
            {
                AudioManager.Instance.PlayDefaultClick();
            }
        }
    }
}