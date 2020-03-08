﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Denounces.Domain.Entities;
using Denounces.Infraestructure;
using Denounces.Infraestructure.Extensions;
using Denounces.Repositories.Contracts;
using Denounces.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Denounces.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProposalsController : ControllerBase
    {
        private readonly IProposalRepository _proposalRepository;
        private readonly IProposalTypeRepository _proposalTypeRepository;
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;

        protected async Task<ApplicationUser> GetUserAsync()
        {
            return await _userHelper.GetUserByEmailAsync(User.Identity.Name);
        }

        public ProposalsController(ApplicationDbContext context, IProposalRepository proposalRepository,
            IProposalTypeRepository proposalTypeRepository, IUserHelper userHelper, 
            ICurrentUserFactory currentUser,
            IConverterHelper converterHelper
) //: base(context, userHelper, currentUser)
        {
            _proposalRepository = proposalRepository;
            _proposalTypeRepository = proposalTypeRepository;
            _userHelper = userHelper;
        }

        [HttpGet]
        public IEnumerable<Proposal> GetProposals()
        {
            return _proposalRepository.GetAll().ToList();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProposal([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await _proposalRepository.GetByIdAsync(id);

            if (entity == null)
            {
                return NotFound();
            }
           // return Ok(_converterHelper.ToProposalViewModel(entity));
            return Ok(entity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProposal([FromRoute] long id, [FromBody] Proposal entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != entity.Id)
            {
                return BadRequest();
            }

            try
            {
                await _proposalRepository.UpdateAsync(entity);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProposalExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> PostProposal([FromBody] Proposal proposal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            proposal.CreatedUser = await GetUserAsync();
            await _proposalRepository.CreateAsync(proposal);

            return CreatedAtAction("GetProposal", new { id = proposal.Id }, proposal);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProposal([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var proposal = await _proposalRepository.GetByIdAsync(id);
            if (proposal == null)
            {
                return NotFound();
            }

            await _proposalRepository.DeleteAsync(proposal);


            return Ok(proposal);
        }

        private bool ProposalExists(long id)
        {
            return _proposalRepository.Exists(id);
        }

    }
}