using UnityEngine;

[ExecuteInEditMode]
public class CombinedBounds : MonoBehaviour {
    public Renderer[] BoundTargets;

    public Color BoundColor = Color.green;
    private Vector3 v3BackBottomLeft;
    private Vector3 v3BackBottomRight;
    private Vector3 v3BackTopLeft;
    private Vector3 v3BackTopRight;
    private Vector3 v3FrontBottomLeft;
    private Vector3 v3FrontBottomRight;

    private Vector3 v3FrontTopLeft;
    private Vector3 v3FrontTopRight;


    // Update is called once per frame
    private void Update() {
        CalcPositons();
        DrawBox();
    }

    private Bounds CalculateLocalBounds() {
        var currentRotation = transform.rotation;
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        var bounds = BoundTargets[0].bounds;
        
        foreach (var boundResult in BoundTargets) {
            if (boundResult != BoundTargets[0])
            bounds.Encapsulate(boundResult.bounds);
        }

        var localCenter = bounds.center - transform.position;
        bounds.center = localCenter;
        //Debug.Log("The local bounds of this model is " + bounds);

        transform.rotation = currentRotation;

        return bounds;
    }


    private void CalcPositons() {
        var bounds = CalculateLocalBounds();

        //Bounds bounds;
        //BoxCollider bc = GetComponent<BoxCollider>();
        //if (bc != null)
        //    bounds = bc.bounds;
        //else
        //return;

        var v3Center = bounds.center;
        var v3Extents = bounds.extents;

        // Front top left corner
        v3FrontTopLeft =new Vector3(
            x: v3Center.x - v3Extents.x, 
            y: v3Center.y + v3Extents.y, 
            z: v3Center.z - v3Extents.z);
        
        // Front top right corner
        v3FrontTopRight = new Vector3(
            x: v3Center.x + v3Extents.x, 
            y: v3Center.y + v3Extents.y, 
            z: v3Center.z - v3Extents.z); 
        
        // Front bottom left corner
        v3FrontBottomLeft = new Vector3(
            x: v3Center.x - v3Extents.x, 
            y: v3Center.y - v3Extents.y, 
            z: v3Center.z - v3Extents.z);
        
        // Front bottom right corner
        v3FrontBottomRight = new Vector3(
            x: v3Center.x + v3Extents.x, 
            y: v3Center.y - v3Extents.y, 
            z: v3Center.z - v3Extents.z); 
        
        // Back top left corner
        v3BackTopLeft = new Vector3(
            x: v3Center.x - v3Extents.x, 
            y: v3Center.y + v3Extents.y, 
            z: v3Center.z + v3Extents.z); 
        
        // Back top right corner
        v3BackTopRight = new Vector3(
            x: v3Center.x + v3Extents.x, 
            y: v3Center.y + v3Extents.y, 
            z: v3Center.z + v3Extents.z); 
        
        // Back bottom left corner
        v3BackBottomLeft = new Vector3(
            x: v3Center.x - v3Extents.x, 
            y: v3Center.y - v3Extents.y, 
            z: v3Center.z + v3Extents.z); 
        
        // Back bottom right corner
        v3BackBottomRight = new Vector3(
            x: v3Center.x + v3Extents.x, 
            y: v3Center.y - v3Extents.y, 
            z: v3Center.z + v3Extents.z);

        v3FrontTopLeft = transform.TransformPoint(v3FrontTopLeft);
        v3FrontTopRight = transform.TransformPoint(v3FrontTopRight);
        v3FrontBottomLeft = transform.TransformPoint(v3FrontBottomLeft);
        v3FrontBottomRight = transform.TransformPoint(v3FrontBottomRight);
        v3BackTopLeft = transform.TransformPoint(v3BackTopLeft);
        v3BackTopRight = transform.TransformPoint(v3BackTopRight);
        v3BackBottomLeft = transform.TransformPoint(v3BackBottomLeft);
        v3BackBottomRight = transform.TransformPoint(v3BackBottomRight);
    }

    private void DrawBox() {
        //if (Input.GetKey (KeyCode.S)) {
        Debug.DrawLine(v3FrontTopLeft, v3FrontTopRight, BoundColor);
        Debug.DrawLine(v3FrontTopRight, v3FrontBottomRight, BoundColor);
        Debug.DrawLine(v3FrontBottomRight, v3FrontBottomLeft, BoundColor);
        Debug.DrawLine(v3FrontBottomLeft, v3FrontTopLeft, BoundColor);

        Debug.DrawLine(v3BackTopLeft, v3BackTopRight, BoundColor);
        Debug.DrawLine(v3BackTopRight, v3BackBottomRight, BoundColor);
        Debug.DrawLine(v3BackBottomRight, v3BackBottomLeft, BoundColor);
        Debug.DrawLine(v3BackBottomLeft, v3BackTopLeft, BoundColor);

        Debug.DrawLine(v3FrontTopLeft, v3BackTopLeft, BoundColor);
        Debug.DrawLine(v3FrontTopRight, v3BackTopRight, BoundColor);
        Debug.DrawLine(v3FrontBottomRight, v3BackBottomRight, BoundColor);
        Debug.DrawLine(v3FrontBottomLeft, v3BackBottomLeft, BoundColor);
        //}
    }
}