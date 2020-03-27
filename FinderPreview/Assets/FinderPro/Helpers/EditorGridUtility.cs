#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Finder {
    public class EditorGridUtility {
        public Vector2 scrollPos;
        public Rect position;
        public float buttonSize = 75f;
        public int countRows = 10;
        public float margin = 15f;
        public int buttonsOnPage = 150;
        public int currentPage = 0;
        public int pagesCount;

        public void Display<T> (T[] data, Action<T> predicate, bool beginScrollView = true, bool endScrollView = true) {
            var width = (position.width > 250 ? position.width : 250);
            if (beginScrollView) {
                scrollPos = EditorGUILayout.BeginScrollView (scrollPos, GUILayout.Width (width), GUILayout.ExpandHeight (true));
            }
            for (int i = 0; i < data.Length;) {
                GUILayout.BeginHorizontal ();
                for (int j = 0; j < (width / (buttonSize + (margin / 2))) - 1.5; j++) {
                    if (data[i] != null)
                        predicate (data[i]);
                    i++;
                    if (i >= data.Length) {
                        break;
                    }
                }
                GUILayout.EndHorizontal ();
                EditorGUILayout.Separator ();
            }
            if (endScrollView) {
                EditorGUILayout.EndScrollView ();
            }
        }

        public void Prev () {
            if (currentPage > 0)
                currentPage--;
        }
        public void Swap () {
            if (currentPage == 0) {
                currentPage = pagesCount;
            } else if (currentPage == pagesCount) {
                currentPage = 0;
            } else {
                currentPage = 0;
            }
        }
        public void Next () {
            if (currentPage < pagesCount)
                currentPage++;
            currentPage = currentPage >= pagesCount ? pagesCount : currentPage;
        }
    }
}
#endif