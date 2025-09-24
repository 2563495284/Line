using UnityEngine;
using UnityEditor;
public class Test2 : MonoBehaviour
{
#if UNITY_EDITOR
    [CustomEditor(typeof(Test2))]
    class TestEditor : Editor
    {
        Test2 obj;
        private void OnEnable()
        {
            obj = (Test2)target;
        }
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("TestBtn"))
            {
                Debug.Log(obj.GetComponent<RectTransform>().sizeDelta);
            }
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(obj.gameObject);
        }
    }
#endif
}