using UnityEngine;
using UnityEngine.Events;

public class MouseClickController : MonoBehaviour
{
    public Vector3 clickPosition;

    public UnityEvent<Vector3> OnClick;

    public GameObject player;

    void Update() { 
        
        if (Input.GetMouseButtonDown(0)) 
        { 
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition); 

            if (Physics.Raycast( mouseRay, out RaycastHit hitInfo )) 
            { 
                Vector3 clickWorldPosition = hitInfo.point;
                
                clickPosition = clickWorldPosition;

                OnClick.Invoke(clickPosition);
            } 
        }
    } 
}
