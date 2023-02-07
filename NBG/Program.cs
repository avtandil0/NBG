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

//optionsBuilder.UseSqlServer("Server=AZENAISHVILI1;database=811;Trusted_Connection=True;User ID=nbguser;Password=NewPass2;");

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
                    .AddJsonFile("appsettings.json");

            IConfiguration config = builder.Build();

            var dbConfig = config.GetSection("DBConfig").Get<DBConfig>();
            //var mySecondClass = config.GetSection("MySecondClass").Get<MySecondClass>();

            string Log_File = dbConfig.LogPath;


            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(Log_File, true))
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
                Console.WriteLine("ExceptionExceptionException.");
                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(Log_File, true))
                {
                    file.WriteLine(" ! ! !  ERROR ! ! !  " + DateTime.Now + " " + e.Message);
                }
                throw;
            }


            using (System.IO.StreamWriter file =
                    new System.IO.StreamWriter(Log_File, true))
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

            var RelationalCurrencyCode = DBConfig.TableValues.RelationalCurrencyCode;

            using (NBGContext db = new NBGContext(_connectionString))
            {
                foreach (var item in DBConfig.Tables)
                {
                    foreach (var currency in currencies)
                    {
                        if (DBConfig.Currencies.Contains(currency.code))
                        {

                            if (item.Contains("Currency"))
                            {


                                string query = "Insert Into [" + item + "] ([Currency Code], [Starting Date], [Exchange Rate Amount]," +
                                    "[Adjustment Exch_ Rate Amount], [Relational Currency Code], [Relational Exch_ Rate Amount]," +
                                    "[Fix Exchange Rate Amount]," +
                                    "[Relational Adjmt Exch Rate Amt],[$systemCreatedAt],[$systemModifiedAt]) " +
                   "VALUES (@CurrencyCode, @StartingDate, @ExchangeRateAmount, @AdjustmentExchRateAmount," +
                   " @RelationalCurrencyCode, @RelationalExchRateAmount, @FixExchangeRateAmount," +
                   "@RelationalAdjmtExchRateAmt,@SystemCreatedAt, @SystemModifiedAt) ";

                                // instance connection and command
                                using (SqlConnection cn = new SqlConnection(_connectionString))
                                {
                                    cn.Open();
                                    SqlCommand check_record = new SqlCommand("SELECT COUNT(*) FROM [" + item + "] " +
                                   "WHERE convert(date,[Starting Date],102) = convert(date,@startingDate,102)" +
                                   " and [Currency Code] = '" + currency.code + "' ", cn);
                                    check_record.Parameters.AddWithValue("@startingDate", dt);
                                    int UserExist = (int)check_record.ExecuteScalar();

                                    if (UserExist > 0)
                                    {
                                        //Username exist
                                    }
                                    else
                                    {
                                        //Username doesn't exist.
                                        using (SqlCommand cmd = new SqlCommand(query, cn))
                                        {
                                            // add parameters and their values
                                            cmd.Parameters.Add("@dbName", System.Data.SqlDbType.NVarChar, 100).Value = item;
                                            cmd.Parameters.Add("@CurrencyCode", System.Data.SqlDbType.NVarChar, 100).Value = currency.code;
                                            cmd.Parameters.Add("@StartingDate", System.Data.SqlDbType.DateTime, 100).Value = dt;
                                            cmd.Parameters.Add("@ExchangeRateAmount", System.Data.SqlDbType.NVarChar, 100).Value = "1";
                                            cmd.Parameters.Add("@AdjustmentExchRateAmount", System.Data.SqlDbType.NVarChar, 100).Value = "1";
                                            cmd.Parameters.Add("@RelationalCurrencyCode", System.Data.SqlDbType.NVarChar, 100).Value = RelationalCurrencyCode;
                                            cmd.Parameters.Add("@RelationalExchRateAmount", System.Data.SqlDbType.Decimal).Value = (decimal)currency.rate;
                                            cmd.Parameters.Add("@FixExchangeRateAmount", System.Data.SqlDbType.Int).Value = 0;
                                            cmd.Parameters.Add("@RelationalAdjmtExchRateAmt", System.Data.SqlDbType.Decimal).Value = (decimal)currency.rate;
                                            cmd.Parameters.Add("@SystemCreatedAt", System.Data.SqlDbType.DateTime).Value = dt;
                                            cmd.Parameters.Add("@SystemModifiedAt", System.Data.SqlDbType.DateTime).Value = dt;

                                            // open connection, execute command and close connection

                                            cmd.ExecuteNonQuery();

                                        }
                                    }
                                    cn.Close();


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
        public TableValues TableValues { get; set; }
        public string LogPath { get; set; }
    }

    public class TableValues
    {
        public string RelationalCurrencyCode { get; set; }
    }
    public class MySecondClass
    {
        public string SettingOne { get; set; }
        public int SettingTwo { get; set; }
    }
}
