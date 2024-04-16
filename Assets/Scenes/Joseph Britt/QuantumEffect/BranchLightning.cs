using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class BranchLightning : MonoBehaviour {

    [SerializeField] private GameObject lightningBoltPrefab;
    [SerializeField] private Vector3 center = new Vector3(0, 0, 0);
    [SerializeField] private Vector3[] corners = { new Vector3(49, 14, 0) };
    [SerializeField] private Color color1 = Color.white;
    [SerializeField] private Color color2 = Color.blue;
    [SerializeField] private float lengthFactor = 0.5f;
    [SerializeField] private int segments = 4;
    [SerializeField] private float branchDampingFactor = 0.8f;
    [SerializeField] private int minAngle = 20;
    [SerializeField] private int maxAngle = 40;
    private LinkedList<VisualEffect> boltVisualEffects;

    // Start is called before the first frame update
    public void Start() {
        boltVisualEffects = new LinkedList<VisualEffect>();
        foreach (Vector3 corner in corners) {
            Vector3 direction = lengthFactor * (segments == 0 ? center - corner : (center - corner) / segments);
            SpawnBranches(segments, corner, direction, 1f);
        }
    }

    private void SpawnBranches(int num, Vector3 startPos, Vector3 dir, float branchProbability) {
        Vector3 endPos = startPos + dir;
        if (num > 1) {
            int angle = (Random.Range(0, 2) == 0 ? -1 : 1) * Random.Range(minAngle, maxAngle);
            if (Random.Range(0, 1f) < branchProbability) {
                SpawnBranches(num - 1, startPos + 0.5f * dir, Quaternion.Euler(0, 0, -angle) * dir, branchProbability * branchDampingFactor);
            }
            SpawnBranches(num - 1, endPos, Quaternion.Euler(0, 0, angle) * dir, branchProbability * branchDampingFactor);
        }
        SpawnBolt(startPos, endPos);
    }

    private void SpawnBolt(Vector3 startPos, Vector3 endPos) {
        if (boltVisualEffects == null) { return; }
        VisualEffect boltVFX = Instantiate(lightningBoltPrefab, startPos, Quaternion.identity).GetComponentInChildren<VisualEffect>();
        boltVisualEffects.AddLast(boltVFX);
        boltVFX.SetVector3("PosA", startPos);
        boltVFX.SetVector3("PosB", startPos);
        boltVFX.SetVector3("PosC", endPos);
        boltVFX.SetVector3("PosD", endPos);
        boltVFX.SetVector4("Color1", color1);
        boltVFX.SetVector4("Color2", color2);
    }

    public void SetEnabled(bool enabled) {
        if (boltVisualEffects == null) { return; }
        foreach (VisualEffect boltVFX in boltVisualEffects) {
            boltVFX.enabled = enabled;
        }
    }

    public void OnDestroy() {
        boltVisualEffects = null;
    }
}
