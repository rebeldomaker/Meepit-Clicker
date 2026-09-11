using Godot;

public partial class Animations : MarginContainer
{
	private TextureButton _clickButton;

	public override void _Ready()
	{
		_clickButton = GetNode<TextureButton>("CenterContainer/ClickButton");
		
		// Sets the pivot point to the exact center of the button for scaling animations
		_clickButton.PivotOffset = _clickButton.Size / 2;
	}

	private void _on_click_button_button_down()
	{
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(_clickButton, "scale", new Vector2(0.9f, 0.9f), 0.1);
	}

	private void _on_click_button_button_up()
	{
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(_clickButton, "scale", new Vector2(1.0f, 1.0f), 0.1);
	}
}
