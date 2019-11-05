using UnityEngine;

namespace SmartwallPackage.SmartwallInput
{
    /// <summary>
    /// Implement this interface in a class and it will have the Hit method exacuted when its gameobject is hit by a ball on the smartwall.
    /// </summary>
    public interface I_SmartwallInteractable
    {
        void Hit(Vector3 hitPosition);
    }
}
