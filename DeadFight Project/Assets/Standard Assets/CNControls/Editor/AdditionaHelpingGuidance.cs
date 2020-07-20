using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace CnControls
{
    /// <summary>
    /// Ajuda e Controlo do Jogador , Deteçao de erros de configuraçao (CNControls Plugin).
    /// </summary>
    [InitializeOnLoad]
    public class AdditionaHelpingGuidance
    {
        static AdditionaHelpingGuidance()
        {
            EditorApplication.playModeStateChanged += PlaymodeStateChanged;
        }

        private static void PlaymodeStateChanged(PlayModeStateChange state)
        {
            // Se nos tivermos mudado o nosso modo de jogo para Playmode
            if (EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
            {
                var types = from t in Assembly.GetAssembly(typeof(SimpleButton)).GetTypes()
                            where 
                                t.IsClass 
                                && t.Namespace == "CnControls"
                                && t.IsSubclassOf(typeof(MonoBehaviour))
                            select t;

                // Se existirem alguns CNControls na Scene.
                bool shouldCheckForErrors = types.Any(type => Object.FindObjectOfType(type));

                if (shouldCheckForErrors)
                {
                    CheckForEventSystemPresence();
                }
            }
        }

        private static void CheckForEventSystemPresence()
        {
            var eventSystem = Object.FindObjectOfType<EventSystem>();

            if (eventSystem == null)
            {
                ErrorPopupWindow.ShowWindow();
            }
        }
    }
}
