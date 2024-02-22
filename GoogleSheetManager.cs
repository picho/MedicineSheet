using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace MedicineSheet
{
    public class GoogleSheetManager : IGoogleSheetManager
    {
        private readonly SheetsService _sheetsService;
        private readonly string _spreadsheetId;
        private readonly string SheetName = "MedicineRegister";
        private string ReadRange = "!A2:F";
        public GoogleSheetManager(GoogleCredential googleCredential, string applicationName, string spreadsheetId) {

            _spreadsheetId = spreadsheetId;

            _sheetsService = new SheetsService(new BaseClientService.Initializer() {
                HttpClientInitializer = googleCredential,
                ApplicationName = applicationName
            });   
        } 

        public IList<IList<Object>> GetMultipleValues() {
            
            try {

                string range = $"{SheetName}{ReadRange}";

                IList<IList<Object>> values = _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, range).Execute().Values;
                
                if(values != null && values.Count > 0) 
                    return values;
                
            }
            catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }     

            return Array.Empty<IList<Object>>();
        }

        public void UpdateMedicineStatus(IEnumerable<ExpiredMedicine> expiredMedicines)
        {
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