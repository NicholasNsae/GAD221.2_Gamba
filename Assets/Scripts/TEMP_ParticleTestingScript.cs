using UnityEngine;

public class TEMP_ParticleTestingScript : MonoBehaviour
{
    [SerializeField] private Sprite sprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Canvas canvas = transform.parent.GetComponent<Canvas>();
        ParticleManager particleManager = GetComponent<ParticleManager>();
        StartCoroutine(particleManager.EmitParticles(100, 5, 5, sprite.texture, new Vector2(0, canvas.pixelRect.height/2f), new Vector2(canvas.pixelRect.width, canvas.pixelRect.height / 2f), new Vector2(-500, 100), new Vector2(500, 500), new Vector2(0, 0), new Vector2(0, -500),
            new Vector3(0,0,0), new Vector3(360,360,360), new Vector3(-360,-360,-360), new Vector3(360,360,360), new Vector3(0, 0, 0), new Vector3(0, 0, 0)));
    }
}
