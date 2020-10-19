using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextScroller : MonoBehaviour
{
    public TextMeshProUGUI m_textBox;
    [SerializeField] float m_scrollSpeed = 10;

    private RectTransform textRectTransform;

    void Awake()
    {
        textRectTransform = m_textBox.GetComponent < RectTransform >();
    }

    IEnumerator Start()
    {
        float height = m_textBox.preferredHeight;
        Vector3 startPos = textRectTransform.position;

        Debug.Log("height starts at " + height);

        float scrollPosition = startPos.y;

        Debug.Log("scrollPosition starts at " + scrollPosition);

        float maxOffset = scrollPosition + height;

        Debug.Log("maxOffset starts at " + maxOffset);

        while (true)
        {
            textRectTransform.position = new Vector3(startPos.x, scrollPosition, startPos.z);

            scrollPosition += m_scrollSpeed * 20 * Time.deltaTime;

            if (scrollPosition > maxOffset)
            {
                scrollPosition = startPos.y;
            }

        yield return null;
        }
    }
}
