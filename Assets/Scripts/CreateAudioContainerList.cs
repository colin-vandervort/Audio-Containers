using UnityEngine;
using UnityEditor;

public class CreateAudioContainerList
{
    [MenuItem("Assets/Create/Audio Container List")]
    public static AudioContainerList Create()
    {
        AudioContainerList asset = ScriptableObject.CreateInstance<AudioContainerList>();

        AssetDatabase.CreateAsset(asset, "Assets/AudioContainerList.asset");
        AssetDatabase.SaveAssets();
        return asset;
    }
}
