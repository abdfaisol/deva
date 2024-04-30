namespace deva.libs.prasi
{
    public class IConfigDeva
    {
        public IConfigPrasi Prasi { get; set; }
        public IConfigDb Db { get; set; }
    }
    public class IConfigPrasi : IConfigDeva
    {
        public string Id { get; set; }
    }
    public class IConfigDb : IConfigDeva
    {
        public string Connection { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string ServiceName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
