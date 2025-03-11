using Com.IsartDigital.ProjectName;
using Com.IsartDigital.WoolyWay;
using Com.IsartDigital.WoolyWay.Components;
using Com.IsartDigital.WoolyWay.GameObjects.Mobiles;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

//Author: Alissa DELATTRE
public partial class Target : GameObject
{
    [Export] private ClickableArea clickable;
    Pathfinding pathfinding = new Pathfinding();
    public override void _Ready()
    {
        base._Ready();
        clickable.Clicked += Clicked;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    private void Clicked()
    {
        Player lPlayer = Player.GetInstance();
        Vector2I lTargetPos = Grid.IndexDict[CurrentTile];
        Vector2I LStartPos = Grid.IndexDict[lPlayer.CurrentTile];
        List<Vector2I> lPath = pathfinding.GetPath(LStartPos.X, LStartPos.Y, lTargetPos.X, lTargetPos.Y);
        if (lPath.Count == 0) return;
        lPath.Remove(lPath.Last());
        lPlayer.path = lPath;
        lPlayer.MoveStepByStep();

    }
}
