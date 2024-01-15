using CryptoXML.Models.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Serialization;
using ServiceReference1;
using System.Data.SqlTypes;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

                    // Provjera dali postoji isti key
                    var existingEntry = await _context.CryptoData
                        .FirstOrDefaultAsync(e => e.Symbol == cryptoRate.Symbol && e.TajmStamp == cryptoRate.TajmStamp);

                    if (existingEntry == null)
                    {
                        // Dodaj ako !postoji
                        _context.CryptoData.Add(cryptoRate);
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Ok("An error occurred: " + ex.Message + ex);
            }
            return Ok("Successfully inserted into the database");
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





