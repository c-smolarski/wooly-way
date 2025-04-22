using Com.IsartDigital.WoolyWay.Managers;
using Godot;
using System.Collections.Generic;

namespace Com.IsartDigital.WoolyWay.Juiciness
{
    public partial class Grass : Node2D
    {
        [Export] private Polygon2D polygon;
        [Export] public GpuParticles2D grassParticles;
        [Export] private Node2D stepTarget;
        private ShaderMaterial canvasItemMat;
        private ShaderMaterial processMat;

        public float ShaderAlpha
        {
            get => (float)canvasItemMat.GetShaderParameter("alpha");
            set => canvasItemMat.SetShaderParameter("alpha", value);
        }

        public override void _Ready()
        {
            Visible = true;
            canvasItemMat = (ShaderMaterial)(grassParticles.Material = (ShaderMaterial)grassParticles.Material.Duplicate());
            processMat = (ShaderMaterial)(grassParticles.ProcessMaterial = (ShaderMaterial)grassParticles.ProcessMaterial.Duplicate());
            SignalBus.Instance.StartLevel += OnLevelStart;

            if (polygon == null)
                return;
            polygon.Color = Colors.Transparent;
            UpdatePolygonInShader();
        }

        private void OnLevelStart(Grid pGrid)
        {
            Visible = false;
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
            UpdatePolygonInShader(); // Update the polygon every frame to adapt to movement
                                     //if (stepTarget != null) 
            canvasItemMat.SetShaderParameter("player_pos", GetGlobalMousePosition());
        }

        private void UpdatePolygonInShader()
        {
            if (polygon == null || processMat == null) return;

            List<Vector2> polygonVertices = new List<Vector2>();
            foreach (Vector2 localPoint in polygon.Polygon)
            {
                polygonVertices.Add(polygon.ToGlobal(localPoint));
            }

            Vector2[] flatPolygon = polygonVertices.ToArray();
            processMat.SetShaderParameter("polygon", flatPolygon);
            processMat.SetShaderParameter("polygon_size", polygonVertices.Count);
        }

        protected override void Dispose(bool pDisposing)
        {
            SignalBus.Instance.StartLevel -= OnLevelStart;
            base.Dispose(pDisposing);
        }
    }
}
