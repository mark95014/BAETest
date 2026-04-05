using BAETest.src.utils.PageData.Elements;

namespace BAETest.utils.PageData
{
    internal class ClientPageData
    {
        internal TextElement breadCrumbs = new TextElement("#bread-crumb-title");
        internal TextElement clientId = new TextElement("#household-number");
        internal TextElement model = new TextElement("#model-desc");
        internal TextElement notes = new TextElement("#notes");
        internal ElementAttribute OOB = new ElementAttribute("#vue-route > div > div.container.main-container > div > div:nth-child(1) > form > div:nth-child(1) > table > tbody > tr:nth-child(1) > td:nth-child(1) > div > span", "className");
        internal ElementAttribute cashNeeds = new ElementAttribute("#vue-route > div > div.container.main-container > div > div:nth-child(1) > form > div:nth-child(1) > table > tbody > tr:nth-child(2) > td:nth-child(1) > div > span", "className");
        internal ElementAttribute tlhOpportunities = new ElementAttribute("#vue-route > div > div.container.main-container > div > div:nth-child(1) > form > div:nth-child(1) > table > tbody > tr:nth-child(3) > td:nth-child(1) > div > span", "className");
        internal TextElement value = new TextElement("#vue-route > div > div.container.main-container.padding-none > div > div:nth-child(1) > form > div:nth-child(2) > table > tbody > tr:nth-child(1) > td.mds-td_trx.mds-td--right_trx > div");
        internal TextElement classOOBPercentage = new TextElement("#vue-route > div > div.container.main-container.padding-none > div > div:nth-child(1) > form > div:nth-child(2) > table > tbody > tr:nth-child(2) > td.mds-td_trx.mds-td--right_trx > div > span.negative-ticker");
        internal TextElement classOOBDollars = new TextElement("#vue-route > div > div.container.main-container.padding-none > div > div:nth-child(1) > form > div:nth-child(2) > table > tbody > tr:nth-child(3) > td.mds-td_trx.mds-td--right_trx > div > span.negative-ticker");
        internal TextElement subClassOOBPercentage = new TextElement("#vue-route > div > div.container.main-container.padding-none > div > div:nth-child(1) > form > div:nth-child(2) > table > tbody > tr:nth-child(4) > td.mds-td_trx.mds-td--right_trx > div > span.negative-ticker");
        internal TextElement subClassOOBDollars = new TextElement("#vue-route > div > div.container.main-container.padding-none > div > div:nth-child(1) > form > div:nth-child(2) > table > tbody > tr:nth-child(5) > td.mds-td_trx.mds-td--right_trx > div > span.negative-ticker");
        internal TextElement cashNeeded = new TextElement("#vue-route > div > div.container.main-container.padding-none > div > div:nth-child(1) > form > div:nth-child(2) > table > tbody > tr:nth-child(6) > td.mds-td_trx.mds-td--right_trx > div > span");
        internal TextElement cashToInvest = new TextElement("#vue-route > div > div.container.main-container.padding-none > div > div:nth-child(1) > form > div:nth-child(2) > table > tbody > tr:nth-child(7) > td.mds-td_trx.mds-td--right_trx > div > span.negative-ticker");
        internal TextElement TRXCriticalErrors = new TextElement("#trx-errors");
        internal TextElement PASCriticalErrors = new TextElement("#pas-errors");
        internal TextElement valueManaged = new TextElement("#vue-route > div > div.container.main-container.padding-none > div > div:nth-child(2) > div:nth-child(3) > table > tbody > tr:nth-child(1) > td.mds-td_trx.mds-td--right_trx > div");
        internal TextElement valueExcluded = new TextElement("#vue-route > div > div.container.main-container.padding-none > div > div:nth-child(2) > div:nth-child(3) > table > tbody > tr:nth-child(2) > td.mds-td_trx.mds-td--right_trx > div");
        internal TextElement cashManaged = new TextElement("#vue-route > div > div.container.main-container.padding-none > div > div:nth-child(2) > div:nth-child(4) > table > tbody > tr:nth-child(1) > td.mds-td_trx.mds-td--right_trx > div");
        internal TextElement cashExcluded = new TextElement("#vue-route > div > div.container.main-container.padding-none > div > div:nth-child(2) > div:nth-child(4) > table > tbody > tr:nth-child(2) > td.mds-td_trx.mds-td--right_trx > div");
        internal TextElement taxableGainsShortTerm = new TextElement("#vue-route > div > div.container.main-container.padding-none > div > div:nth-child(2) > div:nth-child(5) > table > tbody > tr:nth-child(1) > td:nth-child(2) > div");
        internal TextElement nonTaxableGainsShortTerm = new TextElement("#vue-route > div > div.container.main-container.padding-none > div > div:nth-child(2) > div:nth-child(5) > table > tbody > tr:nth-child(1) > td.mds-td_trx.mds-td--right_trx > div");
        internal TextElement taxableGainsLongTerm = new TextElement("#vue-route > div > div.container.main-container.padding-none > div > div:nth-child(2) > div:nth-child(5) > table > tbody > tr:nth-child(2) > td:nth-child(2) > div");
        internal TextElement nonTaxableGainsLongTerm = new TextElement("#vue-route > div > div.container.main-container.padding-none > div > div:nth-child(2) > div:nth-child(5) > table > tbody > tr:nth-child(2) > td.mds-td_trx.mds-td--right_trx > div");
        internal TextElement lastRebalanceDate = new TextElement("#vue-route > div > div.container.main-container.padding-none > div > div:nth-child(2) > div:nth-child(6) > table > tbody > tr:nth-child(1) > td.mds-td_trx.mds-td--right_trx > div");
        internal TextElement lastTransactionDate = new TextElement("#vue-route > div > div.container.main-container.padding-none > div > div:nth-child(2) > div:nth-child(6) > table > tbody > tr:nth-child(2) > td.mds-td_trx.mds-td--right_trx > div");
        internal ValueElement maxGain = new ValueElement("#mgains-amount");
        internal ValueElement maxGainTax = new ValueElement("#mgains-tax-amount");
        internal ValueElement ordinaryTaxRate = new ValueElement("#taxrate-ordinary");
        internal ValueElement capitalGainsTaxRate = new ValueElement("#taxrate-capgain");
        internal ValueElement carryForwardLossShortTerm = new ValueElement("#caploss-sterm");
        internal ValueElement carryForwardLossLongTerm = new ValueElement("#caploss-lterm");
    }
}
