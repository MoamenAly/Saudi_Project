using System.Collections;
using System.Collections.Generic;
using RTLTMPro;
using UnityEngine;

public class ShowInformation : MonoBehaviour
{
    public GameObject Inf;
    public RTLTextMeshPro informationText;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowInf(string text)
    {
        Inf.SetActive(true);
        informationText.text = text;
    }
    public void HideInf()
    {
        Inf.SetActive(false);
    }
}
