using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace MedicineSheet;

class Program
{
    //static readonly string clientId = "225976771643-1ehgl5rsvai35qu80rf8fif0v80hvjj3.apps.googleusercontent.com";
    //static readonly string userSecret = "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";
    //dotnet publish --runtime linux-arm64 --self-contained
    private static readonly string ApplicationName = "MedicineSheet";
    private static readonly string SpreadsheetId = "1LyX9XXCI_raCMoFtp-T0-6-VmpMdTmo5Sj8PY5hmcHw";
    private static IGoogleSheetManager _googleSheetManager;
    private static readonly ILogger _log = Log.ForContext(typeof(Program));


    static void Main(string[] args)
    {
        var host = AppStartUp();

        StartScript(host);
    }

     static IHost AppStartUp() 
     {

        var builder = new ConfigurationBuilder();
        ConfigurationSetup(builder);

        //Serilog configurations
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Build())
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context,services) => {
                services.AddTransient<IGoogleSheetManager, GoogleSheetManager>();
            })
            .UseSerilog()
            .Build();

        return host;
    }

    static void ConfigurationSetup(IConfigurationBuilder builder) 
    {
        builder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();
    }

    static void StartScript(IHost host) 
    {
        GoogleCredential credentials = GoogleAuthenticationManager.Login();

        _googleSheetManager = ActivatorUtilities.CreateInstance<GoogleSheetManager>(host.Services);
        _googleSheetManager.ConfigureGoogleSheetManager(credentials,ApplicationName, SpreadsheetId);

        IList<IList<object>> rows = _googleSheetManager.GetMultipleValues();

        List<ExpiredMedicine> expiredMedicine = RowsManager.GetExpiredMedicine(rows);

        if(expiredMedicine.Any())
        {
            _log.Information("There are expired medicines");
            bool wasEmailSuccess = EmailSender.SendExpiredMedicineEmail(expiredMedicine);
            
            if(wasEmailSuccess)
                _googleSheetManager.UpdateMedicineStatus(expiredMedicine);

            _log.Information("Spreadsheet was updated");

            Environment.Exit(0);
        }     

        _log.Information("There are not expired medicines to update");   
    }
}
