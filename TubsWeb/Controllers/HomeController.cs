
namespace TubsWeb.Controllers
{
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;
    using Spc.Ofp.Tubs.DAL;

    public class HomeController : SuperController
    {
        private static string CountByYear = 
        @"select 
            COUNT(datepart(yy, dep_date)) as year_count
            ,DATEPART(yy, dep_date) as trip_year 
          from 
            obsv.trip 
          group by DATEPART(yy, dep_date) 
          order by DATEPART(yy, dep_date) ASC";

        private string TripsByYear()
        {
            var tripsByYear = TubsDataService.Execute(CountByYear);

            var years =
                from object[] row in tripsByYear
                select (int)row[1];

            int minYear = years.Min();
            int maxYear = years.Max();

            StringBuilder dataBuilder = new StringBuilder(32);

            foreach (int year in Enumerable.Range(minYear, (maxYear - minYear) + 1))
            {
                int tripCount = (
                    from object[] row in tripsByYear
                    where (int)row[1] == year
                    select (int)row[0]).FirstOrDefault();

                if (dataBuilder.Length > 0)
                {
                    dataBuilder.Append(",");
                }
                dataBuilder.Append(tripCount);
            }
            
            return dataBuilder.ToString();
        }
        
        private int TripsEnteredSince(int daysBack)
        {
            string query = @"select count(*) from obsv.trip where DATEADD(dd, ?, GETDATE()) < entered_dtime";
            var results = TubsDataService.Execute(query, daysBack * -1);
            return (int)results[0];
        }

        private int TripsInLastWeekFor(string user)
        {
            // Chomp the domain off user
            int slashIndex = user.IndexOf(@"\");
            if (slashIndex > -1)
            {
                user = user.Substring(slashIndex + 1);
            }

            // Prefix with a wildcard
            user = "%" + user.ToUpper();

            string query = @"select count(*) from obsv.trip where DATEADD(dd, -7, GETDATE()) < entered_dtime AND UPPER(entered_by) like ?";
            var results = TubsDataService.Execute(query, user);
            return (int)results[0];            
        }
        
        public ActionResult Index()
        {
            ViewBag.Message = "Home Page";
            ViewBag.TripsEnteredSince = TripsEnteredSince(30);
            ViewBag.TripsInLastWeek = TripsInLastWeekFor(User.Identity.Name);
            ViewBag.TripsByYear = TripsByYear();
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        // FIXME Add some kind of security here
        public ActionResult Debug()
        {
            ViewBag.Xyzzy = "Xyzzy";
            return View();
        }

        public ActionResult Confidentiality()
        {
            return View();
        }
    }
}
