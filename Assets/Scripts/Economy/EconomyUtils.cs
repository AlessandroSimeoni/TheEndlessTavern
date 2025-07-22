namespace Economy
{
    public static class EconomyUtils
    {
        /// <summary>
        /// Enable/Disable the currency based buttons in the currencyButton input array depending on the currencies owned by the player
        /// </summary>
        /// <param name="currencyButton">the input currency button array</param>
        /// <param name="playerCoins">coins owned by the player</param>
        /// <param name="playerTickets">tickets owned by the player</param>
        /// <param name="playerGems">gems owned by the player</param>
        public static void Toggle(this CurrencyBasedButton[] currencyButton, int playerCoins, int playerTickets, int playerGems)
        {
            foreach (CurrencyBasedButton b in currencyButton)
            {
                switch (b.priceCurrency)
                {
                    case Currency.Coin:
                        if (b.price > playerCoins)
                            b.button.interactable = false;
                        else
                            b.button.interactable = true;
                        break;
                    case Currency.Ticket:
                        if (b.price > playerTickets)
                            b.button.interactable = false;
                        else
                            b.button.interactable = true;
                        break;
                    case Currency.Gem:
                        if (b.price > playerGems)
                            b.button.interactable = false;
                        else
                            b.button.interactable = true;
                        break;
                    default:
                        b.button.interactable = true;
                        break;
                }
            }
        }
    }
}
