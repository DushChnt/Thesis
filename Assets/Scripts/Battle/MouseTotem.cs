using UnityEngine;
using System.Collections;

public class MouseTotem : MonoBehaviour {

    public GameObject Totem;
    private GameObject _totem;
    bool MouseDown;
    bool IsActive;
    BattleController controller;

	// Use this for initialization
	void Start () {
	
	}

    public void SetController(BattleController _controller)
    {
        controller = _controller;
        IsActive = true;
    }
	
	// Update is called once per frame
    void Update()
    {
        if (IsActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.transform.tag.Equals("Ground"))
                    {
                        _totem = Instantiate(Totem, hit.point + Vector3.up, Quaternion.identity) as GameObject;
                        MouseDown = true;
                        controller.SetTotemTarget(_totem);
                    }
                }
            }

            if (MouseDown)
            {
                RaycastHit hit;
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.transform.tag.Equals("Ground"))
                    {                        
                        _totem.transform.position = hit.point + Vector3.up;
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                controller.StopTotemTarget();
                Destroy(_totem);
                MouseDown = false;

            }
        }
    }
}
