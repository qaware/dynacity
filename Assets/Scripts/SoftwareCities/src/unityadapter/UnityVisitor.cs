using System.Collections.Generic;
using SoftwareCities.figures;
using UnityEngine;
using Material = UnityEngine.Material;

namespace SoftwareCities.unityadapter
{
    /// <summary>
    /// UnityVisitor constructs GameObjects from Figures (FigureApi).
    /// </summary>
    public class UnityVisitor : IFigureVisitor
    {
        private readonly Figure figureToVisit;

        public readonly List<GameObject> GameObjects = new List<GameObject>();

        /// <summary>
        /// Creates the Unity visitor.
        /// </summary>
        /// <param name="figure">The figure to visit.</param>
        public UnityVisitor(Figure figure)
        {
            figureToVisit = figure;
            figure.UpdateOrigin(Position.Zero()); // Update world positions
        }

        /// <summary>
        /// Runs the visitor.
        /// </summary>
        public void Run()
        {
            figureToVisit.Accept(this);
        }

        private Stack<GameObject> stack { get; } = new Stack<GameObject>();

        /// <summary>
        /// Figure
        /// </summary>
        public void VisitFigureEnter(Figure figure)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            SetProps(go, figure);
            stack.Push(go);

            // invisible object
            var meshRenderer = go.GetComponent<MeshRenderer>();
            meshRenderer.enabled = false;
        }

        public void VisitFigureLeave(Figure figure)
        {
            stack.Pop();
        }

        /// <summary>
        /// Cuboid
        /// </summary>
        public void VisitCuboidEnter(Cuboid cuboid)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            SetProps(go, cuboid);
            go.AddComponent<MouseClick>();
            go.AddComponent<BoxCollider>(); // for all cuboids add a box collider to support clicks
            go.AddComponent<RayCastSelector>(); // our RayCastLogic 
            stack.Push(go);
            GameObjects.Add(go);
        }

        public void VisitCuboidLeave(Cuboid cuboid)
        {
            stack.Pop();
        }

        /// <summary>
        /// Cylinder
        /// </summary>
        public void VisitCylinderEnter(Cylinder cylinder)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            var scale = V3ToUnity(cylinder.scale);
            go.transform.localScale = scale;

            // The calculation of the cylinder position is different to cubes since the base dimensions are 1,2,1
            go.transform.localPosition = new Vector3(
                cylinder.worldpos.GetX() + scale.x / 2f,
                cylinder.worldpos.GetY() + scale.y,
                cylinder.worldpos.GetZ() + scale.z / 2f
            );

            // set name
            go.name = cylinder.label;

            // set material
            go.GetComponent<Renderer>().material = MaterialToUnity(cylinder.material);

            // set parent
            AddChildToParent(go);

            stack.Push(go);
        }

        public void VisitCylinderLeave(Cylinder cylinder)
        {
            stack.Pop();
        }

        /// <summary>
        /// Sphere
        /// </summary>
        public void VisitSphereEnter(Sphere sphere)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            SetProps(go, sphere);
            stack.Push(go);
        }

        public void VisitSphereLeave(Sphere sphere)
        {
            stack.Pop();
        }

        /// <summary>
        /// Block
        /// </summary>
        public void VisitBlockEnter(Block block)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            SetProps(go, block);
            stack.Push(go);
        }

        public void VisitBlockLeave(Block block)
        {
            stack.Pop();
        }

        /// <summary>
        /// Parent Child Relationship:
        /// GameObject parent = GameObject.CreatePrimitive(PrimitiveType.Cube);
        /// parent.name = "parent";
        /// GameObject child = GameObject.CreatePrimitive(PrimitiveType.Cube);
        /// child.name = "child";
        /// child.transform.parent = parent.transform; // child is now a child of parent in the tree
        /// </summary>
        private void AddChildToParent(GameObject child)
        {
            if (stack.Count > 0)
            {
                child.transform.parent = stack.Peek().transform; // Parent - Child
            }
        }

        /// <summary>
        /// Helper to convert a System.Numerics.Vector3 to a UnityEngine.Vector3
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        private static Vector3 V3ToUnity(System.Numerics.Vector3 scale)
        {
            return new Vector3(scale.X, scale.Y, scale.Z);
        }

        /// <summary>
        /// Helper to convert a material to unity.
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        private static Material MaterialToUnity(figures.Material material)
        {
            return Resources.Load("Materials/" + material, typeof(Material)) as Material;
        }

        /// <summary>
        /// Helper sets the properties position, material, name and parent in the GameObject.
        /// </summary>
        /// <param name="go"></param>
        /// <param name="figure"></param>
        private void SetProps(GameObject go, Figure figure)
        {
            // set scale
            var scale = V3ToUnity(figure.scale);
            go.transform.localScale = scale;

            // set position
            go.transform.localPosition = new Vector3(
                figure.worldpos.GetX() + scale.x / 2f,
                figure.worldpos.GetY() + scale.y / 2f,
                figure.worldpos.GetZ() + scale.z / 2f
            );

            // set material
            Renderer renderer = go.GetComponent<Renderer>();
            renderer.material = MaterialToUnity(figure.material);

            // set name
            go.name = figure.label;

            // set parent
            AddChildToParent(go);
        }
    }
}