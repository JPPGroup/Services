using System;
using System.ComponentModel;
using System.Linq;

namespace CadsRcUsage.Data
{
    public enum LicenseType : byte
    {
        [Description("CADS RC")] CadsRc = 0,
        [Description("CADS Viewport Manager")] CadsViewportManager = 1,
        [Description("CADS Scale")] CadsScale = 2,
        [Description("CADS Drawing Environment")] CadsDrawingEnvironment = 3,
        [Description("A3D Max")] A3dMax = 4,
        [Description("RC Beam Designer")] RcBeamDesigner = 5,
        [Description("RC Column Designer")] RcColumnDesigner = 6,
        [Description("SW Member Designer")] SwMemberDesigner = 7,
        [Description("RC Pad Base Designer")] RcPadBaseDesigner = 8,
        [Description("RC Pile Cap Designer")] RcPileCapDesigner = 9,
        [Description("SW Moment Connections")] SwMomentConnections = 10,
        [Description("SMART Engineer")] SmartEngineer = 11,
        [Description("Masonry Wall Panel Designer MAX")] MasonryWallPanelDesignerMax = 12,
        [Description("VelVenti")] VelVenti = 13,
        [Description("SMART Portal 3D")] SmartPortal3d = 14,
    }

    public enum LicenseCategory : byte
    {
        [Description("Analysis, Modelling & Design")] Design = 0,
        [Description("Detailing")] Detail = 1,
    }

    public static class EnumExtensions
    {
        public static string ToDescription(this Enum enumeration)
        {
            var attribute = enumeration.GetAttribute<DescriptionAttribute>();

            return attribute.Description;
        }

        private static T GetAttribute<T>(this Enum enumeration) where T : Attribute
        {
            var type = enumeration.GetType();

            var memberInfo = type.GetMember(enumeration.ToString());

            if (!memberInfo.Any())
                throw new ArgumentException($"No public members for the argument '{enumeration}'.");

            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);

            if (attributes == null || attributes.Length != 1)
                throw new ArgumentException($"Can't find an attribute matching '{typeof(T).Name}' for the argument '{enumeration}'");

            return attributes.Single() as T;
        }
    }
}