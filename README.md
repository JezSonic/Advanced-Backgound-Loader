## Using GDScript Version of Advanced Background Loader

#### First Steps

* Open Godot
* Open Project Settings
* Go to Plugins
* Enable Addon called: `Advanced Background Loader`

#### Understanding it

since the code provides:
* `BackgroundLoader`

when you open the first scene of your game this instance can be accessed anywhere
without any problem, from any script just type:
* `BackgroundLoader.MethodNameHere`

Basically Background Loader will create a new thread, and load your scene while you still can do other things, (also loads faster due to threading)

### Code Examples

* Simplest case possible

```gd
    //set the scene to load and start thread
    BackgroundLoader.preload_scene("res://scn_menu.tscn"); 
    while (BackgroundLoader.can_change == false):
        BackgroundLoader.change_scene_to_preloaded();
```

## Using CSharp Version of Advanced Background Loader

#### First Steps

* Open Godot
* Open Project Settings
* Go to Autoload
* Select `BackgroundLoader.cs` at `res://addons/advanced_background_loader_csharp`

#### Understanding it

since the code provides:
* `public static BackgrounLoader instance ....`

when you open the first scene of your game this instance can be accessed anywhere
without any problem, from any script just type:
* `BackgroundLoader.instance.MethodNameHere`

Basically Background Loader will create a new thread, and load your scene while you still can do other things, (also loads faster due to threading)

### Code Examples

* Simplest case possible

```csharp
    //set the scene to load and start thread
    BackgroundLoader.instance.PreloadScene("res://scn_menu.tscn"); 
    while(BackgroundLoader.instance.canChange == false) {
            //wait every second to continue, await requires method to have async
            await ToSignal(GetTree().CreateTimer(1,false),"timeout"); 
            //alternatively you can use await ToSignal(GetTree(),"idle_frame")
            //in case you need to load as fast as possible (huge scenes)
        }
    BackgroundLoader.instance.ChangeSceneToPreloaded();
```

* Advanced Case

Lets suppose you want to make a transition everytime you want to change scene
So we have Transition.ChangeScene(string scenePath);

```csharp
    public async void ChangeScene(string scenePath) {
        GD.Print("Starting transition");
        //Make the node goes to the top of the screen (appear in front)
        CallDeferred("raise"); 
        //play fade in animation
        animationPlayer.Play("FadeIn");
        Visible = true;

        //Start Preloading next scene
        BackgroundLoader.instance.PreloadScene(scenePath);
        
        while(BackgroundLoader.instance.canChange == false) {
            await ToSignal(GetTree().CreateTimer(1,false),"timeout");
        }

        BackgroundLoader.instance.ChangeSceneToPreloaded();
        //using this code bellow to avoid bugs on the tree
        await ToSignal(GetTree(),"idle_frame");
        CallDeferred("raise");
        animationPlayer.Play("FadeOut");
        GD.Print("Done");
```


