using System.Threading.Tasks;
using Godot;

public partial class SceneManager : Node
{
    private static readonly System.Lazy<SceneManager> _lazyInstance = new(() => new());

    private ColorRect _fadeRect;

    private SceneManager() { }

    public override void _Ready()
    {
        _fadeRect = new ColorRect();
        _fadeRect.Color = Colors.Black;
        _fadeRect.Size = GetViewport().GetVisibleRect().Size;
        _fadeRect.MouseFilter = Control.MouseFilterEnum.Ignore;
        _fadeRect.Modulate = new Color(0, 0, 0, 0);

        AddChild(_fadeRect);
    }

    public static async Task LoadScene(string tscnPath)
    {
        // Fade out
        var tween = Instance.CreateTween();
        tween.TweenProperty(Instance._fadeRect, "modulate", new Color(0, 0, 0, 1), 0.5f);
        await Instance.ToSignal(tween, "finished");

        // Load scene
        Instance.GetTree().ChangeSceneToFile(tscnPath);

        // Fade in
        tween = Instance.CreateTween();
        tween.TweenProperty(Instance._fadeRect, "modulate", new Color(0, 0, 0, 0), 0.5f);
        await Instance.ToSignal(tween, "finished");
    }

    public static SceneManager Instance => _lazyInstance.Value;
}
