using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DemoWebApi.Models;

using System.Linq;
using DemoWebApi.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;

namespace DemoWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeptController : ControllerBase
    {
        db1045Context db = new db1045Context();
        [HttpGet]
        [Route("ListDept")]
        public IActionResult GetDept()
        {
            //var data = db.Depts.ToList();
            //var data = from dept in db.Depts select dept;
            //Suppose we dont want to print all the 20 columns
            var data = from dept in db.Depts select new { Id = dept.Id, Name = dept.Name, Location = dept.Location };
            //in this we return the status if api that we saw in postman 200 == OK.
            return Ok(data);
        }
        [HttpGet]
        [Route("ListDept/{id}")]
        public IActionResult GetDept(int? id)
        {
            if (id == null)
            {
                return BadRequest("Id cannot be Null");
            }
            //var data = db.Depts.Where(d => d.Id == id).Select(d => new { id = d.Id, name = d.Name, location = d.Location }).FirstOrDefault();
            var data = (from dept in db.Depts where dept.Id == id select new { Id = dept.Id, Name = dept.Name, Location = dept.Location }).FirstOrDefault();
            if(data == null)
            {
                return NotFound($"Department {id} not present");
            }
            return Ok(data);
        }
        [HttpGet]
        [Route("ListCity")]
       // /api/dept/listcity?city=pune
       // this way path is to be written 
        public IActionResult GetCity([FromQuery] string city)
        {
            var data = db.Depts.Where(d => d.Location == city).Select(d => new {id=d.Id, Name=d.Name, Location=d.Location});
            return Ok(data);
        }

        [HttpGet]
        [Route("ShowDept")]
        public IActionResult GetDeptInfo()
        {
            
            var data = db.DeptInfo_VM.FromSqlInterpolated<DeptInfo_VM>($"DeptInfoo  ");
            return Ok(data);
        }
        //Previous were just to display the data
        //Add a record(Now we will add the data.

        [Route("AddDept")]
        public IActionResult PostDept(Dept dept)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    //db.Depts.Add(dept);
                    //db.SaveChanges();
                    //call store procedure to add records
                    db.Database.ExecuteSqlInterpolated($"deptadd {dept.Id},{dept.Name},{dept.Location}");
                }
                catch(Exception e)
                {
                    return BadRequest(e.InnerException.Message);
                }
            }
            return Created("Record Success",dept);
        }
        [HttpPut]
        [Route("EditDept/{id}")]

        public IActionResult PutDept(int id, Dept dept)
        {
            if(ModelState.IsValid)
            {
                Dept odept = db.Depts.Find(id);
                odept.Name = dept.Name;
                odept.Location = dept.Location;
                db.SaveChanges();
                return Ok();
            }
            return BadRequest("Unable to Edit records");
        }
        [HttpDelete]
        [Route("DeleteDept/{id}")]

        public IActionResult DeleteDept(int id)
        {
            var data = db.Depts.Find(id);
            db.Depts.Remove(data);
            db.SaveChanges();   
                return Ok();
        }
    }
}
