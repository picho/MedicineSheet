using System.Globalization;
using Serilog;

namespace MedicineSheet
{
    public static class RowsManager
    {
        private static Dictionary<string,string> _monthsDictionary = new Dictionary<string, string>() {

            {"ene", "01"},
            {"feb", "02"},
            {"mar", "03"},
            {"abr", "04"},
            {"may", "05"},
            {"jun", "06"},
            {"jul", "07"},
            {"ago", "08"},
            {"sept", "09"},
            {"oct", "10"},
            {"nov", "11"},
            {"dic", "12"}
        };

        private static readonly ILogger _log = Log.ForContext(typeof(RowsManager));

        public static List<ExpiredMedicine> GetExpiredMedicine(IList<IList<object>> rows) 
        {
            List<ExpiredMedicine> expiredMedicine = new List<ExpiredMedicine>();
            DateTime currentTime = DateTime.Now;

            int rowsPivot = 2;

            _log.Information("Start looping through all the rows to get the only expired medicines.");

            foreach(var item in rows) 
            {
                if(item[0] != null && item[3] != null) 
                {
                    DateTime medicineDate = MapSpanishDateToEnglishDate(item[3].ToString());

                    bool wasNotified = item[4].ToString().Equals(NotifiedValues.Yes.ToString());  
                    
                    if(currentTime > medicineDate && !wasNotified) 
                    {
                        _log.Information($"{item[0]} is expired, date is {medicineDate} in the row number {rowsPivot}.");
                        expiredMedicine.Add(new ExpiredMedicine(item[0].ToString(), medicineDate, rowsPivot));
                    }
                }

                rowsPivot++;
            }

            _log.Information("Expired medicine looping finished");

            return expiredMedicine;
        }

        /// <summary>
        /// This method was created due to the runtime in the Raspberry pi does not allow the spanish moths
        /// </summary>
        /// <param name="dateSpanishFormat"></param>
        /// <returns>The correct date corresponding to the Spanish name of the month</returns>
        private static DateTime MapSpanishDateToEnglishDate(string dateSpanishFormat) {

            string[] dateElements = dateSpanishFormat.Split('-');

            string newDate = string.Format("{0}-{1}",_monthsDictionary[dateElements[0]], dateElements[1]);

            return DateTime.ParseExact(newDate, "MM-yyyy", CultureInfo.InvariantCulture); 
        }        
    }
}