using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace CadsRcUsage.Data
{
    public class LicenseInformation
    {
        public const string EXTENSTION = ".net";
        public const string LICENSE_PATH = @"\\fileserver\CompanyData\Consulting\CADS_RC";

        private readonly List<string> users;

        public LicenseCategory Category { get; private set; }
        public LicenseType Type { get; private set; }
        public int MaxUsers { get; private set; }
        public string FilePattern { get; private set; }
        public IReadOnlyList<string> Users => users.Distinct().ToList();

        private LicenseInformation()
        {
            users = new List<string>();
        }

        public void Clear()
        {
            users.Clear();
        }

        public void Add(string file)
        {
            users.Add(GetFileOwner(file));
        }

        private static string GetFileOwner(string file)
        {
            var fileSecurity = new FileSecurity(file, AccessControlSections.Owner);
            return fileSecurity.GetOwner(typeof(NTAccount)).ToString();
        }

        public bool IsMatch(string file)
        {
            var filename = Path.GetFileNameWithoutExtension(file);
            var license = Regex.Replace(filename, @"[\d-]", string.Empty);

            return !string.IsNullOrEmpty(FilePattern) && license.ToLower().Equals(FilePattern.ToLower());
        }

        public static LicenseInformation CadsRc()
        {
            return new LicenseInformation
            {
                Category = LicenseCategory.Detail, 
                Type =  LicenseType.CadsRc, 
                MaxUsers = 5,
                FilePattern = "RC"
            };
        }

        public static LicenseInformation CadsViewportManager()
        {
            return new LicenseInformation
            {
                Category = LicenseCategory.Detail,
                Type = LicenseType.CadsViewportManager,
                MaxUsers = 5,
                FilePattern = "VPM"
            };
        }

        public static LicenseInformation CadsScale()
        {
            return new LicenseInformation
            {
                Category = LicenseCategory.Detail,
                Type = LicenseType.CadsScale,
                MaxUsers = 5,
                FilePattern = "SC"
            };
        }

        public static LicenseInformation CadsDrawingEnvironment()
        {
            return new LicenseInformation
            {
                Category = LicenseCategory.Detail,
                Type = LicenseType.CadsDrawingEnvironment,
                MaxUsers = 5,
                FilePattern = "DE"
            };
        }

        public static LicenseInformation A3dMax()
        {
            return new LicenseInformation
            {
                Category = LicenseCategory.Design,
                Type = LicenseType.A3dMax,
                MaxUsers = 4,
                FilePattern = "A3DM"
            };
        }

        public static LicenseInformation RcBeamDesigner()
        {
            return new LicenseInformation
            {
                Category = LicenseCategory.Design,
                Type = LicenseType.RcBeamDesigner,
                MaxUsers = 1,
                FilePattern = "RCBD"
            };
        }

        public static LicenseInformation RcColumnDesigner()
        {
            return new LicenseInformation
            {
                Category = LicenseCategory.Design,
                Type = LicenseType.RcColumnDesigner,
                MaxUsers = 1,
                FilePattern = "RCC"
            };
        }

        public static LicenseInformation SwMemberDesigner()
        {
            return new LicenseInformation
            {
                Category = LicenseCategory.Design,
                Type = LicenseType.SwMemberDesigner,
                MaxUsers = 2,
                FilePattern = "SWM"
            };
        }

        public static LicenseInformation RcPadBaseDesigner()
        {
            return new LicenseInformation
            {
                Category = LicenseCategory.Design,
                Type = LicenseType.RcPadBaseDesigner,
                MaxUsers = 1,
                FilePattern = "RCPB"
            };
        }

        public static LicenseInformation RcPileCapDesigner()
        {
            return new LicenseInformation
            {
                Category = LicenseCategory.Design,
                Type = LicenseType.RcPileCapDesigner,
                MaxUsers = 1,
                FilePattern = "RCPC"
            };
        }

        public static LicenseInformation SwMomentConnections()
        {
            return new LicenseInformation
            {
                Category = LicenseCategory.Design,
                Type = LicenseType.SwMomentConnections,
                MaxUsers = 1,
                FilePattern = "SWMC"
            };
        }

        public static LicenseInformation SmartEngineer()
        {
            return new LicenseInformation
            {
                Category = LicenseCategory.Design,
                Type = LicenseType.SmartEngineer,
                MaxUsers = 1,
                FilePattern = "smrt"
            };
        }

        public static LicenseInformation MasonryWallPanelDesignerMax()
        {
            return new LicenseInformation
            {
                Category = LicenseCategory.Design,
                Type = LicenseType.MasonryWallPanelDesignerMax,
                MaxUsers = 2,
                FilePattern = "MWPD"
            };
        }

        public static LicenseInformation VelVenti()
        {
            return new LicenseInformation
            {
                Category = LicenseCategory.Design,
                Type = LicenseType.VelVenti,
                MaxUsers = 1,
                FilePattern = "VEL"
            };
        }

        public static LicenseInformation SmartPortal3d()
        {
            return new LicenseInformation
            {
                Category = LicenseCategory.Design,
                Type = LicenseType.SmartPortal3d,
                MaxUsers = 1,
                FilePattern = "SPBD"
            };
        }
    }
}
