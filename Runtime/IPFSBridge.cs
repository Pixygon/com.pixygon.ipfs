using System.Collections.Generic;
using System.Threading.Tasks;
using ThreeDISevenZeroR.UnityGifDecoder;
using UnityEngine;
using UnityEngine.Networking;
using Pixygon.DebugTool;
using UnityEngine.UI;
using WebP;
using WebP.Experiment.Animation;

namespace Pixygon.IPFS {
    public class IpfsBridge : MonoBehaviour {
        //private const string IpfsUrl = "https://ipfs.atomichub.io/ipfs/";
        private const string IpfsUrl = "https://atomichub-ipfs.com/ipfs/";
        private const string IpfsThumbnailUrl = "https://ipfs.hivebp.io/thumbnail?hash=";

        public static async Task<T> GetIpfsFile<T>(string hash, bool thumbnail = false) where T : Object {
            if (string.IsNullOrEmpty(hash)) {
                return new ErrorData("Hash is null!") as T;
            }
            if (hash.Contains("http")) {
                var split = hash.Split('/');
                hash = split[split.Length-1];
            }
            var www = UnityWebRequest.Get($"{(thumbnail ? IpfsThumbnailUrl : IpfsUrl)}{hash}");
            www.SendWebRequest();
            while(!www.isDone) await Task.Yield();
            if(www.error != null) {
                Log.DebugMessage(DebugGroup.Nft, $"{www.error}   {IpfsUrl}{hash}");
                return new ErrorData($"{www.error}   {IpfsUrl}{hash}") as T;
            }
            switch(www.GetResponseHeader("Content-Type")) {
                case "image/gif":
                return await GetGif(www.downloadHandler.data) as T;
                case "image/png":
                case "image/jpg":
                case "image/jpeg":
                return GetSprite(www.downloadHandler.data) as T;
                case "image/webp":
                return LoadWebP(www.downloadHandler.data) as T;
                case "video/mp4":
                case "video/quicktime":
                return new VideoData(www.url) as T;
                default:
                return new ErrorData($"Missing type: {www.GetResponseHeader("Content-Type")}") as T;
            }
        }

        private static Sprite GetSprite(byte[] bytes) {
            var t = new Texture2D(1, 1);
            t.LoadImage(bytes);
            return Sprite.Create(t, new Rect(0f, 0f, t.width, t.height), new Vector2(.5f, .5f));
        }

        private static async Task<Gif> GetGif(byte[] bytes) {
            var gifs = new List<GifData>();
            using(var gifStream = new GifStream(bytes)) {
                while(gifStream.HasMoreData) {
                    switch(gifStream.CurrentToken) {
                        case GifStream.Token.Image:
                        var frame = new Texture2D(gifStream.Header.width, gifStream.Header.height, TextureFormat.ARGB32, false);
                        var image = gifStream.ReadImage();
                        frame.SetPixels32(image.colors);
                        frame.Apply();
                        gifs.Add(new GifData(Sprite.Create(frame, new Rect(0f, 0f, gifStream.Header.width, gifStream.Header.height), new Vector2(.5f, .5f)), image.SafeDelaySeconds));
                        break;
                        case GifStream.Token.Comment:
                        var commentText = gifStream.ReadComment();
                        Debug.Log(commentText);
                        break;
                        default:
                        gifStream.SkipToken(); // Other tokens
                        break;
                    }
                }
            }
            return new Gif(gifs);
        }
        private static async Task<Sprite> LoadWebP(byte[] bytes)
        {
            var t = Texture2DExt.CreateTexture2DFromWebP(bytes, lMipmaps: true, lLinear: true, lError: out Error lError);
            Debug.Log("Generated with WebP!");
            if (lError == Error.Success)
                return Sprite.Create(t, new Rect(0f, 0f, t.width, t.height), new Vector2(.5f, .5f));
            Debug.LogError("Webp Load Error : " + lError);
            return null;
            
            //WebPRendererWrapper<Texture2D> t = await WebP.Experiment.Animation.WebP.LoadTexturesAsync(bytes);
            //t.
            
            return Sprite.Create(t, new Rect(0f, 0f, t.width, t.height), new Vector2(.5f, .5f));
        }
    }

    public class VideoData : Object {
        public string _url;
        public VideoData(string s) {
            _url = s;
        }
    }

    public class SpriteData : Object {
        public Sprite _sprite;
        public SpriteData(Sprite s) {
            _sprite = s;
        }
    }

    public class ErrorData : Object {
        public string _error;
        public ErrorData(string s) {
            _error = s;
        }
    }
}
