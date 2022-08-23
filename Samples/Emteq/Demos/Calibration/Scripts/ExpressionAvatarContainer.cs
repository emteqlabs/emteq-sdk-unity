using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class ExpressionAvatarContainer : MonoBehaviour
{
    [SerializeField] 
    private GameObject[] ExpressionsObjects;
    void Start()
    {
        //HideExpressions();
    }

    private void HideExpressions()
    {
        foreach (GameObject expression in ExpressionsObjects)
        {
            expression.SetActive(false);
        }
    }
}
