using Godot;
using System;

public class MainEditor : GraphEdit
{
	Control _parentNode;
	
	public override void _Ready()
	{
		_parentNode = GetParent() as Control;
		
		var _menuNode = _parentNode.GetNode("Menu/MenuNodes") as MenuButton;
		var _nodePopup = _menuNode.GetPopup();
		
		_nodePopup.Connect("id_pressed", this, "NodeEvent");
	}
	
	private void ConnectionRequest(String fromNode, int fromSlot, String toNode, int toSlot)
	{
		if (fromNode == "NodeBegin" && toNode == "NodeEnd")
			GD.Print("Nope!");
			
		else
			this.ConnectNode(fromNode, fromSlot, toNode, toSlot);
	}
	
	private void DisconnectionRequest(String fromNode, int fromSlot, String toNode, int toSlot) => this.DisconnectNode(fromNode, fromSlot, toNode, toSlot);
	
	private void AboutEvent()
	{
		var _dialogWindow = _parentNode.GetNode("DialogAbout") as WindowDialog;
		
		_dialogWindow.WindowTitle = "== Project: Çilekli Muz ==";
		_dialogWindow.Show();
	}
	
	private void NodeEvent(int buttonID)
	{
		PackedScene _nodeScene = null;
		
		switch (buttonID)
		{
			case 0:
				_nodeScene = (PackedScene)ResourceLoader.Load("res://Nodes/NodeBattle.tscn");
				break;
			case 1:
				_nodeScene = (PackedScene)ResourceLoader.Load("res://Nodes/NodeLocation.tscn");
				break;
			case 2:
				_nodeScene = (PackedScene)ResourceLoader.Load("res://Nodes/NodeStage.tscn");
				break;
			case 3:
				_nodeScene = (PackedScene)ResourceLoader.Load("res://Nodes/NodeChoice.tscn");
				break;
		}
		
		var _nodeInstance = (GraphNode)_nodeScene.Instance();
		AddChild(_nodeInstance);
	}
}
