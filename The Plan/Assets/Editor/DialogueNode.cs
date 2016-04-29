using UnityEngine;
using UnityEditor;
using System.Collections;

public abstract class DialogueNode {
    private Rect body;
	public string title;
	private DialogueCreator dc;
	private string text;
	private string sentence;

	private float height;
	private float width;

    public DialogueNode(DialogueCreator dc, float height, float width, string title)
    {
		this.dc = dc;
		body = new Rect(100-dc.panX,100-dc.panY, width, height);
		this.title = title;

		this.height = height;
		this.width = width;
    }

	public DialogueNode(DialogueCreator dc, float x, float y, float height, float width, string title)
	{
		this.dc = dc;
		body = new Rect(x,y, height, width);
		this.title = title;
		
		this.height = height;
		this.width = width;
	}

	public abstract string getNodeType ();

    public Rect getBody()
	{
        return body;
    }

    public void setBody(Rect body)
    {
        this.body = body;
    }

    public string getSentence()
    {
        return sentence;
    }
    public void setSentence(string sentence)
    {
        this.sentence = sentence;
    }

	public void DrawNodeWindow(int id)
	{

		/*text = GUI.TextArea(new Rect(10, 10, 20, 20), text, 25);*/
        sentence = EditorGUI.TextArea(new Rect(10, 20, width - 20, 30), sentence);
        if (getNodeType() == "StatementNode")
        {
            GUI.BeginGroup(new Rect(0, height - 85, width, height / 2));
        }
        else
        {
            GUI.BeginGroup(new Rect(0, height - 90, width, height / 2 + 20));
        }

		if (GUILayout.Button("Attach"))
		{
           dc.attachNode(this);
		}
		if (GUILayout.Button("Detach"))
		{
			int index = dc.attachedWindowsFrom.FindIndex(x => x == this); //No idea what this syntax is but it seems to work
			Debug.Log (index);
			dc.attachedWindowsFrom.Remove (this);
			dc.attachedWindowsTo.RemoveAt (index);
		}
        if (GUILayout.Button("Delete"))
        {

        }
		GUI.EndGroup();
/*
		Event e = Event.current;
		if(e.type == EventType.mouseDown){
		 	if(e.button == 1){
				index = EditorGUI.Popup(
					Rect(0,0,position.width, 20),
					"Component:",
					index, 
					options);
			}
		}*/

		GUI.DragWindow();
	}
}
