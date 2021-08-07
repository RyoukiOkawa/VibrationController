namespace Myspace.Vibrations.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(ScriptableVibration))]
    class ScriptableVibrationEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var tar = target as ScriptableVibration;

            var parameter = tar.Parameter;
        }
    }

}
