using Godot;
using System;

public class NodeLocation : GraphNode
{
	[Export] public byte CurrentMap;
	[Export] public byte DestinationMap;
	
	[Export] public Vector2 CurrentVEC;
	[Export] public Vector2 DestinationVEC;
	
	Node _nodeParent;
	
	public override void _Ready()
	{
		_nodeParent = GetParent().GetParent();
	}
	
	private void CurrentPress()
	{
		var _mapDialog = _nodeParent.GetNode("Dialogs/DialogMap") as DialogMap;
		
		_mapDialog.NodeName = this.Name;
		_mapDialog.MapMode = 0;
		
		_mapDialog.PopupCentered();
	}
	
	private void DestinationPress()
	{
		var _mapDialog = _nodeParent.GetNode("Dialogs/DialogMap") as DialogMap;
		
		_mapDialog.NodeName = this.Name;
		_mapDialog.MapMode = 1;
		
		_mapDialog.PopupCentered();
	}
}
