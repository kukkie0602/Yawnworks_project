using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ProgressBarManager : MonoBehaviour
{
    public Sprite[] progressBarSprites;
    public int playerCoins = 0;

    [Header("Popup Settings")]
    public GameObject maxCoinsPopup;
    private bool popupShown = false;

    private Image progressBarImage;

    void Start()
    {
        progressBarImage = GetComponent<Image>();
        UpdateProgressBar();
    }

    public void UpdateProgressBar()
    {
        if (progressBarSprites.Length == 0 || progressBarImage == null) return;

        int stage = Mathf.Clamp(playerCoins, 0, progressBarSprites.Length - 1);
        progressBarImage.sprite = progressBarSprites[stage];

        if (playerCoins >= 6 && !popupShown)
        {
            ShowMaxCoinsPopup();
            popupShown = true;
        }

    }

    private void ShowMaxCoinsPopup()
    {
        if (maxCoinsPopup != null)
        {
            maxCoinsPopup.SetActive(true);
        }
    }

    public void OnPopupButtonPressed()
    {
        SceneManager.LoadScene("EndScreen");
    }
}
