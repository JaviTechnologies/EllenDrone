using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EllenDrone
{
    /// <summary>
    /// Keeps tracks of the score.
    /// It is a Singleton in order to be referenced globally.
    /// </summary>
    public class Scorer : MonoBehaviour
    {
        protected static Scorer s_Instance;

        // event to notify changes on the score
        public UnityEvent OnScore;

        public static Scorer Instance { get { return s_Instance; } }
        public int CurrentScore { get; private set; }

        private void Awake()
        {
            if (s_Instance == null)
                s_Instance = this;
            else if(s_Instance != this)
                throw new UnityException("There cannot be more than one Scorer script.  The instances are " + s_Instance.name + " and " + name + ".");
        }

        public void AddScore(int score)
        {
            CurrentScore += score;

            if (OnScore != null)
                OnScore.Invoke();
        }
    }
}
