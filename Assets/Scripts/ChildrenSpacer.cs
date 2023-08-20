using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spaces the children of the given 
/// </summary>
public class ChildrenSpacer : MonoBehaviour
{
    [SerializeField] private Transform rightAnchor;

    [SerializeField] private Transform leftAnchor;

    /// <summary>
    /// A list of all child objects
    /// </summary>
    private List<Transform> children;

    private void Awake()
    {
        InitializeBeads();
    }

    /// <summary>
    /// Initialize beads list with every child object
    /// </summary>
    private void InitializeBeads()
    {
        children = new List<Transform>();

        foreach (Transform bead in transform.GetComponentsInChildren<Transform>())
        {
            children.Add(bead);
        }
    }

    void FixedUpdate()
    {
        AdjustBeadPositions();
    }

    private void AdjustBeadPositions()
    {
        // get spacing by dividing difference in position by number of children
        float xSpacing = (rightAnchor.position.x - leftAnchor.position.x) / children.Count;
        float ySpacing = (rightAnchor.position.y - leftAnchor.position.y) / children.Count;

        // reposition each child
        for (int i = 0; i < children.Count; i++)
        {
            float xPosition = leftAnchor.position.x + ((i) * xSpacing); 
            float yPosition = leftAnchor.position.y + ((i) * ySpacing);

            children[i].position = new Vector3(xPosition, yPosition);
        }
    }
}
