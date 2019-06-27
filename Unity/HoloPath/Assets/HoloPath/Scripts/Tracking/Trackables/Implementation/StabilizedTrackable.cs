namespace HoloPath.Scripts.Tracking.Trackables.Implementation
{
    using UnityEngine;

    /// <summary>
    ///   A decorator for <see cref="AbstractTrackable"/>s that transforms
    ///   actual pose into exponentially weighted average pose, thereby
    ///   smoothing out erratic pose changes.
    /// </summary>
    public class StabilizedTrackable : AbstractTrackable
    {
        [SerializeField]
        private AbstractTrackable trackable;

        private Transform thisTransform;
        private Transform trackableTransform;

        private void Start()
        {
            this.thisTransform = this.transform;
            this.trackableTransform = this.trackable.transform;

            this.trackable.OnTrackingStarted += this.HandleTrackingStarted;
            this.trackable.OnTrackingStopped += this.HandleTrackingStopped;
            this.trackable.OnTrackingChanged += this.HandleTrackingChanged;
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
            this.UpdatePose();
            this.InvokeOnTrackingChanged();
        }

        private void UpdatePose()
        {
            this.thisTransform.position = Vector3.Lerp(
                this.thisTransform.position,
                this.trackableTransform.position,
                0.05f);

            this.thisTransform.rotation = Quaternion.Lerp(
                this.thisTransform.rotation,
                this.trackableTransform.rotation,
                0.05f);
        }
    }
}
