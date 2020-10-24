using UnityEngine;
using System.Linq;

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
    [SerializeField] PathfinderAI _currentSeeker;

    SpriteRenderer spriteRenderer;
    private bool lightOn = false;
    private bool _isJackInLight = false;
    private bool _disableAbilityIsReady = false;
    private Vector3 threeDLightOffset;
    private Vector3 threeDAttractorOffset;
    private Vector3 threeDUIOffset;
    private LightManager lightManager;
    private GameObject _helperText;
    private GameObject attractorInstance;

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

        _helperText = Instantiate(interactUIElement, transform.position + threeDUIOffset, Quaternion.identity, transform);
        _helperText.SetActive(false);

        TurnOnLight();
    }

    public void Update()
    {
        //put stuff here to display the helper text is jack is near and disable ability is ready
        if (lightOn && _isJackInLight)
        {
            spriteRenderer.material = outlineMaterial;
            if(_disableAbilityIsReady)
            {
                _helperText.SetActive(true);
            }
            
        }
        else
        {
            spriteRenderer.material = defaultMaterial;
            _helperText.SetActive(false);
        }
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
            if (attractor == null)
            {
                Debug.LogError(name + "Does not have an attractor set");
                return;
            }
            if (attractorInstance != null)
            {
                Destroy(attractorInstance);
            }
            attractorInstance = Instantiate(attractor, transform.position + threeDAttractorOffset, Quaternion.identity, transform);
            lightManager.AddAttractor(attractorInstance);
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
            if (attractor == null)
            {
                Debug.LogError(name + " does not have an attractor set");
                return;
            }
            if (attractorInstance != null)
            {
                foreach (Transform child in transform)
                {
                    if (child.CompareTag("attractor"))
                    {
                        lightManager.RemoveAttractor(child.gameObject);
                        SetCurrentSeeker(null);
                        GameObject.Destroy(child.gameObject);
                    }
                }
            }
        }
    }

    public PathfinderAI GetCurrentSeeker()
    {
        return _currentSeeker;
    }

    public void SetCurrentSeeker(PathfinderAI currentSeeker)
    {
        _currentSeeker = currentSeeker;
    }
    
    public void SetJackInLight(bool isJackInLight)
    {
        _isJackInLight = isJackInLight;
    }

    public void SetDisableAbilityIsReady(bool disableReady)
    {
        _disableAbilityIsReady = disableReady;
    }
}
