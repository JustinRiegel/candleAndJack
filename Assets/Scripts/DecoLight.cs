using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoLight : MonoBehaviour
{
    [SerializeField] GameObject lightType;
    [SerializeField] Vector2 lightOffset;
    [SerializeField] Sprite onSprite;
    [SerializeField] Sprite offSprite;
    [SerializeField] GameObject attractor;
    [SerializeField] Vector2 attractorOffset;
    [SerializeField] float timeToAutoOn = -1;
    [SerializeField] Material defaultMaterial;
    [SerializeField] Material outlineMaterial;
    [SerializeField] GameObject interactUIElement;
    [SerializeField] Vector2 interactUIOffset;

    SpriteRenderer spriteRenderer;
    private bool lightOn = false;
    private Vector3 threeDLightOffset;
    private Vector3 threeDAttractorOffset;
    private Vector3 threeDUIOffset;
    private LightManager lightManager;

    // Start is called before the first frame update
    void Start()
    {
        lightManager = FindObjectOfType<LightManager>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (lightOffset == null)
        {
            threeDLightOffset = new Vector3(0, 0, 0);
        }
        else
        {
            threeDLightOffset = new Vector3(lightOffset.x, lightOffset.y, 0);
        }


        if (attractorOffset == null)
        {
            threeDAttractorOffset = new Vector3(0, 0, 0);
        }
        else
        {
            threeDAttractorOffset = new Vector3(attractorOffset.x, attractorOffset.y, 0);
        }

        if (interactUIOffset == null)
        {
            threeDUIOffset = new Vector3(0, 0, 0);
        }
        else
        {
            threeDUIOffset = new Vector3(interactUIOffset.x, interactUIOffset.y, 0);
        }

        TurnOnLight();
    }

    public void TurnOffLight()
    {
        if (lightOn)
        {
            //light stuff
            spriteRenderer.sprite = offSprite;
            foreach (Transform child in transform)
            {
                if (child.CompareTag("light"))
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
            lightOn = false;

            //attractor stuff
            GameObject newAttractor = Instantiate(attractor, transform.position + threeDAttractorOffset, Quaternion.identity, transform) as GameObject;
            lightManager.AddAttractor(newAttractor);
        }

        //Automatically turn the light back on after some amount of time
        if (timeToAutoOn > 0)
        {
            Invoke("TurnOnLight", timeToAutoOn);
        }
    }


    public void TurnOnLight()
    {
        if (!lightOn)
        {
            //light stuff
            spriteRenderer.sprite = onSprite;
            Instantiate(lightType, transform.position + threeDLightOffset, Quaternion.identity, transform);
            lightOn = true;

            //attractor stuff
            foreach (Transform child in transform)
            {
                if (child.CompareTag("attractor"))
                {
                    lightManager.RemoveAttractor(child.gameObject);
                    GameObject.Destroy(child.gameObject);
                }
            }
        }
    }

    public void SetJackInLight(bool isJackInLight)
    {
        if(lightOn && isJackInLight)
        {
            spriteRenderer.material = outlineMaterial;
            Instantiate(interactUIElement, transform.position + threeDUIOffset, Quaternion.identity, transform);
        }
        else
        {
            spriteRenderer.material = defaultMaterial;
            foreach (Transform child in transform)
            {
                if (child.CompareTag("helperUI"))
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
        }
    }    
}
