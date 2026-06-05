using System.Threading.Tasks;
using Godot;

namespace Minesweeper.Misc;

public partial class SceneManager : Node
{
    private static SceneManager _instance;

    private ColorRect _fadeRect;

    public override void _Ready()
    {
        _instance = this;

        _fadeRect = new ColorRect
        {
            Color = Colors.Black,
            AnchorRight = 1,
            AnchorBottom = 1,
            MouseFilter = Control.MouseFilterEnum.Ignore,
            Modulate = new Color(0, 0, 0, 0),
            TopLevel = true,
            ZIndex = 100,
        };
        _fadeRect.MoveToFront();

        AddChild(_fadeRect);
    }

    public static async Task LoadScene(string tscnPath)
    {
        // Fade out
        var tween = _instance.CreateTween();
        tween.TweenProperty(_instance._fadeRect, "modulate", new Color(0, 0, 0, 1), 0.5f);
        await _instance.ToSignal(tween, "finished");

        // Load scene
        _instance.GetTree().ChangeSceneToFile(tscnPath);

        // Fade in
        tween = _instance.CreateTween();
        tween.TweenProperty(_instance._fadeRect, "modulate", new Color(0, 0, 0, 0), 0.5f);
        await _instance.ToSignal(tween, "finished");
    }
}
