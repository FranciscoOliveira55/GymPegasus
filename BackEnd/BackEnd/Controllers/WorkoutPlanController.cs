using AutoMapper;
using BackEnd.Data;
using BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkoutPlanController : GenericCrudController<WorkoutPlan>
    {
        public WorkoutPlanController(
            GymContext context,
            ILogger<GenericCrudController<WorkoutPlan>> logger,
            IOptions<JsonOptions> jsonOptions,
            IMapper mapper
            ) : base(context, logger, jsonOptions, mapper)
        { }




        /// <summary>
        /// Reads WorkoutPlan in the database with the Id
        /// Includes Exercise and ExerciseTemplate
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("WorkoutPlanWithExercises/{id}")]
        public async Task<ActionResult<IEnumerable<WorkoutPlan>>> ReadWorkoutPlanWithExercises(int id)
        {
            // Read a Object from Database with an Id
            if (table == null)
                return NotFound();

            //Includes Related Entites if Needed
            table.Include(workoutPlan => workoutPlan.WorkoutPlanExercises) // Include the Exercises collection for each WorkoutPlan
                .ThenInclude(workoutPlanExercise => workoutPlanExercise.ExerciseTemplate) // Include the ExerciseTemplate collection for each Exercise
                .ToList();

            //Selects the Item with the property
            try
            {
                var readedObject = await table.FindAsync(id);

                if (readedObject == null)
                    return NotFound();

                return Ok(readedObject);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred: {e.Message}");
            }
        }


    }
}
