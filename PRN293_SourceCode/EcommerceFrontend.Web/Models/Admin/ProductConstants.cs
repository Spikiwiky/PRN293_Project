using System.Collections.Generic;

namespace EcommerceFrontend.Web.Models.Admin
{
    public static class ProductConstants
    {
        public static readonly List<string> ValidSizes = new()
        {
            "XS", "S", "M", "L", "XL", "XXL"
        };

        public static readonly List<string> ValidColors = new()
        {
            "Red", "Blue", "Green", "Black", "White",
            "Yellow", "Purple", "Orange", "Pink",
            "Brown", "Gray", "Navy"
        };

        public static readonly Dictionary<string, string> ColorHexCodes = new()
        {
            { "Red", "#FF0000" },
            { "Blue", "#0000FF" },
            { "Green", "#008000" },
            { "Black", "#000000" },
            { "White", "#FFFFFF" },
            { "Yellow", "#FFFF00" },
            { "Purple", "#800080" },
            { "Orange", "#FFA500" },
            { "Pink", "#FFC0CB" },
            { "Brown", "#A52A2A" },
            { "Gray", "#808080" },
            { "Grey", "#808080" },
            { "Navy", "#000080" }
        };
    }
} 