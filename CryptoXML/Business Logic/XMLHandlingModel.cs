using System;
using System.Globalization;
using System.Xml.Serialization;

[XmlRoot(ElementName = "Data")]
public class Data
{
    [XmlElement(ElementName = "id")]
    public string Id { get; set; }

    [XmlElement(ElementName = "rank")]
    public int Rank { get; set; }

    [XmlElement(ElementName = "symbol")]
    public string Symbol { get; set; }

    [XmlElement(ElementName = "name")]
    public string Name { get; set; }

    [XmlElement(ElementName = "supply")]
    public double Supply { get; set; }

    [XmlElement(ElementName = "marketCapUsd")]
    public double MarketCapUsd { get; set; }

    [XmlElement(ElementName = "volumeUsd24Hr")]
    public double VolumeUsd24Hr { get; set; }

    [XmlElement(ElementName = "priceUsd")]
    public double PriceUsd { get; set; }

    [XmlElement(ElementName = "changePercent24Hr")]
    public double ChangePercent24Hr { get; set; }

    [XmlElement(ElementName = "vwap24Hr")]
    public double Vwap24Hr { get; set; }

    [XmlElement(ElementName = "explorer")]
    public string Explorer { get; set; }

    [XmlElement(ElementName = "maxSupply")]
    public double MaxSupply { get; set; }

    [XmlElement(ElementName = "tajmStamp")]
    public string TajmStampString { get; set; }

    [XmlIgnore]
    public DateTime TajmStamp
    {
        get
        {
            DateTime dateTime;
            DateTime.TryParseExact(TajmStampString, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dateTime);
            return dateTime;
        }
    }
}

[XmlRoot(ElementName = "root")]
public class Root
{
    [XmlElement(ElementName = "Data")]
    public List<Data> Data { get; set; }
}
