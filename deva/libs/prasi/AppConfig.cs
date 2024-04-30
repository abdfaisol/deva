using deva.libs.page;

namespace deva.libs.prasi
{
    public class AppConfig
    {

        public IConfiguration Configuration { get; }
        public AppConfig(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public static void ConfigureServices(IServiceCollection services)
        {        // Inisialisasi konfigurasi
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            IConfigDeva globalConfig = new IConfigDeva();
            configuration.GetSection("GlobalConfig").Bind(globalConfig);

            services.AddSingleton(globalConfig);
            string connection = connectionOracle(globalConfig.Db);
            Global.prasi_id = globalConfig.Prasi.Id;
            Global.ConnectionDb = connection;

        }
        static string connectionOracle(IConfigDb config)
        {
            string result = string.Empty;
            result = $"Data Source=//{config.Host}:{config.Port}/{config.ServiceName};User Id={config.Username};Password={config.Password};";
            return result;
        }
    }

}
