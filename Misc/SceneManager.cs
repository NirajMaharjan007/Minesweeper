using System.Threading.Tasks;
using Godot;

namespace Minesweeper.Misc;

public partial class SceneManager : Node
{
    private static SceneManager _instance;

    private ColorRect _fadeRect;

    private SceneManager() { }

    public override void _Ready()
    {
        _instance = this;

        _fadeRect = new ColorRect
        {
            Color = Colors.Black,
            MouseFilter = Control.MouseFilterEnum.Ignore,
            Modulate = new Color(0, 0, 0, 0),
            TopLevel = true,
            ZIndex = 100,
        };
        _fadeRect.MoveToFront();

        AddChild(_fadeRect);
    }

    public static async Task LoadScene(
        string tscnPath,
        string destinationAnchorPath = "",
        Control anchorNode = null
    )
    {
        if (anchorNode is null || destinationAnchorPath.Equals(""))
        {
            GD.PushWarning("Scene manager has found Null Statement, I HAVE RETURN");
            return;
        }

        _instance._fadeRect.AnchorTop = anchorNode.AnchorTop;
        _instance._fadeRect.AnchorBottom = anchorNode.AnchorBottom;
        _instance._fadeRect.AnchorLeft = anchorNode.AnchorLeft;
        _instance._fadeRect.AnchorRight = anchorNode.AnchorRight;

        _instance._fadeRect.SetPosition(anchorNode.Position);
        _instance._fadeRect.SetSize(anchorNode.Size);

        // Fade out
        var tween = _instance.CreateTween();
        tween.TweenProperty(_instance._fadeRect, "modulate", new Color(0, 0, 0, 1), 0.5f);
        await _instance.ToSignal(tween, "finished");

        // Load scene
        _instance.GetTree().ChangeSceneToFile(tscnPath);

        // Wait for new scene to be ready
        await _instance.ToSignal(_instance.GetTree(), "node_added");
        await _instance.ToSignal(_instance.GetTree(), SceneTree.SignalName.ProcessFrame);

        // Fade in
        var newAnchor = _instance.GetTree().CurrentScene.GetNode<Control>(destinationAnchorPath);
        _instance._fadeRect.SetPosition(newAnchor.GlobalPosition);
        _instance._fadeRect.SetSize(newAnchor.Size);

        tween = _instance.CreateTween();
        tween.TweenProperty(_instance._fadeRect, "modulate", new Color(0, 0, 0, 0), 0.5f);
        await _instance.ToSignal(tween, "finished");
    }

    public static async Task RestartScene(Control anchorNode = null)
    {
        if (anchorNode is null)
        {
            GD.PushWarning("THIS IS THE WARING, NODE SEEMS TO NULL (ｰ̀⤙ｰ́ )");
            return;
        }

        _instance._fadeRect.SetPosition(anchorNode.GlobalPosition);
        _instance._fadeRect.SetSize(anchorNode.Size);

        // Fade out
        var tween = _instance.CreateTween();
        tween.TweenProperty(_instance._fadeRect, "modulate", new Color(0, 0, 0, 1), 0.5f);
        await _instance.ToSignal(tween, "finished");

        _instance.GetTree().ReloadCurrentScene();

        // Wait for new scene
        await _instance.ToSignal(_instance.GetTree(), "node_added");
        await _instance.ToSignal(_instance.GetTree(), SceneTree.SignalName.ProcessFrame);

        // Fade in
        tween = _instance.CreateTween();
        tween.TweenProperty(_instance._fadeRect, "modulate", new Color(0, 0, 0, 0), 0.5f);
        await _instance.ToSignal(tween, "finished");
    }

    public static async Task FadeAndExit(Control anchorNode = null)
    {
        if (anchorNode is null)
        {
            GD.PushWarning("Scene manager Null FOUND, I HAVE RETURN");
            return;
        }

        _instance._fadeRect.AnchorTop = anchorNode.AnchorTop;
        _instance._fadeRect.AnchorBottom = anchorNode.AnchorBottom;
        _instance._fadeRect.AnchorLeft = anchorNode.AnchorLeft;
        _instance._fadeRect.AnchorRight = anchorNode.AnchorRight;

        _instance._fadeRect.SetPosition(anchorNode.Position);
        _instance._fadeRect.SetSize(anchorNode.Size);

        GD.Print($"RECT SIZE {_instance._fadeRect.Size} anchor SIZE {anchorNode.Size}");
        GD.Print(
            $"RECT Position {_instance._fadeRect.Position} anchor Postion {anchorNode.Position}"
        );

        // Force update

        // Move fade rect to be child of anchor node
        _instance._fadeRect.GetParent().RemoveChild(_instance._fadeRect);
        anchorNode.AddChild(_instance._fadeRect);

        var tween = _instance.CreateTween();
        tween.TweenProperty(_instance._fadeRect, "modulate", Colors.Black, 0.5f);
        await _instance.ToSignal(tween, "finished");

        _instance.GetTree().Quit();
    }
}
