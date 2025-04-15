using DG.Tweening;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class OverheadDialogue : MonoBehaviour
{
    TextMeshPro tmPro;

    Sequence seq = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tmPro = GetComponent<TextMeshPro>();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowText(List<string> lines)
    {
        if (lines.Count <= 0) return;

        if (seq != null) return;

        gameObject.SetActive(true);
        string firstText = lines[0];


        seq = DOTween.Sequence(tmPro.text);
		seq.Append(DOTween.To(() => tmPro.text, x => tmPro.text = x, firstText, 1.5f));
		seq.Append(DOTween.To(() => tmPro.text, x => tmPro.text = x, firstText, 1.5f));
		seq.Append(DOTween.To(() => tmPro.text, x => tmPro.text = x, "", 0.1f));
        seq.AppendCallback(() => seq = null);

        seq.Play();

        
    }

}
