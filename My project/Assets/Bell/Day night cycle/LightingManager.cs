using UnityEngine;

[ExecuteInEditMode]
public class LightingManager : MonoBehaviour
{
    [SerializeField] private Light directionalLight;
    [SerializeField] private LightingPreset preset;

    [Tooltip("In hours")]
    [SerializeField, Range(0, 24)] private float timeOfDay;

    [Tooltip("Speed of day and night cycle. 1 is normal, 2 is half of normal, 0.5 is double of normal")]
    [SerializeField] private float timeSpeed;

    //Rain
    [SerializeField] int probabilityOfRain = 1;
    [SerializeField] ParticleSystem rainParticle;
    public bool newDay = false;

    private void Update()
    {
        if (preset == null)
        {
            return;
        }
        if (Application.isPlaying)
        {
            timeOfDay += Time.deltaTime / timeSpeed;
            timeOfDay %= 24;
            UpdateLighting(timeOfDay / 24);
        }
        else
        {
            UpdateLighting(timeOfDay / 24);
        }

        if (timeOfDay > 4 && timeOfDay < 18 && newDay == false)
        {
            newDay = true;
            int random = Random.Range(1, 101);
            if (probabilityOfRain <= random)
            {
                rainParticle.Play();
                Debug.Log("Rain");
            }
            else
            {
                rainParticle.Stop();
                Debug.Log("No rain");
            }
        }
        else if (timeOfDay > 18 && newDay == true)
        {
            newDay = false;
        }
    }
    private void OnValidate()
    {
        if (directionalLight != null)
        {
            return;
        }
        if (RenderSettings.sun != null)
        {
            directionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = FindObjectsByType<Light>(FindObjectsSortMode.None);
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    directionalLight = light;
                }
            }
        }
    }

    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = preset.FogColor.Evaluate(timePercent);

        if (directionalLight != null)
        {
            directionalLight.color = preset.DirectionalColor.Evaluate(timePercent);
            directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, -170, 0));
        }
    }
}
