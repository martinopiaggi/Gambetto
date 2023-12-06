using UnityEngine;
using UnityEngine.Events;

namespace POLIMIGameCollective.Scripts.EventManagerWithScriptableObjects.ScriptableObjects
{
	/// <summary>
	/// This class is used for Events that have no arguments (Example: Exit game event)
	/// </summary>

	[CreateAssetMenu(menuName = "Events/Void Event Channel")]
	public class VoidEventChannelSO : ScriptableObject
	{
		public UnityAction OnEventRaised;

		public void RaiseEvent()
		{
			if (OnEventRaised != null)
				OnEventRaised.Invoke();
			else
			{
				Debug.LogWarning("Void event has been raised but there is no UnityAction associated.");
			}
		}
	}
}