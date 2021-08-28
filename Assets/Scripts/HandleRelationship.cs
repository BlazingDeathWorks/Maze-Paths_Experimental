using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleRelationship : MonoBehaviour
{
    const string playerTag = "Player";
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            PathFinder parentPathFinder = GetComponentInParent<PathFinder>();
            if (parentPathFinder && collision.TryGetComponent(out PathFinder collisionPathFinder))
            {
                if(parentPathFinder.GetDirectionAssignment() != DirectionCheck.None)
                {
                    collisionPathFinder.SetSharedToTrue();
                    collisionPathFinder.AddParentToList(parentPathFinder.gameObject);
                }
            }
            Destroy(gameObject);
        }
    }
}
