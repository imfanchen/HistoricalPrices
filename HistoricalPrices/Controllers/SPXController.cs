using HistoricalPrices.Core;
using HistoricalPrices.Models;
using HistoricalPrices.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HistoricalPrices.Controllers {
    [Route("api/spx")]
    [ApiController]
    public class SPXController : ControllerBase {
        private readonly CsvService _csvService;

        public SPXController(CsvService csvService) => _csvService = csvService;

        // GET: api/spx
        [HttpGet]
        public ActionResult<List<Price>> Get() {
            List<Price> prices = _csvService.Get();
            if (prices == null) return NotFound();
            return prices;
        }

        // GET api/spx/2021-11-08
        // Retrieve the open, high, low, and close for a given date.
        [HttpGet("{date}")]
        public ActionResult<Price> Get(DateTime date) {
            Price? price = _csvService.Get(date);
            if (price == null) return NotFound();
            return Ok(price);
        }

        // GET api/spx/range?start=2021-11-08&end=2021-11-10
        // Retrieve the high and low over a given range of time. Parameters will be a start date and an end date.
        [HttpGet("range")]
        public IActionResult GetRange([FromQuery] DateTime start, [FromQuery] DateTime end) {
            var prices = _csvService.Get()
                ?.Where(p => p.Date >= start && p.Date <= end)
                ?.Select(p => new { p.Date, p.High, p.Low })
                ?.ToList();
            if (prices == null) return NotFound();
            return Ok(prices);
        }

        // GET 
        // Retrieve the 7 and 30 day average, min, and max closing price.
        [HttpGet("rolling/{interval}")]
        public IActionResult GetAggregate(int interval, [FromQuery] DateTime start, [FromQuery] DateTime end) {
            // In the real world we should use an AddBusinessDays funciton with consideration to holiday when market is off.
            var prices = _csvService.Get()
                ?.Where(p => p.Date >= start.AddBusinessDays(-interval) && p.Date <= end)
                ?.ToList();
            if (prices == null) return NotFound();
            Dictionary<DateTime, decimal> averagePrices = GetAverageClosingPrice(prices, interval);
            Dictionary<DateTime, decimal> maxPrices = GetMaxClosingPrice(prices, interval);
            Dictionary<DateTime, decimal> minPrices = GetMinClosingPrice(prices, interval);
            var aggregatedPrices = prices.Where(p => p.Date >= start && p.Date <= end)
                .Select(p => new {
                    p.Date,
                    Average = averagePrices[p.Date],
                    Max = maxPrices[p.Date],
                    Min = minPrices[p.Date]
                });
            return Ok(aggregatedPrices);
        }



        private Dictionary<DateTime, decimal> GetAverageClosingPrice(List<Price> prices, int lookAhead) {
            var dict = new Dictionary<DateTime, decimal>();
            decimal sum = 0;
            int left = 0;
            for (int right = 0; right < prices.Count(); right++) {
                var price = prices[right];
                sum += price.Close;
                int readCount = right + 1; // 1 based indexing
                if (readCount <= lookAhead) {
                    dict.Add(price.Date, sum / readCount);
                } else {
                    sum -= prices[left].Close;
                    left++;
                    dict.Add(price.Date, sum / lookAhead);
                }
            }
            return dict;
        }

        private Dictionary<DateTime, decimal> GetMaxClosingPrice(List<Price> prices, int lookAhead) {
            // prices.ForEach(p => p.Commparing = nameof(Price.Close));
            MaxQueue<Price> maxQueue = new MaxQueue<Price>();
            Dictionary<DateTime, decimal> dict = new Dictionary<DateTime, decimal>();
            for (int i = 0; i < prices.Count; i++) {
                var price = prices[i];
                if (i >= lookAhead) {
                    maxQueue.Dequeue();
                }
                maxQueue.Enqueue(price);
                Price? max = maxQueue.GetMax();
                dict.Add(price.Date, max != null ? max.Close : 0);
            }
            return dict;
        }

        private Dictionary<DateTime, decimal> GetMinClosingPrice(List<Price> prices, int lookAhead) {
            // prices.ForEach(p => p.Commparing = nameof(Price.Close));
            MinQueue<Price> minQueue = new MinQueue<Price>();
            Dictionary<DateTime, decimal> dict = new Dictionary<DateTime, decimal>();
            for (int i = 0; i < prices.Count; i++) {
                var price = prices[i];
                if (i >= lookAhead) {
                    minQueue.Dequeue();
                }
                minQueue.Enqueue(price);
                Price? min = minQueue.GetMin();
                dict.Add(price.Date, min != null ? min.Close : 0);
            }
            return dict;
        }


        // POST api/spx
        /*{
          "date": "2022-03-26",
          "high": 3000,
          "low": 2000,
          "open": 2250,
          "close": 2750
        }*/
        [HttpPost]
        public IActionResult Post(Price price) {
            _csvService.Create(price);
            return Created($"api/spx/{price.Date}", price);
        }


        // PUT api/spx/2022-03-26
        /*{
          "date": "2022-03-26",
          "high": 4300,
          "low": 4000,
          "open": 4100,
          "close": 4200
        }*/
        [HttpPut("{date}")]
        public IActionResult Put(DateTime date, [FromBody] Price price) {
            if (date != price.Date) return BadRequest();
            Price? original = _csvService.Get(date);
            if (original == null) return NotFound();
            _csvService.Update(date, price);
            return Accepted(price);

        }

        // DELETE api/spx/2022-03-26
        [HttpDelete("{date}")]
        public IActionResult Delete(DateTime date) {
            Price? original = _csvService.Get(date);
            if (original == null) return NotFound();
            _csvService.Remove(date);
            return Delete(date);
        }
    }
}
