using UnityEditor;
using UnityEngine;

public class CustomObjectEditorTool : EditorWindow
{
    Color color;
    float scale = 1.0f;


    [MenuItem("Window/Objects Colorizer And Scaler")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<CustomObjectEditorTool>("Objects Colorizer & Scaler");
    }

    void OnGUI()
    {
        GUILayout.Label("Color and scale the selected objects!", EditorStyles.boldLabel);
        color = EditorGUILayout.ColorField("Color", color);
        scale = EditorGUILayout.Slider("Scale",scale, 0.1f, 10f);
        if(Selection.count > 0)
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                obj.transform.localScale = new Vector3(scale, scale, scale);
            }
        }
        if(GUILayout.Button("Colorize"))
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if(renderer != null)
                {
                    renderer.material.color = color;
                }
            }
        }
    }
}
