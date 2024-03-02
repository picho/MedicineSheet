using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Serilog;

namespace MedicineSheet
{
    public static class GoogleAuthenticationManager
    {
        private static readonly ILogger _log = Log.ForContext(typeof(GoogleAuthenticationManager));
        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets};

        public static GoogleCredential Login() 
        {
            GoogleCredential googleCredential = null;

            _log.Information("Starting the login process with Google.");

            try {
                
                using var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read);
            
                googleCredential = GoogleCredential
                    .FromStream(stream)
                    .CreateScoped(Scopes);
            
            }
            catch(Exception ex) {
                _log.Error(ex, ex.Message);
            }

            _log.Information("Login succes.");

            return googleCredential; 
        }
    }
}