using SmartwallPackage.SmartwallInput;
using UnityEngine;
using UnityEngine.Events;

namespace Timeline
{
	public class TimelineHitZone : MonoBehaviour, I_SmartwallInteractable
	{
		public UnityEvent onHit = new UnityEvent();

		public void Hit(Vector3 hitPosition)
		{
			onHit.Invoke();
		}
	}
}