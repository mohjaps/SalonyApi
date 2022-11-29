using Core.TableDb;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Salony.Models.ControllerDTO;
using Salony.PathUrl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salony.Controllers.API
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiExplorerSettings(GroupName = "MobileApi")]
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ReservationsController(
              ApplicationDbContext context
          )
        {
            _context = context;
        }

        [HttpPost(ApiRoutes.Appointment.AddAppoiment)]
        public async Task<IActionResult> AddAppoiment([FromBody] AppoimentDto model)
        {
            try
            {
                bool isExsit = _context.Appointments
                                    .Where(z => z.SallonID == model.SallonId
                                    && z.Date.Hour == model.Date.Hour)
                                    .Any();
                if (isExsit)
                {
                    return Ok(new
                    {
                        status = false,
                        msg = "The appointment is already booked"
                    });
                }
                ProviderAditionalData providerData = await _context.ProviderAditionalData
                                                    .Where(z => z.FK_UserID == model.UserId)
                                                    .FirstOrDefaultAsync();
                if ((model.Date.Hour >= providerData.timeForm.Hour && model.Date.Hour <= providerData.timeTo.Hour) || (model.Date.Hour >= providerData.timeFormEvening.Hour && model.Date.Hour <= providerData.timeToEvening.Hour))
                {
                    bool IsVacation = _context.Vacations
                                              .Where(z => z.UserID == model.UserId
                                              && z.FromDate <= model.Date && z.ToDate >= model.Date
                                              ).Any();
                    if (IsVacation)
                    {
                        return Ok(new
                        {
                            status = false,
                            msg = "It is not possible to request an appointment on this date because it is a holiday"
                        });
                    }
                    Appointment newAppoiment = new Appointment
                    {
                        Date = model.Date,
                        SallonID = model.SallonId,
                        UserID = model.UserId
                    };
                    await _context.Appointments.AddAsync(newAppoiment);
                    await _context.SaveChangesAsync();
                    return Ok(new
                    {
                        status = true,
                        msg = "Appointment Added Successfully"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = false,
                        msg = "The appointment must be within the official working hours"
                    });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    msg = "There Is Somthing Error with Exception : " + ex.Message
                });
            }
        }

        //[HttpPost(ApiRoutes.Appointment.AllAppointmentsBooked)]
        //public async Task<IActionResult> AllAppointmentsBooked([FromBody] GetAppointmentDto model)
        //{
        //    if (string.IsNullOrEmpty(model.userId))
        //    {
        //        return BadRequest(new
        //        {
        //            status = false,
        //            msg = "user Id is Required"
        //        });
        //    }
        //    var appointments = await _context.Appointments
        //        .Where(z => z.SallonID == model.userId && z.Date.Month == model.month)
        //        .ToListAsync();

        //    return Ok(new
        //    {
        //        data = appointments,
        //        status = true,
        //        msg = ""
        //    });
        //}

        //[HttpPost(ApiRoutes.Appointment.AllAppointmentsAvailable)]
        //public async Task<IActionResult> AllAppointmentsAvailable([FromBody] GetAppointmentDto model)
        //{
        //    if (string.IsNullOrEmpty(model.userId))
        //    {
        //        return BadRequest(new
        //        {
        //            status = false,
        //            msg = "user Id is Required"
        //        });
        //    }
        //    var days = await _context.Appointments
        //       .Where(z => z.SallonID == model.userId && z.Date.Month == model.month)
        //       .Select(x => x.Date.Day)
        //       .ToListAsync();
        //    var dates = new List<DateTime>();

        //    // Loop from the first day of the month until we hit the next month, moving forward a day at a time
        //    for (var date = new DateTime(DateTime.Now.Year, model.month, 1); date.Month == model.month; date = date.AddDays(1))
        //    {
        //        if(!days.Contains(date.Day))
        //            dates.Add(date);
        //    }

        //    return Ok(new
        //    {
        //        data = dates,
        //        msg = "",
        //        status = true
        //    });
        //}

        [HttpPost(ApiRoutes.Appointment.AllAppointmentsAvailableDuringDay)]
        public async Task<IActionResult> AllAppointmentsAvailableDuringDay([FromBody] GetAppointmentDto model)
        {
            if (string.IsNullOrEmpty(model.userId))
            {
                return BadRequest(new
                {
                    status = false,
                    msg = "user Id is Required"
                });
            }
            List<int> hours = await _context.Appointments
               .Where(z => z.SallonID == model.userId && z.Date.Month == model.month && z.Date.Day == model.day)
               .Select(x => x.Date.Hour)
               .ToListAsync();
            ProviderAditionalData providerData = await _context.ProviderAditionalData.Where(z => z.FK_UserID == model.userId).FirstOrDefaultAsync();
            List<int> timesMorning = new List<int>();
            List<int> timesEvening = new List<int>();

            for (DateTime date = new DateTime(DateTime.Now.Year, model.month, model.day, providerData.timeForm.Hour, 0, 0); date.Hour <= providerData.timeTo.Hour; date = date.AddHours(1))
            {
                if (!hours.Contains(date.Hour))
                    timesMorning.Add(date.Hour);
            }

            for (DateTime date = new DateTime(DateTime.Now.Year, model.month, model.day, providerData.timeFormEvening.Hour, 0, 0); date.Hour <= providerData.timeToEvening.Hour; date = date.AddHours(1))
            {
                if (!hours.Contains(date.Hour))
                    timesEvening.Add(date.Hour);
            }
            return Ok(new
            {
                morning = timesMorning,
                evening = timesEvening,
                msg = "",
                status = true
            });
        }
        [HttpPost(ApiRoutes.Appointment.AddVacation)]
        public async Task<IActionResult> AddVacation([FromBody] VacationDto model)
        {
            bool anyConflictAppointment = _context.Appointments
                .Where(x => x.SallonID == model.userId && x.Date >= model.FromDate && x.Date <= model.ToDate)
                .Any();
            if (anyConflictAppointment)
            {
                return Ok(new
                {
                    status = false,
                    msg = "يوجد مواعيد مسبقة تتعارض مع هذا التاريخ"
                });
            }
            Vacation newVacation = new Vacation
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                UserID = model.userId
            };
            await _context.Vacations.AddAsync(newVacation);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                status = true,
                msg = ""
            });
        }
    }
}
