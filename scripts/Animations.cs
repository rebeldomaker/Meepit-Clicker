using Godot;
using System.Collections.Generic;

public partial class Animations : MarginContainer
{
	private TextureButton _clickButton;

	public override void _Ready()
	{
		_clickButton = GetNode<TextureButton>("CenterContainer/ClickButton");

		// Sets the pivot point to the exact center of the button for scaling animations
		_clickButton.PivotOffset = _clickButton.Size / 2;

		// Removes the white background connected to the outer border
		RemoveBackgroundContiguous(_clickButton);
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

	private void RemoveBackgroundContiguous(TextureButton button)
	{
		if (button.TextureNormal == null) return;

		Image img = button.TextureNormal.GetImage();
		img.Convert(Image.Format.Rgba8);

		int width = img.GetWidth();
		int height = img.GetHeight();

		Color targetColor = img.GetPixel(0, 0);
		bool[,] visited = new bool[width, height];
		Queue<Vector2I> queue = new Queue<Vector2I>();

		queue.Enqueue(new Vector2I(0, 0));
		visited[0, 0] = true;

		while (queue.Count > 0)
		{
			Vector2I current = queue.Dequeue();
			int x = current.X;
			int y = current.Y;

			img.SetPixel(x, y, new Color(0, 0, 0, 0));

			Vector2I[] neighbors = new Vector2I[]
			{
				new Vector2I(x + 1, y),
				new Vector2I(x - 1, y),
				new Vector2I(x, y + 1),
				new Vector2I(x, y - 1)
			};

			foreach (Vector2I n in neighbors)
			{
				if (n.X >= 0 && n.X < width && n.Y >= 0 && n.Y < height && !visited[n.X, n.Y])
				{
					Color pixelColor = img.GetPixel(n.X, n.Y);
					if (GetColorDistance(pixelColor, targetColor) <= 0.15f)
					{
						visited[n.X, n.Y] = true;
						queue.Enqueue(n);
					}
				}
			}
		}

		button.TextureNormal = ImageTexture.CreateFromImage(img);
	}

	private float GetColorDistance(Color c1, Color c2)
	{
		return Mathf.Abs(c1.R - c2.R) + Mathf.Abs(c1.G - c2.G) + Mathf.Abs(c1.B - c2.B);
	}
}
