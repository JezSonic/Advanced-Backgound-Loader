using Godot;
using System;
using System.Threading;

    
public class BackgroundLoader : Node
{
    public static BackgroundLoader instance = null;
    Resource res;
    public bool canChange = false;
    System.Threading.Thread thread;
    Node newScene;
    string scenePath = "";

    public override void _Ready()
    {
        instance = this;
    }
    private void ThreadLoad() {
        ResourceInteractiveLoader ril = ResourceLoader.LoadInteractive(scenePath);
        int total = ril.GetStageCount();
        instance.res = null;
        while(true) {
            Error err = ril.Poll();
            if (err == Error.FileEof) {
                instance.res = ril.GetResource();
                instance.canChange = true;
                break;
            }
            else if (err != Error.Ok) {
                GD.Print("ERROR LOADING");
                break;
            }

        }
    }

    private void ClearStuff() {
        thread = null;
        newScene = null;
        scenePath = "";
        canChange = false;
        res = null;
    }
    private async void ThreadDone(PackedScene packedScene) {
        while (thread.IsAlive) {
            GD.Print("THread is alive!");
            await ToSignal(GetTree().CreateTimer(1,false),"timeout");
        }
        newScene = packedScene.Instance();
        GetTree().CurrentScene.Free();
        GetTree().CurrentScene = null;
        GetTree().Root.AddChild(newScene);
        GetTree().CurrentScene = newScene;
        ClearStuff();
        
    }

    public void PreloadScene(string path) {
        GD.Print("Calling new Scene");
        scenePath = path;
        canChange = false;
        thread = new System.Threading.Thread(new ThreadStart(ThreadLoad));
        thread.Start();
        Raise();
    }

    public void ChangeSceneToPreloaded() {
        CallDeferred(nameof(ThreadDone), res);
    }


    


}


