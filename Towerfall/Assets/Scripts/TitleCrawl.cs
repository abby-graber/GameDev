using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Unity.Collections.LowLevel.Unsafe;



public class TitleCrawl : MonoBehaviour
{
    [Header("Text Settings")]
    [SerializeField] private TextMeshProUGUI introText;
    [SerializeField] [TextArea(10,20)] private string crawlText;

    [Header("Motion")]
    [SerializeField] private float scrollSpeed = 30f;
    [SerializeField] private float titltAngle = 45f;
    [SerializeField] private float startDistance = 500f;
    [SerializeField] private float endDistance = -1000f;

    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;

    private RectTransform textTransform;
    private Vector3 startPosition;
    private bool isPlaying = false;

    void Start()
    {
        textTransform = introText.GetComponent<RectTransform>();

        introText.text = crawlText;

        startPosition = new Vector3(0,startDistance,0);
        textTransform.localPosition = startPosition;

        textTransform.localRotation = Quaternion.Euler(titltAngle,0,0);

        StartCrawl();


    }
    public void StartCrawl()
    {
        if (!isPlaying)
        {
            isPlaying = true;
            textTransform.localPosition = startPosition;

            if (musicSource != null)
            {
                musicSource.Play();
            }
            StartCoroutine(ScrollText());
        }
    }
    private IEnumerator ScrollText()
    {
        while (textTransform.localPosition.y < endDistance)
        {
            textTransform.localPosition += Vector3.up * scrollSpeed * Time.deltaTime;
            yield return null;
        }
        isPlaying = false;
    }
    // Update is called once per frame
    public void ResetCrawl()
    {
        StopAllCoroutines();
        textTransform.localPosition = startPosition;
        isPlaying = false;

        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }
}
