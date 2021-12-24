#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
//using Physics2D;
//using UnityEditor.Experimental.AssetImporters;


/*
namespace tempSpace
{
    class myClass : UnityEngine.AssetPostProcessor
    {

    }
}
*/

public class TexturePostProcessor : AssetPostprocessor
{
    void OnPostprocessTexture(Texture2D texture)
    {
        TextureImporter importer = assetImporter as TextureImporter;
        importer.maxTextureSize = 8192;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
        importer.mipmapEnabled = false;
        importer.filterMode = FilterMode.Trilinear;

        Object asset = AssetDatabase.LoadAssetAtPath(importer.assetPath, typeof(Texture2D));
        if (asset)
        {
            EditorUtility.SetDirty(asset);
        }
        //else
        //{
        //    texture.maxTextureSize = 8192;
        //    texture.npotScale = TextureImporterNPOTScale.None;
        //}
    }
}
#endif