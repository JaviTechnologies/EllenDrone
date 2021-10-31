using UnityEngine;

namespace EllenDrone
{
    /// <summary>
    /// It references the position (transform) where the drone should hover
    /// </summary>
    public class PlayerDroneTarget : MonoBehaviour
    {
        public Transform droneTarget;

        public Vector3 DroneTargetPosition { get { return droneTarget.position; } }
    }
}

