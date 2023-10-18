using Jpp.Projects.MailAI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jpp.Projects.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class MailController : ControllerBase
    {
        private readonly MailDbContext _db;

        public MailController(MailDbContext context)
        {
            _db = context;
        }

        [HttpPost]
        public async Task PutAsync(MailClassification mc)
        {
            int org = System.Text.ASCIIEncoding.Unicode.GetByteCount(mc.Body);
            mc.Body = mc.Body.Replace("\r", " ").Replace("\n", " ").Replace("\t", " "); ;

            var existing = await _db.Classifications.FindAsync(mc.Id);
            if (existing != null)
            {
                _db.Classifications.Remove(existing);
            }

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Jpp.Projects.MailAI.EmailStopWords.csv";

            string[] customSW;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                customSW = reader.ReadToEnd().Split(",");
            }

            MLContext mlContext = new MLContext();
            var textEstimator = mlContext.Transforms.Text.NormalizeText("Body", keepPunctuations: false, keepNumbers: false)
                .Append(mlContext.Transforms.Text.TokenizeIntoWords("Body"))
                .Append(mlContext.Transforms.Text.RemoveDefaultStopWords("Body"))
                .Append(mlContext.Transforms.Text.RemoveStopWords("Body", "Body", customSW));

            IDataView data = mlContext.Data.LoadFromEnumerable<MailClassification>(new[] { mc });

            ITransformer textTransformer = textEstimator.Fit(data);
            /*IDataView transformedData = textTransformer.Transform(data);
            var row = transformedData.Preview().RowView[0].Values[7].Value;*/

            var _wordEngine = mlContext.Model.CreatePredictionEngine<MailClassification, OutText>(textTransformer);

            var words = _wordEngine.Predict(mc);
            var result = new StringBuilder();
            foreach (var item in words.Body)
            {
                result.Append(" ");
                result.Append(item);
            }
            mc.Body = result.ToString();
            int process = System.Text.ASCIIEncoding.Unicode.GetByteCount(mc.Body);

            _db.Classifications.Add(mc);
            await _db.SaveChangesAsync();
        }
    }

    public class OutText
    {
        public string[] Body { get; set; }
    }
}
