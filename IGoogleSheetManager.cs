
using Google.Apis.Auth.OAuth2;

namespace MedicineSheet
{
    public interface IGoogleSheetManager
    {
        public void ConfigureGoogleSheetManager(GoogleCredential googleCredential, string applicationName, string spreadsheetId);
        IList<IList<Object>> GetMultipleValues();
        void UpdateMedicineStatus(IEnumerable<ExpiredMedicine> expiredMedicines);
    }
}