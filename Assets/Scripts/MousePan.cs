using UnityEngine;
using System.Collections;

public class MousePan : MonoBehaviour {

    public float PanAreaPercent = 0.2f; // The area in which the panning should work [0, 1]
    public float PanSpeed = 2.0f;
    public bool Activated;

    public dfPanel BrainsPanel;

    private float horizontalPanPixels, verticalPanPixels;
    private float leftPanBorder, rightPanBorder;
    private float topPanBorder, bottomPanBorder;

    public float PanMaxX = 10, PanMinX = 0, PanMaxY = 20, PanMinY = -10, ZoomSpeed, ZoomMin, ZoomMax;
    
    public int PanAngleMin = 25;
    public int PanAngleMax = 50;

	// Use this for initialization
	void Start () {
        horizontalPanPixels = Screen.width * PanAreaPercent;
        verticalPanPixels = Screen.height * PanAreaPercent;
        leftPanBorder = horizontalPanPixels;
        rightPanBorder = Screen.width - horizontalPanPixels;
        topPanBorder = verticalPanPixels;
        bottomPanBorder = Screen.height - verticalPanPixels;

        BrainsPanel.MouseEnter += new MouseEventHandler(BrainsPanel_MouseEnter);
        BrainsPanel.MouseLeave += new MouseEventHandler(BrainsPanel_MouseLeave);
	}

    void BrainsPanel_MouseLeave(dfControl control, dfMouseEventArgs mouseEvent)
    {
        Activated = true;
    }

    void BrainsPanel_MouseEnter(dfControl control, dfMouseEventArgs mouseEvent)
    {
        Activated = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (Activated)
        {
            Vector3 translation = new Vector3();
            if (Input.mousePosition.x < leftPanBorder)
            {
                float ratio = (leftPanBorder - Input.mousePosition.x) / horizontalPanPixels;
                translation += Vector3.left * PanSpeed * ratio * Time.deltaTime;
            }
            else if (Input.mousePosition.x > rightPanBorder)
            {
                float ratio = (Input.mousePosition.x - rightPanBorder) / horizontalPanPixels;
                translation += Vector3.right * PanSpeed * ratio * Time.deltaTime;
            }
            if (Input.mousePosition.y < topPanBorder)
            {
                float ratio = (topPanBorder - Input.mousePosition.y) / verticalPanPixels;
                translation += Vector3.back * PanSpeed * ratio * Time.deltaTime;
            }
            else if (Input.mousePosition.y > bottomPanBorder)
            {
                float ratio = (Input.mousePosition.y - bottomPanBorder) / verticalPanPixels;
                translation += Vector3.forward * PanSpeed * ratio * Time.deltaTime;
            }

            var zoomDelta = Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed * Time.deltaTime;
            if (zoomDelta != 0)
            {
                translation -= Vector3.up * ZoomSpeed * zoomDelta;
            }

            var pan = camera.transform.eulerAngles.x - zoomDelta * 32;
      //      print("Pan: " + pan);
            pan = Mathf.Clamp(pan, PanAngleMin, PanAngleMax);
       //     print("Pan clamp: " + pan);
            if (zoomDelta < 0 || camera.transform.position.y < (ZoomMax / 2))
            {
                print("Apply pan: " + pan + ", zoomDelta: " + zoomDelta + ", camera.y: " + camera.transform.position.y);
                camera.transform.eulerAngles = new Vector3(pan, 0, 0);
             //   camera.transform.eulerAngles = Vector3.Lerp(camera.transform.eulerAngles, new Vector3(pan, 0, 0), 0.2f);
            }

            // Move camera with arrow keys
            translation += new Vector3(Input.GetAxis("Horizontal"), 0, -Input.GetAxis("Vertical"));

            // Keep camera within level and zoom area
            var desiredPosition = camera.transform.position + translation;

            if (desiredPosition.x < PanMinX || desiredPosition.x > PanMaxX)
            {
                translation.x = 0;
            }
            if (desiredPosition.z < PanMinY || desiredPosition.z > PanMaxY)
            {
                translation.z = 0;
            }
            if (desiredPosition.y < ZoomMin || desiredPosition.y > ZoomMax)
            {
                translation.y = 0;
            }

            camera.transform.position += translation;



        }
	}
}
