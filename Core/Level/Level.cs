using Godot;
using System;
using System.Collections;
using System.Linq;
using System.Reactive.Linq;
using LanguageExt;

public partial class Level : Node
{
    public static Option<Level> Instance { get; private set; } = Option<Level>.None;

    private Action<(int, double)>? _physicsProcess;
    private int _physicsProcessTick = 0;

    public IObservable<(int, double)> PhysicsProcess => Observable.FromEvent<Action<(int, double)>, (int, double)>(
        handler =>
        {
            Action<(int, double)> oHandler = e => handler(e);
            return oHandler;
        },
        ohandler => _physicsProcess += ohandler,
        ohandler => _physicsProcess -= ohandler);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Instance = Option<Level>.Some(this);

        var subscribe = PhysicsProcess.Subscribe(
            value => GD.Print($"Tick: {value.Item1}, Delta: {value.Item2}"),
            ex => GD.PrintErr(ex.Message),
            () => GD.Print("Completed"));

        // subscribe.Dispose();
    }

    public override void _PhysicsProcess(double delta)
    {
        _physicsProcess?.Invoke((_physicsProcessTick, delta));
        _physicsProcessTick++;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}