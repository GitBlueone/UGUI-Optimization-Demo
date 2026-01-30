using System.Text;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.UI;

namespace UGUOptimization.OptimizationDemo
{
    /// <summary>
    /// 实时显示Draw Call和其他性能数据的监控面板
    /// 使用Unity 2020.2+的ProfilerRecorder API
    /// </summary>
    public class PerformanceMonitor : MonoBehaviour
    {
        [Header("显示设置")]
        [SerializeField] private bool m_ShowOnStart = true;
        [SerializeField] private Vector2 m_Position = new Vector2(10, 10);
        [SerializeField] private int m_FontSize = 16;

        [Header("更新频率")]
        [SerializeField] private float m_UpdateInterval = 0.5f;

        private ProfilerRecorder m_DrawCallsRecorder;
        private ProfilerRecorder m_SetPassCallsRecorder;
        private ProfilerRecorder m_BatchesRecorder;
        private ProfilerRecorder m_TrianglesRecorder;

        private float m_UpdateTimer;
        private long m_LastDrawCallCount;
        private StringBuilder m_StringBuilder;
        private bool m_IsVisible;

        private GUIStyle m_Style;

        private void Awake()
        {
            m_StringBuilder = new StringBuilder(256);
            m_IsVisible = m_ShowOnStart;
        }

        private void OnEnable()
        {
            m_DrawCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
            m_SetPassCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "SetPass Calls Count");
            m_BatchesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Batches Count");
            m_TrianglesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count");
        }

        private void OnDisable()
        {
            m_DrawCallsRecorder.Dispose();
            m_SetPassCallsRecorder.Dispose();
            m_BatchesRecorder.Dispose();
            m_TrianglesRecorder.Dispose();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                m_IsVisible = !m_IsVisible;
            }

            m_UpdateTimer += Time.deltaTime;
            if (m_UpdateTimer >= m_UpdateInterval)
            {
                m_UpdateTimer = 0;
            }
        }

        private void OnGUI()
        {
            if (!m_IsVisible)
                return;

            if (m_Style == null)
            {
                m_Style = new GUIStyle(GUI.skin.label);
                m_Style.fontSize = m_FontSize;
                m_Style.normal.textColor = Color.white;
                m_Style.alignment = TextAnchor.UpperLeft;
            }

            GUILayout.BeginArea(new Rect(m_Position.x, m_Position.y, 400, 300));
            GUILayout.BeginVertical("box");

            GUILayout.Label("=== 性能监控面板 ===", m_Style);
            GUILayout.Label($"按 F1 切换显示/隐藏", m_Style);
            GUILayout.Space(10);

            m_StringBuilder.Clear();

            if (m_DrawCallsRecorder.Valid)
                m_StringBuilder.AppendLine($"Draw Calls: {m_DrawCallsRecorder.LastValue}");

            if (m_SetPassCallsRecorder.Valid)
                m_StringBuilder.AppendLine($"SetPass Calls: {m_SetPassCallsRecorder.LastValue}");

            if (m_BatchesRecorder.Valid)
                m_StringBuilder.AppendLine($"Batches: {m_BatchesRecorder.LastValue}");

            if (m_TrianglesRecorder.Valid)
                m_StringBuilder.AppendLine($"Triangles: {m_TrianglesRecorder.LastValue}");

            GUILayout.Label(m_StringBuilder.ToString(), m_Style);

            GUILayout.Space(10);

            GUILayout.Label($"FPS: {Mathf.RoundToInt(1f / Time.deltaTime)}", m_Style);
            GUILayout.Label($"内存: {System.GC.GetTotalMemory(false) / 1024 / 1024} MB", m_Style);

            GUILayout.Space(10);

            Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            GUILayout.Label($"Canvas数量: {canvases.Length}", m_Style);

            foreach (Canvas canvas in canvases)
            {
                string renderMode = GetRenderModeString(canvas.renderMode);
                GUILayout.Label($"  - {canvas.name} ({renderMode})", m_Style);
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private string GetRenderModeString(RenderMode mode)
        {
            switch (mode)
            {
                case RenderMode.ScreenSpaceOverlay:
                    return "Overlay";
                case RenderMode.ScreenSpaceCamera:
                    return "Camera";
                case RenderMode.WorldSpace:
                    return "World";
                default:
                    return "Unknown";
            }
        }
    }
}
