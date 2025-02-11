using System;
using Godot;

namespace Com.IsartDigital.WoolyWay.Scripts.Utils
{
    public static class NodeCreator
    {
        public static T CreateNode<T>(PackedScene pScene, Node2D pContainer, Vector2 pPos = default) where T : Node2D
        {
            T lNode = pScene.Instantiate() as T;
            pContainer.AddChild(lNode);
            lNode.Position = pPos;
            return lNode;
        }
    }
}
