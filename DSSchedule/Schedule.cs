using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238
/// <summary>
/// OAuth 클라이언트 ID: 423747628775-qde61v5p0qh539vtjl7pfm92lfcunogf.apps.googleusercontent.com
/// 클라이언트 보안 Lm9jECwLQBk_lFk0aO3z22Ha
/// </summary>
namespace DSSchedule
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public class Schedule
    {
        //string clientid = "423747628775-qde61v5p0qh539vtjl7pfm92lfcunogf.apps.googleusercontent.com";
        //string clientsec = "Lm9jECwLQBk_lFk0aO3z22Ha";
        //GoogleCalendarService googleCalendar = null;

        //static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
        //static string ApplicationName = "Google Calendar API .NET Quickstart";

        public async Task<IReadOnlyList<Appointment>> GetMSSchedule()
        {
            AppointmentStore appointmentStore = await AppointmentManager.RequestStoreAsync(AppointmentStoreAccessType.AllCalendarsReadOnly);

            var dateToShow = DateTime.Now.AddDays(0);
            var duration = TimeSpan.FromHours(100);
            //int count = 0;
            IReadOnlyList<Appointment> appCalendars = await appointmentStore.FindAppointmentsAsync(dateToShow, duration);
            return appCalendars;
            //if (appCalendars.Count >= 1)
            //{
            //    foreach (var calendar in appCalendars)
            //    {
            //        string content = calendar.Subject;
            //        DateTime time = calendar.StartTime.DateTime;
            //        string contentLocation = calendar.Location;
            //        if (DateTime.Today.Day - time.Day <= 1)
            //        {
            //            if (count == 0)
            //            {
            //                //comingEvent.Text = "일정 : " + content;
            //                //eventTime.Text = time.ToString("yy년 MM월 dd일 tt HH시 mm분");
            //                //eventLocation.Text = "장소 : " + contentLocation;
            //            }
            //            else if (count == 1)
            //            {
            //                //nextcomingEvent.Text = "일정 : " + content;
            //                //nexteventTime.Text = time.ToString("yy년 MM월 dd일 tt HH시 mm분");
            //                //nexteventLocation.Text = "장소 : " + contentLocation;
            //            }
            //            else { break; }
            //            count++;
            //        }
            //    }
            //}
            //else
            //{
            //    //comingEvent.Text = "다가오는 일정이 없습니다.";
            //}

        }
    }
}