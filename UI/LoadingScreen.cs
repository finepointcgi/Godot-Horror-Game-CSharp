using Godot;
using System;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

public partial class LoadingScreen : Control
{

	private string path;
	private bool loading;
	private bool inputKeyPressed;
	[Export]
	public bool WaitForInput = true;
	private bool waitingForInput;
	[Export]
	Godot.Collections.Array tips;
	private int index;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (loading)
		{
			var progress = new Godot.Collections.Array();
			var status = ResourceLoader.LoadThreadedGetStatus(path, progress);
			
			if(status == ResourceLoader.ThreadLoadStatus.InProgress)
			{
				GetNode<ProgressBar>("ProgressBar").Value = (double)progress[0] * 100;
			}else if (status == ResourceLoader.ThreadLoadStatus.Loaded)
			{
				SetProcess(false);
				GetNode<ProgressBar>("ProgressBar").Value = 100;
				if (WaitForInput)
				{
					waitingForInput = true;
				}
				else
				{
					ChangeScene(ResourceLoader.LoadThreadedGet(path) as PackedScene);
				}
            }else if(status == ResourceLoader.ThreadLoadStatus.InvalidResource)
			{
                ResourceLoader.LoadThreadedRequest(path, "", true);
            }
		}
	}

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
		if (waitingForInput)
		{
			if(@event is InputEventKey)
			{
				InputEventKey key = (InputEventKey) @event;
				if(inputKeyPressed)
				{
                    ChangeScene(ResourceLoader.LoadThreadedGet(path) as PackedScene);
                }

				if (key.Pressed)
				{
					inputKeyPressed= true;
				}
				else
				{
					inputKeyPressed= false;
				}
			}
		}
    }

    public void ChangeScene(PackedScene resource)
	{
		
        foreach (var item in GameManager.Instance.LevelBase.GetChildren())
		{
            GameManager.Instance.LevelBase.RemoveChild(item);
			item.QueueFree();
		}

        Node currentNode = resource.Instantiate();
        GameManager.Instance.LevelBase.AddChild(currentNode);
		GameManager.Instance.CheckForPlayer();
		GameManager.Instance.MovePlayer(index);
        GameManager.Instance.Paused = false;
        QueueFree();
	}

	public void LoadLevel(string path, int index)
	{
		this.path = path;
		this.index = index;
		Show();
		if(tips != null)
		{
			if (tips.Count != 0)
			{
				Random rnd = new Random();
				GetNode<Label>("Control/VBoxContainer2/TipValue").Text = (string)tips[rnd.Next(0, tips.Count - 1)];
			}
		}

		string[] levelNameParts = path.Split('/'); 
		string[] levelListWithExtension = levelNameParts.Last().Split("."); 

        GetNode<Label>("Control/VBoxContainer/LevelName").Text = levelListWithExtension[0];

		GameManager.Instance.Paused = true;

        if (ResourceLoader.HasCached(path))
		{
			ResourceLoader.LoadThreadedGet(path);
			loading = true;
		}
		else
		{
			ResourceLoader.LoadThreadedRequest(path, "" , true);
			loading = true;
		}
	}
}
