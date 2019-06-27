namespace HoloPath.Scripts.Editor
{
    using Pathfinding;
    using UnityEditor;

    [CustomEditor(typeof(LocalSpaceRichAI), true)]
    [CanEditMultipleObjects]
    public class LocalSpaceRichAIEditor : BaseAIEditor
    {
        protected override void Inspector()
        {
            base.Inspector();

            this.PropertyField(nameof(LocalSpaceRichAI.graph));
            this.PropertyField(nameof(LocalSpaceRichAI.targetReachedCallback));
        }
    }
}
