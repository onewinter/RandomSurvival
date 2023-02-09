using UnityEngine;

public class ChunkTrigger : MonoBehaviour
{
    [SerializeField] private Vector2 triggerDirection; // N, E, S, W
    private Chunk parentChunk;
    private bool enableTrigger;
    
    void Start()
    {
        parentChunk = GetComponentInParent<Chunk>();
    }

    public void EnableTrigger(bool enable = true) => enableTrigger = enable;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!enableTrigger) return;
        if (!other.GetComponent<Player>()) return;
        
        enableTrigger = false; // dont run twice
        //Debug.Log("Chunk Trigger Enter " + triggerDirection);
        parentChunk.OnChunkTrigger(triggerDirection);
    }

}
