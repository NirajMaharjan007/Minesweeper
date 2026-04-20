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

    private static readonly System.Collections.Generic.Dictionary<
        ButtonType,
        Texture2D
    > textureMap = new()
    {
        { ButtonType.BUTTON, GD.Load<Texture2D>("res://Assets/icons/masked_tile.png") },
        { ButtonType.EXPLODE, GD.Load<Texture2D>("res://Assets/icons/revealed_tile_bomb.png") },
        {
            ButtonType.QUESTIONMARK,
            GD.Load<Texture2D>("res://Assets/icons/masked_tile_question_mark.png")
        },
        { ButtonType.REDFLAG, GD.Load<Texture2D>("res://Assets/icons/masked_tile_flag.png") },
    };

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

    internal TextureButton GetButton(ButtonType type)
    {
        if (buttonDict.TryGetValue(type, out TextureButton button))
            return button;

        GD.PrintErr($"Button '{type}' not found in dictionary.");
        return null;
    }

    internal static Texture2D GetTexture(ButtonType type)
    {
        return textureMap.TryGetValue(type, out var tex) ? tex : null;
    }

    internal static bool CompareTextures(Texture2D source, Texture2D destination)
    {
        return source.Equals(destination);
    }
}
