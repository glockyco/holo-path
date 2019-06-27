namespace HoloPath.Scripts.Tracking.Trackables
{
    using UnityEngine;

    /// <summary>
    ///   An object that can be tracked by the camera.
    /// </summary>
    public abstract class AbstractTrackable : MonoBehaviour, ITrackable
    {
        /// <inheritdoc/>
        public event TrackableEventHandler OnTrackingStarted;

        /// <inheritdoc/>
        public event TrackableEventHandler OnTrackingStopped;

        /// <inheritdoc/>
        public event TrackableEventHandler OnTrackingChanged;

        /// <inheritdoc/>
        public bool IsTracking { get; private set; }

        /// <summary>
        ///   Notifies listeners that tracking of this object has started.
        /// </summary>
        protected void InvokeOnTrackingStarted()
        {
            this.IsTracking = true;
            this.OnTrackingStarted?.Invoke();
            this.OnTrackingChanged?.Invoke();
        }

        /// <summary>
        ///   Notifies listeners that tracking of this object has ended.
        /// </summary>
        protected void InvokeOnTrackingStopped()
        {
            this.IsTracking = false;
            this.OnTrackingChanged?.Invoke();
            this.OnTrackingStopped?.Invoke();
        }

        /// <summary>
        ///   Notifies listeners that tracking information of this object has changed.
        /// </summary>
        protected void InvokeOnTrackingChanged()
        {
            this.OnTrackingChanged?.Invoke();
        }
    }
}
