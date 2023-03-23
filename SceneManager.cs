using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.Linq;
using UnityEngine;

public class SceneManager : EditorWindow
{
    List<Object> scenes = new List<Object>();
    static int newSceneCount = 0;
    Vector2 scrollpos = Vector2.zero;
    [MenuItem("Window / Scenes Manager")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<SceneManager>("Scenes Manager");
    }

    [System.Obsolete]
    private void OnGUI()
    {
        GUILayout.Label("Scenes", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Add Empty Scene Thresholder"))
        {
            Object newScene = new Object();
            scenes.Add(newScene);
        }

        if(GUILayout.Button("Create New Scene"))
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            string[] scenesPath = EditorSceneManager.GetActiveScene().path.Split(char.Parse("/"));
            while(File.Exists(string.Join("/", scenesPath)))
            {
                scenesPath[scenesPath.Length - 1] = "New Scene" + (++newSceneCount) + ".unity";
            }
            var s = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
            EditorSceneManager.SaveScene(s, string.Join("/", scenesPath));
            Object obj = AssetDatabase.LoadAssetAtPath<SceneAsset>(s.path);
            scenes.Add(obj);
            GUIUtility.ExitGUI();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        
        if(scenes.Count > 0)
        {
            scrollpos = EditorGUILayout.BeginScrollView(scrollpos, false, false, GUILayout.Width(position.width), GUILayout.Height(position.height - 120));
            for(int i = 0; i < scenes.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                scenes[i] = EditorGUILayout.ObjectField(scenes[i], typeof(SceneAsset), false);
                GUI.backgroundColor = Color.green;
                if(GUILayout.Button("Open")) {
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                    EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(scenes[i]));
                }
                GUI.backgroundColor = Color.cyan;
                if(GUILayout.Button("Open Additive")) {
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                    EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(scenes[i]), OpenSceneMode.Additive);
                }
                GUI.backgroundColor = Color.yellow;
                if(GUILayout.Button("Add To Build Scenes"))
                {
                    var original = EditorBuildSettings.scenes; 
                    if(!original.ToList().Any(item => item.path.Equals(AssetDatabase.GetAssetPath(scenes[i])))) {
                        var newSettings = new EditorBuildSettingsScene[original.Length + 1];
                        System.Array.Copy(original, newSettings, original.Length);
                        var sceneToAdd = new EditorBuildSettingsScene(AssetDatabase.GetAssetPath(scenes[i]), true);
                        newSettings[newSettings.Length - 1] = sceneToAdd;
                        EditorBuildSettings.scenes = newSettings;
                    }

                }
                GUI.backgroundColor = Color.red;
                if(GUILayout.Button("Delete"))
                {
                    var original = EditorBuildSettings.scenes; 
                    var newSettings = new EditorBuildSettingsScene[original.Length  - 1];
                    int count = 0;
                    for(int j = 0; j < original.Length; j++)
                    {
                        var s = original[j];
                        if(s.path.Equals(AssetDatabase.GetAssetPath(scenes[i])))
                        {
                            continue;
                        }
                        newSettings[count++] = s;
                    }
                    EditorBuildSettings.scenes = newSettings;
                    scenes.RemoveAt(i);

                }
                EditorGUILayout.EndHorizontal();
                GUI.backgroundColor = Color.white;
            }
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.Space();
        if(GUILayout.Button("Get Build Scenes"))
        {
            GetBuildScenes();
        }

        if(scenes.Count > 0)
        {
            EditorGUILayout.Space();
            if(GUILayout.Button("Remove All Scenes"))
            {
                RemoveAllScenes();
            }
        }
    }

    private void GetBuildScenes()
    {
        var editorScene = EditorBuildSettings.scenes;
        for(int i = 0; i < editorScene.Length; i++)
        {
            SceneAsset _sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(editorScene[i].path);
            if(!scenes.Contains(_sceneAsset))
            {
                scenes.Add(_sceneAsset);
            }
        }

        for(int i = 0; i < scenes.Count; i++)
        {
            if (scenes[i] == null)
            {
                scenes.RemoveAt(i);
            }
        }
    }
    private void RemoveAllScenes()
    {
        scenes.Clear();
    }
}
