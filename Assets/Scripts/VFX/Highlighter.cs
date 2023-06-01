using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Highlighter : MonoBehaviour
{
    [SerializeField]
    private List<Renderer> renderers;
    [SerializeField]
    private Color targetColor = Color.white;
    [SerializeField]
    private ParticleSystem particles;

    private List<Color> originalEmissiveColors = new List<Color>();
    private List<Material> materials = new List<Material>();

    [Header("Listening On")]
    public RuntimeSetBase<Highlighter> highlighters;

    public PlayerProjectileReceiver projectileReceiver;
    
    private void OnEnable()
    {
        if(highlighters)
            highlighters.Add(this);
    }

    private void OnDisable()
    {
        if(highlighters)
            highlighters.Remove(this);
    }
    
    private void Awake()
    {
        foreach (var renderer in renderers)
        {
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                renderer.materials[i].EnableKeyword("_EMISSION");
                originalEmissiveColors.Add(renderer.materials[i].GetColor("_EmissionColor"));
            }
            materials.AddRange(new List<Material>(renderer.materials));
        }
    }

    public void ToggleHighlight(bool val)
    {
        if (projectileReceiver)
        { 
            if (projectileReceiver.projectileCount <= 0)
                return;
        }
        if (val)
        {
            foreach (var material in materials)
            {
                // material.SetColor("_EmissionColor", targetColor);
                material.DOColor(targetColor, "_EmissionColor", 0.5f);
            }
            if(particles)
                particles.Play();
        } 
        else
        {
            for (int i = 0; i < materials.Count; i++)
            {
                materials[i].DOColor(originalEmissiveColors[i], "_EmissionColor", 0.5f);
            }
            if(particles)
                    particles.Stop();
        }
    }
}

