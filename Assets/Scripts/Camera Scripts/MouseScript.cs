using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseScript : MonoBehaviour
{

    private const int LeftMouse = 0;
    public GameObject mousePointer;

    public Texture2D cursorTexture;
    private CursorMode mode = CursorMode.ForceSoftware;
    private Vector2 hotSpot = Vector2.zero;
    private GameObject instantiatedMouse;

    void Start()
    {

    }

    void Update()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, mode);

        if (Input.GetMouseButtonUp(LeftMouse))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider is TerrainCollider)
                {
                    Vector3 temp = hit.point;
                    temp.y = 0.25f;

                    if (instantiatedMouse != null)
                    {
                        Destroy(instantiatedMouse);
                    }

                    instantiatedMouse = Instantiate<GameObject>(mousePointer);
                    instantiatedMouse.transform.position = temp;
                }
            }
        }
    }
}
