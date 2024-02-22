using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace MedicineSheet;

class Program
{
    //static readonly string clientId = "225976771643-1ehgl5rsvai35qu80rf8fif0v80hvjj3.apps.googleusercontent.com";
    //static readonly string userSecret = "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";
    static readonly string ApplicationName = "MedicineSheet";
    static readonly string SpreadsheetId = "1LyX9XXCI_raCMoFtp-T0-6-VmpMdTmo5Sj8PY5hmcHw";
    static IGoogleSheetManager? _googleSheetManager;

    static void Main(string[] args)
    {
        GoogleCredential credentials = GoogleAuthenticationManager.Login();

        _googleSheetManager = new GoogleSheetManager(credentials,ApplicationName, SpreadsheetId);

        IList<IList<Object>> rows = _googleSheetManager.GetMultipleValues();

        List<ExpiredMedicine> expiredMedicine = RowsManager.GetExpiredMedicine(rows);

        if(expiredMedicine.Any()){
            bool wasEmailSuccess = EmailSender.SendExpiredMedicineEmail(expiredMedicine);
            
            if(wasEmailSuccess)
                _googleSheetManager.UpdateMedicineStatus(expiredMedicine);

        }
        
    }
}
