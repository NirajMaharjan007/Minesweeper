using System.Linq;
using Godot;

namespace Minesweeper.Scripts;

public partial class Activity : Control
{
    public enum ButtonType
    {
        BUTTON,
        EXPLODE,
        REVEALEDBOMB,
        REDFLAG,
        QUESTIONMARK,
        NUMBER_ONE,
        NUMBER_TWO,
        NUMBER_THREE,
        NUMBER_FOUR,
        NUMBER_FIVE,
        NUMBER_SIX,
        NUMBER_SEVEN,
        NUMBER_EIGHT,
    }

    private readonly System.Collections.Generic.Dictionary<ButtonType, TextureButton> buttonDict =
    [];

    public override void _Ready()
    {
        // SET — populate dictionary by button name
        foreach (Node child in GetChildren())
        {
            var children = GetChildren().OfType<TextureButton>().ToList();

            foreach (ButtonType type in System.Enum.GetValues<ButtonType>())
            {
                int index = (int)type;
                if (index < children.Count)
                    buttonDict[type] = children[index];
                else
                    GD.PrintErr($"No button found for {type}");
            }
        }
    }

    // GET — retrieve a button by name
    public TextureButton GetButton(ButtonType type)
    {
        if (buttonDict.TryGetValue(type, out TextureButton button))
            return button;

        GD.PrintErr($"Button '{type}' not found in dictionary.");
        return null;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }
}
