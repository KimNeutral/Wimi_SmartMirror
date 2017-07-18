using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;

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

        public async Task<IReadOnlyList<Appointment>> GetMSSchedule()
        {
            AppointmentStore appointmentStore = await AppointmentManager.RequestStoreAsync(AppointmentStoreAccessType.AllCalendarsReadOnly);

            var dateToShow = DateTime.Now.AddDays(0);
            var duration = TimeSpan.FromHours(100);

            IReadOnlyList<Appointment> appCalendars = await appointmentStore.FindAppointmentsAsync(dateToShow, duration);
            return appCalendars;
        }
    }
}