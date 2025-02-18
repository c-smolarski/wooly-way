using Com.IsartDigital.WoolyWay.Managers;
using Godot;

// Author : Camille Smolarski

namespace Com.IsartDigital.WoolyWay.Utils
{
    public static class NodeCreator
    {
        /// <summary>
        /// Instantiates a Node and set's it's parent!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pScene"></param>
        /// <param name="pContainer"></param>
        /// <returns></returns>
        public static T CreateNode<T>(PackedScene pScene, Node pContainer) where T : Node
        {
            T lNode = pScene.Instantiate<T>();
            pContainer.AddChild(lNode);
            return lNode;
        }

        /// <summary>
        /// Instantiates a Node as child of its parent and sets Position!
        /// <para>If T is not Node2D, Control or any derived class the position will not be set.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pScene"></param>
        /// <param name="pContainer"></param>
        /// <param name="pPos"></param>
        /// <returns></returns>
        public static T CreateNode<T>(PackedScene pScene, Node pContainer, Vector2 pPos) where T : Node
        {
            T lNode = CreateNode<T>(pScene, pContainer);

            if (lNode is Node2D)
                (lNode as Node2D).Position = pPos;
            else if (lNode is Control)
                (lNode as Control).Position = pPos;

            return lNode;
        }

        /// <summary>
        /// Basic Node creation with it as child of specified parent.
        /// </summary>
        /// <param name="pScene"></param>
        /// <param name="pContainer"></param>
        /// <returns></returns>
        public static Node CreateNode(PackedScene pScene, Node pContainer)
        {
            return CreateNode<Node>(pScene, pContainer);
        }

        /// <summary>
        /// Creates a GameObject on specified tile.
        /// </summary>
        /// <param name="pScene"></param>
        /// <param name="pTile"></param>
        public static T CreateGameObject<T>(PackedScene pScene, Tile pTile) where T : GameObject
        {
            return CreateNode<T>(pScene, GameManger.Instance.GameContainer, pTile.Position);
        }

        public static T CreateGameObject<T>(PackedScene pScene, Vector2I pTileIndex) where T : GameObject
        {
            return CreateGameObject<T>(pScene, GridManager.TileDict[pTileIndex]);
        }
        
        public static T CreateGameObject<T>(PackedScene pScene, int pIndexX, int pIndexY) where T : GameObject
        {
            return CreateGameObject<T>(pScene, GridManager.TileDict[new Vector2I(pIndexX, pIndexY)]);
        }
    }
}
