﻿using Clanstvo.Repositories;
using Microsoft.AspNetCore.Mvc;
using ClanstvoWebApi.DTOs;
using DbModels = Clanstvo.DataAccess.SqlServer.Data.DbModels;
using Clanstvo.Commons;
using BaseLibrary;
using System;
using System.Data;

namespace ClanstvoWebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ClanController : ControllerBase
{
    private readonly IClanRepository _clanRepository;


    public ClanController(IClanRepository clanRepository)
    {
        _clanRepository = clanRepository;
    }

    // GET: api/Clanovi
    [HttpGet]
    public ActionResult<IEnumerable<Clan>> GetAllClan()
    {

        var clanResults = _clanRepository.GetAll()
            .Map(clan => clan.Select(DtoMapping.ToDto));

        return clanResults
            ? Ok(clanResults.Data)
            : Problem(clanResults.Message, statusCode: 500);
    }

    
    // GET: api/Clanovi
    [HttpGet("/api/[controller]/Neplacene")]
    public ActionResult<IEnumerable<Clan>> GetNisuPlatili()
    {

        var clanResults = _clanRepository.GetNisuPlatili()
            .Map(clan => clan.Select(DtoMapping.ToDto));

        return clanResults
            ? Ok(clanResults.Data)
            : Problem(clanResults.Message, statusCode: 500);
    }
    


    // GET: api/Clanovi/5
    [HttpGet("{id}")]
    public ActionResult<Clan> GetClan(int id)
    {
        var clanResult = _clanRepository.Get(id).Map(DtoMapping.ToDto);

        return clanResult switch
        {
            { IsSuccess: true } => Ok(clanResult.Data),
            { IsFailure: true } => NotFound(),
            { IsException: true } or _ => Problem(clanResult.Message, statusCode: 500)
        };
    }

    [HttpGet("/Aggregate/{id}")]
    public ActionResult<ClanAggregate> GetAggregate(int id)
    {
        var clanResult = _clanRepository.GetAggregate(id).Map(DtoMapping.ToAggregateDto);

        return clanResult switch
        {
            { IsSuccess: true } => Ok(clanResult.Data),
            { IsFailure: true } => NotFound(),
            { IsException: true } or _ => Problem(clanResult.Message, statusCode: 500)
        };
    }

    
    // PUT: api/Clan/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public IActionResult EditClan(int id, Clan clan)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (id != clan.Id)
        {
            return BadRequest();
        }

        if (!_clanRepository.Exists(id))
        {
            return NotFound();
        }

        var updateResult = _clanRepository.Update(clan.ToDomain());

        return updateResult
            ? AcceptedAtAction("EditClan", clan) 
            : Problem(updateResult.Message, statusCode: 500);
    }

    // POST: api/Clan
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public ActionResult<Clan> CreateClan(Clan clan)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createResult = _clanRepository.Insert(clan.ToDomain());

        return createResult
            ? CreatedAtAction("GetClan", new { id = clan.Id }, clan) 
            : Problem(createResult.Message, statusCode: 500);
    }

    // DELETE: api/Clan/5
    [HttpDelete("{id}")]
    public IActionResult DeleteClan(int id)
    {
      if(!_clanRepository.Exists(id))
            return NotFound();

        var deleteResult = _clanRepository.Remove(id);
        return deleteResult
            ? NoContent()
            : Problem(deleteResult.Message, statusCode: 500);
    }

    [HttpPost("DodajRangStarost/{clanId}")]
    public IActionResult AssignClanToRangStarost(int clanId, DodjelaStarost dodjelaStarost)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var clanResult = _clanRepository.GetAggregate(clanId);
        if (clanResult.IsFailure)
        {
            return NotFound();
        }
        if (clanResult.IsException)
        {
            return Problem(clanResult.Message, statusCode: 500);
        }

        var clan = clanResult.Data;

        clan.DodajRangStarost(
            dodjelaStarost.RangStarost.ToDomain(),
            dodjelaStarost.Datum
            );

        var updateResult = _clanRepository.UpdateAggregate(clan);

        return updateResult
            ? Accepted()
            : Problem(updateResult.Message, statusCode: 500);
    }

    [HttpPost("DodajRangZasluga/{clanId}")]
    public IActionResult AssignClanToRangZasluga(int clanId, DodjelaZasluga dodjelaZasluga)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var clanResult = _clanRepository.GetAggregate(clanId);
        if (clanResult.IsFailure)
        {
            return NotFound();
        }
        if (clanResult.IsException)
        {
            return Problem(clanResult.Message, statusCode: 500);
        }

        var clan = clanResult.Data;

        clan.DodajRangZasluga(
            dodjelaZasluga.RangZasluga.ToDomain(),
            dodjelaZasluga.Datum
            );

        var updateResult = _clanRepository.UpdateAggregate(clan);

        return updateResult
            ? Accepted()
            : Problem(updateResult.Message, statusCode: 500);
    }

    [HttpPost("UkloniRangStarost/{clanId}")]
    public IActionResult DismissClanFromRangStarost(int clanId, RangStarost rang)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var clanResult = _clanRepository.GetAggregate(clanId);
        if (clanResult.IsFailure)
        {
            return NotFound();
        }
        if (clanResult.IsException)
        {
            return Problem(clanResult.Message, statusCode: 500);
        }

        var clan = clanResult.Data;


        if (!clan.UkloniRangStarost(rang.ToDomain()))
        {
            return NotFound($"Couldn't find rang {rang.Naziv} on clan");
        }

        var updateResult = _clanRepository.UpdateAggregate(clan);

        return updateResult
            ? Accepted()
            : Problem(updateResult.Message, statusCode: 500);
    }

    [HttpPost("UkloniRangZasluga/{clanId}")]
    public IActionResult DismissClanFromRangZasluga(int clanId, RangZasluga rang)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var clanResult = _clanRepository.GetAggregate(clanId);
        if (clanResult.IsFailure)
        {
            return NotFound();
        }
        if (clanResult.IsException)
        {
            return Problem(clanResult.Message, statusCode: 500);
        }

        var clan = clanResult.Data;


        if (!clan.UkloniRangZasluga(rang.ToDomain()))
        {
            return NotFound($"Couldn't find rang {rang.Naziv} on clan");
        }

        var updateResult = _clanRepository.UpdateAggregate(clan);

        return updateResult
            ? Accepted()
            : Problem(updateResult.Message, statusCode: 500);
    }
}
