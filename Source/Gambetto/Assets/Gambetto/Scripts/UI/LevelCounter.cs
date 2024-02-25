using TMPro;
using UnityEngine;

namespace Gambetto.Scripts.UI
{
    public class LevelCounter : MonoBehaviour
    {

        // Start is called before the first frame update
        void Start()
        {
            var levelCount = GameManager.Instance.GetLevelCount();
            var completedCount = GameManager.Instance.GetLevelCount(true);
            
            gameObject.GetComponent<TextMeshProUGUI>().text = "completed " + completedCount
                                                                                 + "/" + levelCount;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
