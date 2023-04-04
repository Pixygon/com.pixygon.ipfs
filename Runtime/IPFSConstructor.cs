using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Pixygon.IPFS {
    public class IPFSConstructor : MonoBehaviour {
        [SerializeField] private GameObject _imagePrefab;
        [SerializeField] private GameObject _gifPrefab;
        [SerializeField] private GameObject _videoPrefab;
        
        public async Task<GameObject> ConstructIpfsObject(string template) {
            var ipfs = await IpfsBridge.GetIpfsFile<Object>(template);
            GameObject g = null;
            switch(ipfs) {
                case VideoData data:
                g = Instantiate(_videoPrefab, transform);
                g.GetComponent<IPFSVideo>().PlayVideo(data._url);
                break;
                case Sprite sprite:
                g = Instantiate(_imagePrefab, transform);
                g.GetComponent<Image>().sprite = sprite;
                g.GetComponent<Image>().color = Color.white;
                g.GetComponent<Image>().preserveAspect = true;
                break;
                case Gif gif:
                g = Instantiate(_gifPrefab, transform);
                g.GetComponent<IPFSGif>().PlayGif(gif);
                break;
                case null:
                default:
                Debug.Log("Something went wrong... " + ipfs.GetType().ToString());
                break;
            }
            return g;
        }
    }
}