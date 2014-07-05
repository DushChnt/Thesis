using UnityEngine;
using System.Collections;
using Awesomium.Mono;

public class WebViewHandler : MonoBehaviour {

    IWebView view;

	// Use this for initialization
	void Start () {
        print("Start View!!!");
      
       

        WebCore.CreatedView += new CreatedViewEventHandler(WebCore_CreatedView);
	}

    void WebCore_CreatedView(object sender, CreatedViewEventArgs e)
    {
        view = e.NewView;
        if (view != null)
        {
            print("View!!!");
            view.JSConsoleMessageAdded += new JSConsoleMessageAddedEventHandler(view_JSConsoleMessageAdded);
        }
        else
        {
            print("View is null");
        }

    }

    void view_JSConsoleMessageAdded(object sender, JSConsoleMessageEventArgs e)
    {
        print(e.Message);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
