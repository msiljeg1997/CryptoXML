using CryptoXML.Models.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Serialization;
using ServiceReference1;
using System.Data.SqlTypes;

namespace CryptoXML.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BCCommunication : ControllerBase
    {
        private readonly IvekVjezbaKriptoContext _context;
        private readonly MainServisNETCore_PortClient _soapServiceClient;

        public BCCommunication(IvekVjezbaKriptoContext context)
        {
            _context = context;
            _soapServiceClient = new MainServisNETCore_PortClient();
        }

        private static T DeserializeXmlString<T>(string xml)
        {
            T? returnedXmlClass = default(T);

            using (TextReader reader = new StringReader(xml))
            {
                returnedXmlClass = (T?)new XmlSerializer(typeof(T)).Deserialize(reader) ?? throw new ArgumentException("ReturnedXmlClass NULL");
            }

            return returnedXmlClass;
        }

        [HttpGet]
        public async Task<IActionResult> GetTable()
        {
            string dataStream = "ExecuteMethod";

            try
            {
                var executeMethodResponse = await _soapServiceClient.ExecuteMethodAsync(dataStream);

                Root deserializedNavResponse;
                deserializedNavResponse = DeserializeXmlString<Root>(executeMethodResponse.Body.dataStream);
                foreach (var rate in deserializedNavResponse.Data)
                {

                    var cryptoRate = new CryptoData
                    {
                        Id = rate.Id,
                        Rank = rate.Rank,
                        Symbol = rate.Symbol,
                        Name = rate.Name,
                        Supply = (decimal)rate.Supply,
                        MaxSupply = (decimal)rate.MaxSupply,
                        MarketCapUsd = (decimal)rate.MarketCapUsd,
                        VolumeUsd24Hr = (decimal)rate.VolumeUsd24Hr,
                        PriceUsd = (decimal)rate.PriceUsd,
                        ChangePercent24Hr = (decimal)rate.ChangePercent24Hr,
                        Vwap24Hr = (decimal)rate.Vwap24Hr,
                        Explorer = rate.Explorer,
                        TajmStamp = rate.TajmStamp < (DateTime)SqlDateTime.MinValue ? (DateTime)SqlDateTime.MinValue :
                                    rate.TajmStamp > (DateTime)SqlDateTime.MaxValue ? (DateTime)SqlDateTime.MaxValue : rate.TajmStamp
                    };

                    _context.CryptoData.Add(cryptoRate);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Ok("An error occurred: " + ex.Message + ex);
            }
            return Ok("Successfully inserted into the database");
        }
    }
}





