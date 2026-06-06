using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DependentlyInjectYourself.Editor.Windows
{
    public class DiyContainerViewer : EditorWindow
    {
        private float _verticalSpacing;
        private Vector2 _scrollPos;

        private void OnGUI()
        {
            try
            {
                DoGui();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Close();
            }
        }

        private void DoGui()
        {
            _verticalSpacing = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            Rect currentRect = EditorGUILayout.GetControlRect();

            {
                Rect buttonRect =  new  Rect(currentRect.x, currentRect.y, currentRect.width, EditorGUIUtility.singleLineHeight);
                if (GUI.Button(buttonRect, "ResetContainer"))
                {
                    DiyContainer.Reset();
                }
            }
            
            currentRect.y += _verticalSpacing;

            {
                Dictionary<Type, ServiceEntry> serviceEntries = DiyContainer.GetAllServices();
                Rect scrollViewRect = new()
                {
                    x = currentRect.x, y = currentRect.y, width = position.width - 5,
                    height = _verticalSpacing * 6,
                };

                Rect scrollViewContentRect = new()
                {
                    x = scrollViewRect.x, y = scrollViewRect.y, width = scrollViewRect.width - 15,
                    height = _verticalSpacing * serviceEntries.Count,
                };
                
                _scrollPos = GUI.BeginScrollView(scrollViewRect, _scrollPos, scrollViewContentRect, false, true);

                int index = 0;
                foreach ((Type type, ServiceEntry entry) in serviceEntries)
                {
                    Rect content = new()
                    {
                        x = scrollViewRect.x, y = scrollViewRect.y + _verticalSpacing * index, width = scrollViewContentRect.width,
                        height = _verticalSpacing,
                    };
                    
                    Rect labelRect = new Rect(content.x, content.y, content.width/2, EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(labelRect, type.Name);

                    Rect objRect = new Rect(content.x + content.width/2, content.y, content.width/2, EditorGUIUtility.singleLineHeight);
                    switch (entry.lifetime)
                    {
                        case ServiceLifetime.Singleton:
                            EditorGUI.LabelField(objRect, entry.ServiceInstance?.ToString() ?? "Null");
                            
                            break;
                        case ServiceLifetime.Transient:
                            string hasServiceGetter = entry.serviceGetter == null ? "null" : "set";
                            EditorGUI.LabelField(objRect, $"Transient: getter is {hasServiceGetter}!");
                            
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    
                    
                    index++;
                }

                GUI.EndScrollView();
            }
        }

        [MenuItem("Window/DI(Y)/Diy Container Viewer")]
        private static void Init()
        {
            GetWindow<DiyContainerViewer>();
        }
    }
}