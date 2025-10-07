using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonClickSound : MonoBehaviour
{
    
    void Start()
    {
        Button button = GetComponent<Button>();

        if (SFXManager.instance != null)
        {
            button.onClick.AddListener(SFXManager.instance.PlayUIClick);
        }
        else
        {
            Debug.LogWarning("Kon sfxmanager niet vinden klikgeluid te koppelen ");
        }
    }
}
