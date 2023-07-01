using System.Net;
using System.Web.Script.Serialization;

class GameClient
{
    private static WebClient wc = new WebClient();

    // https://api.roblox.com/ is no longer a site so i'll have to figure this out later..
    public static RobloxUniverse GetMainUniverse(string id) // gonna develop a server so i dont have to wait like this every time i wanna call an api
    {
        JavaScriptSerializer jss = new JavaScriptSerializer();

        RobloxPlace place = jss.Deserialize<RobloxPlace>(
            wc.DownloadString("https://api.roblox.com/universes/get-universe-containing-place?placeid=" + id));

        string json = wc.DownloadString("https://games.roblox.com/v1/games?universeIds=" + place.UniverseId);

        return jss.Deserialize<RobloxUniverse>(json);
    }
}