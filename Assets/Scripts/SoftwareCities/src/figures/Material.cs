namespace SoftwareCities.figures
{
    /// <summary>
    /// Material constants.
    /// </summary>
    public struct Material
    {
        // Metal
        public static readonly Material WhiteMetal = new Material("white-metal");
        public static readonly Material GrayMetal = new Material("gray-metal");
        public static readonly Material BlueMetal = new Material("blue-metal");
        public static readonly Material BlueMetal2 = new Material("blue-metal-2");
        public static readonly Material GreenMetal = new Material("green-metal");
        public static readonly Material RedMetal = new Material("red-metal");
        public static readonly Material YellowMetal = new Material("yellow-metal");
        public static readonly Material BlackMetal = new Material("black-metal");
        public static readonly Material CyanMetal = new Material("cyan-metal");
        public static readonly Material OrangeMetal = new Material("orange-metal");
        public static readonly Material MagentaMetal = new Material("magenta-metal");

        // Neon
        public static readonly Material PurpleNeon = new Material("Buildings");
        public static readonly Material PurpleNeonNoEmission = new Material("M_Tileable_Cube_Purple_No_Emission");
        public static readonly Material NeonFloor = new Material("Floor");
        public static readonly Material RedNeon = new Material("M_Tileable_Cube_Red");

        // Glas
        public static readonly Material WhiteGlass = new Material("white-glass");
        public static readonly Material GrayGlass = new Material("gray-glass");
        public static readonly Material BlueGlass = new Material("blue-glass");
        public static readonly Material GreenGlass = new Material("green-glass");
        public static readonly Material RedGlass = new Material("red-glass");
        public static readonly Material YellowGlass = new Material("yellow-glass");
        public static readonly Material BlackGlass = new Material("black-glass");
        public static readonly Material CyanGlass = new Material("cyan-glass");
        public static readonly Material OrangeGlass = new Material("orange-glass");
        public static readonly Material MagentaGlass = new Material("magenta-glass");

        // Granite
        public static readonly Material WhiteGranite = new Material("white-granite");
        public static readonly Material GrayGranite = new Material("gray-granite");
        public static readonly Material BlueGranite = new Material("blue-granite");
        public static readonly Material GreenGranite = new Material("green-granite");
        public static readonly Material RedGranite = new Material("red-granite");
        public static readonly Material YellowGranite = new Material("yellow-granite");
        public static readonly Material BlackGranite = new Material("black-granite");
        public static readonly Material CyanGranite = new Material("cyan-granite");
        public static readonly Material OrangeGranite = new Material("orange-granite");
        public static readonly Material MagentaGranite = new Material("magenta-granite");

        // Sand
        public static readonly Material WhiteSand = new Material("white-sand");
        public static readonly Material GraySand = new Material("gray-sand");
        public static readonly Material BlueSand = new Material("blue-sand");
        public static readonly Material GreenSand = new Material("green-sand");
        public static readonly Material RedSand = new Material("red-sand");
        public static readonly Material YellowSand = new Material("yellow-sand");
        public static readonly Material BlackSand = new Material("black-sand");
        public static readonly Material CyanSand = new Material("cyan-sand");
        public static readonly Material OrangeSand = new Material("orange-sand");
        public static readonly Material MagentaSand = new Material("magenta-sand");

        // ...

        public static readonly Material Glowstone = new Material("glowstone");
        public static readonly Material Sand = new Material("Sand");

        public static readonly Material WoodOrange = new Material("wood-orange");
        public static readonly Material WoodBlue = new Material("wood-blue");
        public static readonly Material WoodGreen = new Material("wood-green");
        public static readonly Material WoodRed = new Material("wood-red");
        public static readonly Material WoodPurple = new Material("wood-purple");
        public static readonly Material WoodGrey = new Material("wood-grey");
        public static readonly Material Default = new Material("black-metal");

        public static readonly Material LineRed = new Material("LineNormalMappedRed");
        public static readonly Material LineGreen = new Material("LineNormalMappedGreen");
        public static readonly Material LineWhite = new Material("LineNormalMappedWhite");

        private string idName;

        private Material(string idName)
        {
            this.idName = idName;
        }

        public override string ToString()
        {
            return idName;
        }
    }
}