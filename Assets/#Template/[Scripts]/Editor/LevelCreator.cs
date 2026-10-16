using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public class LevelCreator : EditorWindow
{
    // 固定的路径配置（请根据您的项目修改）
    private const string TEMPLATE_PATH = "Assets/Resources/LevelTemplate.scenetemplate";
    private const string TARGET_FOLDER = "Assets/#Template/[Levels]/";

    private string levelName = "NewLevel";

    [MenuItem("Template/Level Creator")]
    public static void ShowWindow()
    {
        GetWindow<LevelCreator>("Level Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("创建新关卡", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // 只保留关卡名字输入
        levelName = EditorGUILayout.TextField("关卡名字 (场景名)", levelName);

        EditorGUILayout.Space();

        // 创建按钮
        GUI.enabled = !string.IsNullOrEmpty(levelName);
        if (GUILayout.Button("创建关卡", GUILayout.Height(30)))
        {
            CreateLevel();
        }
        GUI.enabled = true;
    }

    private void CreateLevel()
    {
        // 检查模板是否存在
        if (!File.Exists(TEMPLATE_PATH))
        {
            EditorUtility.DisplayDialog("错误", $"找不到模板场景：{TEMPLATE_PATH}", "确定");
            return;
        }

        // 确保目标文件夹存在
        if (!Directory.Exists(TARGET_FOLDER))
        {
            Directory.CreateDirectory(TARGET_FOLDER);
            AssetDatabase.Refresh();
        }

        // 构建完整的保存路径
        string fullPath = TARGET_FOLDER + levelName + ".unity";

        // 检查是否已存在同名场景
        if (File.Exists(fullPath))
        {
            if (!EditorUtility.DisplayDialog("确认覆盖", $"场景 {levelName} 已存在，是否覆盖？", "覆盖", "取消"))
            {
                return;
            }
        }

        try
        {
            // 使用SceneTemplate方式创建（如果支持）
            bool useTemplate = TryCreateWithSceneTemplate(fullPath);
            
            if (!useTemplate)
            {
                // 如果不支持或失败，使用普通复制
                CopySceneAsFile();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"Scene Template方式创建失败，尝试普通复制：{ex.Message}");
            CopySceneAsFile();
        }
    }

    private bool TryCreateWithSceneTemplate(string fullPath)
    {
        // 检查是否支持SceneTemplate功能
        // 通过检查模板文件是否是SceneTemplate类型
        try
        {
            // 使用反射或直接加载对象来判断
            UnityEngine.Object templateAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(TEMPLATE_PATH);
            
            if (templateAsset == null)
            {
                return false;
            }

            // 检查是否可以使用SceneTemplate功能（Unity 2020.3+）
            // 这里使用更通用的方法：如果加载的对象有特定的类型标记
            string assetType = templateAsset.GetType().Name;
            
            if (assetType.Contains("SceneTemplate"))
            {
                // 尝试使用SceneTemplate相关功能
                // 由于API兼容性问题，我们使用更安全的方法
                return TryCreateWithTemplateAPI(templateAsset, fullPath);
            }
            
            return false;
        }
        catch
        {
            return false;
        }
    }

    private bool TryCreateWithTemplateAPI(UnityEngine.Object templateAsset, string fullPath)
    {
        try
        {
            // 使用UnityEditor.SceneTemplate命名空间（如果可用）
            // 这里使用反射来调用SceneTemplate相关功能
            var templateType = System.Type.GetType("UnityEditor.SceneTemplate.SceneTemplateAsset, UnityEditor");
            
            if (templateType == null || !templateType.IsAssignableFrom(templateAsset.GetType()))
            {
                return false;
            }

            // 尝试使用SceneTemplateService
            var serviceType = System.Type.GetType("UnityEditor.SceneTemplate.SceneTemplateService, UnityEditor");
            if (serviceType != null)
            {
                var instantiateMethod = serviceType.GetMethod("InstantiateTemplate", 
                    new System.Type[] { templateType, typeof(string) });
                
                if (instantiateMethod != null)
                {
                    var result = instantiateMethod.Invoke(null, new object[] { templateAsset, fullPath });
                    if (result is bool boolResult && boolResult)
                    {
                        AssetDatabase.Refresh();
                        UnityEngine.SceneManagement.Scene scene = EditorSceneManager.OpenScene(fullPath);
                        Debug.Log($"成功使用Scene Template创建并打开场景：{fullPath}");
                        EditorUtility.DisplayDialog("成功", $"关卡 '{levelName}' 已使用模板创建在 {TARGET_FOLDER}", "确定");
                        return true;
                    }
                }
            }
            
            return false;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"SceneTemplate API调用失败：{ex.Message}");
            return false;
        }
    }

    private void CopySceneAsFile()
    {
        try
        {
            string fullPath = TARGET_FOLDER + levelName + ".unity";
            
            // 复制场景文件
            File.Copy(TEMPLATE_PATH, fullPath, true);

            // 复制场景相关的.meta文件（如果存在）
            string templateMeta = TEMPLATE_PATH + ".meta";
            if (File.Exists(templateMeta))
            {
                File.Copy(templateMeta, fullPath + ".meta", true);
            }

            // 刷新资源数据库
            AssetDatabase.Refresh();

            // 打开新创建的场景
            UnityEngine.SceneManagement.Scene scene = EditorSceneManager.OpenScene(fullPath);
            if (scene.IsValid())
            {
                Debug.Log($"成功创建并打开场景：{fullPath}");
                EditorUtility.DisplayDialog("成功", $"关卡 '{levelName}' 已创建在 {TARGET_FOLDER}", "确定");
            }
            else
            {
                Debug.LogWarning($"场景已创建但未能自动打开：{fullPath}");
                EditorUtility.DisplayDialog("提示", $"场景已创建，请手动打开：{fullPath}", "确定");
            }
        }
        catch (System.Exception ex)
        {
            EditorUtility.DisplayDialog("错误", $"创建关卡时发生错误：{ex.Message}", "确定");
            Debug.LogError($"创建关卡失败：{ex}");
        }
    }
}