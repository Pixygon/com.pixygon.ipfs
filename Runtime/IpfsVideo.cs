using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Pixygon.Ipfs {
    public class IpfsVideo : MonoBehaviour {
        [SerializeField] private VideoPlayer _video;
        [SerializeField] private RawImage _image;

        private RenderTexture _renderTex;

        public void PlayVideo(string url) {
            _video.url = url;
            _renderTex = new RenderTexture(256, 256, 0);
            _video.targetTexture = _renderTex;
            _image.texture = _renderTex;
            _image.color = Color.white;
        }

        public void OnDestroy() {
            Destroy(_renderTex);
        }
    }
}