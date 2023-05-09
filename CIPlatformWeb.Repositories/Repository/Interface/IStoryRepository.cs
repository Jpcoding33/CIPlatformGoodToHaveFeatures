using CIPlatformWeb.Entities.DataModels;
using CIPlatformWeb.Entities.ViewModels;
using CIPlatformWeb.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatformWeb.Repositories.Repository.Interface
{
    public interface IStoryRepository : IGenericRepository<Story>
    {
        Story GetStoryAndMedia(Expression<Func<Story, bool>> filter);

        Story GetStory(Expression<Func<Story, bool>> filter);

        List<DraftedStoryDetails> GetDraftedStoryDetails(int missId, long userId);

        IEnumerable<Story> GetAllStory(Expression<Func<Story, bool>> filter);

    }
}
