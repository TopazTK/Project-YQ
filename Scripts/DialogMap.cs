using Godot;
using System;

public class DialogMap : WindowDialog
{
	[Export] public string NodeName;
	[Export] public byte MapMode;
	
	bool _holdingMouse;
	bool _insideMouse;
	
	TextureRect _mapNode;
	GraphEdit _editNode;
	Area2D _pinNode;
	
	public override void _Ready()
	{
		_mapNode = GetNode("MapView") as TextureRect;
		_pinNode = GetNode("MapView/PinVL") as Area2D;
		_editNode = GetNode("../../Editor") as GraphEdit;
	}
	
	public override void _Process(float delta)
	{
		if (_holdingMouse && _insideMouse)
			_pinNode.Position = _mapNode.GetLocalMousePosition();
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton _mouseKey && this.Visible == true)
		{
			if (_mouseKey.Pressed && _mouseKey.ButtonIndex == 0x01)
				_holdingMouse = true;
			
			else 
			_holdingMouse = false;
		}
	}
	
	private void MouseEnterEvent() => _insideMouse = true;
	private void MouseExitEvent() => _insideMouse = false;
	
	private void SubmitEvent()
	{
		var _locationNode = GetNode("../../Editor/" + NodeName) as NodeLocation;
		
		switch (MapMode)
		{
			case 0:
				{
					var _destButton = _locationNode.GetNode("DestIN") as Button;
					var _currButton = _locationNode.GetNode("CurrIN") as Button;
					
					_locationNode.CurrentVEC = _pinNode.Position;
					_currButton.Text = "From: [" + _pinNode.Position.x + ", " + (_pinNode.Position.y + 38) + "]";
					
					if (_locationNode.DestinationVEC.x == 0 && _locationNode.DestinationVEC.y == 0)
						_destButton.Text = "To: [" + _pinNode.Position.x + ", " + (_pinNode.Position.y + 38) + "]";
				}
			break;
			
			case 1:
				{
					var _destButton = _locationNode.GetNode("DestIN") as Button;
					
					_locationNode.DestinationVEC = _pinNode.Position;
					_destButton.Text = "To: [" + _pinNode.Position.x + ", " + (_pinNode.Position.y + 38) + "]";
				}
			break;
		}
		
		this.Hide();
	}
	
	private void ShowEvent()
	{ 
		var _locationNode = GetNode("../../Editor/" + NodeName) as NodeLocation;
		
		switch (MapMode)
		{
			case 0:
				_pinNode.Position = _locationNode.CurrentVEC;
			break;
			
			case 1:
				_pinNode.Position = _locationNode.DestinationVEC;
			break;
		}
		
		if (_pinNode.Position.x == 0 || _pinNode.Position.y == 0)
			_pinNode.Position = new Vector2(_mapNode.RectSize.x / 2, _mapNode.RectSize.y / 2);
		
		_editNode.MouseFilter = Godot.Control.MouseFilterEnum.Ignore;
	}
	private void CloseEvent() => _editNode.MouseFilter = Godot.Control.MouseFilterEnum.Stop;
}

