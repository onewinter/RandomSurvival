using UnityEngine;

/// <summary>
/// Displays a configurable health bar for any object with a Damageable as a parent
/// https://github.com/sinbad/UnityInstancedHealthBars
/// </summary>
public class HealthBar : MonoBehaviour {

    MaterialPropertyBlock matBlock;
    MeshRenderer meshRenderer;
    Camera mainCamera;
    Enemy damageable;

    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        matBlock = new MaterialPropertyBlock();
        // get the damageable parent we're attached to
        damageable = GetComponentInParent<Enemy>();
    }

    private void Start() {
        // Cache since Camera.main is super slow
        mainCamera = Camera.main;
    }

    private void Update() {
        
        // Only display on partial health
        //if (damageable.Health < damageable.MaxHealth ) {
          //  meshRenderer.enabled = true;
            AlignCamera();
            UpdateParams();
        //} else {
//            meshRenderer.enabled = false;
  //      }
    }

    private void UpdateParams() {
        meshRenderer.GetPropertyBlock(matBlock);
        matBlock.SetFloat("_Fill", damageable.Health / damageable.MaxHealth);
        meshRenderer.SetPropertyBlock(matBlock);
    }

    private void AlignCamera()
    {
        if (mainCamera == null) return;
        
        var camXform = mainCamera.transform;
        var forward = transform.position - camXform.position;
        forward.Normalize();
        var up = Vector3.Cross(forward, camXform.right);
        transform.rotation = Quaternion.LookRotation(forward, up);
    }

}