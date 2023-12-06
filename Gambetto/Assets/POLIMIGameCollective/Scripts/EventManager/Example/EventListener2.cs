using UnityEngine;

namespace POLIMIGameCollective.Scripts.EventManager.Example
{
	public class EventListener2 : MonoBehaviour {

		void OnEnable() {
			EventManager.StartListening ("Spawn", Spawn);
		}

		void OnDisable() {
			EventManager.StopListening ("Spawn", Spawn);
		}

		// Update is called once per frame
		void Spawn () {
			EventManager.StopListening ("Spawn", Spawn);
			Debug.Log("SPAWN EVENT");
			EventManager.StartListening ("Spawn", Spawn);
		}
	}
}
