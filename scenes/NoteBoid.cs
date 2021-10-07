using Godot;
using System;

public class NoteBoid : CSBoid
{
    [Export]
    private string soundFolder;

    [Export]
    private NodePath audioPlayerNodePath;

    private bool debounce = true;
    private AudioStreamPlayer audioPlayer;
    private readonly Random rand = new Random();
    private static Godot.Collections.Array<string> soundFilePaths = null;
    private static readonly object soundFilePathsLock = new object();

    public override void _Ready()
    {
        base._Ready();

        lock (soundFilePathsLock)
        {
            if (soundFilePaths == null)
            {
                soundFilePaths = new Godot.Collections.Array<string>();

                Directory dir = new Directory();
                if (dir.Open(soundFolder) == Error.Ok)
                {
                    dir.ListDirBegin();
                    string fileName = dir.GetNext();

                    while (fileName != "")
                    {
                        if (!dir.CurrentIsDir())
                        {
                            if (fileName.Contains(".ogg") && !fileName.Contains(".import"))
                            {
                                soundFilePaths.Add(soundFolder + "/" + fileName);
                            }
                        }
                        fileName = dir.GetNext();
                    }
                }
            }
        }

        audioPlayer = GetNode<AudioStreamPlayer>(audioPlayerNodePath);
        string randomSoundFile = soundFilePaths[rand.Next(soundFilePaths.Count)];
        audioPlayer.Stream = GD.Load<AudioStream>(randomSoundFile);
        audioPlayer.Stop();
        audioPlayer.Seek(0);
    }

    public void PlayNote()
    {
        var animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animPlayer.Stop();
        animPlayer.Play("PlayNote");
        audioPlayer.Stop();
        audioPlayer.Play();
    }
}
