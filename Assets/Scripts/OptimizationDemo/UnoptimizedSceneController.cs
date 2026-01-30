using UnityEngine;
using UnityEngine.UI;

namespace UGUOptimization.OptimizationDemo
{
    /// <summary>
    /// 未优化场景控制器 - 产生大量Draw Calls
    /// </summary>
    public class UnoptimizedSceneController : MonoBehaviour
    {
        [Header("自动生成设置")]
        [SerializeField] private int m_ImageCount = 50;
        [SerializeField] private Canvas m_TargetCanvas;

        [Header("性能监控")]
        [SerializeField] private bool m_EnablePerformanceMonitor = true;

        private void Start()
        {
            GenerateUnoptimizedUI();
        }

        private void GenerateUnoptimizedUI()
        {
            if (m_TargetCanvas == null)
            {
                Debug.LogError("未设置目标Canvas！");
                return;
            }

            GameObject container = new GameObject("UnoptimizedUI_Container");
            container.transform.SetParent(m_TargetCanvas.transform, false);

            for (int i = 0; i < m_ImageCount; i++)
            {
                CreateUIImage(i, container.transform);
            }

            Debug.Log($"未优化场景：生成了 {m_ImageCount} 个UI元素");
        }

        private void CreateUIImage(int index, Transform parent)
        {
            GameObject imageObj = new GameObject($"Image_{index}");
            imageObj.transform.SetParent(parent, false);

            Image image = imageObj.AddComponent<Image>();

            RectTransform rectTransform = imageObj.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(50, 50);

            int row = index / 10;
            int col = index % 10;
            rectTransform.anchoredPosition = new Vector2(col * 60, -row * 60);

            if (index % 5 == 0)
            {
                imageObj.AddComponent<Mask>();
            }

            if (index % 3 == 0)
            {
                CanvasGroup canvasGroup = imageObj.AddComponent<CanvasGroup>();
                canvasGroup.alpha = 0.8f;
            }
        }

        [ContextMenu("重新生成UI")]
        public void RegenerateUI()
        {
            ClearExistingUI();
            GenerateUnoptimizedUI();
        }

        [ContextMenu("清除UI")]
        public void ClearExistingUI()
        {
            if (m_TargetCanvas == null)
                return;

            Transform[] children = m_TargetCanvas.transform.GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                if (child.name.StartsWith("UnoptimizedUI_Container"))
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }
    }
}
