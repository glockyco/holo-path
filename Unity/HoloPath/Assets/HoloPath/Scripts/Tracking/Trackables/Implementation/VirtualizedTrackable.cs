namespace HoloPath.Scripts.Tracking.Trackables.Implementation
{
    using UnityEngine;

    /// <summary>
    ///   A decorator for (physical) <see cref="AbstractTrackable"/>s that
    ///   transforms camera-relative pose into global pose.
    /// </summary>
    public class VirtualizedTrackable : AbstractTrackable
    {
        [SerializeField]
        private new Camera camera;

        [SerializeField]
        private AbstractTrackable trackable;

        private Transform thisTransform;
        private Transform cameraTransform;
        private Transform trackableTransform;

        private void Start()
        {
            this.thisTransform = this.transform;
            this.cameraTransform = this.camera.transform;
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
            #if UNITY_EDITOR
                Vector3 cameraPosition = this.cameraTransform.position;
                Quaternion cameraRotation = this.cameraTransform.rotation;

                Vector3 trackablePosition = this.trackableTransform.position;
                Quaternion trackableRotation = this.trackableTransform.rotation;

                this.thisTransform.position = cameraPosition + (cameraRotation * trackablePosition);
                this.thisTransform.rotation = cameraRotation * trackableRotation;
            #else
                this.thisTransform.position = this.trackableTransform.position;
                this.thisTransform.rotation = this.trackableTransform.rotation;
            #endif
        }
    }
}
