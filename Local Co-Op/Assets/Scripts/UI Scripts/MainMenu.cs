using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public GameObject controlsPanel;
    public GameObject settingsPanel;

    public void StartGame()
    {
        StartCoroutine(LoadSceneWithDelay(0.3f));
    }

    public void OpenControls()
    {
        StartCoroutine(AnimatePanel(controlsPanel, true));
    }

    public void CloseControls()
    {
        StartCoroutine(AnimatePanel(controlsPanel, false));
    }

    public void OpenSettings()
    {
        StartCoroutine(AnimatePanel(settingsPanel, true));
    }

    public void CloseSettings()
    {
        StartCoroutine(AnimatePanel(settingsPanel, false));
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            Debug.Log("Quit Game");
        #else
            Application.Quit();
        #endif
    }

    public void ButtonClickEffect(GameObject button)
    {
        StartCoroutine(ButtonPressAnimation(button));
    }

    // 🟢 Smooth Scale Animation for UI Panels
    private IEnumerator AnimatePanel(GameObject panel, bool opening)
    {
        float duration = 0.3f;
        float time = 0f;
        Vector3 startScale = opening ? Vector3.zero : Vector3.one;
        Vector3 endScale = opening ? Vector3.one : Vector3.zero;

        if (opening) panel.SetActive(true);

        while (time < duration)
        {
            panel.transform.localScale = Vector3.Lerp(startScale, endScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        panel.transform.localScale = endScale;

        if (!opening) panel.SetActive(false);
    }

    // 🔴 Button Press Effect (Shrink & Expand)
    private IEnumerator ButtonPressAnimation(GameObject button)
    {
        Vector3 originalScale = button.transform.localScale;
        Vector3 smallScale = originalScale * 0.9f;
        float duration = 0.1f;

        button.transform.localScale = smallScale;
        yield return new WaitForSeconds(duration);
        button.transform.localScale = originalScale;
    }

    // ⚫ Smooth Scene Transition Delay
    private IEnumerator LoadSceneWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
