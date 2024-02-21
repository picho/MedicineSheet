using System.Globalization;

namespace MedicineSheet
{
    public class ExpiredMedicine
    {
        public string Name { get; set;} = "";
        public DateTime ExpirationDate { get; set; }
        public int RowNumber { get; set; }

        public ExpiredMedicine(string name, DateTime expirationDate, int rowNumber) 
        {
            Name = name;
            ExpirationDate = expirationDate;
            RowNumber = rowNumber; 
        }
    }
}