using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class PostProcessController : MonoBehaviour
{
    public Toggle postProcessToggle;
    
    private PostProcessVolume volume;
    private PostProcessLayer layer;
    
    void Start()
    {
        volume = FindObjectOfType<PostProcessVolume>();
        layer = Camera.main.GetComponent<PostProcessLayer>();
        
        if (volume == null)
        {
            Debug.LogError("你搞啥，根本没有PostProcessVolume");
            return;
        }
        
        if (postProcessToggle != null)
        {
            postProcessToggle.isOn = volume.enabled;
            postProcessToggle.onValueChanged.AddListener(OnToggleChanged);
        }
    }
    
    void OnToggleChanged(bool isOn)
    {
        if (volume != null)
        {
            volume.enabled = isOn;
            volume.weight = isOn ? 1f : 0f;
        }
        
        if (layer != null)
            layer.enabled = isOn;
    }
}