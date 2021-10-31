using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EllenDrone
{
    public class PickupsZone : MonoBehaviour
    {
        private Vector2 m_xBounds;
        private Vector2 m_zBounds;
        private Vector3 m_PlayerPosition;
        private WaitForSeconds m_SpawnPickupWait;

        public int maxPickups = 20;
        public float minPickupSpace = 1;            // min space between pickups
        public float minPickupToPlayerSpace = 3;    // min space between each pickup and Ellen
        public float spawnPickupWaitTime = 0.2f;
        public GameObject pickupPrefab;

        public List<Pickup> Pickups { get; private set; }

        private void Awake()
        {
            Pickups = new List<Pickup>();

            m_SpawnPickupWait = new WaitForSeconds(spawnPickupWaitTime);

            // get zone bounds
            var zone = GetComponent<BoxCollider>();
            m_xBounds = new Vector2(zone.bounds.min.x, zone.bounds.max.x);
            m_zBounds = new Vector2(zone.bounds.min.z, zone.bounds.max.z);

            // get initial player position
            var player = FindObjectOfType<CharacterController>();
            m_PlayerPosition = player.transform.position;
        }

        IEnumerator Start()
        {
            // find spawned pickups
            var pickups = FindObjectsOfType<Pickup>();
            Pickups.AddRange(pickups);

            yield return null;

            for (int i = Pickups.Count; i < maxPickups; i++)
            {
                // spawn a pickup in a valid position
                var position = GetValidPickupPosition();
                var pickupGO = Instantiate(pickupPrefab, position, Quaternion.identity);

                // set score
                var pickup = pickupGO.GetComponent<Pickup>();
                pickup.scoreAmount = 1;

                // add observer for pickup event
                pickup.OnPickedUp.AddListener(HandlePickupEvent);

                // register pickup object
                Pickups.Add(pickup);

                yield return m_SpawnPickupWait;
            }
        }

        /// <summary>
        /// Gets a valid random position for a new pickup.
        /// It is recursive.
        /// </summary>
        /// <returns></returns>
        private Vector3 GetValidPickupPosition()
        {
            var position = Vector3.zero;

            position.x = Random.Range(m_xBounds.x, m_xBounds.y);
            position.y = transform.position.y;
            position.z = Random.Range(m_zBounds.x, m_zBounds.y);

            bool valid = true;
            for (int i = 0; i < Pickups.Count; i++)
            {
                if (Vector3.Distance(Pickups[i].transform.position, position) < minPickupSpace
                    || Vector3.Distance(m_PlayerPosition, position) < minPickupToPlayerSpace)
                {
                    valid = false;
                    break;
                }
            }

            return (valid) ? position : GetValidPickupPosition();
        }

        private void HandlePickupEvent(Pickup pickup)
        {
            Pickups.Remove(pickup);
        }
    }
}

