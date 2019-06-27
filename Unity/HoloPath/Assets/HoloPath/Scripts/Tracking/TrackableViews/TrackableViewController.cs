namespace HoloPath.Scripts.Tracking.TrackableViews
{
    using HoloPath.Scripts.Tracking.Trackables;
    using UnityEngine;

    /// <summary>
    ///   A view that is placed at the position of the
    ///   <see cref="AbstractTrackable"/> <see cref="trackable"/>.
    /// </summary>
    public class TrackableViewController : MonoBehaviour
    {
        [SerializeField]
        private AbstractTrackable trackable;

        [SerializeField]
        private GameObject view;

        private Transform trackableTransform;
        private Transform viewTransform;

        private void Start()
        {
            this.trackableTransform = this.trackable.transform;
            this.viewTransform = this.view.transform;

            this.trackable.OnTrackingChanged += this.UpdatePose;
            this.trackable.OnTrackingStarted += this.EnableView;
            //this.trackable.OnTrackingStopped += this.DisableView;

            this.DisableView();
        }

        private void OnDestroy()
        {
            this.trackable.OnTrackingChanged -= this.UpdatePose;
            this.trackable.OnTrackingStarted -= this.EnableView;
            //this.trackable.OnTrackingStopped -= this.DisableView;
        }

        private void UpdatePose()
        {
            this.viewTransform.position = this.trackableTransform.position;
            this.viewTransform.rotation = this.trackableTransform.rotation;
        }

        private void EnableView()
        {
            this.view.SetActive(true);
        }

        private void DisableView()
        {
            this.view.SetActive(false);
        }
    }
}
