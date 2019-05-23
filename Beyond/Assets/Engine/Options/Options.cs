using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;
using UnityEngine.Rendering;

public class Options : MonoBehaviour
{
    public Volume fxVolume1;

    public Tonemapping tonemapLayer;
    public AmbientOcclusion aoLayer;
    public Bloom bloomLayer;
    public MotionBlur mBlurLayer;

    void Start()
    {
        SetVolumeComponent();
    }
    void SetVolumeComponent()
    {
        fxVolume1.profile.TryGet(out tonemapLayer);
        fxVolume1.profile.TryGet(out aoLayer);
        fxVolume1.profile.TryGet(out bloomLayer);
        fxVolume1.profile.TryGet(out mBlurLayer);
    }

    public void EnableToneMapping(){
        EnableVolumeComp(tonemapLayer);
    }

    public void DisableToneMapping()
    {
        DisableVolumeComp(tonemapLayer);
    }
    public void EnableAO()
    {
        EnableVolumeComp(aoLayer);
    }

    public void DisableAO()
    {
        DisableVolumeComp(aoLayer);
    }

    public void EnableBloom()
    {
        EnableVolumeComp(bloomLayer);
    }

    public void DisableBloom()
    {
        DisableVolumeComp(bloomLayer);
    }

    public void EnableMBlur()
    {
        EnableVolumeComp(mBlurLayer);
    }

    public void DisableMBlur()
    {
        DisableVolumeComp(mBlurLayer);
    }

    public void ToggleToneMapping()
    {
        ToggleVolumeComp(tonemapLayer);
    }

    public void ToggleAO()
    {
        ToggleVolumeComp(aoLayer);
    }

    public void ToggleBloom()
    {
        ToggleVolumeComp(bloomLayer);
    }

    public void ToggleMBLur()
    {
        ToggleVolumeComp(mBlurLayer);
    }

    void ToggleVolumeComp(VolumeComponent v)
    {
        v.active = !v.active;
    }

    void EnableVolumeComp(VolumeComponent v)
    {
        v.active = true;
    }

    void DisableVolumeComp(VolumeComponent v)
    {
        v.active = false;
    }
}
