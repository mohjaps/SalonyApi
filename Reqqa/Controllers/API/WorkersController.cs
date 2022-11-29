using Core.TableDb;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Salony.PathUrl;
using Salony.ViewModels.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Salony.Enums.AllEnums;
using static Salony.Helper.Helper;

namespace Salony.Controllers.API
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiExplorerSettings(GroupName = "MobileApi")]
    public class WorkersController : Controller
    {
        private readonly UserManager<ApplicationDbUser> _userManager;
        private readonly IWebHostEnvironment _HostingEnvironment;
        private readonly ApplicationDbContext _context;

        public WorkersController(
                UserManager<ApplicationDbUser> userManager,
                IWebHostEnvironment HostingEnvironment,
                ApplicationDbContext context
            )
        {
            _HostingEnvironment = HostingEnvironment;
            _context = context;
            _userManager = userManager;
        }

        [HttpGet(ApiRoutes.Client.GetWorkers)]
        [ProducesResponseType(200, Type = typeof(ApiResult<WorkerEvaOuptput>))]
        [ProducesResponseType(400, Type = typeof(ApiResult<string>))]
        public async Task<IActionResult> GetWorkers(String SallonID)
        {
            ApiResult<string> Result = new ApiResult<String>
            {
                Errors = new List<String>()
            };
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                ApplicationDbUser Sallon = await _userManager.FindByIdAsync(SallonID);
                if (Sallon is null)
                {
                    Result.Errors.Add("Invalid Sallon Id");
                    return BadRequest(Result);
                }

                IEnumerable<WorkerEvaOuptput> workers = Sallon.Workers.Select(x =>
                {
                    double Points = Sallon.WorkerEvaluations.Where(x => x.WorkerID == x.ID).Average(x => x.Points);

                    return new WorkerEvaOuptput
                    {
                        ID = x.ID,
                        nameAr = x.nameAr,
                        nameEn = x.nameEn,
                        Image = x.Image,
                        Description = x.Description,
                        Points = Points,
                        Services = x.SubServices.Select(service => new SubServicesOutput
                        {
                            ID = service.ID,
                            nameAr = service.nameAr,
                            nameEn = service.nameEn,
                            duration = service.duration,
                            price = service.price,
                            isActive = service.isActive,
                            Image = service.Image,
                            DescriptionAr = service.DescriptionAr,
                            DescriptionEn = service.DescriptionEn,
                        }).ToList()
                    };
                }).ToList();

                return Ok(workers);
            }
            catch (Exception)
            {
                return BadRequest(new ApiResult<String>
                {
                    Result = false,
                    Errors = new List<String>() { "Un Expected Error" }
                });
            }
        }

        /// <summary>
        /// الايام التي يداوم فيا الموظف
        /// </summary>
        /// <remarks>
        /// القيم المسموح ادخالها [Saturday, Sunday, Monday, Tuesday, Wednesday, Thursday, Friday]
        /// 
        /// مثال: saturday,monday,thursday
        /// </remarks>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost(ApiRoutes.Client.NewWorker)]
        [ProducesResponseType(200, Type = typeof(ApiResult<string>))]
        [ProducesResponseType(400, Type = typeof(ApiResult<string>))]
        public async Task<IActionResult> NewWorker(NewWorker model)
        {
            ApiResult<string> Result = new ApiResult<String>
            {
                Errors = new List<String>()
            };
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                ApplicationDbUser Sallon = await _userManager.FindByIdAsync(model.SallonID);
                if (Sallon == null || Sallon.typeUser != (int)Enums.AllEnums.TypeUser.provider)
                {
                    Result.Errors.Add("Invalid Sallon Id");
                    return BadRequest(Result);
                }

                String[] AttendanceDays = model.AttendanceDays.Split(';', StringSplitOptions.RemoveEmptyEntries);
                String[] WeekDays = Enum.GetNames(typeof(WeekDays));
                bool daysExists = AttendanceDays.All(day => WeekDays.Select(x => x.ToLower()).Contains(day.Trim().ToLower()));
                if (daysExists || AttendanceDays.Count() != AttendanceDays.Select(x => x.Trim().ToLower()).Distinct().Count())
                {
                    Result.Errors.Add("Invalid Attendance Days");
                    return BadRequest(Result);
                }
                String imagePath = ProcessUploadedFile(_HostingEnvironment, model.Image, "SalonWorkers");
                Worker worker = new Worker()
                {
                    AttendanceDays = String.Join(',', AttendanceDays.Select(x => x.Trim().ToLower())),
                    nameAr = model.nameAr,
                    nameEn = model.nameEn,
                    Description = model.Description,
                    Image = imagePath,
                    SallonID = model.SallonID,
                };
                _context.Workers.Add(worker);
                int result = await _context.SaveChangesAsync();
                if (result >= 0)
                {
                    Result.Result = true;
                    return Ok(Result);
                }
                throw new Exception();
            }
            catch (Exception)
            {
                return BadRequest(new ApiResult<String>
                {
                    Result = false,
                    Errors = new List<String>() { "Un Expected Error" }
                });
            }
        }

        [HttpPost(ApiRoutes.Client.UpdateWorker)]
        [ProducesResponseType(200, Type = typeof(ApiResult<string>))]
        [ProducesResponseType(400, Type = typeof(ApiResult<string>))]
        public async Task<IActionResult> UpdateWorker(UpdateWorker model)
        {
            ApiResult<string> Result = new ApiResult<String>
            {
                Errors = new List<String>()
            };
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                Worker worker = await _context.Workers.FirstOrDefaultAsync(x => x.ID == model.ID);
                if (worker == null)
                {
                    Result.Errors.Add("Invalid Worker Id");
                    return BadRequest(Result);
                }


                String[] AttendanceDays = model.AttendanceDays.Split(';', StringSplitOptions.RemoveEmptyEntries);
                String[] WeekDays = Enum.GetNames(typeof(WeekDays));
                bool daysExists = AttendanceDays.All(day => WeekDays.Select(x => x.ToLower()).Contains(day.Trim().ToLower()));
                if (daysExists || AttendanceDays.Count() != AttendanceDays.Select(x => x.Trim().ToLower()).Distinct().Count())
                {
                    Result.Errors.Add("Invalid Attendance Days");
                    return BadRequest(Result);
                }

                String imagePath = worker.Image;
                if (model.Image != null)
                    imagePath = ProcessUploadedFile(_HostingEnvironment, model.Image, "SalonWorkers");
                worker.AttendanceDays = String.Join(',', AttendanceDays.Select(x => x.Trim().ToLower()));
                worker.nameAr = model.nameAr;
                worker.nameEn = model.nameEn;
                worker.Description = model.Description;
                worker.Image = imagePath;
                _context.Workers.Update(worker);
                int result = await _context.SaveChangesAsync();
                if (result >= 0)
                {
                    Result.Result = true;
                    return Ok(Result);
                }
                throw new Exception();
            }
            catch (Exception)
            {
                return BadRequest(new ApiResult<String>
                {
                    Result = false,
                    Errors = new List<String>() { "Un Expected Error" }
                });
            }
        }

        [HttpPost(ApiRoutes.Client.NewWorkerEvaluation)]
        [ProducesResponseType(200, Type = typeof(ApiResult<string>))]
        [ProducesResponseType(400, Type = typeof(ApiResult<string>))]
        public async Task<IActionResult> NewEvaluation(NewEvaluation model)
        {
            ApiResult<string> Result = new ApiResult<String>
            {
                Errors = new List<String>()
            };
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                ApplicationDbUser Sallon = await _userManager.FindByIdAsync(model.UserID);
                if (Sallon is null)
                {
                    Result.Errors.Add("Invalid User Id");
                    return BadRequest(Result);
                }
                Worker worker = await _context.Workers.FirstOrDefaultAsync(x => x.ID == int.Parse(model.SenderID));
                if (worker is null)
                {
                    Result.Errors.Add("Invalid Worker Id");
                    return BadRequest(Result);
                }
                WorkerEvaluation data = new WorkerEvaluation()
                {
                    UserID = model.UserID,
                    WorkerID = int.Parse(model.SenderID),
                    Points = model.Points,
                    Comment = model.Comment,
                };
                _context.WorkerEvaluations.Add(data);
                int result = await _context.SaveChangesAsync();
                if (result >= 0)
                {
                    Result.Result = true;
                    return Ok(Result);
                }
                throw new Exception();
            }
            catch (Exception)
            {
                return BadRequest(new ApiResult<String>
                {
                    Result = false,
                    Errors = new List<String>() { "Un Expected Error" }
                });
            }
        }
    }
}
