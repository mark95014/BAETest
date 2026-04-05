namespace BAETest.pages
{
    using BAETest.utils.PageData;
    using OpenQA.Selenium;
    using System.Threading;

        internal static class ClientPage
        {
            internal static class Selectors
            {
                readonly internal static string spinner = "#edge > div > div > div.loader-overlay > div > div";
                //                                       #edge > div > div > div.loader-overlay > div > div > div.mds-loader__item_trx.mds-loader__item--8_trx
                readonly internal static string breadCrumbs = "#bread-crumb-title";
                readonly internal static string reviewAvailable = "#vue-route > div > div.container.main-container.padding-none > div > div:nth-child(1) > form > div:nth-child(1) > table > tbody > tr:nth-child(4) > td:nth-child(1)";
                readonly internal static string reviewButton = "#view-review";
                readonly internal static string viewErrorsButton = "#view-errors";
                readonly internal static string viewPositionsButton = "#view-positions";
                readonly internal static string viewOverridesButton = "#view-overrides";
                readonly internal static string viewAccountsButton = "#view-accounts";
                readonly internal static string manageSettingsButton = "#household-detail-manage-settings-btn";
                readonly internal static string tradesPulldown = "#trade-select";
                readonly internal static string tradeButton = "#trade-btn";
                readonly internal static string rebalanceButton = "#view-oob";
                readonly internal static string tlhRebalanceButton = "#view-tlh";
                readonly internal static string cashRebalanceButton = "#view-model-cash";
                readonly internal static string carryForwardLossLongTerm = "#caploss-lterm";
            }

            internal static void GoTo(string client)
            {
                WaitForPageToLoad();
            }

            internal static void WaitForPageToLoad()
            {
                SeleniumHelpers.WaitForElementToDisappear(Selectors.spinner);
                SeleniumHelpers.WaitForElementToContain(Selectors.breadCrumbs, "Client Details >");
                SeleniumHelpers.FindElement(Selectors.carryForwardLossLongTerm);
            }

            internal static void VerifyPage()
            {
                CommonVerifyPage.Verify(new ClientPageData());
            }
        }
    }
}
