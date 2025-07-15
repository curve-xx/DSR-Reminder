namespace EAWorkerService;

public static class IPAddressClient
{
    public static async Task<string> GetPublicIPAsync()
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                return await client.GetStringAsync("https://api.ipify.org");
            }
            catch
            {
                return "Unable to determine public IP.";
            }
        }
    }
}
