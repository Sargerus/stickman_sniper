using DWTools;
using System;
using UniRx;
using UnityEngine;

public interface IPooledSound : IPooledItem<IPooledSound>
{
    GameObject gameObject { get; }
    AudioSource AudioSource { get; }
    void Play(AudioClip clip);
    void Stop();
}

[RequireComponent(typeof(AudioSource))]
public class SoundPooledItem : MonoBehaviour, IPooledSound
{
    private AudioSource _audioSource;
    private IDisposable _removeSub;

    public IPooledSound Item => this;
    public IPool<IPooledSound> Pool { get; set; }
    public AudioSource AudioSource => _audioSource;

    private void OnEnable()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip)
    {
        AudioSource.PlayOneShot(clip);

        _removeSub = Observable.Timer(TimeSpan.FromSeconds(clip.length)).Subscribe(_ => ReturnToPool());
    }

    public void ReturnToPool()
    {
        AudioSource.Stop();
        _removeSub?.Dispose();
        gameObject.SetActive(false);
        Pool.Add(Item);
    }

    public void Stop()
    {
        AudioSource.Stop();
    }
}
