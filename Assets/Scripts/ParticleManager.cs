using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private int startingPool;
    [SerializeField] private bool poolCanExpand;

    private List<GameObject> particlePool = new();
    private int currentIndex = 0;

    private GameObject GetNextParticle()
    {
        Debug.Log(currentIndex);
        Debug.Log(particlePool.Count);
        GameObject particle = particlePool[currentIndex];
        if (particle.activeSelf && poolCanExpand)
        {
            particle = AddNewParticleToPool();
            currentIndex = particlePool.Count - 1;
        }
        else particle.SetActive(false);
        AdvanceIndex();
        return particle;
    }

    private GameObject AddNewParticleToPool()
    {
        GameObject particle = Instantiate(particlePrefab, transform);
        particlePool.Add(particle);
        return particle;
    }

    private void EmitParticle(GameObject particle, float lifetime, Texture image, Vector2 minPosition, Vector2 maxPosition, Vector2 minVelocity, Vector2 maxVelocity, Vector2 minAcceleration, Vector2 maxAcceleration,
        Vector3 minRotation, Vector3 maxRotation, Vector3 minRotationalVelocity, Vector3 maxRotationalVelocity, Vector3 minRotationalAcceleration, Vector3 maxRotationalAcceleration)
    {
        Particle script = particle.GetComponent<Particle>();
        particle.SetActive(true);
        script.lifetime = lifetime;
        Debug.Log(particle.GetComponent<RawImage>().texture);
        particle.GetComponent<RawImage>().texture = image;
        particle.transform.position = new Vector2(Random.Range(minPosition.x, maxPosition.x), Random.Range(minPosition.y, maxPosition.y));
        script.velocity = new Vector2(Random.Range(minVelocity.x, maxVelocity.x), Random.Range(minVelocity.y, maxVelocity.y));
        script.acceleration = new Vector2(Random.Range(minAcceleration.x, maxAcceleration.x), Random.Range(minAcceleration.y, maxAcceleration.y));
        particle.transform.eulerAngles = new Vector3(Random.Range(minRotation.x, maxRotation.x), Random.Range(minRotation.y, maxRotation.y), Random.Range(minRotation.z, maxRotation.z));
        script.rotationalVelocity = new Vector3(Random.Range(minRotationalVelocity.x, maxRotationalVelocity.x), Random.Range(minRotationalVelocity.y, maxRotationalVelocity.y), Random.Range(minRotationalVelocity.z, maxRotationalVelocity.z));
        script.rotationalAcceleration = new Vector3(Random.Range(minRotationalAcceleration.x, maxRotationalAcceleration.x), Random.Range(minRotationalAcceleration.y, maxRotationalAcceleration.y), Random.Range(minRotationalAcceleration.z, maxRotationalAcceleration.z));
    }

    /// <summary>
    /// Emit a specified number of particles over a given timeframe, with a specified image and physical properties.
    /// Make sure to run this as a coroutine.
    /// </summary>
    /// <param name="number">The number of particles to emit</param>
    /// <param name="time">The time over which the particles are to be emitted</param>
    /// <param name="lifetime">The lifetime of the particles</param>
    /// <param name="image">The image to be displayed by the particles</param>
    /// <param name="minPosition">The minimum position of each particle</param>
    /// <param name="maxPosition">The maximum position of each particle</param>
    /// <param name="minVelocity">The minimum velocity of each particle</param>
    /// <param name="maxVelocity">The maximum velocity of each particle</param>
    /// <param name="minAcceleration">The minimum acceleration of each particle</param>
    /// <param name="maxAcceleration">The maximum acceleration of each particle</param>
    /// <param name="minRotation">The minimum rotation of each particle</param>
    /// <param name="maxRotation">The maximum rotation of each particle</param>
    /// <param name="minRotationalVelocity">The minimum rotational velocity of each particle</param>
    /// <param name="maxRotationalVelocity">The maximum rotational velocity of each particle</param>
    /// <param name="minRotationalAcceleration">The minimum rotational acceleration of each particle</param>
    /// <param name="maxRotationalAcceleration">The maximum rotational acceleration of each particle</param>
    /// <returns>null</returns>
    public IEnumerator EmitParticles(int number, float time, float lifetime, Texture image, Vector2 minPosition, Vector2 maxPosition, Vector2 minVelocity, Vector2 maxVelocity, Vector2 minAcceleration, Vector2 maxAcceleration,
        Vector3 minRotation, Vector3 maxRotation, Vector3 minRotationalVelocity, Vector3 maxRotationalVelocity, Vector3 minRotationalAcceleration, Vector3 maxRotationalAcceleration)
    {
        for (int i = 0; i < number; i++)
        {
            GameObject particle = GetNextParticle();
            EmitParticle(particle, lifetime, image, minPosition, maxPosition, minVelocity, maxVelocity, minAcceleration, maxAcceleration, minRotation, maxRotation, minRotationalVelocity, maxRotationalVelocity, minRotationalAcceleration, maxRotationalAcceleration);
            yield return new WaitForSeconds(time / (number - 1));
        }
        yield return null;
    }

    private void AdvanceIndex()
    {
        currentIndex++;
        if (currentIndex >= particlePool.Count)
        {
            currentIndex -= particlePool.Count;
        }
    }

    void Awake()
    {
        for (int i = 0; i < startingPool; i++)
        {
            AddNewParticleToPool();
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var particle in particlePool)
        {
            if (!particle.activeSelf) continue;
            Particle script = particle.GetComponent<Particle>();
            script.lifetime -= Time.deltaTime;
            if (script.lifetime <= 0)
            {
                particle.SetActive(false);
                continue;
            }
            particle.transform.position += new Vector3(script.velocity.x, script.velocity.y) * Time.deltaTime;
            particle.transform.rotation *= Quaternion.Euler(script.rotationalVelocity* Time.deltaTime);
            script.velocity += script.acceleration * Time.deltaTime;
            script.rotationalVelocity += script.rotationalAcceleration * Time.deltaTime;
        }
    }
}
