using System;
using UnityEngine;

public class CustomUtility : MonoBehaviour
{
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
                applyMeshRendererOperationRecursively(gameObject.transform.GetChild(i).gameObject, operation, depth++);
            }
        };
    }
}
