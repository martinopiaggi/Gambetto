using System.IO;
using UnityEngine;

namespace Gambetto.Scripts.UI
{
    public class Credits : MonoBehaviour
    {
        
        [SerializeField]
        private TextAsset textAsset;
        // Start is called before the first frame update
        void Start()
        {
            var text = textAsset.text;
            var textMesh = gameObject.GetComponent<TMPro.TextMeshProUGUI>();
            textMesh.text = text;
        }
    }
}
