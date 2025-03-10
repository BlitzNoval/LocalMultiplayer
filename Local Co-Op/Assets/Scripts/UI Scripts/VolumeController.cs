using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    private Slider volumeSlider;

    void Start()
    {
        volumeSlider = GetComponent<Slider>();
        volumeSlider.value = AudioManager.Instance.GetComponent<AudioSource>().volume;
        volumeSlider.onValueChanged.AddListener(UpdateVolume);
    }

    public void UpdateVolume(float volume)
    {
        AudioManager.Instance.SaveVolumeSetting(volume);
    }
}