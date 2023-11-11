using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Skymey_main_lib.Models.Currencies.Tinkoff;
using Skymey_stock_tinkoff_currencies.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;

namespace Skymey_stock_tinkoff_currencies.Actions.GetCurrencies
{
    public class GetCurrencies
    {
        private MongoClient _mongoClient;
        private ApplicationContext _db;
        private string _apiKey;
        public GetCurrencies()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();

            _apiKey = config.GetSection("ApiKeys:Tinkoff").Value;
            _mongoClient = new MongoClient("mongodb://127.0.0.1:27017");
            _db = ApplicationContext.Create(_mongoClient.GetDatabase("skymey"));
        }
        public void GetCurrenciesFromTinkoff()
        {
            var client = InvestApiClientFactory.Create(_apiKey);
            InstrumentsRequest request = new InstrumentsRequest();
            var response = client.Instruments.Currencies(request);
            var ticker_finds = (from i in _db.Currencies select i);
            foreach (var item in response.Instruments)
            {
                Console.WriteLine(item.Ticker);
                var ticker_find = (from i in ticker_finds where i.ticker == item.Ticker && i.figi == item.Figi select i).FirstOrDefault();
                if (ticker_find == null)
                {
                    TinkoffCurrenciesInstrument tci = new TinkoffCurrenciesInstrument();
                    tci._id = ObjectId.GenerateNewId();
                    tci.figi = item.Figi;
                    if (ticker_find != null) tci.figi = "";
                    tci.ticker = item.Ticker;
                    if (tci.ticker == null) tci.ticker = "";
                    tci.classCode = item.ClassCode;
                    if (tci.classCode == null) tci.classCode = "";
                    tci.isin = item.Isin;
                    if (tci.isin == null) tci.isin = "";
                    tci.lot = item.Lot;
                    if (tci.lot == null) tci.lot = 0;
                    tci.currency = item.Currency_;
                    if (tci.currency == null) tci.currency = "";
                    tci.shortEnabledFlag = item.ShortEnabledFlag;
                    if (tci.shortEnabledFlag == null) tci.shortEnabledFlag = false;
                    tci.name = item.Name;
                    if (tci.name == null) tci.name = "";
                    tci.exchange = item.Exchange;
                    if (tci.exchange == null) tci.exchange = "";
                    if (item.Nominal != null)
                    {
                        TinkoffCurrenciesNominal tcn = new TinkoffCurrenciesNominal();
                        tcn.currency = item.Nominal.Currency;
                        tcn.units = item.Nominal.Units;
                        tcn.nano = item.Nominal.Nano;
                    }
                    else
                    {
                        tci.nominal = new TinkoffCurrenciesNominal();
                    }
                    tci.countryOfRisk = item.CountryOfRisk;
                    if (tci.countryOfRisk == null) tci.countryOfRisk = "";
                    tci.countryOfRiskName = item.CountryOfRiskName;
                    if (tci.countryOfRiskName == null) tci.countryOfRiskName = "";
                    tci.tradingStatus = item.TradingStatus.ToString();
                    if (tci.tradingStatus == null) tci.tradingStatus = "";
                    tci.otcFlag = item.OtcFlag;
                    if (tci.otcFlag == null) tci.otcFlag = false;
                    tci.buyAvailableFlag = item.BuyAvailableFlag;
                    if (tci.buyAvailableFlag == null) tci.buyAvailableFlag = false;
                    tci.sellAvailableFlag = item.SellAvailableFlag;
                    if (tci.sellAvailableFlag == null) tci.sellAvailableFlag = false;
                    tci.isoCurrencyName = "";
                    if (tci.isoCurrencyName == null) tci.isoCurrencyName = "";
                    if (item.MinPriceIncrement != null)
                    {
                        TinkoffCurrenciesMinPriceIncrement tcmpi = new TinkoffCurrenciesMinPriceIncrement();
                        tcmpi.units = item.MinPriceIncrement.Units;
                        tcmpi.nano = item.MinPriceIncrement.Nano;
                    }
                    else
                    {
                        tci.minPriceIncrement = new TinkoffCurrenciesMinPriceIncrement();
                    }
                    tci.apiTradeAvailableFlag = item.ApiTradeAvailableFlag;
                    if (tci.apiTradeAvailableFlag == null) tci.apiTradeAvailableFlag= false;
                    tci.uid = item.Uid;
                    if (tci.uid == null) tci.uid = "";
                    tci.realExchange = item.RealExchange.ToString();
                    if (tci.realExchange == null) tci.realExchange = "";
                    tci.positionUid = item.PositionUid;
                    if (tci.positionUid == null) tci.positionUid = "";
                    tci.forIisFlag = item.ForIisFlag;
                    if (tci.forIisFlag == null) tci.forIisFlag = false;
                    tci.forQualInvestorFlag = item.ForQualInvestorFlag;
                    if (tci.forQualInvestorFlag == null) tci.forQualInvestorFlag= false;
                    tci.weekendFlag = item.WeekendFlag;
                    if (tci.weekendFlag == null) tci.weekendFlag = false;
                    tci.blockedTcaFlag = item.BlockedTcaFlag;
                    if (tci.blockedTcaFlag == null) tci.blockedTcaFlag = false;
                    tci.first1minCandleDate = item.First1MinCandleDate;
                    if (tci.first1minCandleDate == null) tci.first1minCandleDate = Timestamp.FromDateTime(DateTime.UtcNow);
                    tci.first1dayCandleDate = item.First1DayCandleDate;
                    if (tci.first1dayCandleDate == null) tci.first1dayCandleDate = Timestamp.FromDateTime(DateTime.UtcNow);
                    if (item.Klong != null)
                    {
                        TinkoffCurrenciesKlong tckl = new TinkoffCurrenciesKlong();
                        tckl.units = item.Klong.Nano;
                        tckl.units = item.Klong.Nano;
                        tci.klong = tckl;
                    }
                    else
                    {
                        tci.klong = new TinkoffCurrenciesKlong();
                    }
                    if (item.Kshort != null)
                    {
                        TinkoffCurrenciesKshort tcks = new TinkoffCurrenciesKshort();
                        tcks.units = item.Kshort.Units;
                        tcks.nano = item.Kshort.Nano;
                        tci.kshort = tcks;
                    }
                    else
                    {
                        tci.kshort = new TinkoffCurrenciesKshort();
                    }
                    if (item.Dlong != null)
                    {
                        TinkoffCurrenciesDlong tcdl = new TinkoffCurrenciesDlong();
                        tcdl.units = item.Dlong.Units;
                        tcdl.nano = item.Dlong.Nano;
                        tci.dlong = tcdl;
                    }
                    else
                    {
                        tci.dlong = new TinkoffCurrenciesDlong();
                    }
                    if (item.Dshort != null)
                    {
                        TinkoffCurrenciesDshort tcds = new TinkoffCurrenciesDshort();
                        tcds.units = item.Dshort.Units;
                        tcds.nano = item.Dshort.Nano;
                        tci.dshort = tcds;
                    }
                    else
                    {
                        tci.dshort = new TinkoffCurrenciesDshort();
                    }
                    if (item.DlongMin != null)
                    {
                        TinkoffCurrenciesDlongMin tcdlm = new TinkoffCurrenciesDlongMin();
                        tcdlm.units = item.DlongMin.Units;
                        tcdlm.nano = item.DlongMin.Nano;
                    }
                    else
                    {
                        tci.dlongMin = new TinkoffCurrenciesDlongMin();
                    }
                    if (item.DshortMin != null)
                    {
                        TinkoffCurrenciesDshortMin tcdsm = new TinkoffCurrenciesDshortMin();
                        tcdsm.units = item.DshortMin.Units;
                        tcdsm.nano = item.DshortMin.Nano;

                    }
                    else
                    {
                        tci.dshortMin = new TinkoffCurrenciesDshortMin();
                    }
                    tci.Update = DateTime.UtcNow;
                    _db.Currencies.Add(tci);
                }
            }
            _db.SaveChanges();
        }
    }
}
