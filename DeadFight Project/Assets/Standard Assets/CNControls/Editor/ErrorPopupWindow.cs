using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

namespace CnControls
{
    public class ErrorPopupWindow : EditorWindow
    {
        public static void ShowWindow()
        {
            var window = EditorWindow.GetWindow<ErrorPopupWindow>();
            window.position = new Rect(Screen.width / 2f, Screen.height / 2f, 250, 133);
            window.titleContent = new GUIContent("ERROR");
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField(
                "Nao existe nenhum EventSystem na scene (ou está desactivo), CnControls nao vai funcionar.Por favor adiciona GameObject -> UI -> Event System ou clica no botao embaixo.",
                EditorStyles.wordWrappedLabel);

            GUILayout.Space(12);
            if (GUILayout.Button("Criado!"))
            {
                EditorApplication.isPlaying = false;

                EditorApplication.playModeStateChanged += OnPlaymodeChanged;
            }

            GUILayout.Space(6);

            if (GUILayout.Button("Fechar Janela"))
            {
                Close();
            }
        }

        private void OnPlaymodeChanged(PlayModeStateChange state)
        {
            if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorApplication.playModeStateChanged -= OnPlaymodeChanged;

                var eventSystemGo = new GameObject("EventSystem");
                eventSystemGo.AddComponent<EventSystem>();
                eventSystemGo.AddComponent<StandaloneInputModule>();

                Close();
            }
        }
    }
}
