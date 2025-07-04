using System.Collections;
using System.Collections.Generic;
using RTLTMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ShowInformation : MonoBehaviour
{
    [SerializeField] GameObject info;
    [SerializeField] RTLTextMeshPro TitleText;
    [SerializeField] RTLTextMeshPro DescriptionText;
    [SerializeField] public AudioSource narrationSource;

    public void SetTitle(string titleTxt)
    {
        TitleText.text = titleTxt;
    }

    public void SetDescription(string decriptionTxt)
    {
        DescriptionText.text = decriptionTxt;
    }

    public void PlayNarration(AudioClip descriptionClip)
    {
        narrationSource.clip = descriptionClip;
        if (!narrationSource.isPlaying)
            narrationSource.Play();
    }

    public void ShowInf()
    {
        info.SetActive(true);
    }

    public void HideInf()
    {
        info.SetActive(false);
    }
}
