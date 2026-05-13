using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Minesweeper.Scripts;

public partial class Activity : Control
{
    [Export] private Texture2D _maskedTile;
    [Export] private Texture2D _revealedTileBomb;
    [Export] private Texture2D _maskedTileQuestionMark;
    [Export] private Texture2D _maskedTileFlag;

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

    private readonly Dictionary<ButtonType, TextureButton> buttonDict = [];
    private static readonly Dictionary<ButtonType, Texture2D> textureMap = [];

    public override void _Ready()
    {
        textureMap.Add(ButtonType.BUTTON, _maskedTile);
        textureMap.Add(ButtonType.EXPLODE, _revealedTileBomb);
        textureMap.Add(ButtonType.QUESTIONMARK, _maskedTileQuestionMark);
        textureMap.Add(ButtonType.REDFLAG, _maskedTileFlag);

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
