using UnityEngine;
using UnityEngine.UI;

namespace UGUOptimization.OptimizationDemo
{
    /// <summary>
    /// 快速生成大量UI元素用于Draw Call测试
    /// </summary>
    public class UIGenerator : MonoBehaviour
    {
        [Header("生成设置")]
        [SerializeField] private int m_ImageCount = 50;
        [SerializeField] private bool m_UseRandomSprites = true;
        [SerializeField] private bool m_AddMaskComponent = true;

        [Header("布局设置")]
        [SerializeField] private int m_Columns = 10;
        [SerializeField] private float m_Spacing = 10f;
        [SerializeField] private float m_ImageSize = 50f;

        [Header("Canvas设置")]
        [SerializeField] private Canvas m_TargetCanvas;

        private Sprite[] m_TestSprites;

        public void GenerateUIElements()
        {
            if (m_TargetCanvas == null)
            {
                Debug.LogError("请先指定目标Canvas！");
                return;
            }

            ClearExistingElements();

            GameObject container = new GameObject("GeneratedUI_Container");
            container.transform.SetParent(m_TargetCanvas.transform, false);

            for (int i = 0; i < m_ImageCount; i++)
            {
                GameObject imageObj = new GameObject($"Image_{i}");
                imageObj.transform.SetParent(container.transform, false);

                Image image = imageObj.AddComponent<Image>();

                if (m_TestSprites != null && m_TestSprites.Length > 0)
                {
                    int spriteIndex = m_UseRandomSprites ? Random.Range(0, m_TestSprites.Length) : i % m_TestSprites.Length;
                    image.sprite = m_TestSprites[spriteIndex];
                }

                RectTransform rectTransform = imageObj.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(m_ImageSize, m_ImageSize);

                int row = i / m_Columns;
                int col = i % m_Columns;
                rectTransform.anchoredPosition = new Vector2(
                    col * (m_ImageSize + m_Spacing),
                    -row * (m_ImageSize + m_Spacing)
                );

                if (m_AddMaskComponent && i % 5 == 0)
                {
                    imageObj.AddComponent<Mask>();
                    imageObj.AddComponent<RectMask2D>();
                }
            }

            Debug.Log($"生成了 {m_ImageCount} 个UI元素");
        }

        public void ClearExistingElements()
        {
            if (m_TargetCanvas == null)
                return;

            Transform[] children = m_TargetCanvas.transform.GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                if (child.name.StartsWith("GeneratedUI_Container"))
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }

        public void SetTestSprites(Sprite[] sprites)
        {
            m_TestSprites = sprites;
        }
    }
}
