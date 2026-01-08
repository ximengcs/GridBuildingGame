using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SgFramework.Res;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SgFramework.Audio
{
    public class AudioManager
    {
        public static AudioManager Instance { get; } = new AudioManager();

        public bool SfxOn { get; set; }

        private bool _bgmOn;

        public bool BgmOn
        {
            get => _bgmOn;
            set
            {
                _bgmOn = value;
                if (value)
                {
                    ResumeBgm();
                }
                else
                {
                    StopBgm();
                }
            }
        }

        public float SfxVolume { get; set; }

        private float _bgmVolume;

        public float BgmVolume
        {
            get => _bgmVolume;
            set
            {
                _bgmVolume = value;
                if (_bgmSource == null)
                {
                    return;
                }

                _bgmSource.volume = _bgmVolume;
            }
        }

        private ResourceGroup _group;
        private Transform _playPlace;
        private readonly Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();
        private readonly Queue<AudioSource> _audioSources = new Queue<AudioSource>();

        private AudioSource _bgmSource;
        private string _lastBgm;

        private AudioManager()
        {
        }

        public void Initialize()
        {
            _group = ResourceManager.GetGroup("Audio");

            var go = new GameObject("[AudioManager]");
            Object.DontDestroyOnLoad(go);
            _playPlace = go.transform;
        }

        public void Dispose()
        {
            SfxOn = false;
            BgmOn = false;
            _audioClips.Clear();
            _audioSources.Clear();
            _lastBgm = "";

            Object.Destroy(_playPlace.gameObject);
            ResourceManager.ReleaseGroup(_group);
            _group = null;
        }

        public async void PlaySfx(string key)
        {
            try
            {
                if (string.IsNullOrEmpty(key) || !SfxOn)
                {
                    return;
                }

                if (!_audioClips.TryGetValue(key, out var clip))
                {
                    var resKey = $"Assets/GameRes/Audio/{key}.mp3";
                    clip = await _group.LoadAssetAsync<AudioClip>(resKey);
                    _audioClips[key] = clip;
                }

                var source = GetSource();
                source.name = key;
                source.clip = clip;
                source.volume = SfxVolume;
                source.loop = false;
                source.Play();
                await UniTask.WaitForSeconds(clip.length);
                ReleaseSource(source);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }

        public async void PlayBgm(string key)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    return;
                }

                if (!_audioClips.TryGetValue(key, out var clip))
                {
                    var resKey = $"Assets/GameRes/Audio/{key}.mp3";
                    clip = await _group.LoadAssetAsync<AudioClip>(resKey);
                    _audioClips[key] = clip;
                }

                if (_bgmSource?.name == key)
                {
                    return;
                }

                _lastBgm = key;
                if (!BgmOn)
                {
                    return;
                }

                if (_bgmSource == null)
                {
                    _bgmSource = GetSource();
                }

                _bgmSource.Stop();
                _bgmSource.name = key;
                _bgmSource.volume = BgmVolume;
                _bgmSource.clip = clip;
                _bgmSource.loop = true;
                _bgmSource.Play();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }

        private void ResumeBgm()
        {
            PlayBgm(_lastBgm);
        }

        private void StopBgm()
        {
            if (_bgmSource == null)
            {
                return;
            }

            _bgmSource.Stop();
            ReleaseSource(_bgmSource);
            _bgmSource = null;
        }

        private AudioSource GetSource()
        {
            if (_audioSources.TryDequeue(out var source))
            {
                source.enabled = true;
                return source;
            }

            source = new GameObject("AudioSource").AddComponent<AudioSource>();
            source.transform.SetParent(_playPlace, false);
            return source;
        }

        private void ReleaseSource(AudioSource source)
        {
            _audioSources.Enqueue(source);
            source.enabled = false;
        }
    }
}