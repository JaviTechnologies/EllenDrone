using Gamekit3D;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace EllenDrone
{
    [RequireComponent(typeof(Collider))]
    public class Pickup : MonoBehaviour
    {
        public int scoreAmount = 1;

        // event to notify the pickup
        public class PickUpEvent : UnityEvent<Pickup> { };
        public PickUpEvent OnPickedUp;

        public bool IsDestroying { get; private set; }

        private void Awake()
        {
            OnPickedUp = new PickUpEvent();
        }

        void OnTriggerEnter(Collider other)
        {
            // check if other is the player or the drone
            if (other.GetComponent<PlayerController>() == null && other.GetComponent<DroneController>() == null)
                return;

            Scorer.Instance.AddScore(scoreAmount);

            if (OnPickedUp != null)
                OnPickedUp.Invoke(this);

            IsDestroying = true;

            StartCoroutine(PickupAnimation());
        }

        /// <summary>
        /// Play an awesome animation here.
        /// Then destroy the pickup.
        /// </summary>
        /// <returns></returns>
        IEnumerator PickupAnimation()
        {
            yield return null;

            Destroy(this.gameObject);
        }
    }
}

