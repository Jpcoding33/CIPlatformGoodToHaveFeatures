using CIPlatformWeb.Entities.Data;
using CIPlatformWeb.Entities.DataModels;
using CIPlatformWeb.Entities.ViewModels;
using CIPlatformWeb.Repositories.GenericRepository;
using CIPlatformWeb.Repositories.GenericRepository.Interface;
using CIPlatformWeb.Repositories.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatformWeb.Repositories.Repository
{
    public class StoryRepository : GenericRepository<Story>, IStoryRepository
    {
        private readonly CIDbContext _db;
        public StoryRepository(CIDbContext db) : base(db)
        {
            _db = db;
        }
        public Story GetStoryAndMedia(Expression<Func<Story, bool>> filter)
        {
            IQueryable<Story> query = dbSet;
            query = query.Where(filter).Include(story => story.StoryMedia);
            return query.FirstOrDefault();
        }
        public Story GetStory(Expression<Func<Story, bool>> filter)
        {
            IQueryable<Story> query = dbSet;
            query = query.Where(filter).Include(story => story.User).Include(story => story.StoryMedia);
            return query.FirstOrDefault();
        }

        public List<DraftedStoryDetails> GetDraftedStoryDetails(int missId, long userId)
        {
            var query = (from st in _db.Stories
                         join md in _db.StoryMedia
                         on st.StoryId equals md.StoryId into g
                         from md in g.DefaultIfEmpty()
                         where st.MissionId == missId && st.UserId == userId && (st.Status == "DRAFT" || st.Status == "DECLINED")
                         orderby st.StoryId descending
                         select new DraftedStoryDetails
                         {
                             Title = st.Title,
                             Description = st.Description,
                             CreatedAt = st.CreatedAt,
                             Path = md.Path,
                             Type = md.Type
                         }).ToList();
            return query;
        }

        public IEnumerable<Story> GetAllStory(Expression<Func<Story, bool>> filter)
        {
            IQueryable<Story> query = dbSet;
            query = query.Where(filter).Include(story => story.User).Include(story => story.Mission);
            return query.ToList();
        }
    }
}

