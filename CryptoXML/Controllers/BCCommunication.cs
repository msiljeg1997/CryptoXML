using CryptoXML.Models.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Serialization;
using ServiceReference1;
using System.Data.SqlTypes;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Globalization;

namespace CryptoXML.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BCCommunication : ControllerBase
    {
        private readonly IvekVjezbaKriptoContext _context;
        private readonly ILogger<BCCommunication> _logger;
        private readonly MainServisNETCore_PortClient _soapServiceClient;

        public BCCommunication(IvekVjezbaKriptoContext context, ILogger<BCCommunication> logger)
        {
            _context = context;
            _logger = logger;
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

            List<string> logMessages = new List<string>();
            int successfulAdds = 0;
            int failedAdds = 0;

            try
            {
                var executeMethodResponse = await _soapServiceClient.ExecuteMethodAsync(dataStream);

                Root deserializedNavResponse;
                deserializedNavResponse = DeserializeXmlString<Root>(executeMethodResponse.Body.dataStream);

                for (int i = 1; i < deserializedNavResponse.Data.Count; i++)
                {

                    try
                    { 
                    
                    var rate = deserializedNavResponse.Data[i];

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
                        TajmStamp = rate.TajmStamp
                    };

                        var existingEntry = await _context.CryptoData
                     .FirstOrDefaultAsync(e => e.Symbol == cryptoRate.Symbol && e.TajmStamp == cryptoRate.TajmStamp);
                        if (existingEntry == null)
                        {
                            _context.CryptoData.Add(cryptoRate);
                            await _context.SaveChangesAsync();
                            successfulAdds++;
                        }
                        else
                        {
                          failedAdds++;
                        }
                    }

                    catch (Exception ex)
                    {
                        _logger.LogError($"An error occurred on iteration {i}: {ex.Message}", ex);

                    }
                }
            
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Ok("An error occurred: " + ex.Message + ex);
            }
            return Ok(new { Message = $" inserted {successfulAdds} items into the database failed entries because of same key {failedAdds}", LogMessages = logMessages, SuccessfulAdds = successfulAdds, failedAdds = failedAdds });
        }






        [HttpGet]
        [Route("api/dohvatiSve")]
        public async Task<ActionResult<List<CryptoData>>> GetAllEntries()
        {
            var entries = await _context.CryptoData.ToListAsync();
            if (entries == null)
                return BadRequest("nema nista u bazi sine..lutas");

            return Ok(entries);
        }


        [HttpGet("api/{symbol}")]

        public async Task<ActionResult<List<CryptoData>>> GetOneEntry(string symbol)
        {
            var entry = await _context.CryptoData.FirstOrDefaultAsync(e => e.Symbol == symbol);

            if (entry == null)
                return BadRequest("trazis nevidljivo, trazis nepostojece, lutas...");

            return Ok(entry);
        }


        [HttpPost]
        [Route("api/postavi")]

        public async Task<ActionResult<List<CryptoData>>> PostEntry(CryptoData entry)
        {
            _context.CryptoData.Add(entry);
            await _context.SaveChangesAsync();
            return Ok(entry);
        }

        [HttpPut]
        [Route("api/updejtaj{Symbol}")]

        public async Task<ActionResult<List<CryptoData>>> UpdateEntry(CryptoData updatedEntry, string Symbol)
        {
            var entry = await _context.CryptoData.FirstOrDefaultAsync(e => e.Symbol == Symbol);
            if (entry == null)
                return BadRequest("trazis nevidljivo, trazis nepostojece, lutas...");

            entry.Id = updatedEntry.Id;
            entry.Rank = updatedEntry.Rank;
            entry.Name = updatedEntry.Name;
            entry.Supply = updatedEntry.Supply;
            entry.MarketCapUsd = updatedEntry.MarketCapUsd;
            entry.VolumeUsd24Hr = updatedEntry.VolumeUsd24Hr;
            entry.PriceUsd = updatedEntry.PriceUsd;
            entry.ChangePercent24Hr = updatedEntry.ChangePercent24Hr;
            entry.Vwap24Hr = updatedEntry.Vwap24Hr;
            entry.Explorer = updatedEntry.Explorer;
            entry.MaxSupply = updatedEntry.MaxSupply;

            await _context.SaveChangesAsync();

            return Ok(entry);
        }

        [HttpDelete]
        [Route("api/brisi{Symbol}")]

        public async Task<ActionResult<List<CryptoData>>> DeleteEntry(string Symbol)
        {
            var entry = await _context.CryptoData.FirstOrDefaultAsync(e => e.Symbol == Symbol);
            if (entry == null) 
                return BadRequest("trazis nevidljivo, trazis nepostojece, nema to sta zelis da se obrise...lutas...");

            _context.CryptoData.Remove(entry);
            await _context.SaveChangesAsync();

            return Ok(entry + "izbrisano.");
        }

    }
}





