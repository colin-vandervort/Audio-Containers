using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AudioContainerEditor : EditorWindow
{
    public AudioContainerList audioContainerList;
    private int viewIndex = 1;

    [MenuItem ("Window/Audio Container Editor %#e")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(AudioContainerEditor));
    }

    private void OnEnable()
    {
        if(EditorPrefs.HasKey("ObjectPath"))
        {
            string objectPath = EditorPrefs.GetString("ObjectPath");
            audioContainerList = AssetDatabase.LoadAssetAtPath(objectPath, typeof(AudioContainerList)) as AudioContainerList;
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Audio Container Editor", EditorStyles.boldLabel);

        if (audioContainerList != null)
        {
            if(GUILayout.Button("Show Container List"))
            {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = audioContainerList;
            }
        }

        if (GUILayout.Button("Open Container List"))
        {
            OpenContainerList();
        }

        if(GUILayout.Button("New Container List"))
        {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = audioContainerList;
        }

        GUILayout.EndHorizontal();

        if(audioContainerList == null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);

            if (GUILayout.Button("Create New Container List", GUILayout.ExpandWidth(false)))
            {
                CreateNewContainerList();
            }

            if (GUILayout.Button("Open Existing Container List", GUILayout.ExpandWidth(false)))
            {
                OpenContainerList();
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.Space(20);

        if (audioContainerList != null)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Space(10);

            if(GUILayout.Button("Prev", GUILayout.ExpandWidth(false)))
            {
                if (viewIndex > 1)
                    viewIndex--;
            }

            GUILayout.Space(5);

            if(GUILayout.Button("Next", GUILayout.ExpandWidth(false)))
            {
                if(viewIndex < audioContainerList.audioContainers.Count)
                {
                    viewIndex++;
                }
            }

            GUILayout.Space(60);

            if (GUILayout.Button("Add Container", GUILayout.ExpandWidth(false)))
            {
                AddContainer();
            }

            if(GUILayout.Button("Delete Container", GUILayout.ExpandWidth(false)))
            {
                DeleteContainer(viewIndex - 1);
            }

            GUILayout.EndHorizontal();

            if (audioContainerList.audioContainers == null)
                Debug.Log("Audio container list is empty and/or things are borked.");

            if (audioContainerList.audioContainers.Count > 0)
            {
                GUILayout.BeginHorizontal();
                viewIndex = Mathf.Clamp(EditorGUILayout.IntField("Current Container", viewIndex, GUILayout.ExpandWidth(false)), 1, audioContainerList.audioContainers.Count);
                EditorGUILayout.LabelField("of   " + audioContainerList.audioContainers.Count.ToString() + "  containers", "", GUILayout.ExpandWidth(false));
                GUILayout.EndHorizontal();

                audioContainerList.audioContainers[viewIndex - 1].containerName = EditorGUILayout.TextField("Container Name", audioContainerList.audioContainers[viewIndex - 1].containerName as string);
                audioContainerList.audioContainers[viewIndex - 1].containerType = (AudioContainer.ContainerType)EditorGUILayout.EnumPopup("Container Type", audioContainerList.audioContainers[viewIndex - 1].containerType);

                SerializedObject so = new SerializedObject(audioContainerList.audioContainers[viewIndex - 1]);
                SerializedProperty weightingList = so.FindProperty("playbackWeighting");
                int weightingListSize = weightingList.arraySize;

                switch (audioContainerList.audioContainers[viewIndex - 1].containerType)
                {
                    case AudioContainer.ContainerType.Clips:
                        SerializedProperty clipsList = so.FindProperty("audioClips");
                        int clipsListSize = clipsList.arraySize;
                        weightingListSize = clipsListSize;

                        clipsListSize = EditorGUILayout.IntField("Clips List Size", clipsListSize);
                        if (clipsListSize != clipsList.arraySize)
                        {
                            while (clipsListSize > clipsList.arraySize)
                            {
                                clipsList.InsertArrayElementAtIndex(clipsList.arraySize);
                            }
                            while (clipsListSize < clipsList.arraySize)
                            {
                                clipsList.DeleteArrayElementAtIndex(clipsList.arraySize - 1);
                            }
                        }

                        for (int i = 0; i < clipsList.arraySize; i++)
                        {
                            EditorGUILayout.PropertyField(clipsList.GetArrayElementAtIndex(i));
                            //audioContainerList.audioContainers[viewIndex - 1].audioClips[i] = clipsList.GetArrayElementAtIndex(i);
                        }
                        //audioContainerList.audioContainers[viewIndex - 1].audioClips = clipsList;
                        break;
                    case AudioContainer.ContainerType.Containers:
                        SerializedProperty containersList = so.FindProperty("audioContainers");
                        int containersListSize = containersList.arraySize;
                        weightingListSize = containersListSize;

                        containersListSize = EditorGUILayout.IntField("Containers List Size", containersListSize);
                        if (containersListSize != containersList.arraySize)
                        {
                            while (containersListSize > containersList.arraySize)
                            {
                                containersList.InsertArrayElementAtIndex(containersList.arraySize);
                            }
                            while (containersListSize < containersList.arraySize)
                            {
                                containersList.DeleteArrayElementAtIndex(containersList.arraySize - 1);
                            }
                        }

                        for (int i = 0; i < containersList.arraySize; i++)
                        {
                            EditorGUILayout.PropertyField(containersList.GetArrayElementAtIndex(i));
                            //audioContainerList.audioContainers[viewIndex - 1].audioContainers[i] = containersList.GetArrayElementAtIndex(i);
                        }
                        //audioContainerList.audioContainers[viewIndex - 1].audioContainers = containersList;
                        break;
                    default:
                        Debug.Log("No valid container type used");
                        break;
                }

                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                audioContainerList.audioContainers[viewIndex - 1].pattern = (AudioContainer.Pattern)EditorGUILayout.EnumPopup("Playback Pattern", audioContainerList.audioContainers[viewIndex - 1].pattern);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                audioContainerList.audioContainers[viewIndex - 1].playWholeList = EditorGUILayout.Toggle("Play Entire List", audioContainerList.audioContainers[viewIndex - 1].playWholeList, GUILayout.ExpandWidth(false));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                audioContainerList.audioContainers[viewIndex - 1].repeat = EditorGUILayout.Toggle("Loop Playback", audioContainerList.audioContainers[viewIndex - 1].repeat, GUILayout.ExpandWidth(false));
                GUILayout.EndHorizontal();

                if (audioContainerList.audioContainers[viewIndex - 1].pattern == AudioContainer.Pattern.Random)
                {
                    GUILayout.BeginHorizontal();
                    audioContainerList.audioContainers[viewIndex - 1].avoidRepeatedPlaybackCount = EditorGUILayout.IntField("Don't Repeat Last X Items", audioContainerList.audioContainers[viewIndex - 1].avoidRepeatedPlaybackCount, GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal();
                }

                GUILayout.Space(10);

                GUILayout.BeginHorizontal();


                weightingListSize = EditorGUILayout.IntField("Containers List Size", weightingListSize);
                if (weightingListSize != weightingList.arraySize)
                {
                    while (weightingListSize > weightingList.arraySize)
                    {
                        weightingList.InsertArrayElementAtIndex(weightingList.arraySize);
                    }
                    while (weightingListSize < weightingList.arraySize)
                    {
                        weightingList.DeleteArrayElementAtIndex(weightingList.arraySize - 1);
                    }
                }

                for (int i = 0; i < weightingList.arraySize; i++)
                {
                    GUILayout.EndHorizontal();
                    EditorGUILayout.PropertyField(weightingList.GetArrayElementAtIndex(i));
                    //audioContainerList.audioContainers[viewIndex - 1].playbackWeighting[i] = weightingList.GetArrayElementAtIndex(i);
                    GUILayout.BeginHorizontal();

                }
                //audioContainerList.audioContainers[viewIndex - 1].playbackWeighting = weightingList;

                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                audioContainerList.audioContainers[viewIndex - 1].minWaitBeforePlaying = EditorGUILayout.FloatField("Min Start Delay", audioContainerList.audioContainers[viewIndex - 1].minWaitBeforePlaying, GUILayout.ExpandWidth(false));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                audioContainerList.audioContainers[viewIndex - 1].maxWaitBeforePlaying = EditorGUILayout.FloatField("Max Start Delay", audioContainerList.audioContainers[viewIndex - 1].maxWaitBeforePlaying, GUILayout.ExpandWidth(false));
                GUILayout.EndHorizontal();

                if (audioContainerList.audioContainers[viewIndex - 1].playWholeList || audioContainerList.audioContainers[viewIndex - 1].repeat)
                {
                    GUILayout.BeginHorizontal();
                    audioContainerList.audioContainers[viewIndex - 1].minWaitAfterPlaying = EditorGUILayout.FloatField("Min End Wait", audioContainerList.audioContainers[viewIndex - 1].minWaitAfterPlaying, GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    audioContainerList.audioContainers[viewIndex - 1].maxWaitAfterPlaying = EditorGUILayout.FloatField("Max End Wait", audioContainerList.audioContainers[viewIndex - 1].maxWaitAfterPlaying, GUILayout.ExpandWidth(false));
                    GUILayout.EndHorizontal();
                }
                
                GUILayout.Space(10);

                so.ApplyModifiedProperties();
            }
            else
            {
                GUILayout.Label("This Audio Container List is Empty");
            }
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(audioContainerList);
        }
    }

    void CreateNewContainerList()
    {
        //No overwrite protection
        //This should probably get a string from the user to create a new name and pass it in
        viewIndex = 1;
        audioContainerList = CreateAudioContainerList.Create();
        if (audioContainerList)
        {
            audioContainerList.audioContainers = new List<AudioContainer>();
            string relPath = AssetDatabase.GetAssetPath(audioContainerList);
            EditorPrefs.SetString("ObjectPath", relPath);
        }
    }

    void OpenContainerList()
    {
        string absPath = EditorUtility.OpenFilePanel("Select Audio Container List", "", "");
        if (absPath.StartsWith(Application.dataPath))
        {
            string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
            audioContainerList = AssetDatabase.LoadAssetAtPath(relPath, typeof(AudioContainerList)) as AudioContainerList;
            if (audioContainerList.audioContainers == null)
                audioContainerList.audioContainers = new List<AudioContainer>();
            if (audioContainerList)
            {
                EditorPrefs.SetString("ObjectPath", relPath);
            }
        }
    }

    void AddContainer()
    {
        AudioContainer newContainer = ScriptableObject.CreateInstance<AudioContainer>();

        AssetDatabase.CreateAsset(newContainer, "Assets/NewContainer.asset");
        AssetDatabase.SaveAssets();

        if (newContainer)
        {
            string relPath = AssetDatabase.GetAssetPath(newContainer);
            EditorPrefs.SetString("ObjectPath", relPath);

            newContainer.containerName = "New Container";
            audioContainerList.audioContainers.Add(newContainer);
            viewIndex = audioContainerList.audioContainers.Count;
        }
    }

    void DeleteContainer(int index)
    {
        audioContainerList.audioContainers.RemoveAt(index);
    }
}
