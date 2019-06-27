namespace Pathfinding
{
    using UnityEngine;

    /// <summary>
    /// Sets the destination of an AI to the position of a specified object.
    /// This component should be attached to a GameObject together with a movement script such as AIPath, RichAI or AILerp.
    /// This component will then make the AI move towards the <see cref="target"/> set on this component.
    ///
    /// See: <see cref="Pathfinding.IAstarAI.destination"/>
    ///
    /// [Open online documentation to see images]
    /// </summary>
    [UniqueComponent(tag = "ai.destination")]
    [HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_a_i_destination_setter.php")]
    public class AIDestinationSetter : VersionedMonoBehaviour
    {
        /// <summary>The object that the AI should move to</summary>
        public Transform target;

        private IAstarAI ai;

        public void RemoveTarget()
        {
            this.target = null;
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        private void OnEnable()
        {
            this.ai = this.GetComponent<IAstarAI>();
            // Update the destination right before searching for a path as well.
            // This is enough in theory, but this script will also update the destination every
            // frame as the destination is used for debugging and may be used for other things by other
            // scripts as well. So it makes sense that it is up to date every frame.
            if (this.ai != null) this.ai.onSearchPath += this.Update;
        }

        private void OnDisable()
        {
            if (this.ai != null) this.ai.onSearchPath -= this.Update;
        }

        /// <summary>Updates the AI's destination every frame</summary>
        private void Update()
        {
            if (this.ai != null)
            {
                if (this.target != null)
                {
                    this.ai.destination = this.target.position;
                }
                else
                {
                    this.ai.destination = Vector3.positiveInfinity;
                }
            }
        }
    }
}
