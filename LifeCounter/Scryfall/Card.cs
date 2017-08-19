using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCounter.Scryfall
{
    public class RootObject
    {
        public List<Card> data { get; set; }
    }

    public class Card
    {
        public string id { get; set; }
        public string image_uri { get; set; }
        public string name { get; set; }
        public ImageUris image_uris { get; set; }
    }

    public class ImageUris
    {
        public string small { get; set; }
        public string normal { get; set; }
        public string large { get; set; }
        public string png { get; set; }
    }
}
