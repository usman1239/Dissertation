//using Dissertation.Data;
//using Dissertation.Services.Interfaces;

//namespace Dissertation.Services;

//public class UserProgressService(AppDbContext dbContext) : IUserProgressService
//{
//    //public async Task<UserProgress?> GetUserProgressAsync(string userId)
//    //{
//    //    return await dbContext.UserProgress
//    //        .FirstOrDefaultAsync(up => up.UserId == userId);
//    //}

//    //public async Task SaveUserProgressAsync(UserProgress progress)
//    //{
//    //    var existingProgress = await dbContext.UserProgress
//    //        .FirstOrDefaultAsync(up => up.UserId == progress.UserId);

//    //    if (existingProgress != null)
//    //    {
//    //        existingProgress.ProjectId = progress.ProjectId;
//    //        existingProgress.CurrentScenarioId = progress.CurrentScenarioId;
//    //        existingProgress.Score = progress.Score;

//    //        dbContext.UserProgress.Update(existingProgress);
//    //    }
//    //    else
//    //    {
//    //        dbContext.UserProgress.Add(progress);
//    //    }

//    //    await dbContext.SaveChangesAsync();
//    //}
//}

