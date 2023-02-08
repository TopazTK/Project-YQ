using Godot;
using Godot.Collections;

using System;
using System.Linq;
using System.Collections.Generic;

public class MainEditor : GraphEdit
{
	Control _parentNode;
	
	Node _foreNode;
	Node _selfNode;
	
	int _nodeCount = 0;
	List<byte> _outputArray = new List<byte>() { 0x00, 0x0A, 0x01, 0x05, 0x0F, 0x00, 0x14, 0x01, 0x03, 0x00, 0x19, 0x0A};
	
	public override void _Ready()
	{
		OS.SetWindowTitle("Project: Çilekli Muz [ALPHA | v0.75]");
		
		_parentNode = GetParent() as Control;
		
		_selfNode = _parentNode.GetNode("Editor");
		_foreNode = _parentNode.GetNode("Foreground");
		
		var _menuNode = _parentNode.GetNode("Menu/MenuNodes") as MenuButton;
		var _nodePopup = _menuNode.GetPopup();
		
		GetNode("NodeBegin").SetOwner(this);
		GetNode("NodeEnd").SetOwner(this);
		
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
		var _dialogWindow = _parentNode.GetNode("Dialogs/DialogAbout") as WindowDialog;
		
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
				_nodeScene = (PackedScene)ResourceLoader.Load("res://Nodes/NodeChoice.tscn");
				break;
			case 4:
				_nodeScene = (PackedScene)ResourceLoader.Load("res://Nodes/NodeEnemy.tscn");
				break;
		}
		
		var _nodeInstance = (GraphNode)_nodeScene.Instance();
		
		_nodeInstance.Name = "NODE_INSTANCE_" + _nodeCount;
		_nodeCount++;
		
		
		AddChild(_nodeInstance);
		_nodeInstance.SetOwner(_selfNode);
	}
	
	private void BuildEvent()
	{
		var _nodeList = this.GetConnectionList().Cast<Dictionary>();
		var _nodeMain = _nodeList.Where(x => (int)x["from_port"] == 0x00 && (int)x["to_port"] == 0x00);
		
		GD.Print(this.GetConnectionList());
		
		GraphNode _currentNode = null;
		Dictionary _currentEntry = (Dictionary)_nodeMain.ElementAt(0);
		
		bool _begFound = false;
		bool _endFound = false;
		
		_outputArray = new List<byte>() { 0x00, 0x0A, 0x01, 0x05, 0x0F, 0x00, 0x14, 0x01, 0x03, 0x00, 0x19, 0x0A};
		
		for(int i = 0; i < _nodeMain.Count(); i++)
		{
			var _entry = (Dictionary)_nodeMain.ElementAt(i);
			
			var _fromStr = (String)_entry["from"];
			var _toStr = (String)_entry["to"];
			
			var _tempNode = _fromStr == "NodeBegin" ? GetNode((string)_entry["to"]) as GraphNode : GetNode((string)_entry["from"]) as GraphNode;
			
			_tempNode.HintTooltip = "";
			_tempNode.Overlay = GraphNode.OverlayEnum.Disabled;
			
			if (_fromStr == "NodeBegin")
			{
				_begFound = true;
				_currentEntry = _entry;
			}
			
			if (_toStr == "NodeEnd")
				_endFound = true;
			
			if (_endFound && _begFound)
				break;
			
			if (i == _nodeList.Count() - 1)
			{
				if (!_begFound)
					_tempNode.HintTooltip += "The Begin Block was not connected!\n";
					
				if (!_endFound)
					_tempNode.HintTooltip += "The End Block was not connected!\n";
				
				_tempNode.Overlay = GraphNode.OverlayEnum.Position;
					
				var _errorNode = _parentNode.GetNode("Dialogs/DialogBuildError") as WindowDialog;
				_errorNode.RectPosition = new Vector2(304.5F, 287.5F);
				_errorNode.Visible = true;
				
				return;
			}
		}
		
		while (true)
		{
			_currentEntry = _nodeMain.First(x => (string)x["from"] == (string)_currentEntry["to"]);
			_currentNode = GetNode((string)_currentEntry["from"]) as GraphNode;
			
			var _connectedNodes = _nodeList.Where(x => (string)x["to"] == (string)_currentEntry["from"] && (string)x["from"] != "NodeBegin");
			
			if (_currentNode.Title.Contains("BATTLE"))
			{
				_currentNode.Overlay = GraphNode.OverlayEnum.Disabled;
				_currentNode.HintTooltip = "";
				
				var _stageArray = new List<byte>();
				var _locationArray = new List<byte>();
				var _enemyArray = new List<byte>();
				
				if (_outputArray.Count > 0x0C)
					_outputArray.Add(0x03);
					
				else
					_outputArray.Add(0x02);
				
				var _musicNode = _currentNode.GetNode("MusicCH") as OptionButton;
				
				var _titleID = (_currentNode.GetNode("TitleID") as LineEdit).Text;
				var _loreID = (_currentNode.GetNode("LoreID") as LineEdit).Text;
				
				var _musicID =  _musicNode.GetItemId(_musicNode.Selected);
				
				var _defendCheck = (_currentNode.GetNode("DefendCH") as CheckBox).Pressed;
				var _specialCheck = (_currentNode.GetNode("SpecialCH") as CheckBox).Pressed;
				var _destinedCheck = (_currentNode.GetNode("DestinedCH") as CheckBox).Pressed;
				var _continueCheck = (_currentNode.GetNode("ContinueCH") as CheckBox).Pressed;
				
				_outputArray.AddRange(Helpers.SingleInstruction(0x04, _titleID.ToShort()));
				_outputArray.AddRange(Helpers.SingleInstruction(0x05, _loreID.ToShort()));
				_outputArray.AddRange(Helpers.SingleInstruction(0x19, _musicID));
				
				if (_defendCheck)
					_outputArray.Add(0x1A);
					
				if (_specialCheck)
					_outputArray.Add(0x1E);
					
				if (_destinedCheck)
					_outputArray.AddRange(Helpers.SingleInstruction(0x20, 0x0E));
				
				if (!_continueCheck)
					_outputArray.AddRange(Helpers.MultiInstruction(0x0B, new byte[] { 0x03, 0x05 }));
				
				foreach(Dictionary _s in _connectedNodes)
				{
					var _subNode = GetNode((string)_s["from"]) as GraphNode;
					var _subTitle = _subNode.Title.Replace("Info: ", "");
					
					switch (_subTitle)
					{
						case "LOCATION":
							var _loctNode = _subNode.GetNode("LoctVL");
							var _destNode = _subNode.GetNode("DestVL");
							
							var _loctM = (byte)(_loctNode.GetNode("MAP") as SpinBox).Value;
							var _loctX = (_loctNode.GetNode("X") as LineEdit).Text.ToShort();
							var _loctY = (_loctNode.GetNode("Y") as LineEdit).Text.ToShort();
							
							var _destM = (byte)(_destNode.GetNode("MAP") as SpinBox).Value;
							var _destX = (_destNode.GetNode("X") as LineEdit).Text.ToShort();
							var _destY = (_destNode.GetNode("Y") as LineEdit).Text.ToShort();
							
							_locationArray.AddRange(new byte[] {0x16, _loctM});
							_locationArray.AddRange(_loctX.GetBytes());
							_locationArray.AddRange(_loctY.GetBytes());
							
							_locationArray.AddRange(new byte[] {0x17, 0x00, _destM});
							_locationArray.AddRange(_destX.GetBytes());
							_locationArray.AddRange(_destY.GetBytes());
							
							break;
						case "STAGE":
							var _stageInfo = _subNode.GetNode("StageCH") as OptionButton;
							var _effectInfo = _subNode.GetNode("EffectCH") as OptionButton;
							
							var _stageID = (byte)_stageInfo.GetItemId(_stageInfo.Selected);
							var _effectID = (byte)_effectInfo.GetItemId(_effectInfo.Selected);
							
							var _varByte = (_subNode.GetNode("VariationID") as LineEdit).Text.ToByte();
							
							_stageArray.AddRange(Helpers.MultiInstruction(0x0A, new byte[] { _stageID, _varByte }));
							
							if (_effectID != 0x00)
								_stageArray.AddRange(Helpers.SingleInstruction(0x26, _effectID));
							
							break;
						case "ENEMY":
							var _characterInfo = _subNode.GetNode("CharacterCH") as OptionButton;
							var _weaponInfo = _subNode.GetNode("WeaponCH") as OptionButton;
							var _costumeInfo = _subNode.GetNode("CostumeCH") as OptionButton;
							
							var _orderValue = (byte)(_subNode.GetNode("OrderVL") as Slider).Value;
							var _difficultyValue = (byte)(_subNode.GetNode("DifficultyVL") as Slider).Value * (byte)0x11;
							
							var _healthValue = (byte)(_subNode.GetNode("HealthVL") as Slider).Value;
							var _recoveryValue = (byte)(_subNode.GetNode("RecoverVL") as Slider).Value;
							
							var _powValue = Convert.ToUInt16((_subNode.GetNode("PowerVL") as Slider).Value * 10);
							var _defValue = Convert.ToUInt16((_subNode.GetNode("DefenseVL") as Slider).Value * 10);
							
							var _timeValue = Convert.ToUInt16((_subNode.GetNode("TimeVL") as SpinBox).Value);
							
							var _characterID = _characterInfo.GetItemId(_characterInfo.Selected);
							var _weaponID = _weaponInfo.GetItemId(_weaponInfo.Selected);
							var _costumeID = _costumeInfo.GetItemId(_costumeInfo.Selected);
							
							_enemyArray.AddRange(Helpers.SingleInstruction(0x06, _orderValue));
							_enemyArray.AddRange(Helpers.SingleInstruction(0x07, _characterID));
							_enemyArray.AddRange(Helpers.SingleInstruction(0x08, _costumeID));
							_enemyArray.AddRange(Helpers.SingleInstruction(0x09, _weaponID));
							_enemyArray.AddRange(Helpers.SingleInstruction(0x1D, _healthValue));
							_enemyArray.AddRange(Helpers.SingleInstruction(0x1C, _recoveryValue));
							_enemyArray.AddRange(Helpers.SingleInstruction(0x27, _difficultyValue));
							_enemyArray.AddRange(Helpers.SingleInstruction(0x23, _powValue));
							_enemyArray.AddRange(Helpers.SingleInstruction(0x24, _defValue));
							_enemyArray.AddRange(Helpers.SingleInstruction(0x1B, _timeValue));
							
							break;
					}
				}
				
				if (_stageArray.Count() == 0x00)
					_currentNode.HintTooltip += "No STAGE Node is connected!\n";
				
				if (_locationArray.Count() == 0x00)
					_currentNode.HintTooltip += "No LOCATION Node is connected!\n";
				
				if (_enemyArray.Count() == 0x00)
					_currentNode.HintTooltip += "No ENEMY Node is connected!\n";
				
				if (_currentNode.HintTooltip != "")
				{
					_currentNode.Overlay = GraphNode.OverlayEnum.Position;
					
					var _errorNode = _parentNode.GetNode("Dialogs/DialogBuildError") as WindowDialog;
					_errorNode.RectPosition = new Vector2(304.5F, 287.5F);
					_errorNode.Visible = true;
					
					return;
				}
				
				_outputArray.AddRange(_stageArray);
				_outputArray.AddRange(_locationArray);
				_outputArray.AddRange(_enemyArray);
				
				if ((string)_currentEntry["to"] == "NodeEnd")
					_outputArray.AddRange(new byte[] { 0x11, 0x12, 0x4C, 0x00, 0x13, 0x0D, 0x01, 0x05, 0x00, 0x0E, 0x01, 0x01, 0x00, 0x03, 0x15, 0x02, 0x00 });
					
				else
					_outputArray.AddRange(new byte[] { 0x11, 0x01, 0x01, 0x00 });
			}
			
			if ((string)_currentEntry["to"] == "NodeEnd")
			break;
		}
		
		var _fileNode = _parentNode.GetNode("Dialogs/DialogBuild") as FileDialog;
		_fileNode.Visible = true;
	}
	
	private void BuildFileSelectEvent(String path) => System.IO.File.WriteAllBytes(path.Replace("taleScr", ".taleScr"), _outputArray.ToArray());
}



