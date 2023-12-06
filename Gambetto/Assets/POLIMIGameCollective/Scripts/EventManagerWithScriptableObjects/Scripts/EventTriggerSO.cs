using POLIMIGameCollective.Scripts.EventManagerWithScriptableObjects.ScriptableObjects;
using UnityEngine;

namespace POLIMIGameCollective.Scripts.EventManagerWithScriptableObjects.Scripts
{
	public class EventTriggerSO : MonoBehaviour
	{
		[SerializeField] private VoidEventChannelSO spawnEvent;
		[SerializeField] private VoidEventChannelSO explodeEvent;
		[SerializeField] private VoidEventChannelSO runawayEvent;
	
		// Update is called once per frame
		void Update () {
			if (Input.GetKeyDown(KeyCode.E))
			{
				Debug.Log("Triggering Explode");
				// EventManager.TriggerEvent("Explode");
				explodeEvent.RaiseEvent();
			}

			if (Input.GetKeyDown(KeyCode.R))
			{
				// EventManager.TriggerEvent ("RunAway");			
				runawayEvent.RaiseEvent();
			}

			if (Input.GetKeyDown(KeyCode.S))
			{
				// EventManager.TriggerEvent("Spawn");
				spawnEvent.RaiseEvent();
			}
		}
	}
}
