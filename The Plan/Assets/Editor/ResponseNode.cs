using UnityEngine;
using System.Collections;

public class ResponseNode : DialogueNode {
   /* private Rect body;
	public string title;
	private DialogueCreator dc;

    public ResponseNode(DialogueCreator dc, float height, float width)
    {
		this.dc = dc;
        body = new Rect(10, 10, height, width);
    }

    public Rect getBody()
	{
        return body;
    }

    public void setBody(Rect body)
    {
        this.body = body;
    }

	public void DrawNodeWindow(int id)
	{
		if (GUILayout.Button("Attach"))
		{
			dc.windowsToAttach.Add	(this);
		}
		if (GUILayout.Button("Detach"))
		{
			int index = dc.attachedWindowsFrom.FindIndex(x => x == this); //No idea what this syntax is but it seems to work
			Debug.Log (index);
			dc.attachedWindowsFrom.Remove (this);
			dc.attachedWindowsTo.RemoveAt (index);
		}
		GUI.DragWindow();
	}*/

	public ResponseNode(DialogueCreator dc, float height, float width, string title) : base(dc, height, width, title){
		
	}
	
	public ResponseNode(DialogueCreator dc, float x, float y, float height, float width, string title) : base(dc, x, y, height, width, title){
		
	}

	public override string getNodeType(){
		return "ResponseNode";
	}
}
