using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Logging;

namespace MedicineSheet
{
    public class GoogleSheetManager : IGoogleSheetManager
    {
        private readonly ILogger<GoogleSheetManager> _log;
        public SheetsService _sheetsService { get; private set; }
        public string _spreadsheetId { get; private set;}
        private readonly string SheetName = "MedicineRegister";
        private string ReadRange = "!A2:F";

        public GoogleSheetManager(ILogger<GoogleSheetManager> log) 
        {
            _log = log; 
        } 
        
        public void ConfigureGoogleSheetManager(GoogleCredential googleCredential, string applicationName, string spreadsheetId) 
        {
            _spreadsheetId = spreadsheetId;

            _sheetsService = new SheetsService(new BaseClientService.Initializer() {
                HttpClientInitializer = googleCredential,
                ApplicationName = applicationName
            }); 
        }
         
        public IList<IList<object>> GetMultipleValues() 
        {
            try {

                _log.LogInformation("Getting values from google spreadsheets");

                string range = $"{SheetName}{ReadRange}";

                IList<IList<object>> values = _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, range).Execute().Values;
                
                if(values != null && values.Count > 0) 
                {
                    _log.LogInformation("Medicines gotten successfully");
                    return values;
                }
                
            }
            catch(Exception ex) 
            {
                _log.LogError(ex, ex.Message);
                Console.WriteLine(ex.Message);
            }     

            return Array.Empty<IList<object>>();
        }

        public void UpdateMedicineStatus(IEnumerable<ExpiredMedicine> expiredMedicines) 
        {
            _log.LogInformation("Starting to update the spreadsheet with the medicine expired");

            try {

                foreach(ExpiredMedicine expiredMedicine in expiredMedicines) {

                    string updateRange = $"{SheetName}!E{expiredMedicine.RowNumber}";
                    
                    ValueRange valueRange = new ValueRange();

                    List<Object> notificationValue = new List<Object>(){ "Yes" };
                    valueRange.Values = new List<IList<Object>> { notificationValue };

                    var appedRequest = _sheetsService.Spreadsheets.Values.Append(valueRange, _spreadsheetId, updateRange);
                    appedRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

                    var appedResponse = appedRequest.Execute();
                }

            }
            catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
    }
}