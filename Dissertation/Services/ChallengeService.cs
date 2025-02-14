using Dissertation.Data;
using Dissertation.Models.Challenge;
using Dissertation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dissertation.Services;

    public class ChallengeService : IChallengeService
    {
        private readonly AppDbContext _dbContext;

        public ChallengeService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Project>> GetProjectsAsync()
        {
            return await _dbContext.Projects.ToListAsync();
        }

        public async Task<Project?> GetProjectWithScenariosAsync(int projectId)
        {
            return await _dbContext.Projects.Include(p => p.Scenarios).FirstOrDefaultAsync(p => p.Id == projectId);
        }

        public async Task<List<Choice>> GetChoicesForScenarioAsync(int scenarioId)
        {
            return await _dbContext.Choices.Where(c => c.ScenarioId == scenarioId).OrderBy(x => Guid.NewGuid()).ToListAsync();
        }

        public async Task<Scenario?> GetScenarioByIdAsync(int scenarioId)
        {
            return await _dbContext.Scenarios.FindAsync(scenarioId);
        }

        public async Task<int> GetScenarioCountForPhaseAsync(Phase phase)
        {
            return await _dbContext.Scenarios.CountAsync(s => s.Phase == phase);
        }

        public async Task<List<Scenario>> GetScenariosForPhaseAsync(Phase phase)
        {
            return await _dbContext.Scenarios.Where(s => s.Phase == phase).OrderBy(s => s.Id).ToListAsync();
        }
    }



