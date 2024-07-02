using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Collections.Generic;

namespace UnityToolbarExtender
{
    [InitializeOnLoad]
    public static class SceneLoaderButton
    {

       
        private static List<SceneAsset> scenes = new List<SceneAsset>();
        private static int selectedSceneIndex = 0;

        static SceneLoaderButton()
        {
            // 에디터가 로드될 때 콜백을 추가합니다.
            EditorApplication.update += OnEditorUpdate;

            // 씬 목록을 여기에 직접 추가합니다.
            // 프로젝트 내에서 원하는 씬을 직접 드래그하여 등록합니다.
            scenes.Add(AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/PlayGame/Menu.unity"));
            scenes.Add(AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/PlayGame/MainTown.unity"));
            scenes.Add(AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/PlayGame/Stage5.unity"));
            scenes.Add(AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/PlayGame/Stage6.unity"));
            scenes.Add(AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/PlayGame/Stage7.unity"));
            scenes.Add(AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/PlayGame/Stage8.unity"));
        }

        private static void OnEditorUpdate()
        {
            // 이미 버튼이 추가되어 있다면 다시 추가하지 않습니다.
            if (ToolbarExtender.LeftToolbarGUI.Contains(OnToolbarGUI))
                return;

            // 버튼을 추가합니다.
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
        }

        private static void OnToolbarGUI()
        {
            if (scenes == null || scenes.Count == 0)
            {
                GUILayout.Label("No scenes found");
                return;
            }

            GUILayout.BeginHorizontal();

            GUILayout.Label("Select Scene:", GUILayout.Width(80));

            string[] sceneNames = new string[scenes.Count];
            for (int i = 0; i < scenes.Count; i++)
            {
                sceneNames[i] = scenes[i] != null ? scenes[i].name : "Missing Scene";
            }

            selectedSceneIndex = EditorGUILayout.Popup(selectedSceneIndex, sceneNames, GUILayout.Width(200));

            if (GUILayout.Button("Load Scene", GUILayout.Width(100)))
            {
                // 씬을 로드합니다.
                string scenePath = AssetDatabase.GetAssetPath(scenes[selectedSceneIndex]);
                if (!string.IsNullOrEmpty(scenePath))
                {
                    EditorSceneManager.OpenScene(scenePath);
                }
            }

            GUILayout.EndHorizontal();
        }
    }
}

