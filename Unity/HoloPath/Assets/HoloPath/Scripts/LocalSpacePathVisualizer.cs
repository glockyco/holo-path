namespace HoloPath.Scripts
{
    using System.Collections.Generic;
    using System.Linq;
    using Pathfinding;
    using UnityEngine;

    public class LocalSpacePathVisualizer : MonoBehaviour
    {
        [SerializeField]
        private Seeker seeker;

        [SerializeField]
        private LocalSpaceGraph graph;

        [SerializeField]
        private LineRenderer lineRenderer;

        private void OnEnable()
        {
            this.seeker.pathCallback += this.OnPath;
        }

        private void OnDisable()
        {
            this.seeker.pathCallback -= this.OnPath;
        }

        private void Start()
        {
            this.lineRenderer.enabled = false;
        }

        private void OnPath(Path path)
        {
            IEnumerable<Vector3> localPath = path.vectorPath.Select(p => this.graph.transformation.Transform(p));

            this.lineRenderer.positionCount = path.vectorPath.Count;
            this.lineRenderer.SetPositions(localPath.ToArray());
        }
    }
}
