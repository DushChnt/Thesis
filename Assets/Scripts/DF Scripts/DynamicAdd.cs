using UnityEngine;
using System.Collections;

public class DynamicAdd : MonoBehaviour {

    private dfScrollPanel _scrollPanel;

	// Use this for initialization
	void Start () {
        this._scrollPanel = GetComponent<dfScrollPanel>();
	}

    public void OnClick(dfControl control, dfMouseEventArgs mouseEvent)
    {
        var count = _scrollPanel.Controls.Count;

        var label = _scrollPanel.AddControl<dfLabel>();
        label.Text = "Item " + count;
        label.Width = _scrollPanel.Width - _scrollPanel.ScrollPadding.horizontal;
        label.Height = 24;
        label.BackgroundSprite = "frame-basic";
        label.TextAlignment = TextAlignment.Center;
        label.RelativePosition = new Vector3(0, count * 24);
      
    }
}
