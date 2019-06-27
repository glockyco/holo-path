namespace HoloPath.Scripts.Tracking.Trackables.Implementation
{
    using UnityEngine;

    /// <summary>
    ///   A trackable that allows simulation of physical trackables through virtual
    ///   objects.
    /// </summary>
    public class VirtualTrackable : AbstractTrackable
    {
        [SerializeField]
        private new Camera camera;

        private TrackablePose pose;

        private void Start()
        {
            this.pose = new TrackablePose(this.transform);
        }

        /// <summary>
        ///   Updates the tracking state of this object.
        /// </summary>
        private void Update()
        {
            if (this.IsDetected())
            {
                if (!this.IsTracking)
                {
                    this.InvokeOnTrackingStarted();
                }
            }
            else
            {
                if (this.IsTracking)
                {
                    this.InvokeOnTrackingStopped();
                }
            }

            if (this.HasTrackingChanged())
            {
                this.InvokeOnTrackingChanged();
            }
        }

        /// <summary>
        ///   Determines whether this object is detected by the camera.
        /// </summary>
        /// <remarks>
        ///   An object is reported as detected when its position is within
        ///   a certain region around the center-point of the field-of-view
        ///   of the camera.
        /// </remarks>
        /// <returns>
        ///   <see langword="true"/> if this object is detected by the camera;
        ///   otherwise, <see langword="false"/>.
        /// </returns>
        private bool IsDetected()
        {
            Vector3 point = this.camera.WorldToViewportPoint(this.pose.Position);

            const float minViewportPosition = 0.4f;
            const float maxViewportPosition = 0.6f;

            bool xIsInArea = point.x >= minViewportPosition && point.x <= maxViewportPosition;
            bool yIsInArea = point.y >= minViewportPosition && point.y <= maxViewportPosition;

            return xIsInArea && yIsInArea;
        }

        /// <summary>
        ///   Determines whether the tracking information of this object has changed.
        /// </summary>
        /// <remarks>
        ///   Since the trackable doesn't change its pose on its own and the
        ///   pose is private, the pose can only change if the transform that
        ///   is wrapped by the pose is modified by another script.
        /// </remarks>
        /// <returns>
        ///   <see langword="true"/> if the tracking information has changed;
        ///   otherwise, <see langword="false"/>.
        /// </returns>
        private bool HasTrackingChanged()
        {
            return this.IsTracking && this.pose.HasChanged;
        }
    }
}
