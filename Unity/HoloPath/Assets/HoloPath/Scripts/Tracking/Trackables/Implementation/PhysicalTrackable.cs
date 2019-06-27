namespace HoloPath.Scripts.Tracking.Trackables.Implementation
{
    using HoloPath.Scripts.Tracking.TrackableEventHandlers;
    using UnityEngine;

    /// <summary>
    ///   An adapter from <see cref="TrackableEventPublisher"/> to
    ///   <see cref="AbstractTrackable"/>.
    /// </summary>
    public class PhysicalTrackable : AbstractTrackable
    {
        [SerializeField]
        private TrackableEventPublisher publisher;

        private TrackablePose pose;
        private Transform trackableTransform;

        private void Start()
        {
            this.pose = new TrackablePose(this.transform);
            this.trackableTransform = this.publisher.transform;

            this.publisher.OnTrackingStarted += this.HandleTrackingStarted;
            this.publisher.OnTrackingStopped += this.HandleTrackingStopped;
            this.publisher.OnTrackingChanged += this.HandleTrackingChanged;
        }

        private void HandleTrackingStarted()
        {
            this.InvokeOnTrackingStarted();
        }

        private void HandleTrackingStopped()
        {
            this.InvokeOnTrackingStopped();
        }

        private void HandleTrackingChanged()
        {
            this.pose.Position = this.trackableTransform.position;
            this.pose.Rotation = this.trackableTransform.rotation;

            if (this.pose.HasChanged)
            {
                this.InvokeOnTrackingChanged();
            }
        }
    }
}
