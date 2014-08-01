using UnityEngine;
using System.Collections;

public class ResetButton : MonoBehaviour {

    public dfButton Button;
    public GameObject Controller;
    public float Cooldown = 5f;
    float cooldown = 5f;
    public dfLabel CountdownLabel;

	// Use this for initialization
	void Start () {
        Button.Click += new MouseEventHandler(Button_Click);
	}

    void Button_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        Reset();
    }

    private void Reset()
    {
        if (Controller != null && cooldown >= Cooldown)
        {
            Controller.transform.rotation = Quaternion.identity;
            cooldown = 0;
            Button.Disable();
            CountdownLabel.Show();
            CountdownLabel.Text = string.Format("{0:0.00}", Cooldown - cooldown);
        }
    }
	
	// Update is called once per frame
	void Update () {
        cooldown += Time.deltaTime;
        if (cooldown >= Cooldown)
        {
            Button.Enable();
            CountdownLabel.Hide();
        }
        else
        {
            CountdownLabel.Text = string.Format("{0:0.00}", Cooldown - cooldown);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }
	}
}
