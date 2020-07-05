using P01_StudentSystem.Data;
using P03_FootballBetting.Data;

namespace P01_StudentSystem
{
    class StartUp
    {
        static void Main()
        {
            var dbStudentSystem = new StudentSystemContext();

            var dbFootballBetting = new FootballBettingContext();

            dbStudentSystem.Database.EnsureCreated();

            dbFootballBetting.Database.EnsureCreated();
        }
    }
}
