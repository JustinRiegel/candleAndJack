using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialObject : MonoBehaviour
{
    [TextArea(3, 5)]
    [SerializeField] string tutorialText;
    [SerializeField] Material outlineMaterial;
    [SerializeField] GameObject UIElement;
    [SerializeField] Vector2 UIOffset = new Vector2(0, 1.5f);

    private Material defaultMaterial;
    private SpriteRenderer spriteRenderer;
    private Transform jackTransform;
    private bool jackInTrigger;
    private bool hasSprite;
    private Vector3 threeDUIOffset;

    private GameObject currentUIElement;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        jackTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();

        hasSprite = (spriteRenderer != null);

        if (UIOffset == null)
        {
            threeDUIOffset = new Vector3(0, 1.5f, 0);
        }
        else
        {
            threeDUIOffset = new Vector3(UIOffset.x, UIOffset.y, 0);
        }

        if (tutorialText == null)
        {
            tutorialText = "Missing Tutorial string.";
        }

        if (hasSprite)
        {
            defaultMaterial = spriteRenderer.material;

            if (outlineMaterial == null)
            {
                outlineMaterial = defaultMaterial;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (jackInTrigger)
        {
            if (currentUIElement != null)
            {
                currentUIElement.transform.position = jackTransform.position + threeDUIOffset;
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jackInTrigger = true;
            if (currentUIElement != null)
            {
                GameObject.Destroy(currentUIElement);
            }
            currentUIElement = Instantiate(UIElement, jackTransform.position + threeDUIOffset, Quaternion.identity, transform);
            TextMeshProUGUI textChild = currentUIElement.GetComponentInChildren<TextMeshProUGUI>();
            if (textChild != null)
            {
                textChild.text = tutorialText;
            }
            if (hasSprite)
            {
                spriteRenderer.material = outlineMaterial;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jackInTrigger = false;
            if (currentUIElement != null)
            {
                GameObject.Destroy(currentUIElement);
            }
            if (hasSprite)
            {
                spriteRenderer.material = defaultMaterial;
            }
        }
    }

}
