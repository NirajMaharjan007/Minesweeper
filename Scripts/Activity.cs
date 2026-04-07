using Godot;

namespace Minesweeper.Scripts;

public partial class Activity : Control
{
    private readonly System.Collections.Generic.Dictionary<string, TextureButton> buttonDict = [];

    public override void _Ready()
    {
        // SET — populate dictionary by button name
        foreach (Node child in GetChildren())
        {
            if (child is TextureButton button)
            {
                buttonDict[button.Name] = button;
            }
        }
    }

    // GET — retrieve a button by name
    public TextureButton GetButton(string name)
    {
        if (buttonDict.TryGetValue(name, out TextureButton button))
            return button;

        GD.PrintErr($"Button '{name}' not found in dictionary.");
        return null;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

    public string[] Options => [.. buttonDict.Keys];
}
