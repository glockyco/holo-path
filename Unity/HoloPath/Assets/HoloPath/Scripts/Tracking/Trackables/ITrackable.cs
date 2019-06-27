namespace HoloPath.Scripts.Tracking.Trackables
{
    /// <summary>
    ///   Handler for <see cref="ITrackable"/> events.
    /// </summary>
    public delegate void TrackableEventHandler();

    /// <summary>
    ///   Interface for objects that can be tracked by the camera.
    /// </summary>
    public interface ITrackable
    {
        /// <summary>
        ///   Occurs when tracking of this object has started.
        /// </summary>
        event TrackableEventHandler OnTrackingStarted;

        /// <summary>
        ///   Occurs when tracking of this object has stopped.
        /// </summary>
        event TrackableEventHandler OnTrackingStopped;

        /// <summary>
        ///   Occurs when tracking information of this object has changed.
        /// </summary>
        event TrackableEventHandler OnTrackingChanged;

        /// <summary>
        ///   Gets a value indicating whether this object is currently being tracked.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if this object is currently being tracked;
        ///   otherwise, <see langword="false"/>.
        /// </value>
        bool IsTracking { get; }
    }
}
