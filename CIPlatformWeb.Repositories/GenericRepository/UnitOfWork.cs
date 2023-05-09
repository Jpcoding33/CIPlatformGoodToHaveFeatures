using CIPlatformWeb.Entities.Data;
using CIPlatformWeb.Repositories.GenericRepository.Interface;
using CIPlatformWeb.Repositories.Repository;
using CIPlatformWeb.Repositories.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatformWeb.Repositories.GenericRepository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CIDbContext _db;
        public UnitOfWork(CIDbContext db)
        {
            _db = db;
            City = new CityRepository(_db);
            Country = new CountryRepository(_db);
            Comment = new CommentRepository(_db);
            FavoriteMission = new FavoriteMissionRepository(_db);
            MissionApplication = new MissionApplicationRepository(_db);
            MissionInvite = new MissionInviteRepository(_db);
            Mission = new MissionRepository(_db);
            MissionTheme = new MissionThemeRepository(_db);
            PasswordReset = new PasswordResetRepository(_db);
            StoryMedia = new StoryMediaRepository(_db);
            Story = new StoryRepository(_db);
            User = new UserRepository(_db);
            MissionRating = new MissionRatingRepository(_db);
            StoryInvite = new StoryInviteRepository(_db);   
            Skill = new SkillRepository(_db);
            UserSkill = new UserSkillRepository(_db);
            TimeSheet = new TimeSheetRepository(_db);
            CMSPage = new CMSPageRepository(_db);
            MissionSkill = new MissionSkillRepository(_db);
            Banner = new BannerRepository(_db);
            GoalMisison = new GoalMisisonRepository(_db);
            MissionMedia = new MissionMediaRepository(_db);
            MissionDocuments = new MissionDocumentsRepository(_db);
            ContactUs = new ContactUsRepository(_db);
            NotifType = new NotifTypeRepository(_db);
            NotifToAll = new NotificationToAll(_db);
        }
        public ICityRepository City { get; private set; }

        public ICountryRepository Country { get; private set; }

        public ICommentRepository Comment { get; private set; }

        public IFavoriteMissionRepository FavoriteMission { get; private set; }

        public IMissionApplicationRepository MissionApplication { get; private set; }

        public IMissionInviteRepository MissionInvite { get; private set; }

        public IMissionRepository Mission { get; private set; }

        public IMissionThemeRepository MissionTheme { get; private set; }

        public IPasswordResetRepository PasswordReset { get; private set; }

        public IStoryMediaRepository StoryMedia { get; private set; }

        public IStoryRepository Story { get; private set; }

        public IUserRepository User { get; private set; }

        public IMissionRatingRepository MissionRating { get; private set; }

        public IStoryInviteRepository StoryInvite { get; private set; }

        public ISkillRepository Skill { get; private set; }

        public IUserSkillRepository UserSkill { get; private set; }
        public ITimeSheetRepository TimeSheet { get; private set; }

        public ICMSPageRepository CMSPage { get; private set; }

        public IMissionSkillRepository MissionSkill { get; private set; }

        public IBannerRepository Banner { get; private set; }

        public IGoalMisisonRepository GoalMisison { get; private set; }

        public IMissionMediaRepository MissionMedia { get; private set; }

        public IMissionDocumentsRepository MissionDocuments { get; private set; }

        public IContactUsRepository ContactUs { get; private set; }

        public INotifTypeRepository NotifType { get; private set; }
        public INotificationToAll NotifToAll { get; private set; }

        public int Save()
        {
            return _db.SaveChanges();
        }
        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
