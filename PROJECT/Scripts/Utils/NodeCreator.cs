using System;
using Godot;

namespace Com.IsartDigital.WoolyWay.Scripts.Utils
{
    public static class NodeCreator
    {
        public static T CreateNode<T>(PackedScene pScene, Node pContainer) where T : Node
        {
            T lNode = pScene.Instantiate<T>();
            pContainer.AddChild(lNode);
            return lNode;
        }

        public static T CreateNode<T>(PackedScene pScene, Node pContainer, Vector2 pPos) where T : Node
        {
            T lNode = CreateNode<T>(pScene, pContainer);

            if (lNode is Node2D)
                (lNode as Node2D).Position = pPos;
            else if (lNode is Control)
                (lNode as Control).Position = pPos;

            return lNode;
        }

        public static Node CreateNode(PackedScene pScene, Node pContainer)
        {
            return CreateNode<Node>(pScene, pContainer);
        }
    }
}
