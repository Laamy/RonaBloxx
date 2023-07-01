using System.Collections.Generic;
using System;

public class Datum
{
    public long id { get; set; }
    public long rootPlaceId { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public string sourceName { get; set; }
    public string sourceDescription { get; set; }
    public Creator creator { get; set; }
    public object price { get; set; }
    public List<string> allowedGearGenres { get; set; }
    public List<object> allowedGearCategories { get; set; }
    public bool isGenreEnforced { get; set; }
    public bool copyingAllowed { get; set; }
    public int playing { get; set; }
    public int visits { get; set; }
    public int maxPlayers { get; set; }
    public DateTime created { get; set; }
    public DateTime updated { get; set; }
    public bool studioAccessToApisAllowed { get; set; }
    public bool createVipServersAllowed { get; set; }
    public string universeAvatarType { get; set; }
    public string genre { get; set; }
    public bool isAllGenre { get; set; }
    public bool isFavoritedByUser { get; set; }
    public int favoritedCount { get; set; }
}