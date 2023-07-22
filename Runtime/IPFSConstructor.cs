using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Pixygon.IPFS {
    public class IpfsConstructor : MonoBehaviour {
        [SerializeField] private GameObject _imagePrefab;
        [SerializeField] private GameObject _gifPrefab;
        [SerializeField] private GameObject _videoPrefab;
        
        private GameObject _currentObject;
        
        public async Task<GameObject> ConstructIpfsObject(string template, bool thumbnail = false) {
            ClearIpfs();
            var ipfs = await IpfsBridge.GetIpfsFile<Object>(template, thumbnail);
            if(this == null) return null;
            switch(ipfs) {
                case VideoData data:
                    _currentObject = Instantiate(_videoPrefab, transform);
                    _currentObject.GetComponent<IPFSVideo>().PlayVideo(data._url);
                break;
                case Sprite sprite:
                    _currentObject = Instantiate(_imagePrefab, transform);
                    _currentObject.GetComponent<Image>().sprite = sprite;
                    _currentObject.GetComponent<Image>().color = Color.white;
                    _currentObject.GetComponent<Image>().preserveAspect = true;
                break;
                case Gif gif:
                    _currentObject = Instantiate(_gifPrefab, transform);
                    _currentObject.GetComponent<IPFSGif>().PlayGif(gif);
                break;
                case ErrorData error:
                Debug.Log("Ipfs-error: " + error._error);
                return null;
                default:
                Debug.Log("Something went wrong... " + ipfs.GetType());
                return null;
            }
            return _currentObject;
        }

        public void ClearIpfs() {
            if (_currentObject.GetComponent<Image>() != null)
                Destroy(_currentObject.GetComponent<Image>().sprite);
            Destroy(_currentObject);
        }

        private void OnDestroy() {
            ClearIpfs();
        }
    }
}