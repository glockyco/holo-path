namespace HoloPath.Scripts
{
    using Pathfinding;
    using UnityEngine;
    using UnityEngine.Events;

    public class LocalSpaceRichAI : RichAI
    {
        public LocalSpaceGraph graph;

        public UnityEvent targetReachedCallback;

        protected override void Start()
        {
            this.RefreshTransform();
            base.Start();
        }

        protected override void Update()
        {
            this.RefreshTransform();
            base.Update();
        }

        protected override void CalculatePathRequestEndpoints(out Vector3 start, out Vector3 end)
        {
            this.RefreshTransform();
            base.CalculatePathRequestEndpoints(out start, out end);
            start = this.graph.transformation.InverseTransform(start);
            end = this.graph.transformation.InverseTransform(end);
        }

        protected override void OnTargetReached()
        {
            this.targetReachedCallback?.Invoke();
        }

        private void RefreshTransform()
        {
            this.graph.Refresh();
            this.richPath.transform = this.graph.transformation;
            this.movementPlane = this.graph.transformation;
        }
    }
}
