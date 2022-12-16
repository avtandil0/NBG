using Microsoft.Extensions.Configuration;
using NBG.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace NBG
{
    public class Currency
    {
        public string code { get; set; }
        public int quantity { get; set; }
        public string rateFormated { get; set; }
        public string diffFormated { get; set; }
        public double rate { get; set; }
        public string name { get; set; }
        public double diff { get; set; }
        public string date { get; set; }
        public string validFromDate { get; set; }
    }

    public class NBGService
    {
        public string date { get; set; }
        public List<Currency> currencies { get; set; }

    }
    class Program
    {
        //static async Task Main(string[] args)
        //{
        //    //using var client = new HttpClient();

        //    //client.BaseAddress = new Uri("https://nbg.gov.ge/gw/");
        //    //client.DefaultRequestHeaders.Add("User-Agent", "C# console program");
        //    //client.DefaultRequestHeaders.Accept.Add(
        //    //        new MediaTypeWithQualityHeaderValue("application/json"));

        //    //var url = "api/ct/monetarypolicy/currencies/ka/json";
        //    //HttpResponseMessage response = await client.GetAsync(url);
        //    //response.EnsureSuccessStatusCode();
        //    //var resp = await response.Content.ReadAsStringAsync();


        //    //List<NBGService> currencies = JsonConvert.DeserializeObject<List<NBGService>>(resp);
        //}



        private static IConfiguration _iconfiguration;


        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();

            var dbConfig = config.GetSection("DBConfig").Get<DBConfig>();
            //var mySecondClass = config.GetSection("MySecondClass").Get<MySecondClass>();

            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"D:\logs\ExtRatesLogs.txt", true))
            {
                file.WriteLine(DateTime.Now + " Start process");
            }
            Console.WriteLine("Start Process . . .");

            try
            {
                GetAppSettingsFile();
                await RunJobAsync(dbConfig);
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"D:\logs\ExtRatesLogs.txt", true))
                {
                    file.WriteLine(" ! ! !  ERROR ! ! !  " + DateTime.Now + " " + e.Message);
                }
                throw;
            }


            using (System.IO.StreamWriter file =
                    new System.IO.StreamWriter(@"D:\logs\ExtRatesLogs.txt", true))
            {
                file.WriteLine(DateTime.Now + " Finish process");
            }
            Console.WriteLine("Finish Process.");
        }

        static async Task<List<NBGService>> GetCurrencyAsync()
        {
            using var client = new HttpClient();

            client.BaseAddress = new Uri("https://nbg.gov.ge/gw/");
            client.DefaultRequestHeaders.Add("User-Agent", "C# console program");
            client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

            var url = "api/ct/monetarypolicy/currencies/ka/json";
            List<NBGService> currencies;
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var resp = await response.Content.ReadAsStringAsync();
                currencies = JsonConvert.DeserializeObject<List<NBGService>>(resp);

            }
            catch (Exception ex)
            {
                throw;
            }



            return currencies;
        }

        static void GetAppSettingsFile()
        {
            var builder = new ConfigurationBuilder()
                                 .SetBasePath(Directory.GetCurrentDirectory())
                                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _iconfiguration = builder.Build();
        }

        static async Task RunJobAsync(DBConfig DBConfig)
        {

            string _connectionString = _iconfiguration.GetConnectionString("Default");

            var currentDate = DateTime.Now;
            var currenciesData = await GetCurrencyAsync();
            var currencies = currenciesData[0].currencies;
            var dt = new DateTime(currentDate.Year,
                currentDate.Month,
                currentDate.Day,
                0, 0, 0);
            using (NBGContext db = new NBGContext())
            {
                foreach (var item in DBConfig.Tables)
                {
                    foreach (var currency in currencies)
                    {
                        if (DBConfig.Currencies.Contains(currency.code))
                        {

                            if (item.Contains("Currency"))
                            {
                                var rateNL = new NlCeOct19CurrencyExchangeRate437dbf0e84ff417a965dEd2bb9650972()
                                {
                                    CurrencyCode = currency.code,
                                    StartingDate = currentDate,
                                    ExchangeRateAmount = 1,
                                    AdjustmentExchRateAmount = 1,
                                    RelationalCurrencyCode = "GEL1",
                                    RelationalExchRateAmount = (decimal)currency.rate,
                                    FixExchangeRateAmount = 0,
                                    RelationalAdjmtExchRateAmt = (decimal)currency.rate,
                                    SystemCreatedAt = currentDate,
                                    SystemModifiedAt = currentDate,

                                };

                                if (DBConfig.Currencies.Contains(currency.code))
                                {
                                    var exist = db.NlCeOct19CurrencyExchangeRate437dbf0e84ff417a965dEd2bb9650972s
                                                        .FirstOrDefault(x => (x.CurrencyCode == rateNL.CurrencyCode &&
                                                            x.StartingDate.Day == rateNL.StartingDate.Day &&
                                                             x.StartingDate.Month == rateNL.StartingDate.Month &&
                                                             x.StartingDate.Year == rateNL.StartingDate.Year));

                                    if (exist == null)
                                    {

                                        db.NlCeOct19CurrencyExchangeRate437dbf0e84ff417a965dEd2bb9650972s.Add(rateNL);
                                    }

                                }

                            }
                            else
                            {
                                var rate = new Rate()
                                {
                                    SourceCurrency = currency.code,
                                    TargetCurrency = "GEL",
                                    DateL = dt,
                                    RateExchange = currency.rate,
                                    Syscreator = 0,
                                    Sysmodifier = 0,
                                    RateBuy = 0,
                                    RateOfficial = 0,
                                    RateSell = 0,
                                    Syscreated = currentDate,
                                    Sysmodified = currentDate,
                                    Sysguid = Guid.NewGuid(),
                                    Division = 850
                                };

                                if (DBConfig.Currencies.Contains(currency.code))
                                {
                                    var exist = db.Rates.FirstOrDefault(x => (x.SourceCurrency == rate.SourceCurrency &&
                                                            x.DateL.Value.Day == rate.DateL.Value.Day &&
                                     x.DateL.Value.Month == rate.DateL.Value.Month && x.DateL.Value.Year == rate.DateL.Value.Year));

                                    if (exist == null)
                                    {
                                        if (rate.SourceCurrency == "RUB")
                                        {
                                            rate.RateExchange = rate.RateExchange / 100;
                                            rate.SourceCurrency = "RUR";
                                        }
                                        if (rate.SourceCurrency == "TRY")
                                        {
                                            rate.SourceCurrency = "TRL";
                                        }
                                        db.Rates.Add(rate);
                                    }

                                }
                            }


                        }

                    }

                }

                db.SaveChanges();
            }

        }


    }
    public class DBConfig
    {
        public List<string> Databases { get; set; }
        public List<string> Tables { get; set; }
        public List<string> Currencies { get; set; }
    }

    public class MySecondClass
    {
        public string SettingOne { get; set; }
        public int SettingTwo { get; set; }
    }
}
