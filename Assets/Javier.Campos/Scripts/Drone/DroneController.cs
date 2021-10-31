using Gamekit3D;
using UnityEngine;

namespace EllenDrone
{
    public class DroneController : MonoBehaviour
    {
        /// <summary>
        /// Struct to keep the movement parameters
        /// </summary>
        private struct SmoothDumpParams
        {
            public Vector3 targetPosition;
            public Vector3 velocity;
            public float smoothTime;
        }

        private SmoothDumpParams m_Movement;

        private PlayerController m_Ellen;
        private PlayerDroneTarget m_TargetAnchor; // this has a reference to the idle position (configurable in Ellen prefab)
        private PickupsZone m_PickupsZone;        // it has all the pickups

        public enum DroneState
        {
            Idle,       // hovering close to Ellen
            Fetching,   // fetching when pressing F (manual fetch)
            PickingUp,  // fetching a close pick up (auto fetch)
            MovingUp,   // moving up when player attacks
            Returning   // moving back to Ellen after Fetching
        }

        public DroneState State { get; private set; }

        public float followSmoothTime = 0.3f;
        public float manualFetchDistance = 10f;
        public float manualFetchSmoothTime = 0.5f;
        public float autoFetchDistance = 5f;
        public float autoFetchSmoothTime = 0.5f;
        public float moveUpDistance = 10f;
        public float moveUpSmoothTime = 0.7f;

        private void Awake()
        {
            m_Ellen = FindObjectOfType<PlayerController>();
            m_TargetAnchor = m_Ellen.GetComponent<PlayerDroneTarget>();

            m_PickupsZone = FindObjectOfType<PickupsZone>();

            m_Movement.velocity = Vector3.zero;
            m_Movement.targetPosition = m_TargetAnchor.DroneTargetPosition;
            m_Movement.smoothTime = followSmoothTime;

            State = DroneState.Idle;
        }

        private void Update()
        {
            if(State == DroneState.Idle)
            {
                // check manual fetch
                if(Input.GetKeyDown(KeyCode.F))
                {
                    // params to fetch
                    m_Movement.targetPosition = m_Ellen.transform.position + m_Ellen.transform.forward * manualFetchDistance;
                    m_Movement.targetPosition.y = m_TargetAnchor.DroneTargetPosition.y; // keep height
                    m_Movement.smoothTime = manualFetchSmoothTime;

                    State = DroneState.Fetching;

                    return;
                }

                // check melee attack
                if(PlayerInput.Instance.Attack)
                {
                    // params to move up
                    m_Movement.targetPosition = m_TargetAnchor.DroneTargetPosition + transform.up * moveUpDistance;
                    m_Movement.smoothTime = moveUpSmoothTime;

                    State = DroneState.MovingUp;

                    return;
                }

                // check auto fetch (proximity)
                var pickupInRange = GetPickupInRange();
                if(pickupInRange != null)
                {
                    // params to pick up
                    m_Movement.targetPosition = pickupInRange.transform.position;
                    m_Movement.smoothTime = autoFetchSmoothTime;

                    State = DroneState.PickingUp;

                    return;
                }

                // otherwise, continue following
                m_Movement.targetPosition = m_TargetAnchor.DroneTargetPosition;
                m_Movement.smoothTime = followSmoothTime;
            }
            else if(State == DroneState.Returning)
            {
                // check end condition
                if (Vector3.Distance(m_Movement.targetPosition, transform.position) <= autoFetchDistance / 3)
                {
                    m_Movement.targetPosition = m_TargetAnchor.DroneTargetPosition;
                    m_Movement.smoothTime = followSmoothTime;
                    State = DroneState.Idle;
                }
                else if(PlayerInput.Instance.Attack) // check if player keeps attacking
                {
                    // params to move up
                    m_Movement.targetPosition = m_TargetAnchor.DroneTargetPosition + transform.up * moveUpDistance;
                    m_Movement.smoothTime = moveUpSmoothTime;

                    State = DroneState.MovingUp;
                }
                else // continue moving back
                {
                    m_Movement.targetPosition = m_TargetAnchor.DroneTargetPosition;
                    m_Movement.smoothTime = followSmoothTime;
                }
            }
            else // other states
            {
                // check end condition
                if (Vector3.Distance(m_Movement.targetPosition, transform.position) <= 0.01f)
                {
                    m_Movement.targetPosition = m_TargetAnchor.DroneTargetPosition;
                    State = DroneState.Returning;
                }
            }
        }

        private void FixedUpdate()
        {
            // move smoothly
            transform.position = Vector3.SmoothDamp(transform.position, m_Movement.targetPosition, ref m_Movement.velocity, m_Movement.smoothTime);
        }

        /// <summary>
        /// Gets a pickup in range to fetch
        /// </summary>
        /// <returns>A pickup in range or null</returns>
        private Pickup GetPickupInRange()
        {
            Pickup pickupInRange = null;

            var pickups = m_PickupsZone.Pickups;
            for(int i = 0; i < pickups.Count; i++)
            {
                if (pickups[i].IsDestroying)
                    continue;

                if(Vector3.Distance(pickups[i].transform.position, m_TargetAnchor.DroneTargetPosition) <= autoFetchDistance)
                {
                    pickupInRange = pickups[i];
                    break;
                }
            }

            return pickupInRange;
        }
    }
}

