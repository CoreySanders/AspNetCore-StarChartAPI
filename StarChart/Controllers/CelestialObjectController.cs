using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarChart.Data;
using System.Linq;
using StarChart.Models;
using System;

namespace StarChart.Controllers{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        
        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id ) {
            CelestialObject celObject = _context.CelestialObjects.SingleOrDefault(co => co.Id == id);
            if (celObject == null)
                return NotFound();

            celObject.Satellites = _context.CelestialObjects.Where(co => co.OrbitedObjectId == id).ToList();

            return Ok(celObject);

        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celObjects = _context.CelestialObjects.Where(co => co.Name == name).ToList();

            if (!celObjects.Any())
                return NotFound();

            foreach (var celObject in celObjects )
            {
                celObject.Satellites = _context.CelestialObjects.Where(co => co.OrbitedObjectId == celObject.Id).ToList();
            }
            return Ok(celObjects);
        }

        [HttpGet]
        public IActionResult GetAll() {
            var celObjects = _context.CelestialObjects.ToList();
            foreach (var celObj in celObjects)
            {
                celObj.Satellites = _context.CelestialObjects.Where(co => co.OrbitedObjectId == celObj.Id).ToList(); 
            }
            return Ok(celObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject obj)
        {
            if (obj == null)
                return BadRequest();
            _context.CelestialObjects.Add(obj);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new { id = obj.Id }, obj);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject newObj)
        {
            var celObject = _context.CelestialObjects.SingleOrDefault(co => co.Id == id);
            if (celObject == null)
                return NotFound();

            celObject.Name = newObj.Name;
            celObject.OrbitalPeriod = newObj.OrbitalPeriod;
            celObject.OrbitedObjectId = newObj.OrbitedObjectId;
            _context.Update(celObject);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celObject = _context.CelestialObjects.SingleOrDefault(co => co.Id == id);

            if (celObject == null)
                return NotFound();

            celObject.Name = name;
            _context.Update(celObject);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celObject = _context.CelestialObjects.Where(co => co.Id == id).ToList();

            if (celObject == null)
                return NotFound();

            _context.RemoveRange(celObject);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
