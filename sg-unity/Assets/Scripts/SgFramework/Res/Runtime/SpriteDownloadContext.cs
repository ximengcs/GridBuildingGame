using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace SgFramework.Res
{
    public class SpriteDownloadContext : IProgress<float>
    {
        private Sprite _sprite;

        public Sprite Sprite
        {
            get
            {
                if (_sprite != null)
                {
                    return _sprite;
                }

                if (!IsDone)
                {
                    return null;
                }

                var texture = DownloadHandlerTexture.GetContent(_unityWebRequest);
                _sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                return _sprite;
            }
        }

        public float Progress { get; private set; }

        public string Url { get; private set; }

        public bool IsDone => _webOperation.isDone;

        private readonly UnityWebRequestAsyncOperation _webOperation;
        private readonly UnityWebRequest _unityWebRequest;
        private readonly UniTask _downloadTask;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Action<float> _progressCallback;

        public SpriteDownloadContext(string url)
        {
            Url = url;
            _unityWebRequest = UnityWebRequestTexture.GetTexture(url);
            _webOperation = _unityWebRequest.SendWebRequest();
            _downloadTask = _webOperation.ToUniTask(this, cancellationToken: _cancellationTokenSource.Token);
        }

        public UniTask WaitForCompletion(Action<float> onProgress = null)
        {
            _progressCallback = onProgress;
            return _downloadTask;
        }

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
        }

        public void Report(float value)
        {
            Progress = value;
            _progressCallback?.Invoke(value);
        }

        public void Release()
        {
            if (Sprite)
            {
                UnityEngine.Object.Destroy(Sprite.texture);
            }
            else
            {
                Cancel();
            }
        }
    }
}