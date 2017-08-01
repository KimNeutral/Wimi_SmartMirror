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
            IReadOnlyList<Appointment> schedules = await schedule.GetMSSchedule(500);
            foreach(var appointment in schedules)
            {
                ScheduleInfo info = new ScheduleInfo();
                info.subject = appointment.Subject;
#if false
                string now = DateTime.Now.ToString("yyyyMMdd");
                string scheduleDate = appointment.StartTime.DateTime.ToString("yyyyMMdd");
                if (now.Equals(scheduleDate)) //chris: 오늘 일정이 아니더라도 보여준다.
#endif
                {
                    string str = appointment.StartTime.DateTime.ToString("ddd, MMM dd h:mm");
                    info.dt = str;
                    info.location = appointment.Location;
                    info.details = appointment.Details;
                    lScheduleInfo.Add(info);
                }

            }

            lbScheduleInfo.ItemsSource = lScheduleInfo;
        }

        private void ShowSchedule()
        {
            lbScheduleInfo.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void HideSchedule()
        {
            lbScheduleInfo.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
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
