using LibraryData.Models;
using System;
using System.Collections.Generic;

namespace LibraryServices
{
    class DataHelpers
    {
        public static IEnumerable<string>  HumanizeBusinessHours(IEnumerable<BranchHours> branchours)
        {
            var hours = new List<string>();
            
            foreach (var time in branchours)
            {
                var day = Humanizeday(time.DayOfWeek);
                var openTime = HumanizeTime(time.OpenTime);
                var closeTime = HumanizeTime(time.CloseTime);

                var timeEntry = $"{day} {openTime} to {closeTime}";
                hours.Add(timeEntry);
            }

            return hours;
        }

        public static object Humanizeday(int openTime)
        {
            return Enum.GetName(typeof(DayOfWeek),openTime-1);
        }

        public static string HumanizeTime(int dayOfWeek)
        {
            return TimeSpan.FromHours(dayOfWeek).ToString("hh':'mm");
        }

    }
}
