using Microsoft.EntityFrameworkCore;
using P01_HospitalDatabase.Data;

namespace StartUp
{
    class StartUp
    {
        static void Main()
        {
            var dbHospital = new HospitalContext();
        }
    }
}
