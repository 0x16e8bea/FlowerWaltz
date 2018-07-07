#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.IO;

    public class CreateProjectFolders : ScriptableObject {
    // Add menu to the main menu
    [MenuItem("Custom/Project/Create Project Folders")]
    static void SetupProjectFolders() {
        // Create the Project Folders
        CreateFolders();

        // Refresh the Editor
        RefreshEditor();
    }

    static void RefreshEditor() {
        // Refresh the Editor
        AssetDatabase.Refresh();
    }

    static void CreateFolders() {
        // Debug Log
        Debug.Log("Creating Project Folders");

        // Get the full path
        string FULL = Application.dataPath + "/";

        // Set the Art Path
        string ART = FULL + "Art" + "/";
        Directory.CreateDirectory(ART + "Materials");
        Directory.CreateDirectory(ART + "Textures");
        Directory.CreateDirectory(ART + "Models");
        Directory.CreateDirectory(ART + "Shaders");
        Directory.CreateDirectory(ART + "Fonts");

        // Set the Audio Path
        string AUDIO = FULL + "Audio" + "/";
        Directory.CreateDirectory(AUDIO + "Sounds");
        Directory.CreateDirectory(AUDIO + "Music");

        // Set the Code Path
        string CODE = FULL + "Code" + "/";
        Directory.CreateDirectory(CODE + "Actors");
        Directory.CreateDirectory(CODE + "Managers");
        Directory.CreateDirectory(CODE + "Controllers");
        Directory.CreateDirectory(CODE + "Frameworks");
        Directory.CreateDirectory(CODE + "Misc");

        // Set the Game Path
        string GAME = FULL + "Game" + "/";
        Directory.CreateDirectory(GAME + "Prefabs");
        Directory.CreateDirectory(GAME + "Scenes");
        Directory.CreateDirectory(GAME + "Physics");
        Directory.CreateDirectory(GAME + "Packages");

        // Set the Special Paths
        Directory.CreateDirectory(CODE + "Editor");
        Directory.CreateDirectory(CODE + "Plugins");
        Directory.CreateDirectory(FULL + "Resources");
        Directory.CreateDirectory(FULL + "Gizmos");

        /* Special Folders
         * 
         * Resources
         * The Resources folder is a special folder which allows you to access resources by name in your scripts, rather than by the usual (and recommended) method of direct references. 
         * For this reason, caution is advised when using it, because all items you put in the resources are included in your build (even unused assets), 
         * because Unity has no way of determining which assets may be used by your project.
         * 
         * Editor
         * The Editor folder name is a special name which allows your scripts access to the Unity Editor Scripting API. 
         * If your script uses any classes or functionality from the UnityEditor namespace, it has to be placed in a folder called Editor (or a subfolder).
         * 
         * Plugins
         * The "Plugins" folder is where you must put any C, C++ or Objective-C based Plugins which should be included with your project. 
         * Plugins are a pro-only feature.
         * 
         * Gizmos
         * The gizmos folder holds all the texture/icon assets for use with Gizmos.DrawIcon(). 
         * Texture assets placed inside this folder can be called by name, and drawn on-screen as a gizmo in the editor.
         */
    }
}
#endif
