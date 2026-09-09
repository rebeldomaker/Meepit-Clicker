using Godot;
using System;

public partial class Stats : VBoxContainer
{
    private Label _meepitsLabel;

    public override void _Ready()
    {
        // Finds the MeepitsLabel child node under Stats
        _meepitsLabel = GetNode<Label>("MeepitsLabel");
    }

    private void _on_game_meepits_changed(double amount)
    {
        _meepitsLabel.Text = $"{amount} Meepits";
    }
}