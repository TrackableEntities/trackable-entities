using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TrackableEntities;
using TrackableEntities.EF6;
using TrackableEntities.Common;
using WebApiSample.Service.Entities.Models;
using WebApiSample.Service.Entities.Contexts;

namespace WebApiSample.Service.WebApi.Controllers
{
    public class EmployeeController : ApiController
    {
        private readonly NorthwindSlimContext _dbContext = new NorthwindSlimContext();

        // GET api/Employee
        [ResponseType(typeof(IEnumerable<Employee>))]
        public async Task<IHttpActionResult> GetEmployees()
        {
	        IEnumerable<Employee> employees = await _dbContext.Employees.ToListAsync();
	
            return Ok(employees);
        }

        // GET api/Employee/5
        [ResponseType(typeof(Employee))]
        public async Task<IHttpActionResult> GetEmployee(int id)
        {
	        Employee employee = await _dbContext.Employees
                .Include(e => e.Territories)
                .SingleOrDefaultAsync(e => e.EmployeeId == id);
	
            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        // POST api/Employee
        [ResponseType(typeof(Employee))]
        public async Task<IHttpActionResult> PostEmployee(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            employee.TrackingState = TrackingState.Added;
            _dbContext.ApplyChanges(employee);

            await _dbContext.SaveChangesAsync();

            await _dbContext.LoadRelatedEntitiesAsync(employee);
            employee.AcceptChanges();
            return CreatedAtRoute("DefaultApi", new { id = employee.EmployeeId }, employee);
        }

        // PUT api/Employee
        [ResponseType(typeof(Employee))]
        public async Task<IHttpActionResult> PutEmployee(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // WORKAROUND for BUG (PR #43):
            // Fix up territory-employee references
            foreach (var territory in employee.Territories)
            {
                territory.Employees.Add(employee);
            }

            _dbContext.ApplyChanges(employee);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Employees.Any(e => e.EmployeeId == employee.EmployeeId))
                {
                    return Conflict();
                }
                throw;
            }

			await _dbContext.LoadRelatedEntitiesAsync(employee);
			employee.AcceptChanges();
	        return Ok(employee);
        }

        // DELETE api/Employee/5
        public async Task<IHttpActionResult> DeleteEmployee(int id)
        {
			Employee employee = await _dbContext.Employees
			    .Include(e => e.Territories) // Include child entities
				.SingleOrDefaultAsync(e => e.EmployeeId == id);
			if (employee == null)
            {
                return Ok();
            }

			employee.TrackingState = TrackingState.Deleted;
			_dbContext.ApplyChanges(employee);

            try
            {
	            await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Employees.Any(e => e.EmployeeId == employee.EmployeeId))
                {
                    return Conflict();
                }
                throw;
            }

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}