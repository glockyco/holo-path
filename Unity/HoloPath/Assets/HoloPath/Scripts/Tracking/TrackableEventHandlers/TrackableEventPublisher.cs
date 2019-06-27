namespace HoloPath.Scripts.Tracking.TrackableEventHandlers
{
    using System.Linq;
    using UnityEngine;
    using Vuforia;

    /// <summary>
    ///   A TrackableEventHandler that exposes state changes of the wrapped
    ///   <see cref="trackableBehaviour"/> as C# events.
    /// </summary>
    public class TrackableEventPublisher : MonoBehaviour, ITrackableEventHandler
    {
        /// <summary>The TrackableBehaviour that should be wrapped.</summary>
        [SerializeField]
        private TrackableBehaviour trackableBehaviour;

        /// <summary>
        ///   The <see cref="trackableBehaviour"/> states during which
        ///   <see cref="isTracking"/> should be <see langword="true"/>.
        /// </summary>
        [SerializeField]
        private TrackableBehaviour.Status[] trackingStates =
        {
            TrackableBehaviour.Status.DETECTED,
            TrackableBehaviour.Status.TRACKED,
            TrackableBehaviour.Status.EXTENDED_TRACKED,
        };

        /// <summary>
        ///   <see langword="true"/> if the <see cref="trackableBehaviour"/> is in one
        ///   of the <see cref="trackingStates"/>; otherwise <see langword="false"/>.
        /// </summary>
        private bool isTracking;

        /// <summary>
        ///   Handler for <see cref="TrackableEventPublisher"/> events.
        /// </summary>
        public delegate void TrackingEventHandler();

        /// <summary>
        ///   Occurs when the <see cref="trackableBehaviour"/> is found.
        /// </summary>
        public event TrackingEventHandler OnTrackingStarted;

        /// <summary>
        ///   Occurs when the <see cref="trackableBehaviour"/> is lost.
        /// </summary>
        public event TrackingEventHandler OnTrackingStopped;

        /// <summary>
        ///   Occurs every Update during which the <see cref="trackableBehaviour"/>
        ///   is tracked.
        /// </summary>
        public event TrackingEventHandler OnTrackingChanged;

        /// <inheritdoc/>
        public void OnTrackableStateChanged(
            TrackableBehaviour.Status previousStatus,
            TrackableBehaviour.Status newStatus)
        {
            if (this.trackingStates.Contains(newStatus))
            {
                this.InvokeOnTrackingStarted();
            }
            else
            {
                this.InvokeOnTrackingStopped();
            }
        }

        private void InvokeOnTrackingStarted()
        {
            this.isTracking = true;
            this.OnTrackingStarted?.Invoke();
        }

        private void InvokeOnTrackingStopped()
        {
            this.isTracking = false;
            this.OnTrackingStopped?.Invoke();
        }

        private void InvokeOnTrackingChanged()
        {
            this.OnTrackingChanged?.Invoke();
        }

        private void Start()
        {
            this.trackableBehaviour.RegisterTrackableEventHandler(this);
        }

        private void Update()
        {
            if (this.isTracking)
            {
                this.InvokeOnTrackingChanged();
            }
        }

        private void OnDestroy()
        {
            this.trackableBehaviour.UnregisterTrackableEventHandler(this);
        }
    }
}
