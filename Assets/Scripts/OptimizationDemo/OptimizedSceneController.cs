using UnityEngine;
using UnityEngine.UI;

namespace UGUOptimization.OptimizationDemo
{
    /// <summary>
    /// 优化场景控制器 - 最小化Draw Calls
    /// </summary>
    public class OptimizedSceneController : MonoBehaviour
    {
        [Header("优化设置")]
        [SerializeField] private int m_ImageCount = 50;
        [SerializeField] private Canvas m_StaticCanvas;
        [SerializeField] private Canvas m_DynamicCanvas;

        [Header("性能监控")]
        [SerializeField] private bool m_EnablePerformanceMonitor = true;

        private void Start()
        {
            GenerateOptimizedUI();
        }

        private void GenerateOptimizedUI()
        {
            if (m_StaticCanvas == null || m_DynamicCanvas == null)
            {
                Debug.LogError("未设置Canvas！");
                return;
            }

            GenerateStaticUI();
            GenerateDynamicUI();

            Debug.Log($"优化场景：生成了 {m_ImageCount} 个UI元素（已优化合批）");
        }

        private void GenerateStaticUI()
        {
            GameObject container = new GameObject("StaticUI_Container");
            container.transform.SetParent(m_StaticCanvas.transform, false);

            int staticCount = m_ImageCount / 2;
            for (int i = 0; i < staticCount; i++)
            {
                CreateOptimizedImage(i, container.transform, false);
            }
        }

        private void GenerateDynamicUI()
        {
            GameObject container = new GameObject("DynamicUI_Container");
            container.transform.SetParent(m_DynamicCanvas.transform, false);

            int dynamicCount = m_ImageCount - (m_ImageCount / 2);
            for (int i = 0; i < dynamicCount; i++)
            {
                CreateOptimizedImage(i, container.transform, true);
            }
        }

        private void CreateOptimizedImage(int index, Transform parent, bool isDynamic)
        {
            GameObject imageObj = new GameObject($"Image_{(isDynamic ? "D" : "S")}_{index}");
            imageObj.transform.SetParent(parent, false);

            Image image = imageObj.AddComponent<Image>();

            RectTransform rectTransform = imageObj.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(50, 50);

            int row = index / 10;
            int col = index % 10;

            if (isDynamic)
            {
                rectTransform.anchoredPosition = new Vector2(col * 60 + 300, -row * 60);
            }
            else
            {
                rectTransform.anchoredPosition = new Vector2(col * 60, -row * 60);
            }

            if (index % 10 == 0)
            {
                imageObj.AddComponent<RectMask2D>();
            }
        }

        [ContextMenu("重新生成UI")]
        public void RegenerateUI()
        {
            ClearExistingUI();
            GenerateOptimizedUI();
        }

        [ContextMenu("清除UI")]
        public void ClearExistingUI()
        {
            if (m_StaticCanvas != null)
            {
                Transform[] children = m_StaticCanvas.transform.GetComponentsInChildren<Transform>();
                foreach (Transform child in children)
                {
                    if (child.name.StartsWith("StaticUI_Container"))
                    {
                        DestroyImmediate(child.gameObject);
                    }
                }
            }

            if (m_DynamicCanvas != null)
            {
                Transform[] children = m_DynamicCanvas.transform.GetComponentsInChildren<Transform>();
                foreach (Transform child in children)
                {
                    if (child.name.StartsWith("DynamicUI_Container"))
                    {
                        DestroyImmediate(child.gameObject);
                    }
                }
            }
        }
    }
}
