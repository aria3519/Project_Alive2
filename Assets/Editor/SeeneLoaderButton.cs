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
            // �����Ͱ� �ε�� �� �ݹ��� �߰��մϴ�.
            EditorApplication.update += OnEditorUpdate;

            // �� ����� ���⿡ ���� �߰��մϴ�.
            // ������Ʈ ������ ���ϴ� ���� ���� �巡���Ͽ� ����մϴ�.
            scenes.Add(AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/PlayGame/Menu.unity"));
            scenes.Add(AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/PlayGame/MainTown.unity"));
            scenes.Add(AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/PlayGame/Stage5.unity"));
            scenes.Add(AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/PlayGame/Stage6.unity"));
            scenes.Add(AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/PlayGame/Stage7.unity"));
            scenes.Add(AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/PlayGame/Stage8.unity"));
        }

        private static void OnEditorUpdate()
        {
            // �̹� ��ư�� �߰��Ǿ� �ִٸ� �ٽ� �߰����� �ʽ��ϴ�.
            if (ToolbarExtender.LeftToolbarGUI.Contains(OnToolbarGUI))
                return;

            // ��ư�� �߰��մϴ�.
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
                // ���� �ε��մϴ�.
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

