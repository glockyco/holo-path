namespace HoloPath.Scripts.Tracking.Trackables
{
    using UnityEngine;

    /// <summary>
    ///   A wrapper for <see cref="Transform"/>s that enables detection of pose
    ///   (i.e. position/rotation) changes.
    /// </summary>
    public class TrackablePose
    {
        private readonly Transform transform;

        private Vector3 previousPosition;
        private Quaternion previousRotation;

        /// <summary>
        ///   Initializes a new instance of the <see cref="TrackablePose"/> class.
        /// </summary>
        /// <param name="transform">
        ///   The transform that should be wrapped.
        /// </param>
        public TrackablePose(Transform transform)
        {
            this.transform = transform;
            this.previousPosition = this.transform.position;
            this.previousRotation = this.transform.rotation;
        }

        /// <summary>
        ///   Gets or sets the world space position of the wrapped Transform.
        /// </summary>
        public Vector3 Position
        {
            get { return this.transform.position; }
            set { this.transform.position = value; }
        }

        /// <summary>
        ///   Gets or sets the world space rotation of the wrapped Transform.
        /// </summary>
        public Quaternion Rotation
        {
            get { return this.transform.rotation; }
            set { this.transform.rotation = value; }
        }

        /// <summary>
        ///   Gets a value indicating whether this pose has changed.
        /// </summary>
        /// <remarks>
        ///   The pose is reported as having changed when the position or rotation
        ///   of the <see cref="transform"/> that it wraps have different values
        ///   than the ones which they had when <see cref="HasChanged"/> was
        ///   called the last time.
        /// </remarks>
        /// <returns>
        ///   <see langword="true"/> if this pose has changed;
        ///   otherwise, <see langword="false"/>.
        /// </returns>
        public bool HasChanged
        {
            get
            {
                bool hasChanged = this.HasPositionChanged() || this.HasRotationChanged();

                this.previousPosition = this.transform.position;
                this.previousRotation = this.transform.rotation;

                return hasChanged;
            }
        }

        private bool HasPositionChanged()
        {
            return this.previousPosition != this.Position;
        }

        private bool HasRotationChanged()
        {
            return this.previousRotation != this.Rotation;
        }
    }
}
