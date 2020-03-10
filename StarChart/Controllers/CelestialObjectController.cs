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
    }
}
