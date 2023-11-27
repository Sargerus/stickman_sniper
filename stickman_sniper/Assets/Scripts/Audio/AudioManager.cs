using DWTools;
using UnityEngine;
using UnityEngine.Audio;

public interface IAudioManager
{
    IPooledSound GetSource();
    void Mute(bool mute);
}

public class AudioManager : AbstractMonoPool<IPooledSound>, IAudioManager
{
    [SerializeField] private SoundPooledItem _prefabItem;
    [SerializeField] private AudioMixer _mainMixer;

    public IPooledSound GetSource() => Get().Item;

    public override IPooledItem<IPooledSound> CreateItem()
    {
        var go = Instantiate(_prefabItem, transform);
        go.gameObject.SetActive(false);
        go.Pool = this;

        return go.GetComponent<IPooledSound>();
    }

    public override void DeactivateAllItems()
    {
        _trackingList.ForEach(item => item.ReturnToPool());
    }

    protected override void InitializePool()
    {
        for (int i = 0; i < 5; i++)
        {
            var item = CreateItem();
            _pool.Add(item);
            _trackingList.Add(item);
        }
    }

    public void Mute(bool mute)
    {
        float volume = 0;

        if (mute)
            volume = -80;

        _mainMixer.SetFloat("masterVolume", volume);
    }
}
