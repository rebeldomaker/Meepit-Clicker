using Godot;
using System;

public partial class Game : Control
{
	private const string SavePath = "user://userdata.save";

	// Auto-save timer variables
	private double _saveTimer = 0;
	private const double SaveInterval = 15.0; // Automatically saves every 15 seconds

/*If you want a variable to be editable inside the Godot Editor Inspector panel
 without exposing raw fields, 
 add [Export] above a public property
 
 1. The "Inspector Panel"
   In Godot, when you click on any object in your scene (like a Label, Button, or Node), 
   a menu opens on the right side of your screen showing its settings 
   (like position, color, or text). 
   That right-hand sidebar is the Inspector.

   2. "Editable in the Inspector"
   Instead of opening your C# code every time you want to balance your game 
   (like changing your click power from 1 to 5), 
   Godot lets you type numbers directly into that right-hand sidebar while designing your game.
   
   3. "Field" vs "Property"
   In C#, there are two main ways to create variables inside a class:
	   Field (Raw variable): private double meepits = 0; (A basic variable that just holds a value in memory).
	   Property: public double Meepits { get; set; } = 0; 
	   (A variable that uses { get; set; } 
	   to safely control how other scripts read (get) or change (set) its value).
   
   4. [Export]
   [Export] is a special tag you place directly above a variable. It tells Godot: 
   "Hey, create an input box for this variable in the right-hand Inspector menu so I can change it visually."
   
   You can click on your Game node in Godot, look at the right side of your screen, 
   and see a box labeled Meepits Per Click set to 1. 
   If you want to give the player 100 Meepits per click for testing, 
   you just type 100 in that box instead of opening Rider to rewrite your code */

	[Export] // An attribute that tells Godot to display this property in the Inspector panel when you select the node attached to this script.
	public double MeepitsPerClick { get; set; } = 1; // { get; set; }: C# syntax required for properties. It grants Godot permission to read (get) and write (set) the value behind the scenes.
	// public: Exposes the property so Godot's engine can read and update it.

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		LoadData();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Accumulates delta time to save automatically every 15 seconds
		_saveTimer += delta;
		if (_saveTimer >= SaveInterval)
		{
			_saveTimer = 0;
			SaveData();
		}
	}

	// Engine notification handler to detect window closing
	public override void _Notification(int what)
	{
		if ((long)what == NotificationWMCloseRequest)
		{
			SaveData();
		}
	}
	
// Modifiers in C#
// PRIVATE -- Only inside the current class (Game.cs).
// PUBLIC -- From any other C# script in your project.

// Tracks total Meepits
	private double _meepitCount = 0; 
// private is an access modifier in C# that restricts variable access
// so only code inside that specific class (Game) can read or change it.
// Key reasons to use private:
// Data Safety: Prevents other scripts from accidentally changing or corrupting your meepit count (for example, setting it to a negative number by mistake).
// Code Cleanliness: Keeps internal game state hidden from other parts of your project that don't need to touch it.
// Controlled Logic: Forces changes to go through dedicated methods (like an AddMeepits(double amount) function) where you can check rules or trigger UI updates at the same time.

	// Base amount gained per click
	private double _meepitsPerClick = 1;

	// Passive amount gained per second from upgrades
	private double _meepitsPerSecond = 0;
	
	[Signal]
	public delegate void MeepitsChangedEventHandler(double newCount);

	private void SaveData()
	{
		var data = new Godot.Collections.Dictionary
		{
			{ "meepits", _meepitCount }
		};

		using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
		if (file != null)
		{
			file.StoreVar(data);
		}
	}

	private void LoadData()
	{
		if (FileAccess.FileExists(SavePath))
		{
			using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
			if (file != null)
			{
				var data = file.GetVar();

				// Equivalent to GDScript: if typeof(data) == TYPE_DICTIONARY
				if (data.VariantType == Variant.Type.Dictionary)
				{
					var dict = data.AsGodotDictionary();
					
					// Equivalent to GDScript: cookies = data.get("cookies", 0)
					_meepitCount = dict.ContainsKey("meepits") ? dict["meepits"].AsDouble() : 0;
					
					// Refresh UI label upon load
					EmitSignal(SignalName.MeepitsChanged, _meepitCount);
				}
			}
		}
		else
		{
			SaveData();
		}
	}
	
	// MeepitsChanged(newCount: float)
	// 
	private void _on_click_button_button_down()
	{
		_meepitCount += _meepitsPerClick;
	
		// Broadcasts the new total to any connected listeners/UI
		EmitSignal(SignalName.MeepitsChanged, _meepitCount); // the underscore (_) is a C# naming convention used to show that a variable is a private field declared at the class level.
		// it instantly tells you that it is a class-wide private variable, rather than a temporary variable created inside a function.
		// this naming convention is to also prevent conflicts.
	}
}
