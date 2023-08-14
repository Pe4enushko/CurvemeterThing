using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ButtonSound : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.loop = false;
        if (_audioSource.clip == null)
            Debug.LogError("���������� ���� ��� ������");
    }

    public void Play() => _audioSource.Play();
}
