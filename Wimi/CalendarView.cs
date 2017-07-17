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
                info.dt = appointment.StartTime.DateTime;
                info.location = appointment.Location;
                info.details = appointment.Details;
                lScheduleInfo.Add(info);
            }

            lbScheduleInfo.ItemsSource = lScheduleInfo;
        }
    }

    public class ScheduleInfo
    {
        public string subject { get; set; }
        public DateTime dt { get; set; }
        public string location { get; set; }
        public string details { get; set; }
    }
}
