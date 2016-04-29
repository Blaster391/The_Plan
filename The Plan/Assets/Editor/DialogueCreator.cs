using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

//TODO fix bad conventions

public class DialogueCreator : EditorWindow
{

    public List<DialogueNode> windows = new List<DialogueNode>();
	public List<DialogueNode> windowsToAttach = new List<DialogueNode>();
	public List<DialogueNode> attachedWindowsFrom = new List<DialogueNode>();
	public List<DialogueNode> attachedWindowsTo = new List<DialogueNode>();


	public float panX = -100000;
	public float panY = -100000;

    [MenuItem("Window/Node editor")]
    static void ShowEditor()
    {
        DialogueCreator editor = EditorWindow.GetWindow<DialogueCreator>();

    }


    void OnGUI()
    {

        if (windowsToAttach.Count == 2)
        {
            attachedWindowsFrom.Add(windowsToAttach[0]);
            attachedWindowsTo.Add(windowsToAttach[1]);
			windowsToAttach = new List<DialogueNode>();
		}
		
        if (attachedWindowsFrom.Count >= 1)
        {
            for (int i = 0; i < attachedWindowsFrom.Count; i += 1)
            {
                DrawNodeCurve(attachedWindowsFrom[i].getBody(), attachedWindowsTo[i].getBody());
            }
        }

		if (GUILayout.Button("Create Statement"))
		{
            createStatement();
		}
		if (GUILayout.Button("Create Response"))
		{
			createResponse();
		}
		if (GUILayout.Button("Save"))
		{
			SaveDialogue();
		}
		if (GUILayout.Button("Load"))
		{
			LoadDialogue();
		}
		if (GUILayout.Button("Export"))
		{

		}
        if (startNode != null)
        {
            GUILayout.Label(startNode.getNodeType());
        }

		GUI.BeginGroup(new Rect(panX, panY, 20000000, 200000000)); //TODO Make this better

		BeginWindows();

        for (int i = 0; i < windows.Count; i++)
        {
			windows[i].setBody(GUI.Window(i, windows[i].getBody(), windows[i].DrawNodeWindow, windows[i].title));
			//GUI.Window(i, windows[i].getBody(), DrawNodeWindow, "This is the text for the heading of the window");
        }

        EndWindows();

		GUI.EndGroup();

		Event e = Event.current;
		if(e.type == EventType.keyDown){
			if(e.keyCode == KeyCode.A){
				Debug.Log ("Nailed it");
			}
		}
		if(e.type == EventType.mouseDrag){
			Debug.Log ("X: " + panX);
			Debug.Log ("Y: " + panY);
			
			panX += e.delta.x;
			panY += e.delta.y;

			Repaint ();
		}

    }
	/*
	void DrawNodeWindow(int id)
    {
        if (GUILayout.Button("Attach"))
        {
            windowsToAttach.Add(id);
        }
        GUI.DragWindow();
    }
	*/

    void DrawNodeCurve(Rect start, Rect end)
    {
        Vector3 startPos = new Vector3(start.x + panX + start.width, start.y  + panY + start.height / 2, 0);
        Vector3 endPos = new Vector3(end.x + panX, end.y + panY + end.height / 2, 0);
        Vector3 startTan = startPos + Vector3.right * 50;
        Vector3 endTan = endPos + Vector3.left * 50;
        Color shadowCol = new Color(0, 0, 0, 0.06f);

        for (int i = 0; i < 3; i++)
        {// Draw a shadow
            Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
        }

        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
    }

	DialogueNode createResponse(){
		DialogueNode d = new ResponseNode(this, 175, 200, "Response Node");
		windows.Add(d);
		return d;
	}

	DialogueNode createResponse(float x, float y){
		DialogueNode d = new ResponseNode(this, x, y,200, 200, "Response Node");
		windows.Add(d);
		return d;
	}

	DialogueNode createStatement(){
		DialogueNode d = new StatementNode(this, 200, 200, "Statement Node");
		windows.Add(d);
		return d;
	}
	
	DialogueNode createStatement(float x, float y){
		DialogueNode d = new StatementNode(this, x, y,200, 200, "Statement Node");
		windows.Add(d);
		return d;
	}

	DialogueNode setNodePosition(DialogueNode d, float x, float y){
		d.setBody (new Rect (x,y,d.getBody().width,d.getBody().height));
		return d;
	}

	void SaveDialogue(){
		Debug.Log ("Saved");
		using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"Assets\Dialogue\Raw\SavedDialogue.dia"))
		{
			file.WriteLine("#PanX=" +panX+"#");
			file.WriteLine("#PanY=" +panY+"#");
			file.WriteLine ("   ");

			for(int i = 0; i < windows.Count; i++){ //TODO Change these variable names (i, j, k)
				file.WriteLine("#StartNode=" + i+"#");
				//Dump Node data
				file.WriteLine ("#NodeType="+ windows[i].getNodeType ()+"#");
				file.WriteLine("#X="+windows[i].getBody().x+"#"); 
				file.WriteLine("#Y="+windows[i].getBody().y+"#");
  
                file.WriteLine("#Sentence=" + windows[i].getSentence() + "#"); 


				for(int j = 0; j < attachedWindowsFrom.Count; j++){ //This is for saving node connections
					if(attachedWindowsFrom[j] == windows[i]){
						DialogueNode attachedNode = attachedWindowsTo[j];
						for(int k = 0; k < windows.Count; k++){
							if(windows[k] == attachedNode){
								file.WriteLine("#AttachedTo=" + k +"#");
							}
						}
					}
				}

				file.WriteLine("#EndNode#");

				file.WriteLine ("   ");
			}
			file.WriteLine ("#EOF#");
		}

	}

	void LoadDialogue(){
		Reset ();

		using (System.IO.StreamReader file = new System.IO.StreamReader(@"Assets\Dialogue\Raw\SavedDialogue.dia")) {
			string readText;
			int currentNodeIndex = -1;
			Queue<int> AttachedFromQueue = new Queue<int>();
			Queue<int> AttachedToQueue = new Queue<int>();

			DialogueNode newestNode = null;
			do{ //This has to be a do-while, otherwise things get weird.
				readText = file.ReadLine();

				if(readText.Contains("#PanX=")){ 
					panX = int.Parse(getValue (readText));
				}
				if(readText.Contains("#PanY=")){ 
					panY = int.Parse(getValue (readText));
				}

				if(readText.Contains("#StartNode=")){
					newestNode = createResponse();
					currentNodeIndex = int.Parse(getValue (readText));
				}

				if(readText.Contains ("#NodeType=")){
					if(getValue (readText) == "StatementNode"){
						windows.Remove(newestNode);
						newestNode = createStatement();
					}
					else if(getValue (readText) == "ResponseNode"){
						windows.Remove(newestNode);
						newestNode = createResponse();
					}
				}

                if (readText.Contains("#Sentence="))
                {
                    newestNode.setSentence(getValue(readText));
                }


                /*
                if (readText.Contains("#Content="))
                {
                    newestNode.setText(getValue(readText));
                }
                 */

				if(readText.Contains("#X=")){
					//windows.Remove(newestNode);
					//newestNode = createResponse( float.Parse(getValue (readText)), newestNode.getBody().y);

					setNodePosition(newestNode,float.Parse(getValue (readText)), newestNode.getBody().y);
				}
				if(readText.Contains("#Y=")){

					setNodePosition(newestNode,newestNode.getBody().x,float.Parse(getValue (readText)));
				}
				if(readText.Contains("#AttachedTo=")){
					AttachedFromQueue.Enqueue(currentNodeIndex);
					AttachedToQueue.Enqueue (int.Parse(getValue (readText)));
				}
				if(readText.Contains("#EndNode#")){
					newestNode = null;
					currentNodeIndex = -1;
				}
			
			}while(readText != "#EOF#");
			
			while(AttachedFromQueue.Count > 0){
				attachedWindowsFrom.Add(windows[AttachedFromQueue.Dequeue()]);
				attachedWindowsTo.Add(windows[AttachedToQueue.Dequeue()]);
			}
		}

	}

	void Reset(){
		windows = new List<DialogueNode>();
		windowsToAttach = new List<DialogueNode>();
		attachedWindowsFrom = new List<DialogueNode>();
		attachedWindowsTo = new List<DialogueNode>();
		
		
		panX = -100000;
		panY = -100000;

	}

	string getValue(string text){
		string value = text;
		string[] split = value.Split('=');
		value = split[1];
		value = value.Substring(0, value.Length-1);

		return value;
	}

    private DialogueNode startNode = null;
    public void attachNode(DialogueNode dn)
    {
        if (startNode == null)
        {
            startNode = dn;
        }
        else if(startNode.getNodeType() != dn.getNodeType())
        {
            windowsToAttach.Add(startNode);
            windowsToAttach.Add(dn);
            startNode = null;
        }
        else
        {
            startNode = null;
        }
    }
}