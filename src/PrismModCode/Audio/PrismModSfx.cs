using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.TestSupport;

namespace PrismMod;

internal static class PrismModSfx
{
    public static void Play(string path, float volume = 1f)
    {
        if (TestMode.IsOn)
        {
            return;
        }

        AudioStream? stream = ResourceLoader.Load<AudioStream>(path);
        if (stream == null)
        {
            GD.PrintErr($"Missing Prism sfx: {path}");
            return;
        }

        Node? parent = NCombatRoom.Instance?.CombatVfxContainer;
        if (parent == null)
        {
            return;
        }

        var player = new AudioStreamPlayer
        {
            Stream = stream,
            VolumeLinear = volume,
            Bus = "SFX",
        };

        parent.AddChildSafely(player);
        player.Connect(AudioStreamPlayer.SignalName.Finished, Callable.From(() => player.QueueFree()));
        player.Play();
    }
}
