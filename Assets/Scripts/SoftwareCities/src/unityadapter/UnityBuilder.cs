using SoftwareCities.figures;
using UnityEngine;
using Material = SoftwareCities.figures.Material;

namespace SoftwareCities.unityadapter
{
    public class UnityBuilder : MonoBehaviour
    {
        public void Start()
        {
            var figure = ConstructTestFigure();
            var visitor = new UnityVisitor(figure);
            visitor.Run();
        }

        private Figure ConstructTestFigure()
        {
            var root = new Figure("root", Position.Zero(), Material.BlueMetal);
            var package = new Cuboid("de", Position.Xyz(0, 0, 0), Material.BlueMetal, 17, 22, 1);
            var qaware = new Cuboid("qaware", Position.Xyz(2, 1, 2), Material.BlueMetal, 13, 18, 1);
            var test = new Cuboid("test", Position.Xyz(2, 1, 2), Material.BlueMetal, 11, 14, 1);
            var clazz1 = new Cuboid("Visitor1", Position.Xyz(2, 1, 2), Material.WhiteGlass, 2, 3, 20);
            var clazz2 = new Cuboid("Visitor2", Position.Xyz(6, 1, 2), Material.WhiteGlass, 2, 3, 15);
            var clazz3 = new Cuboid("Visitor3", Position.Xyz(10, 1, 2), Material.WhiteGlass, 2, 3, 20);
            root.AddChild(package);
            package.AddChild(qaware);
            qaware.AddChild(test);
            test.AddChild(clazz1);
            test.AddChild(clazz2);
            test.AddChild(clazz3);
            return root;
        }
    }
}