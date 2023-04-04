using UnityEngine;
using UnityEngine.UI;

namespace Pixygon.IPFS {
    public class IPFSGif : MonoBehaviour {
        [SerializeField] private Image _image;

        private bool gifLoaded;
        private int currentFrame;
        private Gif gif;
        private float delay;

        private void Update() {
            if(!gifLoaded)
                return;
            if(delay > 0f)
                delay -= Time.deltaTime;
            else {
                _image.sprite = gif.Data[currentFrame].sprite;
                if(currentFrame < gif.Data.Count - 1)
                    currentFrame++;
                else
                    currentFrame = 0;
                delay = gif.Data[currentFrame].delay;
            }
        }

        public void PlayGif(Gif g) {
            gif = g;
            _image.color = Color.white;
            _image.preserveAspect = true;
            gifLoaded = true;
        }

        public async void PlayGif(string hash) {
            gif = await IpfsBridge.GetIpfsFile<Gif>(hash);
            _image.preserveAspect = true;
            _image.color = Color.white;
            gifLoaded = true;
        }

        public void OnDestroy() {
            foreach(var s in gif.Data) {
                Destroy(s.sprite);
            }
        }
    }
}