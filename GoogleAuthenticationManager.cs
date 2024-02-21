using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;

namespace MedicineSheet
{
    public static class GoogleAuthenticationManager
    {
        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        public static GoogleCredential Login() 
        {
            using(var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read)) 
            {
                return  GoogleCredential
                    .FromStream(stream)
                    .CreateScoped(Scopes);
            }
        }
    }
}