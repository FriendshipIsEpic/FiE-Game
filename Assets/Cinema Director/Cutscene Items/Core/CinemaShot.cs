// Cinema Suite
using System;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// The representation of a Shot.
    /// </summary>
    [CutsceneItemAttribute("Shots", "Shot", CutsceneItemGenre.CameraShot)]
    public class CinemaShot : CinemaGlobalAction
    {
        public Camera shotCamera;
        private bool cachedState;

        public override void Initialize()
        {
            if (shotCamera != null)
            {
                cachedState = shotCamera.gameObject.activeInHierarchy;
            }
        }

        public override void Trigger()
        {
            if (this.shotCamera != null)
            {
                this.shotCamera.gameObject.SetActive(true);
            }
        }


        public override void End()
        {
            if (this.shotCamera != null)
            {
                this.shotCamera.gameObject.SetActive(false);
            }
        }

        public override void Stop()
        {
            if (shotCamera != null)
            {
                this.shotCamera.gameObject.SetActive(cachedState);
            }
        }

        #region Properties

        /// <summary>
        /// Accesses the time that the cut takes place
        /// </summary>
        public float CutTime
        {
            get { return this.Firetime; }
            set { this.Firetime = value; }
        }

        /// <summary>
        /// The length of this shot in seconds.
        /// </summary>
        public float ShotLength
        {
            get { return this.Duration; }
            set { this.Duration = value; }
        }

        #endregion

    }
}