using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LifeCounter.Scryfall
{
    class Scryfall
    {
        static Scryfall()
        {
            Directory.CreateDirectory("Images");
        }

        public async static Task<string> GetCardImageAsync(string card)
        {
            var cardData = await GetCardDataAsync(card);
            if (cardData == null)
                return null;
            var filename = Path.Combine("Images", cardData.name + ".png");
            if (!File.Exists(filename))
            {
                using (var client = new HttpClient())
                {
                    using (var response = await client.GetAsync(cardData.image_uris.png))
                    {
                        using (var filestream = File.OpenWrite(filename))
                        {
                            var bytes = await response.Content.ReadAsByteArrayAsync();
                            await filestream.WriteAsync(bytes, 0, bytes.Length);
                        }
                    }
                } 
            }
            return filename;
        }

        private static async Task<Card> GetCardDataAsync(string card)
        {
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync($"https://api.scryfall.com/cards/named?fuzzy={card}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var cardData = JsonConvert.DeserializeObject<Card>(result);
                        return cardData;
                    }
                    return null;

                }
            }
        }
    }
}
