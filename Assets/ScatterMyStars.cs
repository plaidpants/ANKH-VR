// https://github.com/ReCogMission/FirstTutorials MIT License
using UnityEngine;

public class ScatterMyStars : MonoBehaviour
{
    [SerializeField] ParticleSystem starsParticleSystem;
    private ParticleSystem.Particle[] particles;
    private byte[] alphas;
    private Color32 color32;
    private float radius;
    
    void LateUpdate()
    {
        if (particles == null)
        {
            particles = new ParticleSystem.Particle[starsParticleSystem.main.maxParticles];
            
            int numParticlesAlive = starsParticleSystem.GetParticles(particles);
            alphas = new byte[starsParticleSystem.main.maxParticles];
            for (int i = 0; i < starsParticleSystem.main.maxParticles; i++)
            {
                alphas[i] = particles[i].startColor.a;
            }

            radius = starsParticleSystem.shape.radius;
        }

        for (int i = 0; i < particles.Length; i++)
        {
            color32 = particles[i].startColor;
            color32.a = (byte)Mathf.Clamp(alphas[i] * (starsParticleSystem.transform.TransformPoint(particles[i].position).y - starsParticleSystem.transform.position.y)
                / radius, 0, alphas[i]);
            particles[i].startColor = color32;
        }
        starsParticleSystem.SetParticles(particles, particles.Length, 0);
    }
}
