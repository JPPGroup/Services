using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CadsRcUsage.Data
{
    public class CadsService
    {
        private readonly List<LicenseInformation> licenses;

        public CadsService()
        {
            licenses = new List<LicenseInformation>
            {
                LicenseInformation.A3dMax(),
                LicenseInformation.CadsDrawingEnvironment(),
                LicenseInformation.CadsRc(),
                LicenseInformation.CadsScale(),
                LicenseInformation.CadsViewportManager(),
                LicenseInformation.MasonryWallPanelDesignerMax(),
                LicenseInformation.RcBeamDesigner(),
                LicenseInformation.RcColumnDesigner(),
                LicenseInformation.RcPadBaseDesigner(),
                LicenseInformation.RcPileCapDesigner(),
                LicenseInformation.SmartEngineer(),
                LicenseInformation.SmartPortal3d(),
                LicenseInformation.SwMemberDesigner(),
                LicenseInformation.SwMomentConnections(),
                LicenseInformation.VelVenti(),
            };
        }

        public async Task<IEnumerable<LicenseInformation>> ReadLicenseInformationAsync()
        {
            return await Task.FromResult(ReadLicenseInformation());
        }

        private IEnumerable<LicenseInformation> ReadLicenseInformation()
        {
            licenses.ForEach(l => l.Clear());

            var files = Directory.EnumerateFiles(LicenseInformation.LICENSE_PATH).Where(f => f.EndsWith(LicenseInformation.EXTENSTION));

            foreach (var file in files)
            {
                foreach (var license in licenses.Where(license => license.IsMatch(file)))
                {
                    license.Add(file);
                }
            }

            return licenses;
        }
    }
}
