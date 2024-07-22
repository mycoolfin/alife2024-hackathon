using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class SoundSphere : MonoBehaviour
{
    public GameObject debugActivator;
    public AudioClip clip;
    public AudioMixerGroup mixerGroup;
    public float pitchMultiplier = 1f;
    public float particleActivationThreshold = 1f;

    private List<List<List<SoundZone>>> soundZones = new();

    void Start()
    {
        int platformIndex = 0;
        foreach (Transform platform in transform)
        {
            List<List<SoundZone>> platformZones = new();
            int rowIndex = 0;
            foreach (Transform row in platform)
            {
                List<SoundZone> rowZones = new();
                int columnIndex = 0;
                foreach (Transform column in row)
                {
                    SoundZone soundZone = column.GetComponent<SoundZone>();
                    if (debugActivator != null) soundZone.debug = true;
                    soundZone.SetMixerGroup(mixerGroup);
                    soundZone.SetAudioClip(clip);
                    soundZone.SetPitchMultiplier(pitchMultiplier);
                    soundZone.SetPitch(platformIndex);
                    soundZone.SetChorusDepth(rowIndex);
                    soundZone.SetChorusRate(columnIndex);
                    rowZones.Add(soundZone);
                    columnIndex++;
                }
                platformZones.Add(rowZones);
                rowIndex++;
            }
            soundZones.Add(platformZones);
            platformIndex++;
        }
    }

    void Update()
    {
        // Debug.
        if (debugActivator != null)
        {
            int[] zoneIndex = ParticlePositionToZoneIndex(debugActivator.transform.position);
            if (!(zoneIndex[0] < 0 || zoneIndex[0] > 9
                || zoneIndex[1] < 0 || zoneIndex[1] > 9
                || zoneIndex[2] < 0 || zoneIndex[2] > 9))
            {
                SoundZone activatedZone = soundZones[zoneIndex[0]][zoneIndex[1]][zoneIndex[2]];
                activatedZone.Activate();
            }
        }
    }

    public void SetParticleData(List<Vector4> particleData)
    {
        foreach (Vector4 activeParticle in particleData.Where(p => p[3] > particleActivationThreshold))
        {
            int[] zoneIndex = ParticlePositionToZoneIndex(new Vector3(activeParticle.x, activeParticle.y, activeParticle.z));
            if (!(zoneIndex[0] < 0 || zoneIndex[0] > 9
                || zoneIndex[1] < 0 || zoneIndex[1] > 9
                || zoneIndex[2] < 0 || zoneIndex[2] > 9))
            {
                SoundZone activatedZone = soundZones[zoneIndex[0]][zoneIndex[1]][zoneIndex[2]];
                activatedZone.Activate();
            }
        }
    }

    // Returns zone index as [Platform Index, Row Index, Column Index].
    private int[] ParticlePositionToZoneIndex(Vector3 particlePosition)
    {
        Vector3 localPos = transform.InverseTransformPoint(particlePosition);

        return new int[3]
        {
            Mathf.RoundToInt(Mathf.LerpUnclamped(0, 9, localPos.y + 0.5f)),
            Mathf.RoundToInt(Mathf.LerpUnclamped(0, 9, localPos.z + 0.5f)),
            Mathf.RoundToInt(Mathf.LerpUnclamped(0, 9, localPos.x + 0.5f))
        };
    }
}
