using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

namespace UGUOptimization.Editor
{
    /// <summary>
    /// Sprite Atlas快速创建工具
    /// </summary>
    public class SpriteAtlasCreator
    {
        [MenuItem("Assets/UGUI优化/创建Sprite Atlas")]
        public static void CreateSpriteAtlas()
        {
            string path = "Assets/Sprites/Atlases";
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parent = System.IO.Path.GetDirectoryName(path);
                string folder = System.IO.Path.GetFileName(path);
                AssetDatabase.CreateFolder(parent, folder);
            }

            string atlasPath = $"{path}/UIAtlas.spriteatlas";
            SpriteAtlas atlas = new SpriteAtlas();
            AssetDatabase.CreateAsset(atlas, atlasPath);

            SpriteAtlasPackingSettings packingSettings = new SpriteAtlasPackingSettings
            {
                blockOffset = 1,
                enableRotation = true,
                enableTightPacking = true,
                padding = 2
            };

            SpriteAtlasTextureSettings textureSettings = new SpriteAtlasTextureSettings
            {
                anisoLevel = 1,
                filterMode = FilterMode.Bilinear,
                generateMipMaps = false,
                maxWidth = 2048,
                maxHeight = 2048
            };

            atlas.SetPlatformSettings(new TextureImporterPlatformSettings
            {
                name = "Default",
                maxTextureSize = 2048,
                format = TextureImporterFormat.RGBA32,
                compressionQuality = 50
            });

            EditorUtility.SetDirty(atlas);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Sprite Atlas创建成功: {atlasPath}");
        }

        [MenuItem("Assets/UGUI优化/添加选中的Sprite到Atlas")]
        public static void AddSelectedSpritesToAtlas()
        {
            string[] guids = Selection.assetGUIDs;
            if (guids.Length == 0)
            {
                Debug.LogWarning("请先选择要添加的Sprite资源");
                return;
            }

            string atlasPath = EditorUtility.OpenFilePanel("选择Sprite Atlas", "Assets", "spriteatlas");
            if (string.IsNullOrEmpty(atlasPath))
                return;

            atlasPath = FileUtil.GetProjectRelativePath(atlasPath);
            SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);

            if (atlas == null)
            {
                Debug.LogError("未找到有效的Sprite Atlas");
                return;
            }

            Object[] sprites = new Object[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                sprites[i] = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            }

            atlas.Add(sprites);
            EditorUtility.SetDirty(atlas);
            AssetDatabase.SaveAssets();

            Debug.Log($"添加了 {sprites.Length} 个Sprite到Atlas");
        }
    }
}
