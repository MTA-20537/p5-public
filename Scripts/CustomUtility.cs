using System;
using UnityEngine;

/**
 * A custom utility class for hosting more broad purpose functionality
 */
public class CustomUtility : MonoBehaviour
{
    /**
     * apply a given lambda to a given gameobject and all it's children
     */
    public static void applyMeshRendererOperationRecursively(GameObject gameObject, Action<GameObject> operation, int depth = 0)
    {
        // some Clue models have multiple sub-meshes which must be updated individually using recursion
        try
        {
            // try updating the MeshRenderer of the current GameObject
            operation(gameObject);
        }
        catch (MissingComponentException e)
        {
            // MeshRenderer component was not found on GameObject, search all child objects for more
            if (gameObject.transform.childCount == 0 /*|| depth > 1*/) return;
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                // invoke self for subtree
                applyMeshRendererOperationRecursively(gameObject.transform.GetChild(i).gameObject, operation, depth++);
            }
        };
    }
}
