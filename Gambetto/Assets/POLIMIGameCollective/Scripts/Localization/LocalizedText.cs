using UnityEngine;
using UnityEngine.UI;

namespace POLIMIGameCollective.Scripts.Localization
{
	public class LocalizedText : MonoBehaviour
	{
		public string key;
		private Text text;
	
	
		// Use this for initialization
		void Start ()
		{
			text = GetComponent<Text>();
			text.text = LocalizationManager.instance.GetLocalizedValue(key);
		}
	
	}
}
