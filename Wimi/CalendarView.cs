using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using DSSchedule;
using Windows.ApplicationModel.Appointments;

namespace Wimi
{
    public partial class MainPage : Page
    {
        Schedule schedule = new Schedule();
        List<ScheduleInfo> lScheduleInfo = new List<ScheduleInfo>();

        async void Getschedule()
        {
            IReadOnlyList<Appointment> schedules = await schedule.GetMSSchedule();
            foreach(var appointment in schedules)
            {
                ScheduleInfo info = new ScheduleInfo();
                info.subject = appointment.Subject;
                //info.dt = appointment.StartTime.DateTime;
                //string str = string.Format("{0}년 {1}월 {2}일 {3}", 
                //    appointment.StartTime.DateTime.Year,
                //    appointment.StartTime.DateTime.Month,
                //    appointment.StartTime.DateTime.Day,
                //    appointment.StartTime.DateTime.ti());
                string now = DateTime.Now.ToString("yyyyMMdd");
                string calstr = appointment.StartTime.DateTime.ToString("yyyyMMdd");
                if (now.Equals(calstr))
                {
                    string str = appointment.StartTime.DateTime.ToString("htt");
                    info.dt = str;
                    info.location = appointment.Location;
                    info.details = appointment.Details;
                    lScheduleInfo.Add(info);
                }

            }

            lbScheduleInfo.ItemsSource = lScheduleInfo;
        }
    }

    public class ScheduleInfo
    {
        public string subject { get; set; }
        public /*DateTime*/string dt { get; set; }
        public string location { get; set; }
        public string details { get; set; }
    }
}
